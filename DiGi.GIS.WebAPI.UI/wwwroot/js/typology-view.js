/**
 * Typology area view - page orchestration (issue #27): the single owner of the area view's page state.
 *
 * It fires the three fetches (outline, centroids, the solve) in parallel, routes each answer to its pure
 * renderer - the map (typology-map.js), the left panel (typology-panel.js) and the inspector
 * (typology-inspector.js) - and owns the selection's cross-cutting controls: the clear button and the
 * Escape chain (error modal first, then the typology selection). The panels stay renderers: they draw
 * the data handed to them and react to the one 'typology:selectionchange' event, which this script
 * follows too for the selection it needs. The shell (resizers, panel toggles) is typology-layout.js;
 * the shared helpers are typology-common.js; both load before this script. Classic script, no imports.
 */
(function () {
    'use strict';

    const shell = document.querySelector('.typology-shell');
    if (shell === null || typeof digiTypologyCommon === 'undefined' || typeof digiTypologyPanel === 'undefined') {
        return;
    }

    const common = digiTypologyCommon;
    const element = common.element;

    const area = {
        id: parseInt(shell.getAttribute('data-area-id'), 10),
        code: shell.getAttribute('data-area-code') || null,
        type: parseInt(shell.getAttribute('data-area-type'), 10)
    };

    // The same key typology.js writes in confirmLoadSelection. A blocked store reads as no definition.
    let definition = null;
    try {
        definition = JSON.parse(window.sessionStorage.getItem('digiTypology.definition'));
    } catch (error) {
        definition = null;
    }
    if (definition === null || typeof definition !== 'object' || !Array.isArray(definition.levels) || definition.levels.length === 0) {
        definition = null;
    }

    // The tree's root group carries the first column's name, so the group reads as what its children
    // classify (e.g. 'Floor area') instead of the generic 'Whole area' fallback.
    if (definition !== null) {
        common.setRootName(definition.levels[0].name);
    }

    let activePath = null;
    let modalOpen = false;
    let errorModal = null;

    // ----- helpers -----

    function baseUrl() {
        return (window.AppBaseUrl || '/').replace(/\/$/, '');
    }

    // Never rejects: a network failure or a non-JSON body resolves to a null body, so the caller routes
    // on {ok, status} and a failed fetch degrades to its surface's "unavailable" line, not a console error.
    function fetchJson(url, options) {
        return fetch(url, options)
            .then(function (response) {
                return response.json().then(
                    function (body) { return { ok: response.ok, status: response.status, body: body }; },
                    function () { return { ok: response.ok, status: response.status, body: null }; }
                );
            })
            .catch(function () {
                return { ok: false, status: 0, body: null };
            });
    }

    // 400 and 413 answer a JSON array of server messages; anything else is described by the fallback line.
    function messagesFromBody(body, fallback) {
        if (Array.isArray(body) && body.length > 0) {
            const messages = [];
            for (let i = 0; i < body.length; i++) {
                messages.push(String(body[i]));
            }
            return messages;
        }
        return [fallback];
    }

    // ----- loading state: the loader and the tree status line never show at once -----

    function showLoader() {
        const loader = element('typology-tree-loader');
        const status = element('typology-tree-status');
        if (loader !== null) {
            loader.hidden = false;
        }
        if (status !== null) {
            status.hidden = true;
        }
    }

    function hideLoader() {
        const loader = element('typology-tree-loader');
        if (loader !== null) {
            loader.hidden = true;
        }
    }

    // ----- error modal: the typology.js pattern, created on demand -----

    function ensureErrorModal() {
        if (errorModal !== null) {
            return errorModal;
        }

        errorModal = document.createElement('div');
        errorModal.className = 'gis-modal-overlay';
        errorModal.style.display = 'none';
        errorModal.innerHTML =
            '<div class="gis-card gis-modal gis-dialog-card" role="dialog" aria-modal="true" aria-labelledby="typology-solve-error-title">' +
            '<h3 class="gis-title gis-dialog-title-error" id="typology-solve-error-title"></h3>' +
            '<ul class="gis-dialog-message gis-typology-error-list"></ul>' +
            '<div class="gis-modal-buttons">' +
            '<button type="button" id="typology-solve-error-close" class="gis-button">Close</button>' +
            '</div></div>';
        document.body.appendChild(errorModal);

        errorModal.querySelector('#typology-solve-error-close').addEventListener('click', closeErrorModal);
        errorModal.addEventListener('click', function (event) {
            if (event.target === errorModal) {
                closeErrorModal();
            }
        });
        return errorModal;
    }

    // The modal carries the detail; the tree card keeps a one-line status behind it, so the card is not
    // blank once the modal is dismissed.
    function showError(lines) {
        digiTypologyPanel.showStatus('The typology could not be solved.');

        const modal = ensureErrorModal();
        modal.querySelector('#typology-solve-error-title').textContent = 'The typology could not be solved';
        let html = '';
        for (let i = 0; i < lines.length; i++) {
            html += '<li>' + common.escapeHtml(lines[i]) + '</li>';
        }
        modal.querySelector('.gis-typology-error-list').innerHTML = html;
        modal.style.display = 'flex';
        modalOpen = true;
        modal.querySelector('#typology-solve-error-close').focus();
    }

    function closeErrorModal() {
        if (errorModal !== null) {
            errorModal.style.display = 'none';
        }
        modalOpen = false;
    }

    // ----- selection ownership: one clear, one Escape chain -----

    function clearSelection() {
        digiTypologyPanel.clear();
    }

    const clearButton = element('typology-tree-clear');
    if (clearButton !== null) {
        clearButton.addEventListener('click', clearSelection);
    }

    document.addEventListener(common.selectionEventName, function (event) {
        activePath = event.detail !== null && event.detail !== undefined ? event.detail.path : null;
    });

    // One Escape chain, owned here: the error modal closes first, then the typology selection. The grid
    // claims the key itself only while a building is selected (typology-inspector.js), so nothing else on
    // the page answers Escape.
    document.addEventListener('keydown', function (event) {
        if (event.key !== 'Escape') {
            return;
        }
        if (modalOpen) {
            event.preventDefault();
            closeErrorModal();
            return;
        }
        if (event.defaultPrevented) {
            return; // the grid already claimed the key (it cleared its building)
        }
        if (activePath !== null) {
            event.preventDefault();
            clearSelection();
        }
    });

    // ----- context: the two read-only fetches the page degrades to without a definition -----

    function loadContext() {
        if (typeof digiTypologyMap === 'undefined') {
            return;
        }

        fetchJson(baseUrl() + '/administrativeareal2D/svg/polygonsbyid?id=' + area.id)
            .then(function (result) {
                digiTypologyMap.setOutline(result.ok && Array.isArray(result.body) ? result.body : null);
            });

        fetchJson(baseUrl() + '/building2D/point2dsbyadministrativeareal2Did?administrativeareal2Did=' + area.id)
            .then(function (result) {
                // A 204 is the API's upstream-failure answer; only a 200 with an array is a real list.
                digiTypologyMap.setCentroids(result.status === 200 && Array.isArray(result.body) ? result.body : null);
            });
    }

    // ----- the solve: one POST, routed on its status -----

    function solve() {
        showLoader();

        fetchJson(baseUrl() + '/typology/buildings', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ definition: definition, id: area.id, code: area.code, administrativeArealType: area.type })
        })
            .then(function (result) {
                hideLoader();

                if (result.ok) {
                    const viewModel = result.body !== null && typeof result.body === 'object' && !Array.isArray(result.body) ? result.body : null;
                    if (viewModel !== null && viewModel.root !== null && viewModel.root !== undefined) {
                        // The happy path: the one DTO routed to its three renderers.
                        if (typeof digiTypologyMap !== 'undefined') {
                            digiTypologyMap.render(viewModel);
                        }
                        digiTypologyPanel.render(viewModel);
                        if (typeof digiTypologyInspector !== 'undefined') {
                            digiTypologyInspector.render(viewModel, definition);
                        }
                        return;
                    }
                    // 200 with a null root: the panel carries the "answered no typology" empty state.
                    digiTypologyPanel.render(viewModel !== null ? viewModel : { root: null, buildings: [] });
                    return;
                }

                if (result.status === 404) {
                    digiTypologyPanel.showStatus('The area has no buildings to solve for.');
                    return;
                }
                if (result.status === 400 || result.status === 413) {
                    showError(messagesFromBody(result.body, 'The typology could not be solved.'));
                    return;
                }
                if (result.status === 503) {
                    showError(['The building data service is unavailable. The area outline is shown; the typology could not be solved.']);
                    return;
                }
                showError(['The typology could not be solved (HTTP ' + result.status + ').']);
            });
    }

    // ----- start -----

    if (!(area.id > 0)) {
        digiTypologyPanel.showStatus('The page was opened without an area.');
        return;
    }

    loadContext();

    if (definition === null) {
        // No definition carried: degrade to the area context; no solve is fired.
        digiTypologyPanel.showStatus('No definition was carried to this page. Define one on the typology page and press Load.');
        return;
    }

    solve();
})();
