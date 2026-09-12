/**
 * Typology definition page.
 *
 * #14 ships the Available Columns section; #15 Selected Columns, #16 Column Properties,
 * #17 Import/Export and #18 the Load modal follow. One object rather than loose globals:
 * names such as import() or load() are too general to own at window scope (same reasoning
 * as user.js).
 */
const digiTypology = (function () {
    'use strict';

    const state = {
        availableColumns: []
    };

    function baseUrl() {
        const base = window.AppBaseUrl || '/';
        return base.endsWith('/') ? base.slice(0, -1) : base;
    }

    function loadAvailableColumns() {
        const container = document.querySelector('#typology-available-columns .gis-column-list');
        if (container === null) {
            return;
        }

        showLoader(container);

        fetch(`${baseUrl()}/typology/columns`)
            .then(function (response) {
                if (response.status === 204 || !response.ok) {
                    showEmptyState(container);
                    return null;
                }
                return response.json();
            })
            .then(function (columns) {
                if (columns === null) {
                    return;
                }
                state.availableColumns = columns;
                renderColumns(container, columns, '');
            })
            .catch(function () {
                showEmptyState(container);
            });
    }

    function renderColumns(container, columns, filter) {
        const filterLower = filter.toLowerCase();
        const visible = filterLower === ''
            ? columns
            : columns.filter(function (column) {
                return (column.name || '').toLowerCase().indexOf(filterLower) !== -1;
            });

        if (visible.length === 0) {
            showEmptyState(container);
            return;
        }

        container.innerHTML = visible.map(function (column) {
            const tooltipParts = [column.category, column.description].filter(Boolean);
            const title = tooltipParts.length > 0 ? escapeHtml(tooltipParts.join(' — ')) : '';
            return '<div class="gis-column-item"' + (title !== '' ? ' title="' + title + '"' : '') + '>' +
                   escapeHtml(column.name || '') + '</div>';
        }).join('');
    }

    function showLoader(container) {
        container.innerHTML = '<div class="gis-loader"></div><p class="gis-loader-text">Loading columns…</p>';
    }

    function showEmptyState(container) {
        container.innerHTML = '<div class="gis-empty-state">No columns available.</div>';
    }

    function escapeHtml(text) {
        return String(text)
            .replace(/&/g, '&amp;')
            .replace(/</g, '&lt;')
            .replace(/>/g, '&gt;')
            .replace(/"/g, '&quot;')
            .replace(/'/g, '&#39;');
    }

    function setupFilter() {
        const filterInput = document.getElementById('typology-column-filter');
        if (filterInput === null) {
            return;
        }
        filterInput.addEventListener('input', function () {
            const container = document.querySelector('#typology-available-columns .gis-column-list');
            if (container !== null && state.availableColumns.length > 0) {
                renderColumns(container, state.availableColumns, filterInput.value);
            }
        });
    }

    function init() {
        setupFilter();
        loadAvailableColumns();
    }

    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', init);
    } else {
        init();
    }

    return {
        state: state
    };
})();
