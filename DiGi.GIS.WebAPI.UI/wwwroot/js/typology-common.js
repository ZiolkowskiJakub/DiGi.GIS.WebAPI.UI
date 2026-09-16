/**
 * Typology area view - the helpers, constants and contracts the view's modules share (#21 review).
 *
 * One definition each of what typology-view.js, typology-panel.js, typology-map.js and typology-inspector.js
 * used to carry separately: the DOM and text helpers, the path and building keys the solve DTO is indexed by,
 * the node accessors and number formats, the bucket palette and its fallback, and the names of the two
 * document events the modules talk through. Loaded first (Views/Typology/View.cshtml); classic script, no
 * module imports, no DOM access at load.
 *
 * Contracts:
 * - 'typology:selectionchange' (detail { path, node }) - the left panel's tree selection; null path clears.
 * - 'typology:buildingselect' (detail { reference, countyId, path }) - a dot click on the map.
 * - A path key is the DTO path joined with '.', so the root's key is '' and a bucket's key prefixes its
 *   descendants'; a building key is reference + '|' + countyId, because a reference is unique only per
 *   county partition.
 */
const digiTypologyCommon = (function () {
    'use strict';

    // Default bucket colours (the definition page's palette, typology.js) for a node whose rule maps no
    // appearance: the DTO's color is then null, and every node still needs a swatch, a slice and a dot fill.
    // Indexed by the node's last path index so siblings never share a fallback and the choice is the same in
    // the tree, the pie and the map.
    const palette = [
        '#1f77b4', '#ff7f0e', '#2ca02c', '#d62728', '#9467bd', '#8c564b',
        '#e377c2', '#7f7f7f', '#bcbd22', '#17becf', '#393b79', '#e7ba52'
    ];

    // The root group's display name. 'Whole area' is the fallback; the view names it after the
    // definition's first column (setRootName), so the tree's top group reads as what its children
    // classify - e.g. 'Floor area', not 'Whole area'.
    let rootName = 'Whole area';
    const unclassifiedName = 'Not classified';
    // The neutral colour of a building the typology could not classify, and of the pie's remainder slice.
    const unclassifiedColor = '#9e9e9e';

    const selectionEventName = 'typology:selectionchange';
    const buildingSelectEventName = 'typology:buildingselect';

    function element(id) {
        return document.getElementById(id);
    }

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

    function buildingKey(reference, countyId) {
        return String(reference) + '|' + String(countyId);
    }

    // Names the root group after the definition's first column; a non-string or empty name keeps the
    // 'Whole area' fallback.
    function setRootName(name) {
        if (typeof name === 'string' && name.trim() !== '') {
            rootName = name.trim();
        }
    }

    // The root's key is empty and every key is under it; a bucket's key prefixes its descendants'. The dot
    // separator keeps '0.1' from matching '0.10'.
    function isUnder(key, parentKey) {
        return parentKey === '' || key === parentKey || key.indexOf(parentKey + '.') === 0;
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

    function colorOf(node) {
        if (node !== null && node !== undefined && typeof node.color === 'string' && node.color !== '') {
            return node.color;
        }
        const path = node !== null && node !== undefined && Array.isArray(node.path) ? node.path : [];
        const index = path.length === 0 ? 0 : path[path.length - 1];
        return palette[Math.abs(index) % palette.length];
    }

    function formatCount(count) {
        return count.toLocaleString();
    }

    function formatPercent(count, total) {
        return (total > 0 ? (count / total * 100) : 0).toFixed(1) + ' %';
    }

    // Every node of the DTO tree by its path key, the root under ''.
    function indexNodes(root) {
        const map = new Map();
        visit(root);
        return map;

        function visit(node) {
            if (node === null || node === undefined) {
                return;
            }
            map.set(pathKey(node.path), node);
            const children = nodeChildren(node);
            for (let i = 0; i < children.length; i++) {
                visit(children[i]);
            }
        }
    }

    return {
        setRootName: setRootName,
        unclassifiedName: unclassifiedName,
        unclassifiedColor: unclassifiedColor,
        selectionEventName: selectionEventName,
        buildingSelectEventName: buildingSelectEventName,
        element: element,
        escapeHtml: escapeHtml,
        pathKey: pathKey,
        buildingKey: buildingKey,
        isUnder: isUnder,
        nodeName: nodeName,
        nodeCount: nodeCount,
        nodeChildren: nodeChildren,
        colorOf: colorOf,
        formatCount: formatCount,
        formatPercent: formatPercent,
        indexNodes: indexNodes
    };
})();
