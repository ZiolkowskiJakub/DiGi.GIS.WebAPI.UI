/**
 * Typology area view - center panel (issue #25): the administrative outline with every building of the
 * area drawn as a dot coloured by its solved typology, dimmed outside the tree selection.
 *
 * The 2D stack of Views/AdministrativeAreal2D (administrative.js): one SVG with a 500x500 viewBox, fitted
 * once from the outline's bounding box (y flipped), scaled by the browser through preserveAspectRatio - so
 * a panel drag, a panel toggle or the header/footer collapse refits the map with no script. The wheel zooms
 * the viewBox about the pointer, a drag pans it and a double click fits the outline again; the dots and the
 * marker keep a readable size through the --typology-point-scale custom property the CSS radii follow.
 * Buildings are dots only, never polygons: positions come from the UI proxy of the area-scoped
 * centroid endpoint (DiGi.GIS.WebAPI#34), joined with the solve DTO by (reference, countyId) because a
 * reference is unique only per county partition - and, when that key misses, by the reference alone when
 * the DTO lists it once: the building data and the building_2d rows of a multi-part county can be filed
 * under different parts, and a solved building must not read as unclassified for that.
 *
 * Dots are grouped in one <g> per typology path, plus one neutral group for buildings the typology could
 * not classify (or every building, while no definition is solved). Dimming toggles a class on the groups
 * - a class toggle per bucket rather than a style write per point - and the selected building is marked
 * by a ring in a top layer instead of reparenting a dot, so it stays emphasised and on top whatever the
 * dimming does.
 *
 * The dot layer is built once per page load, when the fit and the centroids are both known, and a solve
 * re-groups the existing dots instead of drawing them again (issue #52). Above canvasThreshold dots it is
 * a <canvas> beneath the SVG instead of one <circle> per building - 155 307 SVG elements for county 1465
 * made painting and hover slow. The per-path <g> groups stay in the SVG, empty, so dimming is still a class
 * per group; the canvas draws each group's dots in the group's computed fill and opacity. The pointer is
 * resolved through a uniform grid over the projected positions, since there is no element to hit. The selection follows the left panel's 'typology:selectionchange' event; a dot click
 * reports 'typology:buildingselect' on document for the right panel (#26). Shared helpers and event names
 * come from typology-common.js, loaded first. Classic script, no imports.
 */
