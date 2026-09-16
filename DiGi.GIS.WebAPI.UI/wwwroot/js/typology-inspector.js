/**
 * Typology area view - right panel (issue #26): the selected typology node's properties, a windowed grid
 * of its buildings with a text filter over the reference, and the info of the building picked from the
 * grid or the map, with links to the existing 2D details page and 3D viewer.
 *
 * A pure renderer of the solve DTO (POST /typology/buildings) and the definition the Load modal filed in
 * sessionStorage: no request is made per building, the id and county part come from the DTO and the
 * plan coordinates from the map's centroid lookup. The grid stays responsive at tens of thousands of rows
 * by windowing: a spacer keeps the scroll height of the whole filtered list, and only the rows of the
 * visible slice (plus an overscan) are in the DOM, re-rendered on scroll.
 *
 * Contracts: follows the left panel's 'typology:selectionchange' (scopes the grid, fills the inspector,
 * opens the panel; a null selection resets and closes it) and the map's 'typology:buildingselect' (a dot
 * click: selects the bucket in the tree when the building is outside the current selection, then the
 * row). It never dispatches 'typology:buildingselect' itself - the map is driven directly through
 * digiTypologyMap.selectBuilding/clearBuilding, so no event loop can form. A dot is matched to its DTO
 * entry by (reference, countyId), then by the reference alone when the DTO lists it once - the same
 * fallback the map applies. Shared helpers and event names come from typology-common.js, loaded first.
 * Classic script, no imports.
 */
