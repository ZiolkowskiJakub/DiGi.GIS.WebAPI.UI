/**
 * Typology definition page.
 *
 * #14 ships the Available Columns section; #15 the Selected Columns chain — add by double-click or
 * drag & drop, remove by button or drag-back, reorder by Move Up/Down or drag, and single-click
 * selection; #16 Column Properties, #17 Import/Export and #18 the Load modal follow. One object
 * rather than loose globals: names such as import() or load() are too general to own at window
 * scope (same reasoning as user.js).
 */
const digiTypology = (function () {
    'use strict';

    // Custom dataTransfer types. dragover cannot read the payload — only the type list — so the two
    // drag kinds are told apart by type, and the uniqueId payload is read once, on drop.
    const dragType_Available = 'application/x-typology-available';
    const dragType_Level = 'application/x-typology-level';

    const state = {
        availableColumns: [],
        // The ordered grouping chain — its order is the grouping order the solver consumes
        // (ZiolkowskiJakub/DiGi.Gis#5). The level shape follows the DiGi.Typology.Visual document
        // model: plain { min, max } ranges, no per-level fallback appearance. ruleType, ranges and
        // uniqueValueColors start empty — #16 edits them, and #17 files the appearances into the
        // rule's TypologyAppearanceCollection (the client never spells the "[min, max]" key itself).
        levels: [],
        selectedIndex: -1
    };

    function baseUrl() {
        const base = window.AppBaseUrl || '/';
        return base.endsWith('/') ? base.slice(0, -1) : base;
    }

    // ----- data loading -----

    function loadAvailableColumns() {
        const container = availableColumnsContainer();
        if (container === null) {
            return;
        }

        showLoader(container);

        fetch(`${baseUrl()}/typology/columns`)
            .then(function (response) {
                if (response.status === 204 || !response.ok) {
                    showEmptyState(container, 'No columns available.');
                    return null;
                }
                return response.json();
            })
            .then(function (columns) {
                if (columns === null) {
                    return;
                }
                state.availableColumns = columns;
                renderAll();
            })
            .catch(function () {
                showEmptyState(container, 'No columns available.');
            });
    }

    // ----- state operations (each mutates state, then re-renders both lists) -----

    function columnByUniqueId(uniqueId) {
        return state.availableColumns.find(function (column) {
            return column.uniqueId === uniqueId;
        });
    }

    function levelByUniqueId(uniqueId) {
        return state.levels.find(function (level) {
            return level.uniqueId === uniqueId;
        });
    }

    function levelIndex(uniqueId) {
        return state.levels.indexOf(levelByUniqueId(uniqueId));
    }

    // Adds the column to the end of the chain and selects it — the column just added is the one the
    // user is about to configure. The duplicate guard mirrors the dimmed rows in Section 1.
    function addLevel(column) {
        if (column === undefined || column === null) {
            return;
        }
        if (levelByUniqueId(column.uniqueId) !== undefined) {
            return;
        }

        state.levels.push({
            uniqueId: column.uniqueId,
            name: column.name,
            dataType: column.dataType,
            isNumeric: column.isNumeric,
            ruleType: null,
            ranges: [],
            uniqueValueColors: []
        });
        state.selectedIndex = state.levels.length - 1;
        renderAll();
        focusLevelRow(column.uniqueId); // the just-added row is the one to configure next
    }

    function removeLevel(uniqueId) {
        const index = levelIndex(uniqueId);
        if (index === -1) {
            return;
        }
        state.levels.splice(index, 1);
        if (state.selectedIndex === index) {
            state.selectedIndex = -1;
        } else if (state.selectedIndex > index) {
            state.selectedIndex -= 1;
        }
        renderAll();
    }

    // Moves the level at sourceIndex to targetIndex, both in pre-removal coordinates — dropping onto
    // a row takes that row's slot. The selection follows the moved level.
    function moveLevel(sourceIndex, targetIndex) {
        if (sourceIndex === targetIndex) {
            return;
        }
        if (sourceIndex < 0 || sourceIndex >= state.levels.length || targetIndex < 0 || targetIndex >= state.levels.length) {
            return;
        }

        const level = state.levels.splice(sourceIndex, 1)[0];
        state.levels.splice(targetIndex, 0, level);

        if (state.selectedIndex === sourceIndex) {
            state.selectedIndex = targetIndex;
        } else if (sourceIndex < targetIndex && state.selectedIndex > sourceIndex && state.selectedIndex <= targetIndex) {
            state.selectedIndex -= 1;
        } else if (targetIndex < sourceIndex && state.selectedIndex >= targetIndex && state.selectedIndex < sourceIndex) {
            state.selectedIndex += 1;
        }
        renderAll();
    }

    // renderAll rebuilds both lists, which drops the DOM focus with the old nodes. Restore it on the
    // rebuilt row — a keyboard user mid Move-Up/Move-Down sequence must not be sent back to the top
    // of the tab order after every press.
    function focusLevelRow(uniqueId) {
        const container = selectedColumnsContainer();
        if (container === null || uniqueId === null || uniqueId === undefined) {
            return;
        }
        const row = container.querySelector('.gis-typology-level[data-unique-id="' + String(uniqueId).replace(/"/g, '\\"') + '"]');
        if (row !== null) {
            row.focus();
        }
    }

    // Toggles the selection — the hookup point for the Column Properties section (#16).
    function select(index) {
        if (index < 0 || index >= state.levels.length) {
            return;
        }
        state.selectedIndex = state.selectedIndex === index ? -1 : index;
        renderAll();
    }

    // ----- rendering (plain DOM re-render; 194 small rows is fine at this scale) -----

    function availableColumnsContainer() {
        return document.querySelector('#typology-available-columns .gis-column-list');
    }

    function selectedColumnsContainer() {
        return document.querySelector('#typology-selected-columns .gis-column-list');
    }

    function renderAll() {
        renderAvailable();
        renderSelected();
    }

    function renderAvailable() {
        const container = availableColumnsContainer();
        if (container === null) {
            return;
        }

        const filterInput = document.getElementById('typology-column-filter');
        const filter = filterInput === null ? '' : filterInput.value;
        const filterLower = filter.toLowerCase();
        const visible = filterLower === ''
            ? state.availableColumns
            : state.availableColumns.filter(function (column) {
                return (column.name || '').toLowerCase().indexOf(filterLower) !== -1;
            });

        if (visible.length === 0) {
            showEmptyState(container, 'No columns available.');
            return;
        }

        container.innerHTML = visible.map(function (column) {
            const inChain = levelByUniqueId(column.uniqueId) !== undefined;
            const tooltipParts = [column.category, column.description].filter(Boolean);
            const title = tooltipParts.length > 0 ? ' title="' + escapeHtml(tooltipParts.join(' — ')) + '"' : '';
            // In-chain rows leave the tab order (aria-disabled says why) and can no longer be
            // dragged or double-clicked — the duplicate guard, plus the dimmed look.
            const interactive = inChain
                ? ' aria-disabled="true"'
                : ' tabindex="0" role="button" draggable="true"';
            return '<div class="gis-column-item' + (inChain ? ' gis-column-item-disabled' : '') + '"' +
                   ' data-unique-id="' + escapeHtml(column.uniqueId || '') + '"' +
                   interactive + title + '>' +
                   escapeHtml(column.name || '') + '</div>';
        }).join('');
    }

    function renderSelected() {
        const container = selectedColumnsContainer();
        if (container === null) {
            return;
        }

        if (state.levels.length === 0) {
            showEmptyState(container, 'No columns selected — double-click or drag a column here.');
            return;
        }

        const lastIndex = state.levels.length - 1;
        container.innerHTML = state.levels.map(function (level, index) {
            const name = escapeHtml(level.name || level.uniqueId || '');
            const actions =
                '<span class="gis-typology-level-actions">' +
                '<button type="button" class="gis-button gis-button-icon gis-button-secondary" data-action="up" title="Move up" aria-label="Move ' + name + ' up"' + (index === 0 ? ' disabled' : '') + '>&uarr;</button>' +
                '<button type="button" class="gis-button gis-button-icon gis-button-secondary" data-action="down" title="Move down" aria-label="Move ' + name + ' down"' + (index === lastIndex ? ' disabled' : '') + '>&darr;</button>' +
                '<button type="button" class="gis-button gis-button-icon gis-button-secondary" data-action="remove" title="Remove" aria-label="Remove ' + name + '">&times;</button>' +
                '</span>';
            return '<div class="gis-column-item gis-typology-level' + (index === state.selectedIndex ? ' gis-column-item-selected' : '') + '"' +
                   ' data-unique-id="' + escapeHtml(level.uniqueId || '') + '" tabindex="0" role="button" draggable="true">' +
                   '<span class="gis-typology-level-index">' + (index + 1) + '</span>' +
                   '<span class="gis-column-item-text">' + name + '</span>' +
                   actions + '</div>';
        }).join('');
    }

    function showLoader(container) {
        container.innerHTML = '<div class="gis-loader"></div><p class="gis-loader-text">Loading columns…</p>';
    }

    function showEmptyState(container, text) {
        container.innerHTML = '<div class="gis-empty-state">' + escapeHtml(text) + '</div>';
    }

    function escapeHtml(text) {
        return String(text)
            .replace(/&/g, '&amp;')
            .replace(/</g, '&lt;')
            .replace(/>/g, '&gt;')
            .replace(/"/g, '&quot;')
            .replace(/'/g, '&#39;');
    }

    // ----- events (delegated on the two list containers) -----

    // The .gis-column-item under the pointer, or null when the event landed outside one.
    function rowFor(event, container) {
        const row = event.target.closest !== undefined ? event.target.closest('.gis-column-item') : null;
        return (row !== null && container.contains(row)) ? row : null;
    }

    function setupFilter() {
        const filterInput = document.getElementById('typology-column-filter');
        if (filterInput === null) {
            return;
        }
        filterInput.addEventListener('input', function () {
            const container = availableColumnsContainer();
            if (container !== null && state.availableColumns.length > 0) {
                renderAvailable();
            }
        });
    }

    function setupAvailableEvents() {
        const container = availableColumnsContainer();
        if (container === null) {
            return;
        }

        // Double-click adds (the pointer path). The row is also a keyboard button, so every add has
        // a non-pointer path.
        container.addEventListener('dblclick', function (event) {
            const row = rowFor(event, container);
            if (row === null || row.classList.contains('gis-column-item-disabled')) {
                return;
            }
            addLevel(columnByUniqueId(row.getAttribute('data-unique-id')));
        });

        container.addEventListener('keydown', function (event) {
            if (event.key !== 'Enter' && event.key !== ' ') {
                return;
            }
            const row = rowFor(event, container);
            if (row === null || row.classList.contains('gis-column-item-disabled')) {
                return;
            }
            event.preventDefault(); // keep Space from scrolling the page
            addLevel(columnByUniqueId(row.getAttribute('data-unique-id')));
        });

        // Drag out of Section 1 toward Section 2 (add).
        container.addEventListener('dragstart', function (event) {
            const row = rowFor(event, container);
            if (row === null || row.classList.contains('gis-column-item-disabled')) {
                event.preventDefault();
                return;
            }
            event.dataTransfer.setData(dragType_Available, row.getAttribute('data-unique-id'));
            event.dataTransfer.effectAllowed = 'copy';
        });

        // A Section 2 row dragged back onto Section 1 removes it from the chain.
        container.addEventListener('dragover', function (event) {
            if (hasDragType(event.dataTransfer, dragType_Level) !== true) {
                return;
            }
            event.preventDefault();
            container.classList.add('gis-drag-over');
        });

        container.addEventListener('dragleave', function (event) {
            if (container.contains(event.relatedTarget)) {
                return;
            }
            container.classList.remove('gis-drag-over');
        });

        container.addEventListener('drop', function (event) {
            const uniqueId = event.dataTransfer !== null ? event.dataTransfer.getData(dragType_Level) : '';
            if (uniqueId === '') {
                return;
            }
            event.preventDefault();
            container.classList.remove('gis-drag-over');
            removeLevel(uniqueId);
        });
    }

    function setupSelectedEvents() {
        const container = selectedColumnsContainer();
        if (container === null) {
            return;
        }

        // One delegated handler covers both shapes: a button action first, otherwise a row selection.
        // Disabled buttons never fire click, so the end-of-chain guards need no handling here.
        container.addEventListener('click', function (event) {
            const button = event.target.closest !== undefined ? event.target.closest('button[data-action]') : null;
            if (button !== null) {
                const row = button.closest('.gis-typology-level');
                if (row === null) {
                    return;
                }
                const uniqueId = row.getAttribute('data-unique-id');
                const index = levelIndex(uniqueId);
                if (index === -1) {
                    return;
                }
                const action = button.getAttribute('data-action');
                if (action === 'up') {
                    moveLevel(index, index - 1);
                    focusLevelRow(uniqueId); // the row moved — keep the keyboard hand on it
                } else if (action === 'down') {
                    moveLevel(index, index + 1);
                    focusLevelRow(uniqueId);
                } else if (action === 'remove') {
                    removeLevel(uniqueId);
                    // The row is gone — park on its neighbour so the keyboard hand stays in the list.
                    if (state.levels.length > 0) {
                        focusLevelRow(state.levels[Math.min(index, state.levels.length - 1)].uniqueId);
                    }
                }
                return;
            }

            const row = rowFor(event, container);
            if (row === null) {
                return;
            }
            select(levelIndex(row.getAttribute('data-unique-id')));
        });

        // Enter/Space on a focused row toggles its selection (the non-pointer path for selection).
        container.addEventListener('keydown', function (event) {
            if (event.key !== 'Enter' && event.key !== ' ') {
                return;
            }
            if (event.target.closest !== undefined && event.target.closest('button') !== null) {
                return; // the button handles its own activation
            }
            const row = rowFor(event, container);
            if (row === null) {
                return;
            }
            event.preventDefault();
            select(levelIndex(row.getAttribute('data-unique-id')));
        });

        // Drag reorder within Section 2.
        container.addEventListener('dragstart', function (event) {
            const row = rowFor(event, container);
            if (row === null) {
                event.preventDefault();
                return;
            }
            event.dataTransfer.setData(dragType_Level, row.getAttribute('data-unique-id'));
            event.dataTransfer.effectAllowed = 'move';
        });

        container.addEventListener('dragover', function (event) {
            if (hasDragType(event.dataTransfer, dragType_Available) !== true && hasDragType(event.dataTransfer, dragType_Level) !== true) {
                return;
            }
            event.preventDefault();
            const row = rowFor(event, container);
            clearDragOver(container);
            // A reorder lands on the hovered row's slot; an add appends to the end of the chain
            // wherever it is dropped, so the list — not the row — says where it goes.
            if (hasDragType(event.dataTransfer, dragType_Level) === true && row !== null) {
                row.classList.add('gis-drag-over');
            } else {
                container.classList.add('gis-drag-over');
            }
        });

        container.addEventListener('dragleave', function (event) {
            if (container.contains(event.relatedTarget)) {
                return;
            }
            clearDragOver(container);
        });

        container.addEventListener('drop', function (event) {
            if (hasDragType(event.dataTransfer, dragType_Available) !== true && hasDragType(event.dataTransfer, dragType_Level) !== true) {
                return;
            }
            event.preventDefault();
            clearDragOver(container);

            const availableId = event.dataTransfer.getData(dragType_Available);
            if (availableId !== '') {
                addLevel(columnByUniqueId(availableId));
                return;
            }

            const levelId = event.dataTransfer.getData(dragType_Level);
            if (levelId === '') {
                return;
            }
            const sourceIndex = levelIndex(levelId);
            if (sourceIndex === -1) {
                return;
            }
            const row = rowFor(event, container);
            if (row === null || row.getAttribute('data-unique-id') === levelId) {
                // Dropped onto the list padding (or its own row): settle at the end of the chain.
                moveLevel(sourceIndex, state.levels.length - 1);
                return;
            }
            moveLevel(sourceIndex, levelIndex(row.getAttribute('data-unique-id')));
        });
    }

    // dragover exposes only the type list, never the payload — so both the "is this one of ours?"
    // check and the drag-kind check read the types.
    function hasDragType(dataTransfer, dragType) {
        if (dataTransfer === null || dataTransfer.types === null) {
            return false;
        }
        const types = Array.prototype.slice.call(dataTransfer.types);
        return types.indexOf(dragType) !== -1;
    }

    function clearDragOver(container) {
        container.classList.remove('gis-drag-over');
        const rows = container.querySelectorAll('.gis-drag-over');
        for (let i = 0; i < rows.length; i++) {
            rows[i].classList.remove('gis-drag-over');
        }
    }

    // ----- boot -----

    function init() {
        setupFilter();
        setupAvailableEvents();
        setupSelectedEvents();
        renderSelected(); // the empty state, until the columns arrive
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
