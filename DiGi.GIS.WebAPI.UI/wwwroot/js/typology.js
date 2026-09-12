/**
 * Typology definition page.
 *
 * #14 ships the Available Columns section; #15 the Selected Columns chain — add by double-click or
 * drag & drop, remove by button or drag-back, reorder by Move Up/Down or drag, and single-click
 * selection; #16 the Column Properties editor of the selected level — rule type, range rows with
 * inline validation, unique values loaded through the /typology/uniquevalues proxy, a colour per
 * bucket; #17 Import/Export — the page state posted to the server, which alone composes and parses
 * the DiGi document; #18 the Load modal follows. One object rather than loose globals:
 * names such as import() or load() are too general to own at window scope (same reasoning as user.js).
 */
const digiTypology = (function () {
    'use strict';

    // Custom dataTransfer types. dragover cannot read the payload — only the type list — so the two
    // drag kinds are told apart by type, and the uniqueId payload is read once, on drop.
    const dragType_Available = 'application/x-typology-available';
    const dragType_Level = 'application/x-typology-level';

    // The concrete rule class names of the DiGi.Typology.Visual document model (#17 puts them on the
    // wire as _type). The editor offers two kinds — unique values or ranges — and picks the integer or
    // double range rule from the column's DataType, so the user never sees three options.
    const ruleType_UniqueValue = 'VisualUniqueValueFilterRule';
    const ruleType_IntegerRange = 'VisualIntegerRangeFilterRule';
    const ruleType_DoubleRange = 'VisualDoubleRangeFilterRule';

    // DiGi.Core.Enums.DataType on the wire: 1 SByte … 8 ULong are integers, 9 Float … 11 Decimal are
    // floating point; everything else (Bool, String, DateTime, …) takes unique values only.
    const dataType_IntegerMax = 8;

    // Default bucket colours, handed out in order so every new range or value is visible in the
    // colour-coded view before the user touches a picker. Hex is the picker's native form; #17 turns it
    // into the ARGB TypologyAppearance the document carries.
    const palette = [
        '#1f77b4', '#ff7f0e', '#2ca02c', '#d62728', '#9467bd', '#8c564b',
        '#e377c2', '#7f7f7f', '#bcbd22', '#17becf', '#393b79', '#e7ba52'
    ];

    const state = {
        availableColumns: [],
        // The ordered grouping chain — its order is the grouping order the solver consumes
        // (ZiolkowskiJakub/DiGi.Gis#5). The level shape follows the DiGi.Typology.Visual document
        // model: ruleType is one of the rule class names above (null until chosen), ranges are plain
        // { min, max, color } rows, uniqueValueColors are { value, color } rows, and there is no
        // per-level fallback appearance. Both row lists survive a rule-type switch so nothing typed
        // is lost; #17 exports the active one and files the colours into the rule's
        // TypologyAppearanceCollection (the client never spells the "[min, max]" key itself).
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

    // ----- Column Properties state (#16): every edit writes into the selected level -----

    function isRangeRuleType(ruleType) {
        return ruleType === ruleType_IntegerRange || ruleType === ruleType_DoubleRange;
    }

    // The range rule a column takes: integer DataTypes get the integer rule, floating point the double
    // rule. null for a column that cannot take ranges at all (Bool, String, DateTime, …).
    function rangeRuleTypeFor(level) {
        if (level.isNumeric !== true) {
            return null;
        }
        return level.dataType <= dataType_IntegerMax ? ruleType_IntegerRange : ruleType_DoubleRange;
    }

    // kind is the select's value: 'unique', 'range' or '' (undecided). Both row lists are kept across
    // the switch — the persistence criterion — only the active kind changes.
    function setRuleType(level, kind) {
        if (kind === 'unique') {
            level.ruleType = ruleType_UniqueValue;
        } else if (kind === 'range') {
            level.ruleType = rangeRuleTypeFor(level);
        } else {
            level.ruleType = null;
        }
        renderProperties();
        // Arrow keys on a focused select fire change at once; the rebuild would drop the focus and
        // strand a keyboard user after the first press.
        focusPropertiesField('select[data-field="ruleType"]');
    }

    function nextColor(count) {
        return palette[count % palette.length];
    }

    // A new row starts where the previous one ended, so a typical ascending list needs only its Max
    // typed; the first row starts empty.
    function addRange(level) {
        const previous = level.ranges.length > 0 ? level.ranges[level.ranges.length - 1] : null;
        const start = previous !== null && typeof previous.max === 'number' && Number.isFinite(previous.max)
            ? previous.max + (level.ruleType === ruleType_IntegerRange ? 1 : 0)
            : null;
        level.ranges.push({ min: start, max: null, color: nextColor(level.ranges.length) });
        renderProperties();
        focusPropertiesField('.gis-typology-row[data-index="' + (level.ranges.length - 1) + '"] input[data-field="min"]');
    }

    function removeRange(level, index) {
        if (index < 0 || index >= level.ranges.length) {
            return;
        }
        level.ranges.splice(index, 1);
        renderProperties();
        focusPropertiesField('button[data-action="add-range"]');
    }

    // Bounds are written on every keystroke without a re-render — a rebuild would drop the caret —
    // so only the validation block is refreshed in place. An empty box is null, never NaN.
    function setRangeBound(level, index, field, rawValue) {
        if (index < 0 || index >= level.ranges.length) {
            return;
        }
        const trimmed = String(rawValue).trim();
        level.ranges[index][field] = trimmed === '' ? null : Number(trimmed);
        renderRangeValidation(level);
    }

    function setRangeColor(level, index, color) {
        if (index >= 0 && index < level.ranges.length) {
            level.ranges[index].color = color;
        }
    }

    // Validation of the closed intervals the solver walks by Min (ZiolkowskiJakub/DiGi.Gis#5 §4.5.3):
    // both bounds present (whole numbers on an integer rule), Min <= Max, and every row starting after
    // the previous valid row ends — two closed intervals meeting at one value overlap there. Returns
    // the messages to list and the "row:field" keys of the boxes to outline. No auto-sort: the issue
    // asks for an inline error, and silently reordering typed rows would hide a mistake.
    function rangeErrors(level) {
        const errors = [];
        const invalid = {};
        const integer = level.ruleType === ruleType_IntegerRange;

        function bound(value) {
            return typeof value === 'number' && Number.isFinite(value) && (!integer || Number.isInteger(value));
        }

        let previous = null;
        let previousIndex = -1;
        for (let i = 0; i < level.ranges.length; i++) {
            const range = level.ranges[i];
            const row = i + 1;
            const minOk = bound(range.min);
            const maxOk = bound(range.max);
            if (!minOk || !maxOk) {
                errors.push('Row ' + row + ': enter both bounds' + (integer ? ' as whole numbers.' : '.'));
                if (!minOk) {
                    invalid[i + ':min'] = true;
                }
                if (!maxOk) {
                    invalid[i + ':max'] = true;
                }
                continue;
            }
            if (range.min > range.max) {
                errors.push('Row ' + row + ': Min exceeds Max.');
                invalid[i + ':min'] = true;
                invalid[i + ':max'] = true;
                continue;
            }
            if (previous !== null && range.min <= previous.max) {
                errors.push('Row ' + row + ' overlaps row ' + (previousIndex + 1) + ': ranges must be ascending, and closed intervals meeting at ' + previous.max + ' share that value.');
                invalid[i + ':min'] = true;
            }
            previous = range;
            previousIndex = i;
        }

        return { errors: errors, invalid: invalid };
    }

    // Unique-value rows are keyed by the JSON form of the value, which tells "1" from 1 and keeps null
    // addressable. Loaded values merge into the existing rows so recoloured rows keep their colour.
    function valueKey(value) {
        return JSON.stringify(value === undefined ? null : value);
    }

    function mergeUniqueValues(level, values) {
        const present = {};
        for (let i = 0; i < level.uniqueValueColors.length; i++) {
            present[valueKey(level.uniqueValueColors[i].value)] = true;
        }
        for (let i = 0; i < values.length; i++) {
            const key = valueKey(values[i]);
            if (present[key] === true) {
                continue;
            }
            present[key] = true;
            level.uniqueValueColors.push({ value: values[i] === undefined ? null : values[i], color: nextColor(level.uniqueValueColors.length) });
        }
    }

    function removeUniqueValue(level, index) {
        if (index < 0 || index >= level.uniqueValueColors.length) {
            return;
        }
        level.uniqueValueColors.splice(index, 1);
        renderProperties();
        focusPropertiesField('button[data-action="load-values"]');
    }

    function clearUniqueValues(level) {
        level.uniqueValueColors = [];
        uniqueValuesMessage = null;
        renderProperties();
        focusPropertiesField('button[data-action="load-values"]');
    }

    function setUniqueValueColor(level, index, color) {
        if (index >= 0 && index < level.uniqueValueColors.length) {
            level.uniqueValueColors[index].color = color;
        }
    }

    // The county part that scopes "Load values" — a load scope, not part of the definition, so it lives
    // beside the state rather than on a level. null means the whole table (slow; may answer nothing).
    let uniqueValuesCountyId = null;
    // The in-flight request (so a second click or a level switch cancels the first), the uniqueId of
    // the level whose values are loading, and the outcome message of the last load.
    let uniqueValuesAbortController = null;
    let uniqueValuesLoadingId = null;
    let uniqueValuesMessage = null;

    function loadUniqueValues(level) {
        abortUniqueValuesLoad();

        const abortController = new AbortController();
        uniqueValuesAbortController = abortController;
        uniqueValuesLoadingId = level.uniqueId;
        uniqueValuesMessage = null;
        renderProperties();

        let url = baseUrl() + '/typology/uniquevalues?columnuniqueid=' + encodeURIComponent(level.uniqueId);
        if (uniqueValuesCountyId !== null) {
            url += '&countyid=' + encodeURIComponent(String(uniqueValuesCountyId));
        }

        function finish(values) {
            if (uniqueValuesAbortController !== abortController) {
                return; // superseded by a later load or a level switch
            }
            uniqueValuesAbortController = null;
            uniqueValuesLoadingId = null;
            if (Array.isArray(values)) {
                mergeUniqueValues(level, values);
                uniqueValuesMessage = values.length === 0 ? 'No values returned.' : null;
            } else {
                // 204 from the proxy covers an empty column, an unknown column and an upstream
                // timeout alike; a national load is the usual cause of the last one.
                uniqueValuesMessage = 'No values returned' + (uniqueValuesCountyId === null ? ' — try a county id; a load over the whole table can exceed the service timeout.' : '.');
            }
            renderProperties();
        }

        fetch(url, { signal: abortController.signal })
            .then(function (response) {
                if (response.status === 204 || !response.ok) {
                    return null;
                }
                return response.json();
            })
            .then(finish)
            .catch(function (error) {
                if (error !== null && error !== undefined && error.name === 'AbortError') {
                    return;
                }
                finish(null);
            });
    }

    function abortUniqueValuesLoad() {
        if (uniqueValuesAbortController !== null) {
            uniqueValuesAbortController.abort();
            uniqueValuesAbortController = null;
        }
        uniqueValuesLoadingId = null;
    }

    function focusPropertiesField(selector) {
        const container = propertiesContainer();
        if (container === null) {
            return;
        }
        const field = container.querySelector(selector);
        if (field !== null) {
            field.focus();
        }
    }

    // ----- rendering (plain DOM re-render; 194 small rows is fine at this scale) -----

    function availableColumnsContainer() {
        return document.querySelector('#typology-available-columns .gis-column-list');
    }

    function selectedColumnsContainer() {
        return document.querySelector('#typology-selected-columns .gis-column-list');
    }

    function propertiesContainer() {
        return document.querySelector('#typology-column-properties .gis-typology-properties');
    }

    function selectedLevel() {
        return state.selectedIndex >= 0 && state.selectedIndex < state.levels.length ? state.levels[state.selectedIndex] : null;
    }

    function renderAll() {
        renderAvailable();
        renderSelected();
        renderProperties();
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

    // The uniqueId last rendered into Section 3, so a level switch can drop the previous level's
    // in-flight load and outcome message instead of showing them under the new level.
    let renderedPropertiesId = null;

    function renderProperties() {
        const container = propertiesContainer();
        if (container === null) {
            return;
        }

        const level = selectedLevel();
        const levelId = level === null ? null : level.uniqueId;
        if (levelId !== renderedPropertiesId) {
            abortUniqueValuesLoad();
            uniqueValuesMessage = null;
            renderedPropertiesId = levelId;
        }

        if (level === null) {
            showEmptyState(container, 'Select a column in Selected Columns to edit its properties.');
            return;
        }

        const rangeRuleType = rangeRuleTypeFor(level);
        const kind = level.ruleType === ruleType_UniqueValue ? 'unique' : (isRangeRuleType(level.ruleType) ? 'range' : '');

        let html =
            '<p class="gis-typology-properties-name">' + escapeHtml(level.name || level.uniqueId || '') + '</p>' +
            '<label class="gis-field-label">Rule type' +
            '<select data-field="ruleType" class="gis-select">' +
            '<option value=""' + (kind === '' ? ' selected' : '') + '>— choose —</option>' +
            '<option value="unique"' + (kind === 'unique' ? ' selected' : '') + '>Unique values</option>' +
            '<option value="range"' + (kind === 'range' ? ' selected' : '') + (rangeRuleType === null ? ' disabled' : '') + '>Ranges</option>' +
            '</select></label>';

        if (kind === 'range') {
            html += renderRangeEditor(level);
        } else if (kind === 'unique') {
            html += renderUniqueValueEditor(level);
        } else {
            html += '<p class="gis-typology-hint">Choose how this level groups rows: by each distinct value, or by ranges' +
                (rangeRuleType === null ? ' (ranges need a numeric column)' : '') + '.</p>';
        }

        container.innerHTML = html;
        renderRangeValidation(level);
    }

    function renderRangeEditor(level) {
        const integer = level.ruleType === ruleType_IntegerRange;
        const step = integer ? '1' : 'any';

        let rows;
        if (level.ranges.length === 0) {
            rows = '<div class="gis-empty-state">No ranges — add one.</div>';
        } else {
            rows = level.ranges.map(function (range, index) {
                const row = index + 1;
                return '<div class="gis-typology-row" data-index="' + index + '">' +
                    '<input type="number" step="' + step + '" data-field="min" value="' + numberAttribute(range.min) + '" placeholder="Min" aria-label="Range ' + row + ' minimum" />' +
                    '<span class="gis-typology-row-separator" aria-hidden="true">–</span>' +
                    '<input type="number" step="' + step + '" data-field="max" value="' + numberAttribute(range.max) + '" placeholder="Max" aria-label="Range ' + row + ' maximum" />' +
                    '<input type="color" data-field="color" value="' + escapeHtml(range.color || nextColor(index)) + '" aria-label="Range ' + row + ' colour" title="Colour" />' +
                    '<button type="button" class="gis-button gis-button-icon gis-button-secondary" data-action="remove-range" title="Remove" aria-label="Remove range ' + row + '">&times;</button>' +
                    '</div>';
            }).join('');
        }

        return '<div class="gis-typology-ranges" aria-describedby="typology-range-errors">' + rows + '</div>' +
            '<div class="gis-typology-actions">' +
            '<button type="button" class="gis-button gis-button-secondary" data-action="add-range">Add range</button>' +
            '</div>' +
            '<ul id="typology-range-errors" class="gis-typology-errors" role="alert"></ul>' +
            '<p class="gis-typology-hint">Closed intervals' + (integer ? ' of whole numbers' : '') + ', ascending and non-overlapping — the solver walks them by Min and a disordered list matches nothing. ' +
            'Rows with no value in this column fall out of this level. For an open end use a sentinel (for years, 0 and 9999).</p>';
    }

    function renderUniqueValueEditor(level) {
        const loading = uniqueValuesLoadingId === level.uniqueId;

        let rows;
        if (loading) {
            rows = '<div class="gis-loader"></div><p class="gis-loader-text">Loading values…</p>';
        } else if (level.uniqueValueColors.length === 0) {
            rows = '<div class="gis-empty-state">' + escapeHtml(uniqueValuesMessage || 'No values yet — load them from the building data.') + '</div>';
        } else {
            rows = level.uniqueValueColors.map(function (entry, index) {
                const text = displayValue(entry.value);
                return '<div class="gis-typology-row" data-index="' + index + '">' +
                    '<span class="gis-typology-row-text" title="' + escapeHtml(text) + '">' + escapeHtml(text) + '</span>' +
                    '<input type="color" data-field="color" value="' + escapeHtml(entry.color || nextColor(index)) + '" aria-label="Colour of ' + escapeHtml(text) + '" title="Colour" />' +
                    '<button type="button" class="gis-button gis-button-icon gis-button-secondary" data-action="remove-value" title="Remove" aria-label="Remove ' + escapeHtml(text) + '">&times;</button>' +
                    '</div>';
            }).join('');
            if (uniqueValuesMessage !== null) {
                rows += '<p class="gis-typology-hint">' + escapeHtml(uniqueValuesMessage) + '</p>';
            }
        }

        return '<div class="gis-typology-actions">' +
            '<input type="number" min="1" step="1" data-field="countyId" value="' + numberAttribute(uniqueValuesCountyId) + '" placeholder="County id" aria-label="County id (optional)" title="County part id that scopes the values; empty loads the whole table" />' +
            '<button type="button" class="gis-button" data-action="load-values"' + (loading ? ' disabled' : '') + '>Load values</button>' +
            '<button type="button" class="gis-button gis-button-secondary" data-action="clear-values"' + (level.uniqueValueColors.length === 0 ? ' disabled' : '') + '>Clear</button>' +
            '</div>' +
            '<div class="gis-typology-values">' + rows + '</div>' +
            '<p class="gis-typology-hint">Each distinct value is its own bucket; a missing value is bucketed as (null). ' +
            'Loading takes several seconds per county and much longer over the whole table.</p>';
    }

    // Refreshes the error list and the outlined boxes of the range editor in place — called after a
    // full render and after every keystroke in a bound, without rebuilding the rows.
    function renderRangeValidation(level) {
        const container = propertiesContainer();
        if (container === null) {
            return;
        }
        const list = container.querySelector('#typology-range-errors');
        if (list === null || !isRangeRuleType(level.ruleType)) {
            return;
        }

        const result = rangeErrors(level);
        list.innerHTML = result.errors.map(function (error) {
            return '<li>' + escapeHtml(error) + '</li>';
        }).join('');

        const inputs = container.querySelectorAll('.gis-typology-row input[type="number"]');
        for (let i = 0; i < inputs.length; i++) {
            const row = inputs[i].closest('.gis-typology-row');
            const key = (row === null ? '' : row.getAttribute('data-index')) + ':' + inputs[i].getAttribute('data-field');
            inputs[i].classList.toggle('gis-typology-invalid', result.invalid[key] === true);
            inputs[i].setAttribute('aria-invalid', result.invalid[key] === true ? 'true' : 'false');
        }
    }

    function numberAttribute(value) {
        return typeof value === 'number' && Number.isFinite(value) ? String(value) : '';
    }

    function displayValue(value) {
        if (value === null || value === undefined) {
            return '(null)';
        }
        if (typeof value === 'object') {
            return JSON.stringify(value);
        }
        return String(value);
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

    // Column Properties events (#16), delegated on the section container. Every handler resolves the
    // selected level first: the rendered editor always belongs to it, so a stale event after a
    // selection change finds no level and does nothing.
    function setupPropertiesEvents() {
        const container = propertiesContainer();
        if (container === null) {
            return;
        }

        function rowIndex(target) {
            const row = target.closest('.gis-typology-row');
            return row === null ? -1 : parseInt(row.getAttribute('data-index'), 10);
        }

        // A colour picker sits in either list; the enclosing list says which state array it edits.
        function setRowColor(level, picker) {
            if (picker.closest('.gis-typology-values') !== null) {
                setUniqueValueColor(level, rowIndex(picker), picker.value);
            } else {
                setRangeColor(level, rowIndex(picker), picker.value);
            }
        }

        // Selects and colour pickers commit on change. Colour writes need no re-render: the picker
        // already shows the value, and rebuilding would close a native picker mid-drag on some browsers.
        container.addEventListener('change', function (event) {
            const level = selectedLevel();
            const target = event.target;
            if (level === null || target === null || target.closest === undefined) {
                return;
            }
            const field = target.getAttribute('data-field');
            if (field === 'ruleType') {
                setRuleType(level, target.value);
            } else if (field === 'color') {
                setRowColor(level, target);
            }
        });

        // Bounds and the county id are written on every keystroke; the range validation refreshes in
        // place so the caret stays where the user left it.
        container.addEventListener('input', function (event) {
            const level = selectedLevel();
            const target = event.target;
            if (level === null || target === null || target.getAttribute === undefined) {
                return;
            }
            const field = target.getAttribute('data-field');
            if (field === 'min' || field === 'max') {
                setRangeBound(level, rowIndex(target), field, target.value);
            } else if (field === 'countyId') {
                const trimmed = target.value.trim();
                const parsed = parseInt(trimmed, 10);
                uniqueValuesCountyId = trimmed === '' || !Number.isFinite(parsed) || parsed <= 0 ? null : parsed;
            } else if (field === 'color') {
                setRowColor(level, target); // live preview while the native picker is open; change commits the same value
            }
        });

        container.addEventListener('click', function (event) {
            const level = selectedLevel();
            const button = event.target.closest !== undefined ? event.target.closest('button[data-action]') : null;
            if (level === null || button === null) {
                return;
            }
            const action = button.getAttribute('data-action');
            if (action === 'add-range') {
                addRange(level);
            } else if (action === 'remove-range') {
                removeRange(level, rowIndex(button));
            } else if (action === 'load-values') {
                loadUniqueValues(level);
            } else if (action === 'clear-values') {
                clearUniqueValues(level);
            } else if (action === 'remove-value') {
                removeUniqueValue(level, rowIndex(button));
            }
        });

        // Enter in a bound or the county box acts like the row's natural next step instead of doing
        // nothing: a new range after the last Max, a load from the county box.
        container.addEventListener('keydown', function (event) {
            if (event.key !== 'Enter') {
                return;
            }
            const level = selectedLevel();
            const target = event.target;
            if (level === null || target === null || target.getAttribute === undefined) {
                return;
            }
            const field = target.getAttribute('data-field');
            if (field === 'max' && rowIndex(target) === level.ranges.length - 1) {
                event.preventDefault();
                addRange(level);
            } else if (field === 'countyId') {
                event.preventDefault();
                loadUniqueValues(level);
            }
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

    // ----- import / export (#17) -----

    // The document is composed and parsed on the server: Export posts the page state to
    // /typology/definition/export and receives the DiGi JSON of the VisualColumnTypologyFilter
    // chain; Import posts a chosen file to /typology/definition/validate and receives page state
    // back. This script therefore never spells a _type or a TypologyAppearanceCollection key
    // ("[min, max]" is rendered by .NET with invariant formatting, which String(x) need not match).
    // Both actions answer 400 with a JSON array of messages naming the level and the row.

    const definitionFileName = 'typology.json';

    function definitionRequestUrl(action) {
        return baseUrl() + '/typology/definition/' + action;
    }

    // Only the rows of the active rule are sent: both row lists survive a rule-type switch in the
    // page state so nothing typed is lost, but the document has one rule per level.
    function definitionPayload() {
        const levels = [];
        for (let i = 0; i < state.levels.length; i++) {
            const level = state.levels[i];
            const isRange = level.ruleType === ruleType_IntegerRange || level.ruleType === ruleType_DoubleRange;
            levels.push({
                uniqueId: level.uniqueId,
                name: level.name,
                dataType: level.dataType,
                isNumeric: level.isNumeric,
                ruleType: level.ruleType,
                ranges: isRange ? level.ranges : [],
                uniqueValueColors: level.ruleType === ruleType_UniqueValue ? level.uniqueValueColors : []
            });
        }
        return { levels: levels };
    }

    // Reads a 400 body as the message list, or falls back to the status when the body is not one.
    function definitionErrors(response) {
        return response.text().then(function (text) {
            try {
                const parsed = JSON.parse(text);
                if (Array.isArray(parsed) && parsed.length > 0) {
                    return parsed.map(function (item) { return String(item); });
                }
            } catch (error) {
                // not a message list
            }
            if (response.status === 503) {
                return ['The building data column catalog is unavailable — try again later.'];
            }
            return ['The request failed (' + response.status + ').'];
        });
    }

    function exportDefinition() {
        if (state.levels.length === 0) {
            showDefinitionErrors('Export', ['Select at least one column before exporting.']);
            return;
        }

        fetch(definitionRequestUrl('export'), {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(definitionPayload())
        })
            .then(function (response) {
                if (!response.ok) {
                    return definitionErrors(response).then(function (errors) {
                        showDefinitionErrors('Export', errors);
                    });
                }
                return response.text().then(showExportModal);
            })
            .catch(function () {
                showDefinitionErrors('Export', ['The request could not be sent.']);
            });
    }

    function importDefinition(file) {
        if (!file) {
            return;
        }

        const reader = new FileReader();
        reader.onerror = function () {
            showDefinitionErrors('Import', ['The file could not be read.']);
        };
        reader.onload = function () {
            fetch(definitionRequestUrl('validate'), {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: String(reader.result)
            })
                .then(function (response) {
                    if (!response.ok) {
                        return definitionErrors(response).then(function (errors) {
                            showDefinitionErrors('Import', errors);
                        });
                    }
                    return response.json().then(applyDefinition);
                })
                .catch(function () {
                    showDefinitionErrors('Import', ['The request could not be sent.']);
                });
        };
        reader.readAsText(file);
    }

    // Replaces the page state with what validate answered. Reached only on 200, so a rejected or
    // malformed file never touches the levels the visitor has.
    function applyDefinition(definition) {
        if (definition === null || typeof definition !== 'object' || !Array.isArray(definition.levels)) {
            showDefinitionErrors('Import', ['The definition could not be read.']);
            return;
        }

        abortUniqueValuesLoad();
        uniqueValuesMessage = null;

        const levels = [];
        for (let i = 0; i < definition.levels.length; i++) {
            const level = definition.levels[i];
            levels.push({
                uniqueId: level.uniqueId,
                name: level.name,
                dataType: level.dataType,
                isNumeric: level.isNumeric === true,
                ruleType: level.ruleType || null,
                ranges: Array.isArray(level.ranges) ? level.ranges : [],
                uniqueValueColors: Array.isArray(level.uniqueValueColors) ? level.uniqueValueColors : []
            });
        }

        state.levels = levels;
        state.selectedIndex = -1;
        renderAll();
    }

    // ----- modals -----

    let exportModal = null;

    function ensureExportModal() {
        if (exportModal !== null) {
            return exportModal;
        }

        exportModal = document.createElement('div');
        exportModal.className = 'gis-modal-overlay';
        exportModal.style.display = 'none';
        exportModal.innerHTML =
            '<div class="gis-card gis-modal gis-modal-wide" role="dialog" aria-modal="true" aria-labelledby="typology-export-title">' +
            '<h3 class="gis-title" id="typology-export-title">Export</h3>' +
            '<pre class="gis-modal-pre"></pre>' +
            '<div class="gis-modal-buttons">' +
            '<button type="button" id="typology-export-close-button" class="gis-button">Close</button>' +
            '<button type="button" id="typology-export-copy-button" class="gis-button">Copy</button>' +
            '<button type="button" id="typology-export-download-button" class="gis-button">Download</button>' +
            '</div></div>';
        document.body.appendChild(exportModal);

        const pre = exportModal.querySelector('.gis-modal-pre');
        const copyButton = exportModal.querySelector('#typology-export-copy-button');

        exportModal.querySelector('#typology-export-close-button').addEventListener('click', hideExportModal);
        exportModal.addEventListener('click', function (event) {
            if (event.target === exportModal) {
                hideExportModal();
            }
        });
        copyButton.addEventListener('click', function () {
            copyText(pre.textContent, copyButton);
        });
        exportModal.querySelector('#typology-export-download-button').addEventListener('click', function () {
            downloadText(pre.textContent, definitionFileName);
        });
        document.addEventListener('keydown', function (event) {
            if (event.key === 'Escape' && exportModal.style.display !== 'none') {
                hideExportModal();
            }
        });

        return exportModal;
    }

    function showExportModal(json) {
        const modal = ensureExportModal();
        let text = json;
        try {
            text = JSON.stringify(JSON.parse(json), null, 2);
        } catch (error) {
            // shown as received
        }
        modal.querySelector('.gis-modal-pre').textContent = text;
        modal.style.display = 'flex';
        modal.querySelector('#typology-export-download-button').focus();
    }

    function hideExportModal() {
        if (exportModal !== null) {
            exportModal.style.display = 'none';
        }
    }

    function copyText(text, button) {
        function flash() {
            const label = button.textContent;
            button.textContent = 'Copied';
            setTimeout(function () { button.textContent = label; }, 1200);
        }

        if (navigator.clipboard && navigator.clipboard.writeText) {
            navigator.clipboard.writeText(text).then(flash).catch(function () { fallbackCopy(text); flash(); });
        } else {
            fallbackCopy(text);
            flash();
        }
    }

    function fallbackCopy(text) {
        const textarea = document.createElement('textarea');
        textarea.value = text;
        textarea.setAttribute('readonly', '');
        textarea.style.position = 'fixed';
        textarea.style.left = '-9999px';
        document.body.appendChild(textarea);
        textarea.select();
        try {
            document.execCommand('copy');
        } catch (error) {
            // clipboard unavailable
        }
        document.body.removeChild(textarea);
    }

    function downloadText(text, fileName) {
        const blob = new Blob([text], { type: 'application/json' });
        const url = URL.createObjectURL(blob);
        const anchor = document.createElement('a');
        anchor.href = url;
        anchor.download = fileName;
        document.body.appendChild(anchor);
        anchor.click();
        document.body.removeChild(anchor);
        setTimeout(function () { URL.revokeObjectURL(url); }, 0);
    }

    let errorModal = null;

    // Every message is escaped: the list carries column names and file content echoed back.
    function showDefinitionErrors(title, errors) {
        if (errorModal === null) {
            errorModal = document.createElement('div');
            errorModal.className = 'gis-modal-overlay';
            errorModal.style.display = 'none';
            errorModal.innerHTML =
                '<div class="gis-card gis-modal gis-dialog-card" role="dialog" aria-modal="true" aria-labelledby="typology-error-title">' +
                '<h3 class="gis-title gis-dialog-title-error" id="typology-error-title"></h3>' +
                '<ul class="gis-dialog-message gis-typology-error-list"></ul>' +
                '<div class="gis-modal-buttons">' +
                '<button type="button" id="typology-error-close-button" class="gis-button">Close</button>' +
                '</div></div>';
            document.body.appendChild(errorModal);

            errorModal.querySelector('#typology-error-close-button').addEventListener('click', hideDefinitionErrors);
            errorModal.addEventListener('click', function (event) {
                if (event.target === errorModal) {
                    hideDefinitionErrors();
                }
            });
            document.addEventListener('keydown', function (event) {
                if (event.key === 'Escape' && errorModal.style.display !== 'none') {
                    hideDefinitionErrors();
                }
            });
        }

        errorModal.querySelector('#typology-error-title').textContent = title + ' failed';
        let html = '';
        for (let i = 0; i < errors.length; i++) {
            html += '<li>' + escapeHtml(errors[i]) + '</li>';
        }
        errorModal.querySelector('.gis-typology-error-list').innerHTML = html;
        errorModal.style.display = 'flex';
        errorModal.querySelector('#typology-error-close-button').focus();
    }

    function hideDefinitionErrors() {
        if (errorModal !== null) {
            errorModal.style.display = 'none';
        }
    }

    function setupDefinitionEvents() {
        const exportButton = document.getElementById('typology-export-button');
        const importButton = document.getElementById('typology-import-button');
        const fileInput = document.getElementById('typology-import-file');

        if (exportButton !== null) {
            exportButton.addEventListener('click', exportDefinition);
        }
        if (importButton !== null && fileInput !== null) {
            importButton.addEventListener('click', function () {
                fileInput.value = ''; // so choosing the same file again fires change
                fileInput.click();
            });
            fileInput.addEventListener('change', function () {
                importDefinition(fileInput.files && fileInput.files.length > 0 ? fileInput.files[0] : null);
            });
        }
    }

    // ----- boot -----

    function init() {
        setupFilter();
        setupAvailableEvents();
        setupSelectedEvents();
        setupPropertiesEvents();
        setupDefinitionEvents();
        renderSelected(); // the empty state, until the columns arrive
        renderProperties();
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