const digiTypologyMap = (function () {
    'use strict';

    const common = digiTypologyCommon;
    const element = common.element;
    const pathKey = common.pathKey;
    const buildingKey = common.buildingKey;

    const svgNamespace = 'http://www.w3.org/2000/svg';

    // The fit of administrative.js: a 500-unit canvas, the outline scaled into 450 with a 25 margin.
    const canvasSize = 500;
    const padding = 25;
    // Small on purpose: a county fits ~40 km into the canvas, so one unit is tens of metres and dots of a
    // town centre overlap whatever the radius; the browser scales them with the panel.
    const pointRadius = 0.7;
    const markerRadius = 4;

    // Wheel zoom: one notch scales the view by this factor, up to zoomMaximum times the fit (a county's 40 km
    // becomes ~150 m across the viewport); below the fit there is nothing to see, so 1 is the floor. A drag
    // shorter than dragThreshold pixels is a click, not a pan.
    const zoomStep = 1.25;
    const zoomMaximum = 256;
    const dragThreshold = 3;

    // Above this many dots the layer is a canvas rather than SVG circles. 33k circles paint and hit-test
    // fine; 155k (county 1465) do not. The browser tests lower it through window.digiTypologyMapCanvasThreshold
    // to compare the two modes on the same area.
    const canvasThreshold = 50000;
    // The canvas has no element to hit, so a dot counts as under the pointer within its drawn radius plus
    // this many CSS pixels; the grid cell is in viewBox units.
    const hitTolerance = 3;
    const gridCellSize = 2;

    let scaleParameters = null;
    let centroids = null;
    let centroidsByKey = new Map();
    let centroidsFailed = false;
    let nodesByKey = new Map();
    let buildingsByKey = new Map();
    let buildingsByReference = new Map();
    let selectedPath = null;
    let selectedBuildingKey = null;

    // The dot layer, built once per (centroids, fit): the projected positions in viewBox units, then either
    // one <circle> per centroid (SVG mode) or the centroid indices of each group and a hit-test grid (canvas).
    let pointsBuilt = false;
    let canvasMode = false;
    let projectedX = null;
    let projectedY = null;
    let circles = null;
    let groupMembers = new Map();
    let grid = null;
    let canvas = null;
    let drawRequested = false;
    let dimmedOpacity = null;

    // The viewBox: the fit is (0, 0, 500, 500); zooming narrows it, panning moves it, both within the canvas.
    let view = { x: 0, y: 0, size: canvasSize };

    // ----- helpers -----

    function project(x, y) {
        return {
            x: (x - scaleParameters.minX) * scaleParameters.scale + padding,
            y: canvasSize - ((y - scaleParameters.minY) * scaleParameters.scale + padding)
        };
    }

    function colorOfPath(key) {
        const node = nodesByKey.get(key);
        return node === undefined ? common.unclassifiedColor : common.colorOf(node);
    }

    // The DTO entry of a dot: by the full key first, then by the reference alone when the DTO lists it once.
    function buildingOf(reference, countyId) {
        const building = buildingsByKey.get(buildingKey(reference, countyId));
        return building !== undefined ? building : buildingsByReference.get(String(reference));
    }

    // ----- status -----

    // The viewport's single status line: the placeholder until the outline draws, then only an empty or
    // unavailable-dots message over the outline.
    function showStatus(text) {
        const status = element('typology-viewport-empty');
        if (status === null) {
            return;
        }
        const paragraph = status.querySelector('p');
        if (paragraph !== null) {
            paragraph.textContent = text;
        }
        status.hidden = false;
    }

    function hideStatus() {
        const status = element('typology-viewport-empty');
        if (status !== null) {
            status.hidden = true;
        }
    }

    // ----- outline -----

    function renderOutline(outlines) {
        const layer = element('typology-map-outline');
        if (layer === null || !Array.isArray(outlines) || outlines.length === 0) {
            return false;
        }

        // Bounding box of every outline, computed once; every later projection (the dots, the marker) uses it.
        let minX = Infinity, maxX = -Infinity, minY = Infinity, maxY = -Infinity;
        for (let i = 0; i < outlines.length; i++) {
            const coordinates = outlines[i];
            for (let j = 0; j + 1 < coordinates.length; j += 2) {
                const x = coordinates[j];
                const y = coordinates[j + 1];
                if (x < minX) { minX = x; }
                if (x > maxX) { maxX = x; }
                if (y < minY) { minY = y; }
                if (y > maxY) { maxY = y; }
            }
        }
        if (!isFinite(minX) || !isFinite(minY) || maxX - minX <= 0 || maxY - minY <= 0) {
            return false;
        }

        const scale = Math.min((canvasSize - 2 * padding) / (maxX - minX), (canvasSize - 2 * padding) / (maxY - minY));
        scaleParameters = { minX: minX, minY: minY, scale: scale };
        resetView();

        const fragment = document.createDocumentFragment();
        for (let i = 0; i < outlines.length; i++) {
            const coordinates = outlines[i];
            if (coordinates.length < 4) {
                continue;
            }
            const points = [];
            for (let j = 0; j + 1 < coordinates.length; j += 2) {
                const point = project(coordinates[j], coordinates[j + 1]);
                points.push(point.x.toFixed(2) + ',' + point.y.toFixed(2));
            }
            const polygon = document.createElementNS(svgNamespace, 'polygon');
            polygon.setAttribute('class', 'typology-outline');
            polygon.setAttribute('points', points.join(' '));
            fragment.appendChild(polygon);
        }
        layer.replaceChildren(fragment);
        return true;
    }

    // ----- dots -----

    function canvasThresholdValue() {
        return typeof window.digiTypologyMapCanvasThreshold === 'number' ? window.digiTypologyMapCanvasThreshold : canvasThreshold;
    }

    // The group a centroid belongs to: the path its DTO entry names, or null for the neutral group.
    function groupKeyOf(index) {
        const centroid = centroids[index];
        const building = buildingOf(centroid.reference, centroid.countyId);
        return building === undefined ? null : building.pathKey;
    }

    function createGroup(groupKey) {
        const group = document.createElementNS(svgNamespace, 'g');
        group.setAttribute('class', groupKey === null ? 'typology-point-group typology-point-unclassified' : 'typology-point-group');
        if (groupKey !== null) {
            group.setAttribute('data-path', groupKey);
            group.setAttribute('fill', colorOfPath(groupKey));
        }
        return group;
    }

    // Builds the dot layer once the fit and the centroids are both known; the DTO is optional. Projects every
    // centroid once, then creates one <circle> per centroid - carrying only its index into the centroid list -
    // or, above the threshold, the canvas's hit-test grid instead. A later solve only re-groups (groupPoints).
    function buildPoints() {
        const layer = element('typology-map-points');
        if (layer === null || scaleParameters === null || centroids === null) {
            return;
        }

        const count = centroids.length;
        projectedX = new Float64Array(count);
        projectedY = new Float64Array(count);
        for (let i = 0; i < count; i++) {
            const point = project(centroids[i].x, centroids[i].y);
            projectedX[i] = point.x;
            projectedY[i] = point.y;
        }

        canvasMode = count > canvasThresholdValue();
        circles = null;
        grid = null;
        if (canvasMode) {
            buildGrid();
            ensureCanvas();
        } else {
            circles = new Array(count);
            for (let i = 0; i < count; i++) {
                const circle = document.createElementNS(svgNamespace, 'circle');
                circle.setAttribute('class', 'typology-point');
                circle.setAttribute('cx', projectedX[i].toFixed(2));
                circle.setAttribute('cy', projectedY[i].toFixed(2));
                circle.setAttribute('r', pointRadius);
                circle.setAttribute('data-i', i);
                circles[i] = circle;
            }
        }

        const viewport = element('typology-viewport');
        if (viewport !== null) {
            viewport.classList.toggle('typology-viewport-canvas', canvasMode);
        }

        pointsBuilt = true;
        groupPoints();
    }

    // Files every dot under the group of its current assignment: one <g> per bucket with its fill, the neutral
    // group first so classified dots paint over it, then the buckets in path order so the paint order (which
    // colour wins where dots overlap) does not depend on the centroid row order. SVG mode moves the existing
    // circles into the new groups - appendChild moves a node - while the groups are detached, so the page
    // takes one insertion; canvas mode lists the centroid indices per group and leaves the groups empty.
    function groupPoints() {
        const layer = element('typology-map-points');
        if (layer === null || !pointsBuilt) {
            return;
        }

        layer.replaceChildren();

        const groups = new Map();
        groupMembers = new Map();
        for (let i = 0; i < centroids.length; i++) {
            const groupKey = groupKeyOf(i);
            let group = groups.get(groupKey);
            if (group === undefined) {
                group = createGroup(groupKey);
                groups.set(groupKey, group);
                groupMembers.set(groupKey, []);
            }

            if (canvasMode) {
                groupMembers.get(groupKey).push(i);
            } else {
                group.appendChild(circles[i]);
            }
        }

        const fragment = document.createDocumentFragment();
        const unclassified = groups.get(null);
        if (unclassified !== undefined) {
            fragment.appendChild(unclassified);
        }
        const groupKeys = [];
        groups.forEach(function (group, groupKey) {
            if (groupKey !== null) {
                groupKeys.push(groupKey);
            }
        });
        groupKeys.sort();
        for (let i = 0; i < groupKeys.length; i++) {
            fragment.appendChild(groups.get(groupKeys[i]));
        }
        layer.replaceChildren(fragment);

        applyDimming();
        renderMarker();
        requestDraw();
    }

    // A uniform grid over the projected positions in compressed rows: cell c holds items[starts[c]] up to
    // items[starts[c + 1]]. Built with the projection, so a hit test reads a handful of cells.
    function buildGrid() {
        const columns = Math.ceil(canvasSize / gridCellSize);
        const cellCount = columns * columns;
        const count = projectedX.length;
        const cells = new Int32Array(count);
        const starts = new Int32Array(cellCount + 1);
        for (let i = 0; i < count; i++) {
            const column = Math.min(columns - 1, Math.max(0, Math.floor(projectedX[i] / gridCellSize)));
            const row = Math.min(columns - 1, Math.max(0, Math.floor(projectedY[i] / gridCellSize)));
            cells[i] = row * columns + column;
            starts[cells[i] + 1]++;
        }
        for (let c = 0; c < cellCount; c++) {
            starts[c + 1] += starts[c];
        }
        const fill = starts.slice(0, cellCount);
        const items = new Int32Array(count);
        for (let i = 0; i < count; i++) {
            items[fill[cells[i]]++] = i;
        }
        grid = { columns: columns, starts: starts, items: items };
    }

    // The canvas sits beneath the SVG in the viewport, so the outline, the marker and every pointer event stay
    // the SVG's; the viewport's .typology-viewport-canvas class shows it.
    function ensureCanvas() {
        if (canvas !== null) {
            return;
        }
        const viewport = element('typology-viewport');
        const svg = element('typology-map');
        if (viewport === null || svg === null) {
            return;
        }
        canvas = document.createElement('canvas');
        canvas.className = 'typology-map-canvas';
        canvas.setAttribute('aria-hidden', 'true');
        viewport.insertBefore(canvas, svg);
    }

    function pointScale() {
        return 1 / Math.sqrt(canvasSize / view.size);
    }

    // The opacity of a dimmed group, read from the stylesheet once through a probe without the transition, so
    // the canvas matches the SVG without restating the CSS value.
    function dimmedOpacityValue() {
        if (dimmedOpacity === null) {
            const layer = element('typology-map-points');
            dimmedOpacity = 0.15;
            if (layer !== null) {
                const probe = document.createElementNS(svgNamespace, 'g');
                probe.setAttribute('class', 'typology-point-group typology-point-dimmed');
                probe.style.transition = 'none';
                layer.appendChild(probe);
                const value = parseFloat(getComputedStyle(probe).opacity);
                probe.remove();
                if (isFinite(value)) {
                    dimmedOpacity = value;
                }
            }
        }
        return dimmedOpacity;
    }

    // Coalesces the redraws of a wheel burst, a pan or a resize into one per frame.
    function requestDraw() {
        if (canvas === null || drawRequested) {
            return;
        }
        drawRequested = true;
        requestAnimationFrame(draw);
    }

    // Draws the canvas layer: every group in the SVG's order, in its computed fill, at full or dimmed opacity by
    // its class, each group's dots one path and one fill. The viewBox-to-pixel mapping is the SVG's own screen
    // transform, so the letterboxing of preserveAspectRatio and every zoom and pan are followed exactly. In SVG
    // mode the canvas is only cleared.
    function draw() {
        drawRequested = false;
        if (canvas === null) {
            return;
        }

        const width = canvas.clientWidth;
        const height = canvas.clientHeight;
        const ratio = window.devicePixelRatio || 1;
        const width_Backing = Math.max(1, Math.round(width * ratio));
        const height_Backing = Math.max(1, Math.round(height * ratio));
        if (canvas.width !== width_Backing || canvas.height !== height_Backing) {
            canvas.width = width_Backing;
            canvas.height = height_Backing;
        }

        const context = canvas.getContext('2d');
        context.setTransform(1, 0, 0, 1, 0, 0);
        context.clearRect(0, 0, canvas.width, canvas.height);

        const svg = element('typology-map');
        const layer = element('typology-map-points');
        if (!canvasMode || !pointsBuilt || svg === null || layer === null) {
            return;
        }
        const matrix = svg.getScreenCTM();
        if (matrix === null) {
            return;
        }

        const bounds = canvas.getBoundingClientRect();
        const offsetX = matrix.e - bounds.left;
        const offsetY = matrix.f - bounds.top;
        const radius = Math.max(0.5, pointRadius * pointScale() * matrix.a);
        const dimmed = dimmedOpacityValue();

        context.setTransform(ratio, 0, 0, ratio, 0, 0);
        const groups = layer.children;
        for (let g = 0; g < groups.length; g++) {
            const group = groups[g];
            const members = groupMembers.get(group.hasAttribute('data-path') ? group.getAttribute('data-path') : null);
            if (members === undefined || members.length === 0) {
                continue;
            }

            context.fillStyle = getComputedStyle(group).fill;
            context.globalAlpha = group.classList.contains('typology-point-dimmed') ? dimmed : 1;
            context.beginPath();
            for (let m = 0; m < members.length; m++) {
                const i = members[m];
                const x = matrix.a * projectedX[i] + offsetX;
                const y = matrix.d * projectedY[i] + offsetY;
                if (x < -radius || y < -radius || x > width + radius || y > height + radius) {
                    continue;
                }
                context.moveTo(x + radius, y);
                context.arc(x, y, radius, 0, 2 * Math.PI);
            }
            context.fill();
        }
        context.globalAlpha = 1;
    }

    // The centroid index nearest the client position within the drawn radius plus hitTolerance pixels, or -1.
    function indexAt(clientX, clientY) {
        const svg = element('typology-map');
        if (!canvasMode || grid === null || svg === null) {
            return -1;
        }
        const matrix = svg.getScreenCTM();
        const point = userPoint(svg, clientX, clientY);
        if (matrix === null || point === null || matrix.a <= 0) {
            return -1;
        }

        const reach = (pointRadius * pointScale() * matrix.a + hitTolerance) / matrix.a;
        const columns = grid.columns;
        const column_Min = Math.max(0, Math.floor((point.x - reach) / gridCellSize));
        const column_Max = Math.min(columns - 1, Math.floor((point.x + reach) / gridCellSize));
        const row_Min = Math.max(0, Math.floor((point.y - reach) / gridCellSize));
        const row_Max = Math.min(columns - 1, Math.floor((point.y + reach) / gridCellSize));

        let result = -1;
        let distance_Best = reach * reach;
        for (let row = row_Min; row <= row_Max; row++) {
            for (let column = column_Min; column <= column_Max; column++) {
                const cell = row * columns + column;
                for (let k = grid.starts[cell]; k < grid.starts[cell + 1]; k++) {
                    const i = grid.items[k];
                    const deltaX = projectedX[i] - point.x;
                    const deltaY = projectedY[i] - point.y;
                    const distance = deltaX * deltaX + deltaY * deltaY;
                    if (distance <= distance_Best) {
                        distance_Best = distance;
                        result = i;
                    }
                }
            }
        }
        return result;
    }

    // The centroid under the pointer: the circle the event hit in SVG mode, the grid's nearest dot in canvas mode.
    function centroidOfEvent(event) {
        if (centroids === null) {
            return null;
        }
        if (canvasMode) {
            const index = indexAt(event.clientX, event.clientY);
            return index === -1 ? null : centroids[index];
        }
        const circle = event.target !== null && event.target.closest !== undefined ? event.target.closest('.typology-point') : null;
        if (circle === null) {
            return null;
        }
        const centroid = centroids[parseInt(circle.getAttribute('data-i'), 10)];
        return centroid === undefined ? null : centroid;
    }

    // ----- selection -----

    function applyDimming() {
        const layer = element('typology-map-points');
        if (layer === null) {
            return;
        }
        const groups = layer.children;
        for (let i = 0; i < groups.length; i++) {
            const group = groups[i];
            const key = group.getAttribute('data-path');
            const dimmed = selectedPath !== null && (key === null || !common.isUnder(key, selectedPath));
            group.classList.toggle('typology-point-dimmed', dimmed);
        }
        requestDraw();
    }

    function setSelection(path) {
        // The root ('') is the whole area: dimming exists to contrast a bucket against the rest, and the root
        // has no rest, so a root selection dims nothing - the neutral group included (issue #28).
        const key = path === null || path === undefined ? null : pathKey(path);
        selectedPath = key === '' ? null : key;
        applyDimming();
    }

    function renderMarker() {
        const layer = element('typology-map-marker');
        if (layer === null) {
            return;
        }
        const entry = selectedBuildingKey === null ? null : pointOfKey(selectedBuildingKey);
        if (entry === null) {
            layer.replaceChildren();
            return;
        }
        const point = project(entry.x, entry.y);
        const ring = document.createElementNS(svgNamespace, 'circle');
        ring.setAttribute('class', 'typology-point-marker');
        ring.setAttribute('cx', point.x.toFixed(2));
        ring.setAttribute('cy', point.y.toFixed(2));
        ring.setAttribute('r', markerRadius);
        layer.replaceChildren(ring);
    }

    function pointOfKey(key) {
        if (centroids === null) {
            return null;
        }
        const centroid = centroidsByKey.get(key);
        if (centroid === undefined) {
            return null;
        }
        const building = buildingOf(centroid.reference, centroid.countyId);
        return { x: centroid.x, y: centroid.y, path: building === undefined ? null : building.path };
    }

    // Plan coordinates and typology path of a building, or null when the area carries no such dot.
    function pointOf(reference, countyId) {
        return pointOfKey(buildingKey(reference, countyId));
    }

    function selectBuilding(reference, countyId) {
        const key = buildingKey(reference, countyId);
        selectedBuildingKey = centroidsByKey.has(key) ? key : null;
        renderMarker();
    }

    function clearBuilding() {
        selectedBuildingKey = null;
        renderMarker();
    }

    // ----- viewport: wheel zoom, drag pan, double-click fit -----

    function applyView() {
        const svg = element('typology-map');
        if (svg === null) {
            return;
        }
        svg.setAttribute('viewBox', view.x.toFixed(3) + ' ' + view.y.toFixed(3) + ' ' + view.size.toFixed(3) + ' ' + view.size.toFixed(3));
        // The radius shrinks with the square root of the zoom: a dot grows on screen as the map expands, but far
        // slower than the map, so a town centre separates into buildings instead of one blob.
        const zoom = canvasSize / view.size;
        svg.style.setProperty('--typology-point-scale', (1 / Math.sqrt(zoom)).toFixed(4));
        requestDraw();
    }

    function resetView() {
        view = { x: 0, y: 0, size: canvasSize };
        applyView();
    }

    // Keeps the view inside the canvas, so the outline can never be panned out of sight.
    function clampView() {
        view.size = Math.min(canvasSize, Math.max(canvasSize / zoomMaximum, view.size));
        view.x = Math.min(canvasSize - view.size, Math.max(0, view.x));
        view.y = Math.min(canvasSize - view.size, Math.max(0, view.y));
    }

    // The point under a client position in viewBox units, through the SVG's own screen transform (which
    // includes the letterboxing of preserveAspectRatio).
    function userPoint(svg, clientX, clientY) {
        const matrix = svg.getScreenCTM();
        if (matrix === null) {
            return null;
        }
        return new DOMPoint(clientX, clientY).matrixTransform(matrix.inverse());
    }

    // Scales the view by the factor about the given client position: the map point under the pointer stays
    // under it, so the wheel zooms into what is looked at.
    function zoomAt(svg, factor, clientX, clientY) {
        const anchor = userPoint(svg, clientX, clientY);
        if (anchor === null) {
            return;
        }
        const size = Math.min(canvasSize, Math.max(canvasSize / zoomMaximum, view.size / factor));
        const ratio = size / view.size;
        view = {
            x: anchor.x - (anchor.x - view.x) * ratio,
            y: anchor.y - (anchor.y - view.y) * ratio,
            size: size
        };
        clampView();
        applyView();
    }

    function setupViewport() {
        const svg = element('typology-map');
        if (svg === null) {
            return;
        }

        svg.addEventListener('wheel', function (event) {
            event.preventDefault();
            zoomAt(svg, event.deltaY < 0 ? zoomStep : 1 / zoomStep, event.clientX, event.clientY);
        }, { passive: false });

        svg.addEventListener('dblclick', function (event) {
            event.preventDefault();
            resetView();
        });

        // Pan: the primary button dragged on the map. A press that never travels dragThreshold pixels stays a
        // click (on a dot, or on nothing); one that does pans, and the click that follows the release is
        // swallowed so lifting the pointer over a dot does not select it.
        let drag = null;
        svg.addEventListener('pointerdown', function (event) {
            if (event.button !== 0 || view.size >= canvasSize) {
                return;
            }
            drag = { pointerId: event.pointerId, startX: event.clientX, startY: event.clientY, view: { x: view.x, y: view.y, size: view.size }, panning: false };
            svg.setPointerCapture(event.pointerId);
        });

        svg.addEventListener('pointermove', function (event) {
            if (drag === null || event.pointerId !== drag.pointerId) {
                return;
            }
            const deltaX = event.clientX - drag.startX;
            const deltaY = event.clientY - drag.startY;
            if (!drag.panning) {
                if (Math.abs(deltaX) < dragThreshold && Math.abs(deltaY) < dragThreshold) {
                    return;
                }
                drag.panning = true;
                svg.classList.add('typology-map-panning');
            }
            const start = userPoint(svg, drag.startX, drag.startY);
            const current = userPoint(svg, event.clientX, event.clientY);
            if (start === null || current === null) {
                return;
            }
            view = { x: drag.view.x - (current.x - start.x), y: drag.view.y - (current.y - start.y), size: drag.view.size };
            clampView();
            applyView();
        });

        function endDrag(event) {
            if (drag === null || event.pointerId !== drag.pointerId) {
                return;
            }
            const panned = drag.panning;
            drag = null;
            svg.classList.remove('typology-map-panning');
            if (svg.hasPointerCapture(event.pointerId)) {
                svg.releasePointerCapture(event.pointerId);
            }
            if (panned) {
                suppressClick = true;
            }
        }

        svg.addEventListener('pointerup', endDrag);
        svg.addEventListener('pointercancel', endDrag);
    }

    // Set by a pan on release and consumed by the click that follows it.
    let suppressClick = false;

    // ----- hover label and clicks -----

    // The hover label of the 3D viewer: one positioned element in the viewport, shown while a dot is under
    // the pointer. The listeners sit on the SVG rather than on the dot layer, so the canvas mode - which has no
    // element per dot - resolves the pointer the same way (centroidOfEvent).
    function setupEvents() {
        const svg = element('typology-map');
        const viewport = element('typology-viewport');
        const label = element('typology-map-label');

        document.addEventListener(common.selectionEventName, function (event) {
            setSelection(event.detail !== null && event.detail !== undefined ? event.detail.path : null);
        });

        if (svg === null || viewport === null) {
            return;
        }

        svg.addEventListener('mousemove', function (event) {
            if (label === null) {
                return;
            }
            const centroid = svg.classList.contains('typology-map-panning') ? null : centroidOfEvent(event);
            if (centroid === null) {
                label.hidden = true;
                return;
            }
            const bounds = viewport.getBoundingClientRect();
            label.textContent = centroid.reference || '';
            label.style.left = (event.clientX - bounds.left + 12) + 'px';
            label.style.top = (event.clientY - bounds.top + 12) + 'px';
            label.hidden = false;
        });

        svg.addEventListener('mouseleave', function () {
            if (label !== null) {
                label.hidden = true;
            }
        });

        svg.addEventListener('click', function (event) {
            if (suppressClick) {
                suppressClick = false;
                return;
            }
            const centroid = centroidOfEvent(event);
            if (centroid === null) {
                return;
            }
            selectBuilding(centroid.reference, centroid.countyId);
            const entry = pointOf(centroid.reference, centroid.countyId);
            document.dispatchEvent(new CustomEvent(common.buildingSelectEventName, {
                detail: { reference: centroid.reference, countyId: centroid.countyId, path: entry === null ? null : entry.path }
            }));
        });

        // A panel drag or toggle resizes the SVG with no script; the canvas has to follow it.
        if (typeof ResizeObserver !== 'undefined') {
            new ResizeObserver(requestDraw).observe(svg);
        }
    }

    // ----- data in (issue #27): the view owns the fetches; the map only receives the data it draws -----

    // An empty centroid list is a real empty area for every type: a Subdivision is answered by its polygon
    // (DiGi.GIS.PostgreSQL#75), the others by their subdivision children.
    function statusAfterCentroids() {
        if (centroidsFailed) {
            showStatus('Building positions are unavailable.');
        } else if (centroids !== null && centroids.length === 0) {
            showStatus('The area has no buildings to draw.');
        }
    }

    // The outline draws on arrival; the dots draw once both the fit and the centroids are in. A failed
    // outline leaves the placeholder standing (there is no fit to draw dots into).
    function setOutline(outlines) {
        if (renderOutline(outlines)) {
            hideStatus();
            statusAfterCentroids();
            // A new fit moves every dot, so the layer is built again.
            buildPoints();
        } else {
            showStatus('The area outline is unavailable.');
        }
    }

    // A failed or missing centroid fetch arrives as null: the unavailable line stands until a solve can
    // colour the dots.
    function setCentroids(items) {
        if (!Array.isArray(items)) {
            centroidsFailed = true;
            centroids = null;
        } else {
            centroids = items;
            centroidsByKey = new Map();
            for (let i = 0; i < items.length; i++) {
                centroidsByKey.set(buildingKey(items[i].reference, items[i].countyId), items[i]);
            }
        }
        pointsBuilt = false;
        if (scaleParameters !== null) {
            statusAfterCentroids();
            buildPoints();
        }
    }

    // The solve DTO: indexes the tree by path and the buildings by (reference, countyId) - and by reference
    // alone where that is unambiguous - then re-groups the dots in their colours if they are already drawn;
    // otherwise they draw coloured on arrival.
    function render(model) {
        nodesByKey = new Map();
        buildingsByKey = new Map();
        buildingsByReference = new Map();
        if (model !== null && model !== undefined) {
            nodesByKey = common.indexNodes(model.root);
            const buildings = Array.isArray(model.buildings) ? model.buildings : [];
            const ambiguous = new Set();
            for (let i = 0; i < buildings.length; i++) {
                const building = buildings[i];
                const entry = { path: building.path, pathKey: pathKey(building.path) };
                buildingsByKey.set(buildingKey(building.reference, building.countyId), entry);
                const reference = String(building.reference);
                if (buildingsByReference.has(reference)) {
                    ambiguous.add(reference);
                } else {
                    buildingsByReference.set(reference, entry);
                }
            }
            ambiguous.forEach(function (reference) {
                buildingsByReference.delete(reference);
            });
        }

        // The dots exist already: a solve only moves them between groups (issue #52).
        if (pointsBuilt) {
            groupPoints();
        } else {
            buildPoints();
        }
    }

    setupEvents();
    setupViewport();

    return {
        setOutline: setOutline,
        resetView: resetView,
        setCentroids: setCentroids,
        render: render,
        setSelection: setSelection,
        selectBuilding: selectBuilding,
        clearBuilding: clearBuilding,
        pointOf: pointOf
    };
})();
