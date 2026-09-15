/**
 * Typology area view - center panel (issue #25): the administrative outline with every building of the
 * area drawn as a dot coloured by its solved typology, dimmed outside the tree selection.
 *
 * The 2D stack of Views/AdministrativeAreal2D (administrative.js): one SVG with a fixed 500x500 viewBox,
 * fitted once from the outline's bounding box (y flipped), scaled by the browser through
 * preserveAspectRatio - so a panel drag, a panel toggle or the header/footer collapse refits the map with
 * no script. Buildings are dots only, never polygons: positions come from the UI proxy of the area-scoped
 * centroid endpoint (DiGi.GIS.WebAPI#34), joined with the solve DTO by (reference, countyId) because a
 * reference is unique only per county partition.
 *
 * Dots are grouped in one <g> per typology path, plus one neutral group for buildings the typology could
 * not classify (or every building, while no definition is solved). Dimming toggles a class on the groups
 * - a class toggle per bucket rather than a style write per point - and the selected building is marked
 * by a ring in a top layer instead of reparenting a dot, so it stays emphasised and on top whatever the
 * dimming does. The selection follows the left panel's 'typology:selectionchange' event; a dot click
 * reports 'typology:buildingselect' on document for the right panel (#26). Classic script, no imports.
 */
const digiTypologyMap = (function () {
    'use strict';

    const svgNamespace = 'http://www.w3.org/2000/svg';

    // The fit of administrative.js: a 500-unit canvas, the outline scaled into 450 with a 25 margin.
    const canvasSize = 500;
    const padding = 25;
    // Small on purpose: a county fits ~40 km into the canvas, so one unit is tens of metres and dots of a
    // town centre overlap whatever the radius; the browser scales them with the panel.
    const pointRadius = 0.7;
    const markerRadius = 4;

    // The panel's neutral colour, so an unclassified dot reads as its remainder slice.
    const unclassifiedColor = '#9e9e9e';

    const selectionEventName = 'typology:selectionchange';
    const buildingSelectEventName = 'typology:buildingselect';

    let scaleParameters = null;
    let centroids = null;
    let centroidsByKey = new Map();
    let centroidsFailed = false;
    let viewModel = null;
    let nodesByKey = new Map();
    let buildingsByKey = new Map();
    let selectedPath = null;
    let selectedBuildingKey = null;

    // ----- helpers -----

    function element(id) {
        return document.getElementById(id);
    }

    function baseUrl() {
        return (window.AppBaseUrl || '/').replace(/\/$/, '');
    }

    function pathKey(path) {
        return Array.isArray(path) ? path.join('.') : '';
    }

    function buildingKey(reference, countyId) {
        return String(reference) + '|' + String(countyId);
    }

    function isUnder(key, selectedKey) {
        return key === selectedKey || key.indexOf(selectedKey + '.') === 0;
    }

    function project(x, y) {
        return {
            x: (x - scaleParameters.minX) * scaleParameters.scale + padding,
            y: canvasSize - ((y - scaleParameters.minY) * scaleParameters.scale + padding)
        };
    }

    function colorOfPath(key) {
        const node = nodesByKey.get(key);
        if (node === undefined) {
            return unclassifiedColor;
        }
        if (typeof digiTypologyPanel !== 'undefined' && typeof digiTypologyPanel.colorOf === 'function') {
            return digiTypologyPanel.colorOf(node);
        }
        return typeof node.color === 'string' && node.color !== '' ? node.color : unclassifiedColor;
    }

    function indexNodes(node) {
        if (node === null || node === undefined) {
            return;
        }
        nodesByKey.set(pathKey(node.path), node);
        const children = Array.isArray(node.children) ? node.children : [];
        for (let i = 0; i < children.length; i++) {
            indexNodes(children[i]);
        }
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

    // Draws every centroid once the fit and the centroids are both known; the DTO is optional. One pass:
    // centroids bucketed by the path their assignment names (or the neutral bucket), one <g> per bucket
    // with its fill, the circles inside, all into a fragment and one insertion.
    function renderPoints() {
        const layer = element('typology-map-points');
        if (layer === null || scaleParameters === null || centroids === null) {
            return;
        }

        const groups = new Map();
        for (let i = 0; i < centroids.length; i++) {
            const centroid = centroids[i];
            const key = buildingKey(centroid.reference, centroid.countyId);
            const building = buildingsByKey.get(key);
            const groupKey = building === undefined ? null : building.pathKey;
            let group = groups.get(groupKey);
            if (group === undefined) {
                group = document.createElementNS(svgNamespace, 'g');
                group.setAttribute('class', groupKey === null ? 'typology-point-group typology-point-unclassified' : 'typology-point-group');
                if (groupKey !== null) {
                    group.setAttribute('data-path', groupKey);
                    group.setAttribute('fill', colorOfPath(groupKey));
                }
                groups.set(groupKey, group);
            }

            const point = project(centroid.x, centroid.y);
            const circle = document.createElementNS(svgNamespace, 'circle');
            circle.setAttribute('class', 'typology-point');
            circle.setAttribute('cx', point.x.toFixed(2));
            circle.setAttribute('cy', point.y.toFixed(2));
            circle.setAttribute('r', pointRadius);
            circle.setAttribute('data-reference', centroid.reference);
            circle.setAttribute('data-county', centroid.countyId);
            group.appendChild(circle);
        }

        // The neutral group first so classified dots paint over it, then the buckets in path order so the
        // paint order (which colour wins where dots overlap) does not depend on the centroid row order.
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
            const dimmed = selectedPath !== null && (key === null || !isUnder(key, selectedPath));
            group.classList.toggle('typology-point-dimmed', dimmed);
        }
    }

    function setSelection(path) {
        selectedPath = path === null || path === undefined ? null : pathKey(path);
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
        const building = buildingsByKey.get(key);
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

    // ----- hover label -----

    // The hover label of the 3D viewer: one positioned element in the viewport, shown while a dot is under
    // the pointer. One delegated listener pair on the layer rather than one per dot.
    function setupEvents() {
        const layer = element('typology-map-points');
        const viewport = element('typology-viewport');
        const label = element('typology-map-label');
        if (layer === null || viewport === null) {
            return;
        }

        layer.addEventListener('mouseover', function (event) {
            const circle = event.target.closest !== undefined ? event.target.closest('.typology-point') : null;
            if (circle === null || label === null) {
                return;
            }
            label.textContent = circle.getAttribute('data-reference') || '';
            label.hidden = false;
        });

        layer.addEventListener('mousemove', function (event) {
            if (label === null || label.hidden) {
                return;
            }
            const bounds = viewport.getBoundingClientRect();
            label.style.left = (event.clientX - bounds.left + 12) + 'px';
            label.style.top = (event.clientY - bounds.top + 12) + 'px';
        });

        layer.addEventListener('mouseout', function (event) {
            const circle = event.target.closest !== undefined ? event.target.closest('.typology-point') : null;
            if (circle !== null && label !== null) {
                label.hidden = true;
            }
        });

        layer.addEventListener('click', function (event) {
            const circle = event.target.closest !== undefined ? event.target.closest('.typology-point') : null;
            if (circle === null) {
                return;
            }
            const reference = circle.getAttribute('data-reference');
            const countyId = parseInt(circle.getAttribute('data-county'), 10);
            selectBuilding(reference, countyId);
            const entry = pointOf(reference, countyId);
            document.dispatchEvent(new CustomEvent(buildingSelectEventName, { detail: { reference: reference, countyId: countyId, path: entry === null ? null : entry.path } }));
        });

        document.addEventListener(selectionEventName, function (event) {
            setSelection(event.detail !== null && event.detail !== undefined ? event.detail.path : null);
        });
    }

    // ----- loading -----

    function statusAfterCentroids() {
        if (centroidsFailed) {
            showStatus('Building positions are unavailable.');
        } else if (centroids !== null && centroids.length === 0) {
            showStatus('The area has no buildings to draw.');
        }
    }

    // Two parallel fetches: the outline draws on arrival; the dots draw once both the fit and the centroids
    // are in. A failed outline leaves the placeholder standing (there is no fit to draw dots into).
    function load(areaId) {
        const id = parseInt(areaId, 10);
        if (!(id > 0)) {
            return;
        }

        fetch(baseUrl() + '/administrativeareal2D/svg/polygonsbyid?id=' + id)
            .then(function (response) { return response.ok ? response.json() : null; })
            .catch(function () { return null; })
            .then(function (outlines) {
                if (renderOutline(outlines)) {
                    hideStatus();
                    statusAfterCentroids();
                    renderPoints();
                } else {
                    showStatus('The area outline is unavailable.');
                }
            });

        fetch(baseUrl() + '/building2D/point2dsbyadministrativeareal2Did?administrativeareal2Did=' + id)
            .then(function (response) { return response.status === 200 ? response.json() : null; })
            .catch(function () { return null; })
            .then(function (items) {
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
                if (scaleParameters !== null) {
                    statusAfterCentroids();
                    renderPoints();
                }
            });
    }

    // The solve DTO: indexes the tree by path and the buildings by (reference, countyId), then redraws the
    // dots in their colours if the centroids are already in; otherwise they draw coloured on arrival.
    function render(model) {
        viewModel = model !== null && model !== undefined ? model : null;
        nodesByKey = new Map();
        buildingsByKey = new Map();
        if (viewModel !== null) {
            indexNodes(viewModel.root);
            const buildings = Array.isArray(viewModel.buildings) ? viewModel.buildings : [];
            for (let i = 0; i < buildings.length; i++) {
                const building = buildings[i];
                buildingsByKey.set(buildingKey(building.reference, building.countyId), { path: building.path, pathKey: pathKey(building.path) });
            }
        }
        renderPoints();
    }

    setupEvents();

    return {
        load: load,
        render: render,
        setSelection: setSelection,
        selectBuilding: selectBuilding,
        clearBuilding: clearBuilding,
        pointOf: pointOf
    };
})();
