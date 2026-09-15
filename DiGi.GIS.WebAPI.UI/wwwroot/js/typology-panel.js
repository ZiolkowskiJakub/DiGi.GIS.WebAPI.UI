/**
 * Typology area view - left panel (issue #24): the solved typology tree with calculated counts over a
 * hand-rolled SVG pie of the current selection's children.
 *
 * A pure renderer of the POST /typology/buildings DTO ({ root, buildings }): it draws the tree, holds
 * the selected node, redraws the pie on every selection change and reports the selection outward -
 * through the callback the page registers and through a 'typology:selectionchange' event on document,
 * so the centre viewport (#25) and the right panel (#26) can follow it without a reference to this
 * module. The page state itself belongs to the orchestration sub-issue (#27).
 *
 * One object rather than loose globals, as typology.js and user.js: names such as render() or select()
 * are too general to own at window scope. Classic script, no module imports.
 */
const digiTypologyPanel = (function () {
    'use strict';

    // Default bucket colours (the definition page's palette, typology.js) for a node whose rule maps no
    // appearance: the DTO's color is then null, and every node still needs a swatch and a slice. Indexed
    // by the node's last path index so siblings never share a fallback and the choice is deterministic
    // across the tree, the pie and the map dots.
    const palette = [
        '#1f77b4', '#ff7f0e', '#2ca02c', '#d62728', '#9467bd', '#8c564b',
        '#e377c2', '#7f7f7f', '#bcbd22', '#17becf', '#393b79', '#e7ba52'
    ];

    // The remainder slice: what the selected node counts but none of its children does - rows the solver
    // dropped at the next level. Neutral, so it never competes with a bucket colour.
    const remainderColor = '#9e9e9e';
    const remainderName = 'Not classified at the next level';

    const rootName = 'Whole area';
    const selectionEventName = 'typology:selectionchange';

    // Pie geometry: a 200x200 viewBox scales with any panel width the shell allows.
    const pieSize = 200;
    const pieRadius = 80;

    let root = null;
    let nodesByKey = new Map();
    let selectedKey = null;
    let selectionCallback = null;

    // ----- helpers -----

    function escapeHtml(text) {
        return String(text)
            .replace(/&/g, '&amp;')
            .replace(/</g, '&lt;')
            .replace(/>/g, '&gt;')
            .replace(/"/g, '&quot;')
            .replace(/'/g, '&#39;');
    }

    function pathKey(path) {
        return Array.isArray(path) ? path.join('.') : '';
    }

    function nodeName(node) {
        if (node === null || node === undefined) {
            return '';
        }
        const path = Array.isArray(node.path) ? node.path : [];
        return path.length === 0 ? rootName : (node.name || '');
    }

    function nodeCount(node) {
        return node !== null && node !== undefined && typeof node.count === 'number' ? node.count : 0;
    }

    function nodeChildren(node) {
        return node !== null && node !== undefined && Array.isArray(node.children) ? node.children : [];
    }

    function formatCount(count) {
        return count.toLocaleString();
    }

    function formatPercent(count, total) {
        return (total > 0 ? (count / total * 100) : 0).toFixed(1) + ' %';
    }

    function colorOf(node) {
        if (node !== null && node !== undefined && typeof node.color === 'string' && node.color !== '') {
            return node.color;
        }
        const path = node !== null && node !== undefined && Array.isArray(node.path) ? node.path : [];
        const index = path.length === 0 ? 0 : path[path.length - 1];
        return palette[Math.abs(index) % palette.length];
    }

    function element(id) {
        return document.getElementById(id);
    }

    // ----- status -----

    // The single status line of the tree card: the solve's progress, an error, or an empty state. Showing
    // it hides the tree; rendering a tree hides it.
    function showStatus(text) {
        const status = element('typology-tree-status');
        if (status !== null) {
            status.textContent = text;
            status.hidden = false;
        }
        const tree = element('typology-tree');
        if (tree !== null) {
            tree.hidden = true;
        }
    }

    // ----- tree -----

    function indexNodes(node, map) {
        if (node === null || node === undefined) {
            return;
        }
        map.set(pathKey(node.path), node);
        const children = nodeChildren(node);
        for (let i = 0; i < children.length; i++) {
            indexNodes(children[i], map);
        }
    }

    // One row per node, its children in a role="group" sibling. Every row is in the tab order and takes
    // Enter/Space (the Load modal result-row pattern); the chevron is a real button so expand/collapse is
    // keyboard operable on its own, and it stops the click from selecting the row.
    function rowHtml(node, level) {
        const children = nodeChildren(node);
        const key = pathKey(node.path);
        const name = nodeName(node);
        const isRoot = key === '';
        const hasChildren = children.length > 0;

        let html =
            '<div class="typology-tree-row" role="treeitem" tabindex="0" aria-selected="false" aria-level="' + level + '"' +
            (hasChildren ? ' aria-expanded="true"' : '') +
            ' data-path="' + escapeHtml(key) + '"' +
            (node.description ? ' title="' + escapeHtml(node.description) + '"' : '') + '>';

        if (hasChildren) {
            html += '<button type="button" class="typology-tree-toggle" aria-label="Collapse ' + escapeHtml(name) + '" aria-expanded="true">' +
                '<svg xmlns="http://www.w3.org/2000/svg" width="12" height="12" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5" stroke-linecap="round" stroke-linejoin="round" aria-hidden="true"><path d="m9 18 6-6-6-6" /></svg>' +
                '</button>';
        } else {
            html += '<span class="typology-tree-toggle typology-tree-toggle-spacer" aria-hidden="true"></span>';
        }

        if (!isRoot) {
            html += '<span class="typology-tree-swatch" style="background:' + escapeHtml(colorOf(node)) + '" aria-hidden="true"></span>';
        }

        html += '<span class="typology-tree-name">' + escapeHtml(name) + '</span>' +
            '<span class="typology-tree-count">' + escapeHtml(formatCount(nodeCount(node))) + '</span>' +
            '</div>';

        if (hasChildren) {
            html += '<div role="group" class="typology-tree-group">';
            for (let i = 0; i < children.length; i++) {
                html += rowHtml(children[i], level + 1);
            }
            html += '</div>';
        }

        return html;
    }

    function render(viewModel) {
        root = viewModel !== null && viewModel !== undefined && viewModel.root ? viewModel.root : null;
        nodesByKey = new Map();
        selectedKey = null;

        const tree = element('typology-tree');
        const status = element('typology-tree-status');
        if (tree === null) {
            return;
        }

        if (root === null) {
            showStatus('The solve answered no typology.');
            renderPie(null);
            return;
        }

        indexNodes(root, nodesByKey);

        // One pass into a fragment, then one DOM insertion.
        const template = document.createElement('template');
        template.innerHTML = rowHtml(root, 1);
        const fragment = document.createDocumentFragment();
        fragment.appendChild(template.content);
        tree.replaceChildren(fragment);
        tree.hidden = false;
        if (status !== null) {
            status.hidden = true;
        }

        updateClearButton();
        renderPie(root);
    }

    function setExpanded(row, expanded) {
        const toggle = row.querySelector('.typology-tree-toggle');
        const group = row.nextElementSibling;
        if (toggle === null || toggle.tagName !== 'BUTTON' || group === null || group.getAttribute('role') !== 'group') {
            return;
        }
        const name = row.querySelector('.typology-tree-name');
        row.setAttribute('aria-expanded', expanded ? 'true' : 'false');
        toggle.setAttribute('aria-expanded', expanded ? 'true' : 'false');
        toggle.setAttribute('aria-label', (expanded ? 'Collapse ' : 'Expand ') + (name !== null ? name.textContent : ''));
        group.hidden = !expanded;
    }

    function updateClearButton() {
        const clearButton = element('typology-tree-clear');
        if (clearButton !== null) {
            clearButton.disabled = selectedKey === null;
        }
    }

    function selection() {
        if (selectedKey === null) {
            return null;
        }
        const node = nodesByKey.get(selectedKey);
        return node === undefined ? null : { path: node.path, node: node };
    }

    // The single place a selection changes: flips every row's state (as the Load modal's
    // setLoadSelection), refocuses the pie, and reports outward.
    function setSelection(key) {
        selectedKey = key;

        const tree = element('typology-tree');
        if (tree !== null) {
            const rows = tree.querySelectorAll('.typology-tree-row');
            for (let i = 0; i < rows.length; i++) {
                const selected = key !== null && rows[i].getAttribute('data-path') === key;
                rows[i].setAttribute('aria-selected', selected ? 'true' : 'false');
                rows[i].classList.toggle('typology-tree-row-selected', selected);
            }
        }

        updateClearButton();

        const current = selection();
        renderPie(current !== null ? current.node : root);

        const path = current !== null ? current.path : null;
        const node = current !== null ? current.node : null;
        if (typeof selectionCallback === 'function') {
            selectionCallback(path, node);
        }
        document.dispatchEvent(new CustomEvent(selectionEventName, { detail: { path: path, node: node } }));
    }

    function select(path) {
        const key = pathKey(path);
        if (!nodesByKey.has(key)) {
            return;
        }
        setSelection(key);
    }

    function clear() {
        if (selectedKey === null) {
            return;
        }
        setSelection(null);
    }

    // Row activation: selecting the selected row again clears, so every row is its own toggle.
    function activateRow(row) {
        const key = row.getAttribute('data-path');
        if (key === null) {
            return;
        }
        if (key === selectedKey) {
            clear();
        } else {
            setSelection(key);
        }
    }

    function setupEvents() {
        const tree = element('typology-tree');
        if (tree === null) {
            return;
        }

        tree.addEventListener('click', function (event) {
            const toggle = event.target.closest !== undefined ? event.target.closest('.typology-tree-toggle') : null;
            if (toggle !== null && toggle.tagName === 'BUTTON' && tree.contains(toggle)) {
                event.stopPropagation();
                const row = toggle.closest('.typology-tree-row');
                if (row !== null) {
                    setExpanded(row, row.getAttribute('aria-expanded') !== 'true');
                }
                return;
            }

            const row = event.target.closest !== undefined ? event.target.closest('.typology-tree-row') : null;
            if (row !== null && tree.contains(row)) {
                activateRow(row);
            }
        });

        tree.addEventListener('keydown', function (event) {
            // Escape is owned by the view (issue #27): one document-level chain closes the error modal,
            // then clears the selection. This tree only answers Enter and Space.
            if (event.key !== 'Enter' && event.key !== ' ') {
                return;
            }

            // A focused chevron button takes Enter/Space natively (it fires click); leave it alone.
            if (event.target.closest !== undefined && event.target.closest('.typology-tree-toggle') !== null) {
                return;
            }

            const row = event.target.closest !== undefined ? event.target.closest('.typology-tree-row') : null;
            if (row !== null && tree.contains(row)) {
                event.preventDefault(); // keep Space from scrolling the panel
                activateRow(row);
            }
        });

        // #typology-tree-clear is wired by the view (issue #27) so the selection has one clear owner;
        // this module only keeps the button's disabled state in sync (updateClearButton).
    }

    // ----- pie -----

    // The slices of a node: its children plus, when the node counts more than they do together, one
    // neutral remainder slice, so the slices always sum to the node's own count.
    function slicesOf(node) {
        const children = nodeChildren(node);
        const slices = [];
        let childrenTotal = 0;
        for (let i = 0; i < children.length; i++) {
            const count = nodeCount(children[i]);
            childrenTotal += count;
            slices.push({ name: nodeName(children[i]), count: count, color: colorOf(children[i]), path: children[i].path });
        }
        const remainder = nodeCount(node) - childrenTotal;
        if (children.length > 0 && remainder > 0) {
            slices.push({ name: remainderName, count: remainder, color: remainderColor, path: null });
        }
        return slices;
    }

    function polar(angle) {
        const radians = (angle - 90) * Math.PI / 180;
        const center = pieSize / 2;
        return {
            x: center + pieRadius * Math.cos(radians),
            y: center + pieRadius * Math.sin(radians)
        };
    }

    function slicePath(startAngle, endAngle) {
        const center = pieSize / 2;
        const start = polar(startAngle);
        const end = polar(endAngle);
        const largeArc = endAngle - startAngle > 180 ? 1 : 0;
        return 'M ' + center + ' ' + center +
            ' L ' + start.x.toFixed(3) + ' ' + start.y.toFixed(3) +
            ' A ' + pieRadius + ' ' + pieRadius + ' 0 ' + largeArc + ' 1 ' + end.x.toFixed(3) + ' ' + end.y.toFixed(3) +
            ' Z';
    }

    function renderPie(node) {
        const pie = element('typology-pie');
        const empty = element('typology-pie-empty');
        const legend = element('typology-pie-legend');
        const focus = element('typology-pie-focus');
        if (pie === null || empty === null || legend === null) {
            return;
        }

        if (focus !== null) {
            focus.textContent = node !== null && node !== undefined ? nodeName(node) : '';
        }

        pie.replaceChildren();
        legend.replaceChildren();

        if (node === null || node === undefined) {
            empty.textContent = '';
            empty.hidden = true;
            return;
        }

        const slices = slicesOf(node);
        const total = nodeCount(node);
        const drawn = slices.filter(function (slice) { return slice.count > 0; });

        if (drawn.length < 2) {
            empty.textContent = nodeChildren(node).length === 0
                ? nodeName(node) + ' is a leaf; there is nothing below it to chart.'
                : 'Select a node with two or more populated sub-typologies to chart its distribution.';
            empty.hidden = false;
            return;
        }

        empty.textContent = '';
        empty.hidden = true;

        let svg = '<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 ' + pieSize + ' ' + pieSize + '" role="img" aria-label="Distribution of ' + escapeHtml(formatCount(total)) + ' buildings across ' + drawn.length + ' slices">';
        let angle = 0;
        for (let i = 0; i < drawn.length; i++) {
            const slice = drawn[i];
            const sweep = slice.count / total * 360;
            // The last slice closes the circle exactly rather than where the accumulated floating-point
            // sweep lands, so no hairline gap opens at 12 o'clock; a full-circle slice cannot occur here
            // (two or more drawn slices).
            const endAngle = i === drawn.length - 1 ? 360 : Math.min(360, angle + sweep);
            svg += '<path class="typology-pie-slice" d="' + slicePath(angle, endAngle) + '" fill="' + escapeHtml(slice.color) + '"' +
                (slice.path !== null ? ' data-path="' + escapeHtml(pathKey(slice.path)) + '"' : '') + '>' +
                '<title>' + escapeHtml(slice.name) + ': ' + escapeHtml(formatCount(slice.count)) + ' (' + escapeHtml(formatPercent(slice.count, total)) + ')</title>' +
                '</path>';
            angle = endAngle;
        }
        svg += '</svg>';

        let items = '';
        for (let i = 0; i < slices.length; i++) {
            const slice = slices[i];
            items += '<li class="typology-pie-legend-item">' +
                '<span class="typology-tree-swatch" style="background:' + escapeHtml(slice.color) + '" aria-hidden="true"></span>' +
                '<span class="typology-pie-legend-name">' + escapeHtml(slice.name) + '</span>' +
                '<span class="typology-pie-legend-count">' + escapeHtml(formatCount(slice.count)) + '</span>' +
                '<span class="typology-pie-legend-percent">' + escapeHtml(formatPercent(slice.count, total)) + '</span>' +
                '</li>';
        }

        const pieTemplate = document.createElement('template');
        pieTemplate.innerHTML = svg;
        pie.appendChild(pieTemplate.content);

        const legendTemplate = document.createElement('template');
        legendTemplate.innerHTML = items;
        legend.appendChild(legendTemplate.content);
    }

    function setSelectionCallback(callback) {
        selectionCallback = typeof callback === 'function' ? callback : null;
    }

    setupEvents();

    return {
        render: render,
        select: select,
        clear: clear,
        selection: selection,
        colorOf: colorOf,
        setSelectionCallback: setSelectionCallback,
        showStatus: showStatus
    };
})();