const digiTypologyInspector = (function () {
    'use strict';

    const common = digiTypologyCommon;
    const element = common.element;
    const escapeHtml = common.escapeHtml;
    const pathKey = common.pathKey;
    const buildingKey = common.buildingKey;
    const isUnder = common.isUnder;
    const nodeName = common.nodeName;
    const nodeCount = common.nodeCount;
    const formatCount = common.formatCount;
    const formatPercent = common.formatPercent;

    // The row height of the windowed grid; must equal the .typology-grid-row height in gis-theme.css.
    const rowHeight = 26;
    // Rows rendered beyond each edge of the visible slice, so a scroll step never shows a blank band.
    const overscan = 10;
    const filterDebounceMs = 150;

    const unknownText = '–';

    let definition = null;
    let root = null;
    let nodesByKey = new Map();
    let buildings = [];
    let pathKeys = [];
    let referencesLower = [];
    let indexByKey = new Map();
    let indexByReference = new Map();

    let selectedKey = null;
    let scoped = [];
    let filtered = [];
    let filterTerm = '';
    let filterTimer = null;
    let rafHandle = null;
    let selectedBuildingIndex = null;
    let focusPosition = -1;

    // ----- helpers -----

    function baseUrl() {
        return (window.AppBaseUrl || '/').replace(/\/$/, '');
    }

    function formatMetre(value) {
        return typeof value === 'number' && isFinite(value) ? value.toFixed(2) : unknownText;
    }

    function setHidden(id, hidden) {
        const target = element(id);
        if (target !== null) {
            target.hidden = hidden;
        }
    }

    function setText(id, text) {
        const target = element(id);
        if (target !== null) {
            target.textContent = text;
        }
    }

    // The DTO index of a dot: by the full key first, then by the reference alone when the DTO lists it once.
    function indexOf(reference, countyId) {
        const index = indexByKey.get(buildingKey(reference, countyId));
        return index !== undefined ? index : indexByReference.get(String(reference));
    }

    function openPanel() {
        if (typeof digiTypologyLayout !== 'undefined' && digiTypologyLayout.isRightPanelCollapsed()) {
            digiTypologyLayout.setRightPanelCollapsed(false);
        }
    }

    function closePanel() {
        if (typeof digiTypologyLayout !== 'undefined' && !digiTypologyLayout.isRightPanelCollapsed()) {
            digiTypologyLayout.setRightPanelCollapsed(true);
        }
    }

    // ----- inspector card -----

    function fillInspector(node) {
        const path = Array.isArray(node.path) ? node.path : [];

        const swatch = element('typology-inspector-swatch');
        if (swatch !== null) {
            swatch.hidden = path.length === 0;
            swatch.style.backgroundColor = common.colorOf(node);
        }

        setText('typology-inspector-name', nodeName(node));

        // One item per ancestor from the root to the node; the level's column name goes in the title, as the
        // node names already read "column + rule".
        const breadcrumb = element('typology-inspector-breadcrumb');
        if (breadcrumb !== null) {
            const levels = definition !== null && Array.isArray(definition.levels) ? definition.levels : [];
            let html = '';
            for (let depth = 0; depth <= path.length; depth++) {
                const ancestor = nodesByKey.get(pathKey(path.slice(0, depth)));
                if (ancestor === undefined) {
                    continue;
                }
                const level = depth > 0 && depth - 1 < levels.length ? levels[depth - 1] : null;
                const title = level !== null && typeof level.name === 'string' ? ' title="' + escapeHtml(level.name) + '"' : '';
                const current = depth === path.length;
                if (html !== '') {
                    html += '<span class="typology-inspector-breadcrumb-separator" aria-hidden="true">&rsaquo;</span>';
                }
                html += '<span class="typology-inspector-breadcrumb-item' + (current ? ' typology-inspector-breadcrumb-current' : '') + '"' + title + (current ? ' aria-current="page"' : '') + '>' + escapeHtml(nodeName(ancestor)) + '</span>';
            }
            breadcrumb.innerHTML = html;
        }

        setText('typology-inspector-count', formatCount(nodeCount(node)));
        setText('typology-inspector-share', formatPercent(nodeCount(node), nodeCount(root)));

        const description = element('typology-inspector-description');
        if (description !== null) {
            const text = typeof node.description === 'string' ? node.description.trim() : '';
            description.textContent = text;
            description.hidden = text === '';
        }

        setHidden('typology-inspector-empty', true);
        setHidden('typology-inspector', false);
    }

    function clearInspector() {
        setHidden('typology-inspector', true);
        setHidden('typology-inspector-empty', false);
    }

    // ----- grid -----

    function scopeGrid(key) {
        scoped = [];
        if (key !== null) {
            for (let i = 0; i < pathKeys.length; i++) {
                if (isUnder(pathKeys[i], key)) {
                    scoped.push(i);
                }
            }
        }
        applyFilter();
    }

    function applyFilter() {
        if (filterTerm === '') {
            filtered = scoped;
        } else {
            filtered = [];
            for (let i = 0; i < scoped.length; i++) {
                if (referencesLower[scoped[i]].indexOf(filterTerm) !== -1) {
                    filtered.push(scoped[i]);
                }
            }
        }

        setText('typology-grid-count', selectedKey === null ? '' : formatCount(filtered.length) + ' of ' + formatCount(scoped.length));

        const empty = element('typology-grid-empty');
        if (empty !== null) {
            if (selectedKey === null) {
                empty.textContent = 'No typology selected.';
            } else if (scoped.length === 0) {
                empty.textContent = 'This typology lists no buildings.';
            } else if (filtered.length === 0) {
                empty.textContent = 'No buildings match the filter.';
            }
            empty.hidden = filtered.length !== 0;
        }

        const grid = element('typology-grid');
        const spacer = element('typology-grid-spacer');
        if (grid !== null && spacer !== null) {
            grid.hidden = filtered.length === 0;
            spacer.style.height = (filtered.length * rowHeight) + 'px';
            grid.scrollTop = 0;
        }

        focusPosition = selectedBuildingIndex === null ? -1 : filtered.indexOf(selectedBuildingIndex);
        renderSlice();
    }

    // Only the rows of the visible window (plus the overscan) exist in the DOM; the slice is translated to
    // the first rendered row's offset inside the spacer.
    function renderSlice() {
        const grid = element('typology-grid');
        const slice = element('typology-grid-slice');
        if (grid === null || slice === null) {
            return;
        }

        if (filtered.length === 0) {
            slice.innerHTML = '';
            slice.style.transform = '';
            grid.removeAttribute('aria-activedescendant');
            return;
        }

        const first = Math.max(0, Math.floor(grid.scrollTop / rowHeight) - overscan);
        const visible = Math.ceil(Math.max(grid.clientHeight, rowHeight) / rowHeight) + 2 * overscan;
        const last = Math.min(filtered.length, first + visible);

        let html = '';
        for (let position = first; position < last; position++) {
            const index = filtered[position];
            const building = buildings[index];
            const selected = index === selectedBuildingIndex;
            const focused = position === focusPosition;
            // Reference only: the county id is an internal partition key, kept in the DTO and in
            // buildingKey/details-link resolution but not shown (issue #31).
            html += '<div class="typology-grid-row' + (selected ? ' typology-grid-row-selected' : '') + (focused ? ' typology-grid-row-focus' : '') + '" role="option" id="typology-grid-row-' + index + '" data-index="' + index + '" aria-selected="' + (selected ? 'true' : 'false') + '">' +
                '<span class="typology-grid-reference">' + escapeHtml(building.reference) + '</span>' +
                '</div>';
        }

        slice.innerHTML = html;
        slice.style.transform = 'translateY(' + (first * rowHeight) + 'px)';

        if (focusPosition >= first && focusPosition < last) {
            grid.setAttribute('aria-activedescendant', 'typology-grid-row-' + filtered[focusPosition]);
        } else {
            grid.removeAttribute('aria-activedescendant');
        }
    }

    function scheduleSlice() {
        if (rafHandle !== null) {
            return;
        }
        rafHandle = window.requestAnimationFrame(function () {
            rafHandle = null;
            renderSlice();
        });
    }

    // Scrolls the grid so the row at the position is inside the window; the scroll event re-renders.
    function ensureVisible(position) {
        const grid = element('typology-grid');
        if (grid === null || position < 0) {
            return;
        }
        const top = position * rowHeight;
        if (top < grid.scrollTop) {
            grid.scrollTop = top;
        } else if (top + rowHeight > grid.scrollTop + grid.clientHeight) {
            grid.scrollTop = top + rowHeight - grid.clientHeight;
        }
    }

    function setFilterEnabled(enabled) {
        const filter = element('typology-grid-filter');
        if (filter !== null) {
            filter.disabled = !enabled;
            if (!enabled) {
                filter.value = '';
            }
        }
        if (!enabled) {
            filterTerm = '';
        }
    }

    // ----- building card -----

    // One row of the card's value list: the same typology-area-item shape the other rows use, so a
    // per-level typology list reads like the rest of the card.
    function typologyRow(label, value) {
        return '<div class="typology-area-item">' +
            '<dt class="typology-area-label typology-info-label">' + escapeHtml(label) + '</dt>' +
            '<dd class="typology-area-value">' + escapeHtml(value) + '</dd>' +
            '</div>';
    }

    // The card's typology block: one row per level of the building's path, the last (deepest) level last,
    // the level's column name as the label and the corresponding node's name as the value - the same
    // ancestor/level walk as fillInspector's breadcrumb. An unclassified building (no path) keeps the
    // single "Not classified" value (issue #31).
    function fillBuildingTypologyRows(path) {
        const container = element('typology-building-typology');
        if (container === null) {
            return;
        }

        const node = path === null ? undefined : nodesByKey.get(pathKey(path));
        if (node === undefined) {
            container.innerHTML = typologyRow('Typology', common.unclassifiedName);
            return;
        }

        const levels = definition !== null && Array.isArray(definition.levels) ? definition.levels : [];
        const buildingPath = Array.isArray(path) ? path : [];
        let html = '';
        for (let depth = 1; depth <= buildingPath.length; depth++) {
            const ancestor = nodesByKey.get(pathKey(buildingPath.slice(0, depth)));
            const level = depth - 1 < levels.length ? levels[depth - 1] : null;
            const label = level !== null && typeof level.name === 'string' ? level.name : 'Level ' + depth;
            const value = ancestor !== undefined ? nodeName(ancestor) : '';
            html += typologyRow(label, value);
        }
        container.innerHTML = html;
    }

    function fillBuildingCard(reference, countyId, id, path) {
        const point = typeof digiTypologyMap !== 'undefined' ? digiTypologyMap.pointOf(reference, countyId) : null;

        setText('typology-building-reference', reference);
        setText('typology-building-id', id > 0 ? String(id) : unknownText);
        setText('typology-building-county', countyId > 0 ? String(countyId) : unknownText);
        setText('typology-building-x', point !== null ? formatMetre(point.x) : unknownText);
        setText('typology-building-y', point !== null ? formatMetre(point.y) : unknownText);
        fillBuildingTypologyRows(path);

        // The details page resolves the county part from the coordinates when the id is not carried; with
        // neither, the page could not find the building, so the link is not offered.
        const details = element('typology-building-details');
        if (details !== null) {
            let href = null;
            if (countyId > 0) {
                href = baseUrl() + '/building2D/detailsbyreference?reference=' + encodeURIComponent(reference) + '&countyid=' + countyId;
            } else if (point !== null) {
                href = baseUrl() + '/building2D/detailsbyreference?reference=' + encodeURIComponent(reference) + '&x=' + point.x + '&y=' + point.y;
            }
            details.hidden = href === null;
            details.href = href === null ? '' : href;
        }

        const model = element('typology-building-model');
        if (model !== null) {
            model.hidden = !(id > 0);
            model.href = id > 0 ? baseUrl() + '/buildingmodel/buildingmodelbyid?id=' + id + (countyId > 0 ? '&countyid=' + countyId : '') : '';
        }

        setHidden('typology-building-empty', true);
        setHidden('typology-building', false);
    }

    function selectBuildingByIndex(index) {
        const building = buildings[index];
        if (building === undefined) {
            return;
        }

        selectedBuildingIndex = index;
        focusPosition = filtered.indexOf(index);
        renderSlice();
        fillBuildingCard(building.reference, building.countyId, building.id, building.path);

        if (typeof digiTypologyMap !== 'undefined') {
            digiTypologyMap.selectBuilding(building.reference, building.countyId);
        }
    }

    function selectBuilding(reference, countyId) {
        const index = indexOf(reference, countyId);
        if (index !== undefined) {
            selectBuildingByIndex(index);
        }
    }

    function clearBuilding() {
        selectedBuildingIndex = null;
        focusPosition = -1;
        setHidden('typology-building', true);
        setHidden('typology-building-empty', false);
        renderSlice();

        if (typeof digiTypologyMap !== 'undefined') {
            digiTypologyMap.clearBuilding();
        }
    }

    // ----- selection -----

    function applySelection(path) {
        if (path === null) {
            selectedKey = null;
            clearInspector();
            setFilterEnabled(false);
            scopeGrid(null);
            clearBuilding();
            closePanel();
            return;
        }

        const key = pathKey(path);
        const node = nodesByKey.get(key);
        if (node === undefined) {
            return;
        }

        selectedKey = key;
        fillInspector(node);
        setFilterEnabled(true);

        // A building outside the new scope leaves the card; one inside keeps its row and marker.
        if (selectedBuildingIndex !== null && !isUnder(pathKeys[selectedBuildingIndex], key)) {
            clearBuilding();
        }

        // Open before scoping: the window is sized from the grid's height, which is 0 while the panel is collapsed.
        openPanel();
        scopeGrid(key);
    }

    // A dot click on the map: the building's bucket becomes the tree selection when it is outside the
    // current one, so the grid scopes to it and the row can be highlighted; an unclassified dot only
    // fills the card.
    function onBuildingSelect(detail) {
        if (detail === null || detail === undefined) {
            return;
        }

        const index = indexOf(detail.reference, detail.countyId);
        if (index === undefined) {
            selectedBuildingIndex = null;
            focusPosition = -1;
            renderSlice();
            fillBuildingCard(detail.reference, detail.countyId, 0, null);
            openPanel();
            return;
        }

        if (selectedKey === null || !isUnder(pathKeys[index], selectedKey)) {
            if (typeof digiTypologyPanel !== 'undefined') {
                digiTypologyPanel.select(buildings[index].path);
            }
        }

        selectBuildingByIndex(index);

        // A dot matched by the reference alone was drawn under another county part than the DTO entry names:
        // the marker and the plan coordinates follow the dot that was clicked, not the entry's key.
        if (typeof digiTypologyMap !== 'undefined' && detail.countyId !== buildings[index].countyId) {
            digiTypologyMap.selectBuilding(detail.reference, detail.countyId);
            const point = digiTypologyMap.pointOf(detail.reference, detail.countyId);
            setText('typology-building-x', point !== null ? formatMetre(point.x) : unknownText);
            setText('typology-building-y', point !== null ? formatMetre(point.y) : unknownText);
        }

        ensureVisible(focusPosition);
        openPanel();
    }

    // ----- events -----

    function setupEvents() {
        document.addEventListener(common.selectionEventName, function (event) {
            applySelection(event.detail !== null && event.detail !== undefined ? event.detail.path : null);
        });

        document.addEventListener(common.buildingSelectEventName, function (event) {
            onBuildingSelect(event.detail);
        });

        const grid = element('typology-grid');
        if (grid !== null) {
            grid.addEventListener('scroll', scheduleSlice);

            grid.addEventListener('click', function (event) {
                const row = event.target.closest !== undefined ? event.target.closest('.typology-grid-row') : null;
                if (row === null) {
                    return;
                }
                selectBuildingByIndex(parseInt(row.getAttribute('data-index'), 10));
            });

            grid.addEventListener('keydown', function (event) {
                if (filtered.length === 0) {
                    return;
                }

                let position = focusPosition;
                switch (event.key) {
                    case 'ArrowDown':
                        position = Math.min(filtered.length - 1, position + 1);
                        break;
                    case 'ArrowUp':
                        position = Math.max(0, position - 1);
                        break;
                    case 'Home':
                        position = 0;
                        break;
                    case 'End':
                        position = filtered.length - 1;
                        break;
                    case 'Enter':
                    case ' ':
                        event.preventDefault();
                        if (position >= 0) {
                            selectBuildingByIndex(filtered[position]);
                        }
                        return;
                    case 'Escape':
                        // Claim Escape only while a building is selected (issue #27); otherwise let it
                        // bubble to the view's chain, which clears the typology selection.
                        if (selectedBuildingIndex !== null) {
                            event.preventDefault();
                            clearBuilding();
                        }
                        return;
                    default:
                        return;
                }

                event.preventDefault();
                focusPosition = position;
                ensureVisible(position);
                renderSlice();
            });
        }

        const filter = element('typology-grid-filter');
        if (filter !== null) {
            filter.addEventListener('input', function () {
                if (filterTimer !== null) {
                    window.clearTimeout(filterTimer);
                }
                filterTimer = window.setTimeout(function () {
                    filterTimer = null;
                    filterTerm = filter.value.trim().toLowerCase();
                    applyFilter();
                }, filterDebounceMs);
            });
        }

        // A panel drag or toggle changes the grid's height, so the window may need more rows.
        window.addEventListener('resize', scheduleSlice);
    }

    // ----- public -----

    function render(viewModel, definitionModel) {
        definition = definitionModel !== undefined ? definitionModel : null;
        root = viewModel !== null && viewModel !== undefined ? viewModel.root : null;
        buildings = viewModel !== null && viewModel !== undefined && Array.isArray(viewModel.buildings) ? viewModel.buildings : [];

        nodesByKey = common.indexNodes(root);

        pathKeys = new Array(buildings.length);
        referencesLower = new Array(buildings.length);
        indexByKey = new Map();
        indexByReference = new Map();
        const ambiguous = new Set();
        for (let i = 0; i < buildings.length; i++) {
            const reference = String(buildings[i].reference);
            pathKeys[i] = pathKey(buildings[i].path);
            referencesLower[i] = reference.toLowerCase();
            indexByKey.set(buildingKey(buildings[i].reference, buildings[i].countyId), i);
            if (indexByReference.has(reference)) {
                ambiguous.add(reference);
            } else {
                indexByReference.set(reference, i);
            }
        }
        ambiguous.forEach(function (reference) {
            indexByReference.delete(reference);
        });

        selectedBuildingIndex = null;
        focusPosition = -1;
        setHidden('typology-building', true);
        setHidden('typology-building-empty', false);
        applySelection(null);
    }

    setupEvents();

    return {
        render: render,
        selectBuilding: selectBuilding,
        clearBuilding: clearBuilding
    };
})();
