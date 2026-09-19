/**
 * Orto Data verification page logic (issue #47).
 *
 * Draws the next unverified building (or the one named in the query), renders one card per photo
 * year the service listed - never a range generated here - and records the reviewer's answer:
 * a card (exact year), "existed in oldest photo" (at-or-before the oldest listed year) or
 * "not visible in newest photo" (after the newest listed year).
 *
 * Single-select throughout: a click selects exactly one of the cards and the two bound buttons;
 * a click on the selected one unselects it. Next submits the selection (skipping the POST
 * entirely when there is none), then draws the next building without a page reload.
 */

(function () {
    'use strict';

    const recentStorageKey = 'ortodataRecentKeys';
    const recentMax = 50;

    /**
     * Resolves a root-relative path relative to the application's base URL.
     * Same helper as building.js; kept local so this page does not load 343 lines of
     * paging and SVG code for one function.
     */
    function getResolvedUrl(path) {
        const baseUrl = window.AppBaseUrl || '/';
        const cleanBase = baseUrl.endsWith('/') ? baseUrl.slice(0, -1) : baseUrl;
        return `${cleanBase}${path.startsWith('/') ? path : '/' + path}`;
    }

    const elements = {
        county: document.getElementById('ortodata-county'),
        reference: document.getElementById('ortodata-reference'),
        provenance: document.getElementById('ortodata-provenance'),
        status: document.getElementById('ortodata-status'),
        grid: document.getElementById('ortodata-grid'),
        empty: document.getElementById('ortodata-empty'),
        details: document.getElementById('ortodata-details'),
        existed: document.getElementById('ortodata-existed'),
        notVisible: document.getElementById('ortodata-notvisible'),
        clear: document.getElementById('ortodata-clear'),
        next: document.getElementById('ortodata-next')
    };

    // The building on screen. mode is 'random' or 'direct'; countyId is always the building_2d
    // part the server resolved, never re-derived here.
    const state = {
        countyId: null,
        reference: null,
        years: [],
        mode: 'random',
        countyIds: []
    };

    // The on-screen answer: { year, relation } with relation 0 exact / 1 at-or-before / 2 after,
    // or null for no answer. The bound buttons are selections like a card is.
    let selection = null;
    let busy = false;

    function parseCountyIds() {
        const values = new URLSearchParams(window.location.search).getAll('countyids');
        const countyIds = [];
        for (const value of values) {
            const parsed = parseInt(value, 10);
            if (!isNaN(parsed)) {
                countyIds.push(parsed);
            }
        }
        return countyIds;
    }

    function readRecent() {
        try {
            const stored = sessionStorage.getItem(recentStorageKey);
            const parsed = stored ? JSON.parse(stored) : [];
            return Array.isArray(parsed) ? parsed : [];
        } catch (error) {
            return [];
        }
    }

    function writeRecent(recent) {
        try {
            sessionStorage.setItem(recentStorageKey, JSON.stringify(recent));
        } catch (error) {
            // A full or unavailable store only costs the repeat-avoidance, never the page.
        }
    }

    function isRecent(key) {
        return readRecent().indexOf(key) !== -1;
    }

    function pushRecent(key) {
        if (key === null || key === undefined) {
            return;
        }

        const recent = readRecent().filter(function (existing) { return existing !== key; });
        recent.push(key);
        while (recent.length > recentMax) {
            recent.shift();
        }
        writeRecent(recent);
    }

    function buildingKey() {
        return state.countyId === null || state.reference === null ? null : `${state.countyId}:${state.reference}`;
    }

    function countyIdsQuery(prefix) {
        if (state.countyIds.length === 0) {
            return '';
        }

        return prefix + state.countyIds.map(function (id) { return 'countyids=' + encodeURIComponent(id); }).join('&');
    }

    function showStatus(message) {
        elements.status.textContent = message;
        elements.status.hidden = false;
    }

    function hideStatus() {
        elements.status.hidden = true;
        elements.status.textContent = '';
    }

    function imageUrl(year) {
        const query = 'countyid=' + encodeURIComponent(state.countyId ?? '') +
            '&reference=' + encodeURIComponent(state.reference ?? '') +
            '&year=' + encodeURIComponent(year);
        return getResolvedUrl('/ortodata/image?' + query);
    }

    function setSelection(newSelection) {
        selection = newSelection;

        for (const card of elements.grid.querySelectorAll('.ortodata-card')) {
            const selected = selection !== null && selection.relation === 0 && parseInt(card.dataset.year, 10) === selection.year;
            card.classList.toggle('selected', selected);
            card.setAttribute('aria-pressed', selected ? 'true' : 'false');
        }

        elements.existed.setAttribute('aria-pressed', selection !== null && selection.relation === 1 ? 'true' : 'false');
        elements.notVisible.setAttribute('aria-pressed', selection !== null && selection.relation === 2 ? 'true' : 'false');
    }

    function selectCard(year) {
        if (selection !== null && selection.relation === 0 && selection.year === year) {
            setSelection(null);
            return;
        }

        setSelection({ year: year, relation: 0 });
    }

    function selectBound(relation) {
        if (selection !== null && selection.relation === relation) {
            setSelection(null);
            return;
        }

        // The bound answers name the oldest / newest listed year, so they need a listed year to name.
        if (state.years.length === 0) {
            return;
        }

        const year = relation === 1 ? state.years[0] : state.years[state.years.length - 1];
        setSelection({ year: year, relation: relation });
    }

    function buildCard(year) {
        const card = document.createElement('button');
        card.type = 'button';
        card.className = 'ortodata-card';
        card.dataset.year = String(year);
        card.setAttribute('aria-pressed', 'false');
        card.setAttribute('aria-label', `Orthophoto from ${year}`);

        const image = document.createElement('img');
        image.loading = 'lazy';
        image.alt = `Orthophoto from ${year}`;
        image.src = imageUrl(year);
        // A year the service listed but holds no photo for loses its card rather than showing a broken one.
        image.onerror = function () {
            card.hidden = true;
        };

        const label = document.createElement('span');
        label.className = 'ortodata-year';
        label.textContent = year;

        card.appendChild(image);
        card.appendChild(label);
        card.addEventListener('click', function () {
            selectCard(year);
        });

        return card;
    }

    function renderProvenance(selected) {
        if (selected === null || selected === undefined) {
            elements.provenance.hidden = true;
            elements.provenance.textContent = '';
            return;
        }

        const parts = [];
        if (selected.userName) {
            parts.push(`Recorded by ${selected.userName}`);
        }
        if (selected.dateTime) {
            const recorded = new Date(selected.dateTime);
            if (!isNaN(recorded.getTime())) {
                parts.push(`on ${recorded.toLocaleString()}`);
            }
        }

        if (parts.length === 0) {
            elements.provenance.hidden = true;
            elements.provenance.textContent = '';
            return;
        }

        elements.provenance.textContent = parts.join(' ');
        elements.provenance.hidden = false;
    }

    function render(data, mode) {
        state.countyId = data.countyId ?? null;
        state.reference = data.reference ?? null;
        state.years = Array.isArray(data.years) ? data.years : [];
        state.mode = mode;

        elements.county.textContent = state.countyId === null ? '\u2013' : String(state.countyId);
        elements.reference.textContent = state.reference === null ? '\u2013' : state.reference;

        renderProvenance(mode === 'direct' ? data.selected : null);

        elements.grid.textContent = '';
        for (const year of state.years) {
            elements.grid.appendChild(buildCard(year));
        }

        // The bound answers have nothing to name while no year is listed.
        const boundDisabled = state.years.length === 0;
        elements.existed.disabled = boundDisabled;
        elements.notVisible.disabled = boundDisabled;

        setSelection(null);
        hideStatus();
        elements.empty.hidden = true;
        elements.grid.hidden = false;

        if (state.years.length === 0) {
            showStatus('No photos are available for this building. Select Next to skip it.');
        }

        // Direct mode: the answer already on record lights the card or bound button it belongs to.
        const selected = mode === 'direct' ? data.selected : null;
        if (selected && selected.year !== null && selected.year !== undefined) {
            if (selected.relation === 1) {
                selectBound(1);
            } else if (selected.relation === 2) {
                selectBound(2);
            } else if (state.years.indexOf(selected.year) !== -1) {
                selectCard(selected.year);
            }
        }
    }

    function showEmpty() {
        state.countyId = null;
        state.reference = null;
        state.years = [];
        state.mode = 'random';

        elements.county.textContent = '\u2013';
        elements.reference.textContent = '\u2013';
        elements.grid.textContent = '';
        elements.grid.hidden = true;
        elements.provenance.hidden = true;
        elements.existed.disabled = true;
        elements.notVisible.disabled = true;
        setSelection(null);
        hideStatus();
        elements.empty.hidden = false;
    }

    async function readJson(response) {
        try {
            return await response.json();
        } catch (error) {
            return null;
        }
    }

    async function loadRandom(isRetry) {
        let response;
        try {
            response = await digiUser.fetchWithAuth(getResolvedUrl('/ortodata/random') + countyIdsQuery('?'), { method: 'GET' });
        } catch (error) {
            showStatus('The service could not be reached.');
            return;
        }

        if (!response.ok) {
            showStatus(`The next building could not be drawn (status ${response.status}).`);
            return;
        }

        const data = await readJson(response);
        if (data === null) {
            showStatus('The next building could not be read.');
            return;
        }

        if (!data.reference) {
            showEmpty();
            return;
        }

        // The draw is memoryless, so a nearly finished county hands a just-skipped building straight
        // back. One re-request when the answer is a recent one; give up after it so an exhausted
        // county still shows its last building.
        const key = `${data.countyId}:${data.reference}`;
        if (!isRetry && isRecent(key)) {
            await loadRandom(true);
            return;
        }

        render(data, 'random');
    }

    async function loadBuilding(countyId, reference) {
        // countyid is optional here as it is on the page address: without it the service resolves
        // the reference to the lowest county part holding it. encodeURIComponent(null) would put
        // the string "null" on the wire, which the server's integer binding refuses, so the part
        // is added only when it was given.
        const parts = [];
        if (countyId !== null && countyId !== undefined && countyId !== '') {
            parts.push('countyid=' + encodeURIComponent(countyId));
        }
        parts.push('reference=' + encodeURIComponent(reference));

        let response;
        try {
            response = await digiUser.fetchWithAuth(getResolvedUrl('/ortodata/building?' + parts.join('&')), { method: 'GET' });
        } catch (error) {
            showStatus('The service could not be reached.');
            return;
        }

        if (response.status === 404) {
            // Shown after showEmpty, which clears the status line as part of resetting the page.
            showEmpty();
            showStatus('Building not found.');
            return;
        }

        if (!response.ok) {
            showStatus(`The building could not be read (status ${response.status}).`);
            return;
        }

        const data = await readJson(response);
        if (data === null || !data.reference) {
            showEmpty();
            return;
        }

        render(data, 'direct');
    }

    async function next() {
        if (busy) {
            return;
        }

        busy = true;
        try {
            let notFound = false;

            if (selection !== null) {
                const body = {
                    countyId: state.countyId,
                    reference: state.reference,
                    year: selection.year,
                    relation: selection.relation
                };

                let response;
                try {
                    response = await digiUser.fetchWithAuth(getResolvedUrl('/ortodata/useryearbuilt'), {
                        method: 'POST',
                        headers: { 'Content-Type': 'application/json' },
                        body: JSON.stringify(body)
                    });
                } catch (error) {
                    showStatus('The service could not be reached. The answer was not saved.');
                    return;
                }

                if (!response.ok) {
                    if (response.status === 404) {
                        // No building under the named part holds the reference: a stale entry the
                        // page cannot fix, so it is reported and left behind. The message waits
                        // until after the next render, which clears the status line.
                        notFound = true;
                    } else {
                        showStatus(`The answer could not be saved (status ${response.status}).`);
                        return;
                    }
                }
            }

            pushRecent(buildingKey());

            // A direct-mode submit continues in random mode afterwards; the query values are
            // cleared so a refresh does not resubmit them, while a county filter is kept.
            if (state.mode === 'direct') {
                history.replaceState(null, '', getResolvedUrl('/ortodata') + countyIdsQuery('?'));
            }

            await loadRandom(false);

            // The toolbar is where the reviewer is working; after a re-render the focus returns to it.
            elements.next.focus();

            if (notFound) {
                showStatus('Building not found. The answer was not saved.');
            }
        } finally {
            busy = false;
        }
    }

    elements.existed.addEventListener('click', function () {
        selectBound(1);
    });

    elements.notVisible.addEventListener('click', function () {
        selectBound(2);
    });

    // Clear resets only the on-screen selection; it issues no request.
    elements.clear.addEventListener('click', function () {
        setSelection(null);
    });

    elements.next.addEventListener('click', next);

    elements.details.addEventListener('click', function () {
        const query = 'reference=' + encodeURIComponent(state.reference ?? '') + '&countyid=' + encodeURIComponent(state.countyId ?? '');
        window.open(getResolvedUrl('/building2D/detailsbyreference?' + query), '_blank');
    });

    state.countyIds = parseCountyIds();

    const queryParameters = new URLSearchParams(window.location.search);
    const directCountyId = queryParameters.get('countyid');
    const directReference = queryParameters.get('reference');
    if (directReference) {
        loadBuilding(directCountyId, directReference);
    } else {
        loadRandom(false);
    }
})();
