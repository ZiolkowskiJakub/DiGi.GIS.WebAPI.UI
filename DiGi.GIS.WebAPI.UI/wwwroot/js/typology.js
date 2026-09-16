/**
 * Typology definition page.
 *
 * #14 ships the Available Columns section; #15 the Selected Columns chain — add by double-click or
 * drag & drop, remove by button or drag-back, reorder by Move Up/Down or drag, and single-click
 * selection; #16 the Column Properties editor of the selected level — rule type, range rows with
 * inline validation, unique values loaded through the /typology/uniquevalues proxy, a colour per
 * bucket; #17 Import/Export — the page state posted to the server, which alone composes and parses
 * the DiGi document; #18 the Load modal — live administrative-area search and the redirect to the
 * stub view; #30/#37 the range Load — equal-count ranges from the area's histogram, the count the
 * visitor's. One object rather than loose globals:
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
    const dataType_DecimalMax = 11;
    const dataType_Bool = 13;

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

    function removeRange(level, index) {
        if (index < 0 || index >= level.ranges.length) {
            return;
        }
        level.ranges.splice(index, 1);
        renderProperties();
        focusPropertiesField('button[data-action="add"]');
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

    // The range count the next Load generates (#37): a whole number clamped to the allowed span; a blank
    // or unparseable field keeps the current count, so a half-typed value never resets it.
    function setGeneratedRangeCount(rawValue) {
        const trimmed = String(rawValue).trim();
        const parsed = trimmed === '' ? NaN : Number(trimmed);
        if (!Number.isFinite(parsed)) {
            return;
        }
        generatedRangeCount = Math.min(Math.max(Math.round(parsed), generatedRangeCountMin), generatedRangeCountMax);
    }

    // Validation of the intervals the solver walks by Min (ZiolkowskiJakub/DiGi.Gis#5 §4.5.3): both
    // bounds present (whole numbers on an integer rule), Min <= Max, and every row starting where or
    // after the previous valid row ends — a shared boundary is not an overlap, the solver hands it to
    // the later row ([min, max) for every row another one follows on, [min, max] for the last). Returns
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
            if (previous !== null && range.min < previous.max) {
                errors.push('Row ' + row + ' overlaps row ' + (previousIndex + 1) + ': ranges must be ascending; a row may start at ' + previous.max + ', where the previous one ends.');
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
        focusPropertiesField('button[data-action="add"]');
    }

    // "Clear" of either editor asks for confirmation first — one stray click must not drop a
    // hand-built list. The modal is opened here rather than in the click handler so the count in the
    // message is read from the level, not the event.
    function clearUniqueValues(level) {
        openConfirmModal('Clear Values', 'Remove all ' + level.uniqueValueColors.length + ' values?', function () {
            level.uniqueValueColors = [];
            uniqueValuesMessage = null;
            renderProperties();
            focusPropertiesField('button[data-action="clear"]');
        });
    }

    function clearRanges(level) {
        openConfirmModal('Clear Ranges', 'Remove all ' + level.ranges.length + ' ranges?', function () {
            level.ranges = [];
            uniqueValuesMessage = null;
            renderProperties();
            focusPropertiesField('button[data-action="clear"]');
        });
    }

    function setUniqueValueColor(level, index, color) {
        if (index >= 0 && index < level.uniqueValueColors.length) {
            level.uniqueValueColors[index].color = color;
        }
    }

    // The administrative area that scopes the values load — a load scope, not part of the definition,
    // so it lives beside the state rather than on a level. null until an area is chosen, the only
    // state in which no load can start. Picked in the same modal as Load:
    // { name, code, administrativeArealType, countyIds }, where countyIds is the resolved list of
    // county part ids (null until resolved, empty for a country, which loads the whole table).
    let uniqueValuesScope = null;
    // The in-flight request (so a second click or a level switch cancels the first), the uniqueId of
    // the level whose values are loading, the progress line shown while loading, and the outcome
    // message of the last load.
    let uniqueValuesAbortController = null;
    let uniqueValuesLoadingId = null;
    let uniqueValuesProgress = null;
    let uniqueValuesMessage = null;
    // The upstream endpoint filters by one county part at a time and takes several seconds per part,
    // so an area is loaded county by county, a few in flight at once, and every answer is merged as it
    // arrives.
    const uniqueValuesConcurrency = 3;

    // A values load is capped: a colour-per-value list longer than this is too large to work with, so
    // the load is refused with a message instead of rendering the values.
    const uniqueValuesLimit = 100;

    // The number of ranges a Load generates for a range rule — the visitor's choice in the range editor
    // (issue #37; #30 fixed it at 4, #34 grew it with the building count up to 50, which gave 50 uneven
    // ranges on any county). A Load setting like the area, not part of the definition, so it is page
    // state rather than level state and survives a level switch.
    const generatedRangeCountDefault = 10;
    const generatedRangeCountMin = 2;
    const generatedRangeCountMax = 50;
    let generatedRangeCount = generatedRangeCountDefault;

    // The drift count-aware rounding accepts (issue #37): a rounded bound may move its position on the
    // merged CDF by at most this fraction of one range's share — a tenth of a tenth is one percentage
    // point of the buildings. A candidate that moves more is refused, so a readable bound never empties
    // its neighbour the way the old value-relative snap (a fifth of the value) did.
    const generatedRangeShareTolerance = 0.1;

    // "Load" of either editor: the Load Area modal picks the scope, and a chosen area loads at once.
    // A unique-value rule merges every answer into its rows as it arrives (capped at
    // uniqueValuesLimit); a range rule collects each part's value-distribution histogram and, at the
    // end, replaces its rows with generatedRangeCount ranges that split the area's buildings by count
    // (issue #30) — one comparable share per range, instead of equal widths over a span set by outliers.
    function loadColumnValues(level) {
        const forRanges = isRangeRuleType(level.ruleType);
        // A range load asks each part for its histogram (issue #30); a unique-value load asks for its
        // distinct values. Both answer null for 204 and every failure, so the worker chain is shared.
        const fetchOne = forRanges ? fetchHistogram : fetchValues;
        abortUniqueValuesLoad();

        const abortController = new AbortController();
        uniqueValuesAbortController = abortController;
        uniqueValuesLoadingId = level.uniqueId;
        uniqueValuesProgress = null;
        uniqueValuesMessage = null;
        renderProperties();

        function current() {
            return uniqueValuesAbortController === abortController; // false once superseded by a later load or a level switch
        }

        function finish(message) {
            if (!current()) {
                return;
            }
            uniqueValuesAbortController = null;
            uniqueValuesLoadingId = null;
            uniqueValuesProgress = null;
            uniqueValuesMessage = message;
            renderProperties();
        }

        // A unique-value load whose merged values pass uniqueValuesLimit is refused: the rows it added
        // are rolled back to this snapshot, the county requests still in flight are cancelled, and the
        // outcome message says why. The rollback matters because a merge happens county by county —
        // without it a refused load would leave the values of its first few counties behind.
        const rowsBeforeLoad = level.uniqueValueColors.slice();

        function exceedLimit() {
            level.uniqueValueColors = rowsBeforeLoad;
            abortController.abort(); // the county requests still in flight; their AbortError lands in the catch below
            finish('The selected area has more than ' + uniqueValuesLimit + ' unique values — they cannot be loaded. Select a smaller area.');
        }

        // What one answer does: a unique-value load merges it (refusing past the limit, which stops
        // the worker chain); a range load collects the part's histogram rows — the quantiles need
        // every part before the distribution is known. Answers false when the load was refused.
        function accept(payload) {
            if (forRanges) {
                for (let i = 0; i < payload.length; i++) {
                    collectedBuckets.push(payload[i]);
                }
                return true;
            }
            mergeUniqueValues(level, payload);
            if (level.uniqueValueColors.length > uniqueValuesLimit) {
                exceedLimit();
                return false;
            }
            return true;
        }

        const collectedBuckets = [];

        function finishRanges(missedCounties) {
            const rangeCount = generatedRangeCount;
            const generated = quantileRanges(collectedBuckets, level.ruleType === ruleType_IntegerRange, rangeCount);
            if (generated.ranges.length === 0) {
                finish('No values returned.');
                return;
            }
            level.ranges = generated.ranges;
            const messageParts = ['Generated ' + generated.ranges.length + ' ranges from ' + generated.total + ' buildings (' +
                displayValue(generated.min) + ' – ' + displayValue(generated.max) + ')'];
            if (generated.ranges.length > 1) {
                messageParts.push('each holds ' + displayShare(generated.shareMin) + ' – ' + displayShare(generated.shareMax) + ' of them');
            }
            if (generated.ranges.length < rangeCount) {
                messageParts.push(rangeCount + ' were asked — the rest fell on the same bound, the values repeat too much to split further');
            }
            if (missedCounties > 0) {
                messageParts.push(missedCounties + ' counties answered nothing');
            }
            finish(messageParts.join('; ') + '.');
        }

        // One histogramsummary request per county part (issue #30); resolves to the bucket rows, or
        // null for 204 and every failure — 204 from the relay covers an empty column and an upstream
        // timeout alike, so a part that answers nothing is simply missed by the merge.
        function fetchHistogram(countyId) {
            let url = baseUrl() + '/typology/histogramsummary?columnuniqueid=' + encodeURIComponent(level.uniqueId);
            if (countyId !== null) {
                url += '&countyid=' + encodeURIComponent(String(countyId));
            }
            return fetch(url, { signal: abortController.signal })
                .then(function (response) {
                    if (response.status === 204 || !response.ok) {
                        return null;
                    }
                    return response.json();
                })
                .then(function (buckets) {
                    return Array.isArray(buckets) ? buckets : null;
                })
                .catch(function (error) {
                    if (error !== null && error !== undefined && error.name === 'AbortError') {
                        throw error;
                    }
                    return null;
                });
        }

        // One uniquevalues request; resolves to the value array, or null for 204 and every failure —
        // 204 from the proxy covers an empty column, an unknown column and an upstream timeout alike.
        function fetchValues(countyId) {
            let url = baseUrl() + '/typology/uniquevalues?columnuniqueid=' + encodeURIComponent(level.uniqueId);
            if (countyId !== null) {
                url += '&countyid=' + encodeURIComponent(String(countyId));
            }
            return fetch(url, { signal: abortController.signal })
                .then(function (response) {
                    if (response.status === 204 || !response.ok) {
                        return null;
                    }
                    return response.json();
                })
                .then(function (values) {
                    return Array.isArray(values) ? values : null;
                })
                .catch(function (error) {
                    if (error !== null && error !== undefined && error.name === 'AbortError') {
                        throw error;
                    }
                    return null;
                });
        }

        function loadWholeTable() {
            return fetchOne(null).then(function (values) {
                if (!current()) {
                    return;
                }
                if (values === null) {
                    finish('No values returned — select an area; a load over the whole table can exceed the service timeout.');
                    return;
                }
                if (!accept(values)) {
                    return;
                }
                if (forRanges) {
                    finishRanges(0);
                    return;
                }
                finish(values.length === 0 ? 'No values returned.' : null);
            });
        }

        function loadCounties(countyIds) {
            const total = countyIds.length;
            let next = 0;
            let done = 0;
            let answered = 0;
            let added = 0;

            function progress() {
                uniqueValuesProgress = 'Loading values… ' + done + ' of ' + total + ' ' + (total === 1 ? 'county' : 'counties') + '.';
                renderProperties();
            }

            function worker() {
                if (!current() || next >= total) {
                    return Promise.resolve();
                }
                const countyId = countyIds[next++];
                return fetchOne(countyId).then(function (values) {
                    if (!current()) {
                        return;
                    }
                    done++;
                    if (values !== null) {
                        answered++;
                        const before = level.uniqueValueColors.length;
                        if (!accept(values)) {
                            return;
                        }
                        added += level.uniqueValueColors.length - before;
                    }
                    progress();
                    return worker();
                });
            }

            progress();
            const workers = [];
            for (let i = 0; i < uniqueValuesConcurrency && i < total; i++) {
                workers.push(worker());
            }
            return Promise.all(workers).then(function () {
                if (!current()) {
                    return;
                }
                if (forRanges) {
                    finishRanges(answered === 0 ? 0 : total - answered);
                    return;
                }
                if (answered === 0) {
                    finish('No values returned.');
                } else if (answered < total) {
                    finish(added + ' new ' + (added === 1 ? 'value' : 'values') + '; ' + (total - answered) + ' of ' + total + ' counties answered nothing.');
                } else {
                    finish(null);
                }
            });
        }

        function resolveCountyIds() {
            if (uniqueValuesScope.countyIds !== null) {
                return Promise.resolve(uniqueValuesScope.countyIds);
            }
            const scope = uniqueValuesScope;
            const url = baseUrl() + '/typology/countyids?code=' + encodeURIComponent(scope.code) + '&administrativearealtype=' + scope.administrativeArealType;
            return fetch(url, { signal: abortController.signal })
                .then(function (response) {
                    if (response.status === 204 || !response.ok) {
                        return null;
                    }
                    return response.json();
                })
                .then(function (ids) {
                    if (!Array.isArray(ids)) {
                        return null;
                    }
                    scope.countyIds = ids;
                    return ids;
                });
        }

        // A load starts only from a chosen area — the confirm callback of the Load Area modal sets the
        // scope and loads at once — so every load resolves counties; a country resolves to none and
        // loads the whole table.
        const chain = resolveCountyIds().then(function (countyIds) {
            if (!current()) {
                return;
            }
            if (countyIds === null) {
                finish('The area could not be resolved into counties — try another area.');
                return;
            }
            return countyIds.length === 0 ? loadWholeTable() : loadCounties(countyIds);
        });

        chain.catch(function (error) {
            if (error !== null && error !== undefined && error.name === 'AbortError') {
                return;
            }
            finish('The load failed — try again.');
        });
    }

    // "Load" in Column Properties: the Load Area modal picks the scope; a chosen area starts a load
    // at once, so the button is one step rather than two.
    function selectUniqueValuesScope(level) {
        const container = propertiesContainer();
        openLoadModal({
            opener: container !== null ? container.querySelector('button[data-action="load"]') : null,
            title: 'Load Values From Area',
            confirm: function (target) {
                uniqueValuesScope = { name: target.name, code: target.code, administrativeArealType: target.administrativeArealType, countyIds: null };
                loadColumnValues(level);
            }
        });
    }


    // Splits the area's buildings into rangeCount ranges of comparable size (issues #30, #37) — the
    // cartographic "quantile" (equal count) classification.
    //
    // The parts answered with their histograms — one {rangeStart, rangeEnd, count} row per bucket: the
    // bucket's actual min/max and its building count. The relay asks for equal-COUNT buckets (ntile), so
    // every bucket holds a thousandth of its part's buildings and the resolution follows the buildings
    // rather than the value span (#37: equal-width buckets over a span set by outliers put 44 % of a
    // county in one bucket, and every bound placed inside it was interpolation). The parts' buckets sit
    // on their own grids, so the rows are modelled as masses rather than summed by bucket index: a bucket
    // is its count spread over [rangeStart, rangeEnd] (a point mass when they coincide — a tie, or the
    // overflow row width_bucket files a part's max into when an older host still answers equal width).
    // The sum is a piecewise-linear cumulative distribution, and the boundaries are its i / rangeCount
    // points, inverted exactly inside the segment that crosses the level — the same uniform-within-bucket
    // interpolation the histogram_quantile implementations use.
    //
    // Then the rounding, count-aware (#37): each interior bound takes the coarsest readable candidate
    // (1/2/2.5/5 x 10^n, then one, two and three significant digits, then the two-decimal grid; whole
    // numbers for integer rules) that stays strictly above the previous bound and moves the bound's
    // position on the CDF by no more than generatedRangeShareTolerance of one range's share. The first
    // Min floors and the last Max ceils the observed span, so every value is covered; the rows touch —
    // [min, max) except the last, which includes its Max, the DiGi.Typology boundary rule; and a row that
    // would be empty is dropped, so a narrow span or a column of repeating values (Storeys: half the
    // buildings have one) yields fewer than rangeCount rows, down to the single value [v, v].
    function quantileRanges(buckets, integer, rangeCount) {
        // Two decimals through a hundredth-scaled integer, with a hair of slack so a value already at two
        // decimals is not pushed a hundredth further by the floating-point product (72470.74 * 100 is not 7247074).
        function floor2(value) {
            return Math.floor(value * 100 + 1e-6) / 100;
        }

        function ceil2(value) {
            return Math.ceil(value * 100 - 1e-6) / 100;
        }

        function round2(value) {
            return Math.round(value * 100) / 100;
        }

        // The readable candidates for a bound, coarsest first: the 1/2/2.5/5 x 10^n "nice numbers" a reader
        // expects on a break or an axis (Heckbert's family, as refined by Wilkinson) at the value's own and
        // the next lower magnitude, then the value at one, two and three significant digits, then the grid
        // the rule lives on (whole numbers, or two decimals). Sorted by significant-digit count and, within
        // a count, by distance from the value, so the first acceptable candidate is the most readable one.
        // Integer rules keep whole numbers only.
        function candidates(value) {
            const fine = integer ? Math.round(value) : round2(value);
            if (!(Math.abs(value) > 0)) {
                return [fine];
            }
            const sign = value < 0 ? -1 : 1;
            const magnitude = Math.pow(10, Math.floor(Math.log10(Math.abs(value))));
            const found = new Map();

            function add(candidate) {
                const normalised = integer ? Math.round(candidate) : round2(candidate);
                if (Number.isFinite(normalised) && !found.has(normalised)) {
                    found.set(normalised, significantDigits(normalised));
                }
            }

            const niceFractions = [1, 2, 2.5, 5, 10];
            for (let i = 0; i < niceFractions.length; i++) {
                add(sign * niceFractions[i] * magnitude);
                add(sign * niceFractions[i] * magnitude / 10);
            }
            for (let digits = 1; digits <= 3; digits++) {
                add(Number(Math.abs(value).toPrecision(digits)) * sign);
            }
            add(0); // the most readable bound of all, for a level that sits near zero on a signed column
            add(fine);

            const list = Array.from(found.keys());
            list.sort(function (a, b) {
                const byDigits = found.get(a) - found.get(b);
                return byDigits !== 0 ? byDigits : Math.abs(a - value) - Math.abs(b - value);
            });
            return list;
        }

        // The count of significant digits of a candidate — its readability: 100 has one, 120 two, 116 three, 0 none.
        function significantDigits(value) {
            const digits = Math.abs(value).toPrecision(12).replace('.', '').replace(/^0+/, '').replace(/0+$/, '');
            return digits.length;
        }

        // One event per edge: the point-mass jump, and the slope delta — a spread bucket adds its
        // count/(stop - start) at its start edge and removes it at its stop edge.
        const events = new Map();

        function addEvent(edge, jump, slopeDelta) {
            let entry = events.get(edge);
            if (entry === undefined) {
                entry = { jump: 0, slope: 0 };
                events.set(edge, entry);
            }
            entry.jump += jump;
            entry.slope += slopeDelta;
        }

        let min = Infinity;
        let max = -Infinity;
        let total = 0;

        for (let i = 0; i < buckets.length; i++) {
            const bucket = buckets[i];
            if (bucket === null || typeof bucket !== 'object') {
                continue;
            }
            // A row without usable numeric bounds is dropped: Number(null) is 0, which would file the
            // bucket's mass at zero instead of skipping the row. The upstream JSON may answer null for
            // rangeStart or rangeEnd — an all-NULL partition 500s today (ZiolkowskiJakub/DiGi.PostgreSQL#6),
            // and a fixed upstream must not turn the absence of values into a value of zero here.
            const count = bucket.count;
            const start = bucket.rangeStart;
            const stop = bucket.rangeEnd;
            if (typeof count !== 'number' || !Number.isFinite(count) || count <= 0 ||
                typeof start !== 'number' || !Number.isFinite(start) ||
                typeof stop !== 'number' || !Number.isFinite(stop)) {
                continue;
            }
            const lo = Math.min(start, stop);
            const hi = Math.max(start, stop);
            if (lo < min) {
                min = lo;
            }
            if (hi > max) {
                max = hi;
            }
            total += count;
            if (hi === lo) {
                addEvent(lo, count, 0); // a bucket holding one value, including the bucket 0 overflow row
                continue;
            }
            const slope = count / (hi - lo);
            addEvent(lo, 0, slope);
            addEvent(hi, 0, -slope);
        }

        if (total === 0 || min === Infinity) {
            return { ranges: [], min: null, max: null, total: 0 };
        }

        // Sweep the edges in order: integrate the running slope over the previous segment, apply this
        // edge's point-mass jump, record the CDF, then apply the edge's slope delta for the next
        // segment. The CDF is piecewise-linear between the edges with a jump at every tie, ending at
        // the total; jumps[i] is the mass sitting exactly at edges[i], and cumulative[i] includes it.
        const edges = Array.from(events.keys()).sort(function (a, b) {
            return a - b;
        });
        const cumulative = new Array(edges.length);
        const jumps = new Array(edges.length);
        let running = 0;
        let slope = 0;
        for (let i = 0; i < edges.length; i++) {
            if (i > 0) {
                running += slope * (edges[i] - edges[i - 1]);
            }
            const entry = events.get(edges[i]);
            running += entry.jump;
            cumulative[i] = running;
            jumps[i] = entry.jump;
            slope += entry.slope;
        }

        // The first edge whose CDF reaches the absolute building level; when the level is crossed
        // strictly inside the sloped part of the previous segment, invert that segment exactly. A level
        // reached only by the jump at an edge — a tie: the equal-count buckets of a repeating value are
        // point masses — sits at the edge itself, so a bound placed there files the whole tie into the
        // row starting at it (the [min, max) rule), not a made-up fraction of it (issue #37; #30 smeared
        // a jump into the preceding segment, which put the bounds of a tied column short of the value).
        function valueAtCDF(level) {
            for (let i = 0; i < edges.length; i++) {
                if (cumulative[i] < level) {
                    continue;
                }
                if (i === 0) {
                    return edges[i];
                }
                const span = edges[i] - edges[i - 1];
                const rise = cumulative[i] - cumulative[i - 1] - jumps[i];
                if (span > 0 && rise > 0 && cumulative[i - 1] < level && level < cumulative[i - 1] + rise) {
                    return edges[i - 1] + (level - cumulative[i - 1]) * (span / rise);
                }
                return edges[i];
            }
            return edges[edges.length - 1];
        }

        function quantile(fraction) {
            return valueAtCDF(fraction * total);
        }

        // The forward of valueAtCDF: the buildings with value strictly below v — the linear interpolation
        // through the sloped part of the segment holding v, without the jump at v when v is an edge, so a
        // bound at a tie counts the tie into the row starting there, as the [min, max) rule files it. 0 at
        // or below the first edge, the total above the last. The exact forward of valueAtCDF, so the
        // count-aware rounding and the reported shares stay consistent with how the split placed its bounds.
        function countBelow(v) {
            if (v <= edges[0]) {
                return 0;
            }
            if (v > edges[edges.length - 1]) {
                return total;
            }
            for (let i = 0; i < edges.length - 1; i++) {
                if (v <= edges[i + 1]) {
                    const span = edges[i + 1] - edges[i];
                    const rise = cumulative[i + 1] - cumulative[i] - jumps[i + 1];
                    return cumulative[i] + (v - edges[i]) * (rise / span);
                }
            }
            return total;
        }

        // rangeCount - 1 interior levels, evenly spaced across the CDF: i / rangeCount for i = 1 .. rangeCount - 1
        // (issue #30 placed 25/50/75 %; #37 makes the count the visitor's).
        const rawBounds = [];
        for (let i = 1; i < rangeCount; i++) {
            rawBounds.push(quantile(i / rangeCount));
        }

        let first;
        let last;
        if (integer) {
            first = Math.round(min);
            last = Math.round(max);
        } else {
            first = floor2(min);
            last = ceil2(max);
        }

        // Count-aware rounding (#37): each interior bound, in order, takes the coarsest of its candidates
        // that stays strictly above the previous kept bound and below the last, and whose position on the
        // CDF is within the tolerance of the level the bound stands for — so a readable number is taken
        // only when it keeps the share, and the two-decimal (or whole-number) value otherwise. A bound
        // whose every candidate collides with its neighbour is dropped: the level falls inside a run of
        // one repeated value (a tie), and the rows on either side of it are the same row.
        const tolerance = generatedRangeShareTolerance * total / rangeCount;

        const snapped = [];
        let keptBound = first;
        for (let i = 0; i < rawBounds.length; i++) {
            const bound = rawBounds[i];
            const level = (i + 1) / rangeCount * total;
            const list = candidates(bound);
            let chosen = null;
            for (let c = 0; c < list.length; c++) {
                const candidate = list[c];
                if (candidate <= keptBound || candidate >= last) {
                    continue;
                }
                if (Math.abs(countBelow(candidate) - level) <= tolerance) {
                    chosen = candidate;
                    break;
                }
            }
            if (chosen === null) {
                // No readable candidate keeps the share; the bound on the rule's own grid (a whole number, or
                // two decimals) is as exact as the rule allows and is kept when it still fits between its
                // neighbours — a tie collapses it onto the previous bound, and the row is dropped.
                const fine = integer ? Math.round(bound) : round2(bound);
                if (fine > keptBound && fine < last) {
                    chosen = fine;
                }
            }
            if (chosen !== null) {
                snapped.push(chosen);
                keptBound = chosen;
            }
        }

        // Build the touching [min, max) rows (the last includes its Max — the DiGi.Typology boundary
        // rule); the bounds are strictly increasing by construction, and a span that rounds to a single
        // value is one closed row [v, v].
        const bounds = [first];
        for (let i = 0; i < snapped.length; i++) {
            bounds.push(snapped[i]);
        }
        if (last > bounds[bounds.length - 1]) {
            bounds.push(last);
        }

        const ranges = [];
        let shareMin = 1;
        let shareMax = 0;
        if (bounds.length === 1) {
            ranges.push({ min: first, max: last, color: nextColor(0) });
            shareMin = 1;
            shareMax = 1;
        } else {
            for (let i = 0; i < bounds.length - 1; i++) {
                const rangeMin = bounds[i];
                const rangeMax = bounds[i + 1];
                ranges.push({ min: rangeMin, max: rangeMax, color: nextColor(ranges.length) });
                // The share the row holds on the same CDF the split was placed on — [min, max) for every row
                // but the last, which includes its Max — so the message names the spread and the visitor sees
                // at once when a tie or a coarse bound has skewed a row.
                const isLast = i === bounds.length - 2;
                const share = ((isLast ? total : countBelow(rangeMax)) - countBelow(rangeMin)) / total;
                if (share < shareMin) {
                    shareMin = share;
                }
                if (share > shareMax) {
                    shareMax = share;
                }
            }
        }

        return { ranges: ranges, min: first, max: last, total: total, shareMin: shareMin, shareMax: shareMax };
    }

    function abortUniqueValuesLoad() {
        if (uniqueValuesAbortController !== null) {
            uniqueValuesAbortController.abort();
            uniqueValuesAbortController = null;
        }
        uniqueValuesLoadingId = null;
        uniqueValuesProgress = null;
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

        // The name, the catalog's description of the column (when it carries one), then the rule type on
        // one row. A text-based column offers unique values only: the Ranges option is absent rather than
        // disabled, so the choice the select shows is exactly the choice the column admits.
        const column = columnByUniqueId(level.uniqueId);
        const description = column !== undefined && typeof column.description === 'string' ? column.description.trim() : '';
        let html =
            '<p class="gis-typology-properties-name">' + escapeHtml(level.name || level.uniqueId || '') + '</p>' +
            (description !== '' ? '<p class="gis-typology-properties-description">' + escapeHtml(description) + '</p>' : '') +
            '<label class="gis-field-label gis-field-inline"><span>Rule type</span>' +
            '<select data-field="ruleType" class="gis-select">' +
            '<option value=""' + (kind === '' ? ' selected' : '') + '>— choose —</option>' +
            '<option value="unique"' + (kind === 'unique' ? ' selected' : '') + '>Unique values</option>' +
            (rangeRuleType !== null ? '<option value="range"' + (kind === 'range' ? ' selected' : '') + '>Ranges</option>' : '') +
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

    // The chrome both editors share: the Add / Load / Clear row — the same three actions in the same
    // order for both rule kinds, so switching a level's kind never moves the buttons under the pointer.
    // Add and Load wait out a running load; Clear waits out an empty list. Load picks its area in the
    // Load Area modal every time, so no scope line is shown. The range editor adds its Load setting —
    // the number of ranges to generate (#37) — after the buttons; the unique-value editor has none.
    function renderEditorActions(loading, listEmpty, settings) {
        return '<div class="gis-typology-actions">' +
            '<button type="button" class="gis-button" data-action="add"' + (loading ? ' disabled' : '') + '>Add</button>' +
            '<button type="button" class="gis-button" data-action="load"' + (loading ? ' disabled' : '') + '>Load</button>' +
            '<button type="button" class="gis-button gis-button-secondary" data-action="clear"' + (listEmpty ? ' disabled' : '') + '>Clear</button>' +
            (settings || '') +
            '</div>';
    }

    // The range count a Load generates: a short number field on the action row, page state rather than
    // level state (a Load setting like the area), clamped to generatedRangeCountMin..Max on input.
    function renderRangeCountField(loading) {
        return '<label class="gis-typology-range-count" title="How many ranges Load generates, each holding about the same number of buildings">' +
            '<span>Ranges</span>' +
            '<input type="number" data-field="generatedRangeCount" min="' + generatedRangeCountMin + '" max="' + generatedRangeCountMax + '" step="1" value="' + generatedRangeCount + '"' +
            (loading ? ' disabled' : '') + ' aria-label="Number of ranges Load generates" />' +
            '</label>';
    }

    function renderLoading() {
        return '<div class="gis-loader"></div><p class="gis-loader-text">' + escapeHtml(uniqueValuesProgress || 'Loading values…') + '</p>';
    }

    function renderRangeEditor(level) {
        const loading = uniqueValuesLoadingId === level.uniqueId;
        const integer = level.ruleType === ruleType_IntegerRange;
        const step = integer ? '1' : 'any';

        let rows;
        if (loading) {
            rows = renderLoading();
        } else if (level.ranges.length === 0) {
            rows = '<div class="gis-empty-state">' + escapeHtml(uniqueValuesMessage || 'No ranges — add one or load them from an area.') + '</div>';
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
            if (uniqueValuesMessage !== null) {
                rows += '<p class="gis-typology-hint">' + escapeHtml(uniqueValuesMessage) + '</p>';
            }
        }

        return renderEditorActions(loading, level.ranges.length === 0, renderRangeCountField(loading)) +
            '<div class="gis-typology-ranges" aria-describedby="typology-range-errors">' + rows + '</div>' +
            '<ul id="typology-range-errors" class="gis-typology-errors" role="alert"></ul>' +
            '<p class="gis-typology-hint">Ascending, non-overlapping intervals' + (integer ? ' of whole numbers' : '') + '; a row may start where the previous one ends, and that value belongs to the later row — [min, max) for every row but the last, which includes its Max. ' +
            'Add files the new range by its Min, so a gap between two rows can be filled. ' +
            'Load divides the buildings of an area into the number of ranges set beside it, each holding about the same number of buildings (a municipality or subdivision loads its whole county, a voivodeship every county in it); ' +
            'bounds are rounded to readable' + (integer ? ' whole' : '') + ' numbers only as far as that keeps the shares, and a value that repeats too much to split yields fewer ranges. ' +
            'Rows with no value in this column fall out of this level. For an open end use a sentinel (for years, 0 and 9999).</p>';
    }

    function renderUniqueValueEditor(level) {
        const loading = uniqueValuesLoadingId === level.uniqueId;

        let rows;
        if (loading) {
            rows = renderLoading();
        } else if (level.uniqueValueColors.length === 0) {
            rows = '<div class="gis-empty-state">' + escapeHtml(uniqueValuesMessage || 'No values yet — add one or load them from an area.') + '</div>';
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

        return renderEditorActions(loading, level.uniqueValueColors.length === 0) +
            '<div class="gis-typology-values">' + rows + '</div>' +
            '<p class="gis-typology-hint">Each distinct value is its own bucket; a missing value is bucketed as (null). ' +
            'Values load a county at a time — a municipality or subdivision loads its whole county, a voivodeship every county in it.</p>';
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

    // A share of the buildings as a percentage with one decimal ("8.9 %").
    function displayShare(fraction) {
        return (Math.round(fraction * 1000) / 10).toFixed(1) + ' %';
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

        // Bounds are written on every keystroke; the range validation refreshes in place so the caret
        // stays where the user left it.
        container.addEventListener('input', function (event) {
            const level = selectedLevel();
            const target = event.target;
            if (level === null || target === null || target.getAttribute === undefined) {
                return;
            }
            const field = target.getAttribute('data-field');
            if (field === 'min' || field === 'max') {
                setRangeBound(level, rowIndex(target), field, target.value);
            } else if (field === 'color') {
                setRowColor(level, target); // live preview while the native picker is open; change commits the same value
            } else if (field === 'generatedRangeCount') {
                setGeneratedRangeCount(target.value); // a Load setting, read at the next Load; no re-render, the field shows it
            }
        });

        // The three editor actions are shared by both rule kinds; the active kind decides what Add
        // and Clear act on. The per-row × buttons act on their own row.
        container.addEventListener('click', function (event) {
            const level = selectedLevel();
            const button = event.target.closest !== undefined ? event.target.closest('button[data-action]') : null;
            if (level === null || button === null) {
                return;
            }
            const action = button.getAttribute('data-action');
            const forRanges = isRangeRuleType(level.ruleType);
            if (action === 'add') {
                if (forRanges) {
                    openAddRangePrompt(level);
                } else {
                    openAddValuePrompt(level);
                }
            } else if (action === 'load') {
                selectUniqueValuesScope(level);
            } else if (action === 'clear') {
                if (forRanges) {
                    clearRanges(level);
                } else {
                    clearUniqueValues(level);
                }
            } else if (action === 'remove-range') {
                removeRange(level, rowIndex(button));
            } else if (action === 'remove-value') {
                removeUniqueValue(level, rowIndex(button));
            }
        });

        // Enter in the last Max acts like the row's natural next step instead of doing nothing: the
        // Add prompt for the range after it.
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
                openAddRangePrompt(level);
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
        if (!replaceDefinition(definition)) {
            showDefinitionErrors('Import', ['The definition could not be read.']);
        }
    }

    // The one place the page state is replaced wholesale: by an imported file (applyDefinition) and by the
    // definition the last Load filed (restoreDefinition). Answers whether the shape was a definition at all.
    function replaceDefinition(definition) {
        if (definition === null || typeof definition !== 'object' || !Array.isArray(definition.levels)) {
            return false;
        }

        abortUniqueValuesLoad();
        uniqueValuesMessage = null;

        const levels = [];
        for (let i = 0; i < definition.levels.length; i++) {
            const level = definition.levels[i];
            if (level === null || typeof level !== 'object') {
                continue;
            }
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
        return true;
    }

    // The definition the last Load filed for the area view comes back when the page is revisited in the
    // same tab - "Back to the definition page" and the browser's back button included - so the levels the
    // visitor built are not lost to the round trip. Silent: an absent, blocked or malformed entry simply
    // leaves the page empty, as a first visit is.
    function restoreDefinition() {
        let definition = null;
        try {
            definition = JSON.parse(window.sessionStorage.getItem(loadDefinitionStorageKey));
        } catch (error) {
            return;
        }
        if (definition === null || typeof definition !== 'object' || !Array.isArray(definition.levels) || definition.levels.length === 0) {
            return;
        }
        replaceDefinition(definition);
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

    // ----- Add prompt and Clear confirmation (#16) -----

    // One modal serves the two Add prompts — a single value box for the unique-value editor, a
    // min/max pair for the range editor. The fields are rebuilt on open; the confirm callback
    // validates and answers false to keep the modal open with the message under the fields.
    let promptModal = null;
    let promptModalConfirm = null;

    function ensurePromptModal() {
        if (promptModal !== null) {
            return promptModal;
        }

        promptModal = document.createElement('div');
        promptModal.className = 'gis-modal-overlay';
        promptModal.style.display = 'none';
        promptModal.innerHTML =
            '<div class="gis-card gis-modal gis-dialog-card" role="dialog" aria-modal="true" aria-labelledby="typology-prompt-title">' +
            '<h3 class="gis-title" id="typology-prompt-title"></h3>' +
            '<div id="typology-prompt-fields"></div>' +
            '<p id="typology-prompt-error" class="gis-typology-prompt-error" role="alert"></p>' +
            '<div class="gis-modal-buttons">' +
            '<button type="button" id="typology-prompt-cancel-button" class="gis-button gis-button-secondary">Cancel</button>' +
            '<button type="button" id="typology-prompt-ok-button" class="gis-button">OK</button>' +
            '</div></div>';
        document.body.appendChild(promptModal);

        promptModal.querySelector('#typology-prompt-cancel-button').addEventListener('click', closePromptModal);
        promptModal.querySelector('#typology-prompt-ok-button').addEventListener('click', confirmPromptModal);
        promptModal.addEventListener('click', function (event) {
            if (event.target === promptModal) {
                closePromptModal();
            }
        });
        promptModal.addEventListener('keydown', function (event) {
            if (event.key === 'Enter' && event.target.tagName === 'INPUT') {
                event.preventDefault();
                confirmPromptModal();
            }
        });
        document.addEventListener('keydown', function (event) {
            if (event.key === 'Escape' && promptModal.style.display !== 'none') {
                closePromptModal();
            }
        });

        return promptModal;
    }

    // fields: [{ name, label, type ('text'|'number'), step, value, placeholder }]
    function openPromptModal(settings) {
        const modal = ensurePromptModal();
        promptModalConfirm = typeof settings.confirm === 'function' ? settings.confirm : null;

        modal.querySelector('#typology-prompt-title').textContent = settings.title || '';

        const fields = settings.fields || [];
        modal.querySelector('#typology-prompt-fields').innerHTML = fields.map(function (field) {
            const type = field.type || 'text';
            const step = type === 'number' ? ' step="' + (field.step || 'any') + '"' : '';
            const value = typeof field.value === 'number' ? ' value="' + numberAttribute(field.value) + '"' : '';
            const placeholder = field.placeholder !== undefined ? ' placeholder="' + escapeHtml(field.placeholder) + '"' : '';
            return '<label class="gis-field-label">' + escapeHtml(field.label || '') +
                '<input type="' + type + '"' + step + value + placeholder + ' data-prompt-field="' + escapeHtml(field.name || '') + '" /></label>';
        }).join('');

        modal.querySelector('#typology-prompt-error').textContent = '';
        modal.style.display = 'flex';
        const input = modal.querySelector('#typology-prompt-fields input');
        if (input !== null) {
            input.focus();
        }
    }

    function closePromptModal() {
        if (promptModal !== null) {
            promptModal.style.display = 'none';
        }
        promptModalConfirm = null;
        focusPropertiesField('button[data-action="add"]'); // back to the button that opened it
    }

    function confirmPromptModal() {
        if (promptModalConfirm === null) {
            closePromptModal();
            return;
        }

        const inputs = promptModal.querySelectorAll('#typology-prompt-fields input');
        const values = [];
        for (let i = 0; i < inputs.length; i++) {
            values.push(inputs[i].value);
        }

        const errorElement = promptModal.querySelector('#typology-prompt-error');
        errorElement.textContent = '';
        if (promptModalConfirm(values, function (message) {
            errorElement.textContent = message;
        }) !== false) {
            closePromptModal();
        }
    }

    // "Add" of the unique-value editor: one value typed by hand, parsed to the column's type — the
    // same admission the export runs on every row (Query.TryConvertValue server side), so what the
    // prompt accepts is what the document can carry.
    function openAddValuePrompt(level) {
        const integer = level.dataType >= 1 && level.dataType <= dataType_IntegerMax;
        const floating = level.dataType > dataType_IntegerMax && level.dataType <= dataType_DecimalMax;
        const boolean = level.dataType === dataType_Bool;

        openPromptModal({
            title: 'Add Value',
            fields: [{
                name: 'value',
                label: integer ? 'Value (whole number)' : floating ? 'Value (number)' : boolean ? 'Value (true or false)' : 'Value',
                type: 'text'
            }],
            confirm: function (values, error) {
                const raw = String(values[0]).trim();
                if (raw === '') {
                    error('Enter a value.');
                    return false;
                }

                let value = null;
                if (integer) {
                    if (!/^-?\d+$/.test(raw)) {
                        error('Enter a whole number.');
                        return false;
                    }
                    value = Number(raw);
                } else if (floating) {
                    value = Number(raw);
                    if (!Number.isFinite(value)) {
                        error('Enter a number.');
                        return false;
                    }
                } else if (boolean) {
                    const lowered = raw.toLowerCase();
                    if (lowered !== 'true' && lowered !== 'false') {
                        error('Enter true or false.');
                        return false;
                    }
                    value = lowered === 'true';
                } else {
                    value = raw; // a textual column (String, DateTime, …) keeps the text
                }

                const key = valueKey(value);
                for (let i = 0; i < level.uniqueValueColors.length; i++) {
                    if (valueKey(level.uniqueValueColors[i].value) === key) {
                        error('"' + displayValue(value) + '" is already in the list.');
                        return false;
                    }
                }

                level.uniqueValueColors.push({ value: value, color: nextColor(level.uniqueValueColors.length) });
                renderProperties();
                return true;
            }
        });
    }

    // "Add" of the range editor: the prompt asks for both bounds up front, the start prefilled where
    // the last row ended — the carry-over the inline editor's Add used to make. The new row is filed by
    // its Min among the existing rows, not appended, so a gap can be filled ([1, 2) and [6, 7) take
    // [2, 3) between them); it must fit between its neighbours - start where or after the row before
    // it ends, end where or before the row after it starts. The checks mirror the inline validation,
    // so a row the list would outline red never enters it.
    function openAddRangePrompt(level) {
        const integer = level.ruleType === ruleType_IntegerRange;
        const last = level.ranges.length > 0 ? level.ranges[level.ranges.length - 1] : null;
        const start = last !== null && isBound(last.max) ? last.max : null;

        openPromptModal({
            title: 'Add Range',
            fields: [
                { name: 'min', label: 'Min', type: 'number', step: integer ? '1' : 'any', value: start },
                { name: 'max', label: 'Max', type: 'number', step: integer ? '1' : 'any' }
            ],
            confirm: function (values, error) {
                const min = parseBound(values[0]);
                const max = parseBound(values[1]);
                if (min === null || max === null) {
                    error('Enter both bounds.');
                    return false;
                }
                if (integer && (!Number.isInteger(min) || !Number.isInteger(max))) {
                    error('Enter whole numbers.');
                    return false;
                }
                if (min > max) {
                    error('Min exceeds Max.');
                    return false;
                }

                // The slot: after every row whose Min is below the new one. Rows with a bound still empty or
                // unparseable are not neighbours the new row can be checked against; the list outlines them.
                let index = 0;
                while (index < level.ranges.length && isBound(level.ranges[index].min) && level.ranges[index].min < min) {
                    index++;
                }
                const previous = index > 0 ? level.ranges[index - 1] : null;
                const next = index < level.ranges.length ? level.ranges[index] : null;

                if (next !== null && isBound(next.min) && next.min === min) {
                    error('A range already starts at ' + min + '.');
                    return false;
                }
                if (previous !== null && isBound(previous.max) && min < previous.max) {
                    error('Min must be at least ' + previous.max + ', where the range before it ends — ranges ascend and must not overlap.');
                    return false;
                }
                if (next !== null && isBound(next.min) && max > next.min) {
                    error('Max must be at most ' + next.min + ', where the range after it starts — ranges ascend and must not overlap.');
                    return false;
                }

                // A colour no row uses yet: the rows carry the palette by position, so the next one after the count is free.
                level.ranges.splice(index, 0, { min: min, max: max, color: nextColor(level.ranges.length) });
                renderProperties();
                return true;
            }
        });
    }

    function isBound(value) {
        return typeof value === 'number' && Number.isFinite(value);
    }

    // '' to null — an empty or unparseable box never enters the list as NaN.
    function parseBound(rawValue) {
        const trimmed = String(rawValue).trim();
        if (trimmed === '') {
            return null;
        }
        const value = Number(trimmed);
        return Number.isFinite(value) ? value : null;
    }

    // The Clear confirmation: Cancel is focused so one Enter through the dialog dismisses rather
    // than destroys.
    let confirmModal = null;
    let confirmModalCallback = null;

    function ensureConfirmModal() {
        if (confirmModal !== null) {
            return confirmModal;
        }

        confirmModal = document.createElement('div');
        confirmModal.className = 'gis-modal-overlay';
        confirmModal.style.display = 'none';
        confirmModal.innerHTML =
            '<div class="gis-card gis-modal gis-dialog-card" role="dialog" aria-modal="true" aria-labelledby="typology-confirm-title">' +
            '<h3 class="gis-title" id="typology-confirm-title"></h3>' +
            '<p id="typology-confirm-message" class="gis-dialog-message"></p>' +
            '<div class="gis-modal-buttons">' +
            '<button type="button" id="typology-confirm-cancel-button" class="gis-button gis-button-secondary">Cancel</button>' +
            '<button type="button" id="typology-confirm-ok-button" class="gis-button">OK</button>' +
            '</div></div>';
        document.body.appendChild(confirmModal);

        confirmModal.querySelector('#typology-confirm-cancel-button').addEventListener('click', closeConfirmModal);
        confirmModal.querySelector('#typology-confirm-ok-button').addEventListener('click', function () {
            const callback = confirmModalCallback;
            closeConfirmModal();
            if (callback !== null) {
                callback();
            }
        });
        confirmModal.addEventListener('click', function (event) {
            if (event.target === confirmModal) {
                closeConfirmModal();
            }
        });
        document.addEventListener('keydown', function (event) {
            if (event.key === 'Escape' && confirmModal.style.display !== 'none') {
                closeConfirmModal();
            }
        });

        return confirmModal;
    }

    function openConfirmModal(title, message, onOk) {
        const modal = ensureConfirmModal();
        confirmModalCallback = typeof onOk === 'function' ? onOk : null;
        modal.querySelector('#typology-confirm-title').textContent = title;
        modal.querySelector('#typology-confirm-message').textContent = message;
        modal.style.display = 'flex';
        modal.querySelector('#typology-confirm-cancel-button').focus();
    }

    function closeConfirmModal() {
        if (confirmModal !== null) {
            confirmModal.style.display = 'none';
        }
        confirmModalCallback = null;
        focusPropertiesField('button[data-action="clear"]'); // back to the button that opened it
    }

    // ----- load modal (#18) -----

    // The search goes through this application's own proxy (AdministrativeAreal2DController), which binds
    // [FromBody] string: the body is a raw JSON string with the JSON content type. The proxy relays to the
    // GIS Web API, which matches every administrative type by name and answers full reference paths -
    // each path's AdministrativeAreal2DReferences runs from the country down to the matched area.
    const loadSearchDelay = 300;
    const loadSearchMinimum = 2;
    const loadResultCap = 50;

    // The definition travels to the redirect target in sessionStorage rather than the query string: a
    // chain of levels with ranges and colours would bloat the URL past every reasonable limit. The area
    // view reads it to solve, and this page reads it back on its next visit in the tab (restoreDefinition).
    const loadDefinitionStorageKey = 'digiTypology.definition';

    // AdministrativeArealType on the wire: 1 Voivodeship, 2 County, 3 Municipality, 4 Subdivision. The
    // modal selects among all four; a Subdivision loads its own area (the district), not the municipality
    // or county before it in the path.
    const loadTypeNames = { 1: 'Voivodeship', 2: 'County', 3: 'Municipality', 4: 'Subdivision' };

    let loadSearchTimer = null;
    let loadSearchController = null;
    // Bumped on every abort, so a response that outlives its keystroke (or the modal's closing) is
    // dropped instead of overwriting the newer list. The abort itself only rejects the fetch, not a
    // response already parsed.
    let loadSearchSequence = 0;
    let loadRows = []; // { id, code, administrativeArealType, name } - the target of each rendered row
    let loadSelectionIndex = -1;
    let loadOpener = null; // the button that opened the modal, to return focus to when it closes
    // The modal serves two callers: Load (no callback - OK redirects to the area view) and "Select
    // area…" in Column Properties (OK hands the row to the callback and closes).
    let loadConfirm = null;
    const loadTitle_Default = 'Load Area';

    function abortLoadSearch() {
        if (loadSearchTimer !== null) {
            clearTimeout(loadSearchTimer);
            loadSearchTimer = null;
        }
        if (loadSearchController !== null) {
            loadSearchController.abort();
            loadSearchController = null;
        }
        loadSearchSequence++;
    }

    function openLoadModal(options) {
        const modal = document.getElementById('typology-load-modal');
        if (modal === null) {
            return;
        }

        const settings = options !== null && options !== undefined ? options : {};
        loadOpener = settings.opener !== null && settings.opener !== undefined ? settings.opener : document.getElementById('typology-load-button');
        loadConfirm = typeof settings.confirm === 'function' ? settings.confirm : null;

        const title = document.getElementById('typology-load-title');
        if (title !== null) {
            title.textContent = settings.title || loadTitle_Default;
        }

        const input = document.getElementById('typology-load-input');
        if (input !== null) {
            input.value = '';
        }

        showLoadMessage('Type at least ' + loadSearchMinimum + ' characters to search.');

        modal.style.display = 'flex';
        if (input !== null) {
            input.focus();
        }
    }

    function closeLoadModal() {
        const modal = document.getElementById('typology-load-modal');
        if (modal === null) {
            return;
        }

        abortLoadSearch();
        modal.style.display = 'none';
        loadConfirm = null;

        if (loadOpener !== null) {
            loadOpener.focus();
            loadOpener = null;
        }
    }

    // Every message state is a non-result state: the rows are gone, and with them any selection, so OK
    // must not confirm a selection the visitor can no longer see (a row picked before the query was
    // shortened below the minimum, or a failed search).
    function showLoadMessage(text) {
        loadRows = [];
        setLoadSelection(-1);

        const results = document.getElementById('typology-load-results');
        if (results !== null) {
            results.innerHTML = '<div class="gis-empty-state">' + escapeHtml(text) + '</div>';
        }
    }

    function searchLoadAreas(text) {
        abortLoadSearch();

        const results = document.getElementById('typology-load-results');
        if (results === null) {
            return;
        }

        results.innerHTML = '<div class="gis-loader"></div><p class="gis-loader-text">Searching areas…</p>';

        loadSearchController = new AbortController();
        const sequence = loadSearchSequence;

        fetch(baseUrl() + '/administrativeareal2D/administrativeareal2Dreferencepathsbyname', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(text),
            signal: loadSearchController.signal
        })
            .then(function (response) {
                // The proxy answers an empty 200 for blank text; that path is unreachable through the
                // minimum-length guard, but an empty body still has to parse as nothing, not throw.
                return response.text().then(function (responseText) {
                    if (!response.ok || responseText.trim() === '') {
                        return null;
                    }
                    try {
                        return JSON.parse(responseText);
                    } catch (error) {
                        return null;
                    }
                });
            })
            .then(function (paths) {
                if (sequence !== loadSearchSequence) {
                    return; // superseded by a newer keystroke or by the modal closing
                }
                if (paths === null) {
                    showLoadMessage('The search failed - try again.');
                    return;
                }
                renderLoadResults(Array.isArray(paths) ? paths : []);
            })
            .catch(function (error) {
                if (error !== null && error.name === 'AbortError') {
                    return;
                }
                if (sequence !== loadSearchSequence) {
                    return;
                }
                showLoadMessage('The search failed - try again.');
            });
    }

    function renderLoadResults(paths) {
        const results = document.getElementById('typology-load-results');
        if (results === null) {
            return;
        }

        loadRows = [];
        const rows = [];

        for (let i = 0; i < paths.length && loadRows.length < loadResultCap; i++) {
            const references = paths[i] !== null && Array.isArray(paths[i].AdministrativeAreal2DReferences) ? paths[i].AdministrativeAreal2DReferences : [];
            if (references.length === 0) {
                continue;
            }

            // The path runs from the country down to the matched area, so the last entry is the match - and it
            // is the target: a Subdivision loads its own district, not the municipality or county before it in
            // the path. The row keeps its full breadcrumb, and the redirect carries the subdivision's own id/code/type.
            const target = references[references.length - 1];
            const matchedTypeName = loadTypeNames[target.AdministrativeArealType] || 'Area';

            const breadcrumb =
                '<span class="gis-path-breadcrumb">' +
                references.map(function (reference) {
                    return '<span class="breadcrumb-item-wrapper">' + escapeHtml(reference.Name || '') + '</span>';
                }).join('<span class="breadcrumb-separator">&rsaquo;</span>') +
                '</span>';

            rows.push(
                '<div class="gis-typology-result-row" role="option" tabindex="0" aria-selected="false" data-load-index="' + loadRows.length + '">' +
                breadcrumb +
                '<span class="gis-typology-type-badge">' + escapeHtml(matchedTypeName) + '</span>' +
                '</div>'
            );

            loadRows.push({
                id: target.Id,
                code: target.Code,
                administrativeArealType: target.AdministrativeArealType,
                name: (target.Name || '') + ' (' + (loadTypeNames[target.AdministrativeArealType] || 'Area') + ')'
            });
        }

        if (loadRows.length === 0) {
            showLoadMessage('No matching areas.');
            return;
        }

        let html = rows.join('');
        const remaining = paths.length - loadRows.length;
        if (remaining > 0) {
            html += '<div class="gis-typology-more-row">' + remaining + ' more - keep typing to narrow the list.</div>';
        }

        setLoadSelection(-1);
        results.innerHTML = html;
    }

    function setLoadSelection(index) {
        loadSelectionIndex = index;

        const okButton = document.getElementById('typology-load-ok-button');
        if (okButton !== null) {
            okButton.disabled = index < 0;
        }

        const results = document.getElementById('typology-load-results');
        if (results !== null) {
            const rows = results.querySelectorAll('.gis-typology-result-row');
            for (let i = 0; i < rows.length; i++) {
                const selected = String(index) === rows[i].getAttribute('data-load-index');
                rows[i].setAttribute('aria-selected', selected ? 'true' : 'false');
                rows[i].classList.toggle('gis-typology-result-selected', selected);
            }
        }
    }

    // OK (and Enter in the search box): with a caller's callback, hand it the row and close; otherwise
    // file the definition where the target page reads it, then redirect with the selected area. The
    // type travels as the integer the wire already carries - never a name.
    function confirmLoadSelection() {
        if (loadSelectionIndex < 0 || loadSelectionIndex >= loadRows.length) {
            return;
        }

        const target = loadRows[loadSelectionIndex];
        if (target === null || target === undefined || !target.id) {
            return;
        }

        if (loadConfirm !== null) {
            const confirm = loadConfirm;
            closeLoadModal();
            confirm(target);
            return;
        }

        try {
            window.sessionStorage.setItem(loadDefinitionStorageKey, JSON.stringify(definitionPayload()));
        } catch (error) {
            // A full or blocked store must not eat the redirect; the target page treats absence as no
            // definition carried.
        }

        window.location.href = baseUrl() + '/typology/view?id=' + target.id +
            '&code=' + encodeURIComponent(target.code === null || target.code === undefined ? '' : target.code) +
            '&administrativearealtype=' + target.administrativeArealType;
    }

    function setupLoadEvents() {
        const modal = document.getElementById('typology-load-modal');
        const loadButton = document.getElementById('typology-load-button');
        if (modal === null || loadButton === null) {
            return;
        }

        loadButton.addEventListener('click', function () {
            openLoadModal();
        });

        const input = document.getElementById('typology-load-input');
        if (input !== null) {
            input.addEventListener('input', function () {
                if (loadSearchTimer !== null) {
                    clearTimeout(loadSearchTimer);
                    loadSearchTimer = null;
                }

                const text = input.value.trim();
                if (text.length < loadSearchMinimum) {
                    abortLoadSearch();
                    showLoadMessage('Type at least ' + loadSearchMinimum + ' characters to search.');
                    return;
                }

                loadSearchTimer = setTimeout(function () {
                    loadSearchTimer = null;
                    searchLoadAreas(text);
                }, loadSearchDelay);
            });

            // Enter confirms the current selection.
            input.addEventListener('keydown', function (event) {
                if (event.key === 'Enter') {
                    event.preventDefault();
                    confirmLoadSelection();
                }
            });
        }

        const results = document.getElementById('typology-load-results');
        if (results !== null) {
            results.addEventListener('click', function (event) {
                const row = event.target.closest !== undefined ? event.target.closest('.gis-typology-result-row') : null;
                if (row !== null && results.contains(row)) {
                    setLoadSelection(parseInt(row.getAttribute('data-load-index'), 10));
                }
            });

            results.addEventListener('keydown', function (event) {
                if (event.key !== 'Enter' && event.key !== ' ') {
                    return;
                }
                const row = event.target.closest !== undefined ? event.target.closest('.gis-typology-result-row') : null;
                if (row !== null && results.contains(row)) {
                    event.preventDefault(); // keep Space from scrolling the page
                    setLoadSelection(parseInt(row.getAttribute('data-load-index'), 10));
                }
            });
        }

        const cancelButton = document.getElementById('typology-load-cancel-button');
        if (cancelButton !== null) {
            cancelButton.addEventListener('click', closeLoadModal);
        }

        const okButton = document.getElementById('typology-load-ok-button');
        if (okButton !== null) {
            okButton.addEventListener('click', confirmLoadSelection);
        }

        modal.addEventListener('click', function (event) {
            if (event.target === modal) {
                closeLoadModal();
            }
        });

        document.addEventListener('keydown', function (event) {
            if (event.key === 'Escape' && modal.style.display !== 'none') {
                closeLoadModal();
            }
        });
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
        setupLoadEvents();
        renderSelected(); // the empty state, until the columns arrive
        renderProperties();
        restoreDefinition();
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
