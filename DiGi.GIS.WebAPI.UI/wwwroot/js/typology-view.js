// Typology area view shell (issue #23): the 3D viewer's resizer/splitter and fold/unfold toggle
// logic ported from gltf-viewer.js, bound to the typology-* ids/classes and its own localStorage
// keys. It has no module imports, so it is loaded as a classic script (see Views/Typology/View.cshtml).
//
// Inverted defaults vs the 3D viewer are set in the markup (typology-panel-collapsed on the row,
// no typology-left-panel-collapsed), not here: the code below only toggles a class and reads the
// resulting state, so it is symmetric for either starting state.

// Right side panel resizer/splitter logic. The right panel is docked on the right, so dragging left
// (negative deltaX) increases its width. Widths are clamped to 200-600 and persisted under a
// typology-specific key so they do not share state with the 3D viewer.
(function () {
    const resizer = document.getElementById('typology-resizer');
    const sidePanel = document.getElementById('typology-side-panel');

    if (!resizer || !sidePanel) {
        return;
    }

    // Load saved width on start
    const savedWidth = localStorage.getItem('typology-side-panel-width');
    if (savedWidth) {
        const widthVal = parseInt(savedWidth, 10);
        if (widthVal >= 200 && widthVal <= 600) {
            sidePanel.style.flex = `0 0 ${widthVal}px`;
        }
    }

    resizer.addEventListener('mousedown', function (mouseDownEvent) {
        mouseDownEvent.preventDefault();
        resizer.classList.add('is-dragging');

        const startX = mouseDownEvent.clientX;
        const startWidth = sidePanel.getBoundingClientRect().width;

        function onMouseMove(mouseMoveEvent) {
            const deltaX = mouseMoveEvent.clientX - startX;
            // The side panel is on the right, so dragging left (negative deltaX) increases its width.
            const newWidth = Math.max(200, Math.min(600, startWidth - deltaX));

            sidePanel.style.flex = `0 0 ${newWidth}px`;

            // Dispatch resize so the centre viewport (and the header/footer collapse) refits instantly.
            window.dispatchEvent(new Event('resize'));
        }

        function onMouseUp() {
            resizer.classList.remove('is-dragging');
            document.removeEventListener('mousemove', onMouseMove);
            document.removeEventListener('mouseup', onMouseUp);

            // Save width preference
            const finalWidth = sidePanel.getBoundingClientRect().width;
            localStorage.setItem('typology-side-panel-width', finalWidth);

            // Final resize dispatch
            window.dispatchEvent(new Event('resize'));
        }

        document.addEventListener('mousemove', onMouseMove);
        document.addEventListener('mouseup', onMouseUp);
    });
})();

// Left side panel resizer/splitter logic. The left panel is docked on the left, so dragging right
// (positive deltaX) increases its width. Widths are clamped to 150-500 and persisted under a
// typology-specific key.
(function () {
    const resizer = document.getElementById('typology-left-resizer');
    const leftPanel = document.getElementById('typology-left-panel');

    if (!resizer || !leftPanel) {
        return;
    }

    // Load saved width on start
    const savedWidth = localStorage.getItem('typology-left-panel-width');
    if (savedWidth) {
        const widthVal = parseInt(savedWidth, 10);
        if (widthVal >= 150 && widthVal <= 500) {
            leftPanel.style.flex = `0 0 ${widthVal}px`;
        }
    }

    resizer.addEventListener('mousedown', function (mouseDownEvent) {
        mouseDownEvent.preventDefault();
        resizer.classList.add('is-dragging');

        const startX = mouseDownEvent.clientX;
        const startWidth = leftPanel.getBoundingClientRect().width;

        function onMouseMove(mouseMoveEvent) {
            const deltaX = mouseMoveEvent.clientX - startX;
            // The left panel is on the left, so dragging right (positive deltaX) increases its width.
            const newWidth = Math.max(150, Math.min(500, startWidth + deltaX));

            leftPanel.style.flex = `0 0 ${newWidth}px`;

            // Dispatch resize so the centre viewport (and the header/footer collapse) refits instantly.
            window.dispatchEvent(new Event('resize'));
        }

        function onMouseUp() {
            resizer.classList.remove('is-dragging');
            document.removeEventListener('mousemove', onMouseMove);
            document.removeEventListener('mouseup', onMouseUp);

            // Save width preference
            const finalWidth = leftPanel.getBoundingClientRect().width;
            localStorage.setItem('typology-left-panel-width', finalWidth);

            // Final resize dispatch
            window.dispatchEvent(new Event('resize'));
        }

        document.addEventListener('mousemove', onMouseMove);
        document.addEventListener('mouseup', onMouseUp);
    });
})();

// Right side panel fold/unfold. Collapsing the panel widens the centre viewport; the class toggle plus
// a resize dispatch is all that is needed (there is no 3D engine here, so the refit is purely the CSS
// layout reacting). Exposed as a small module (issue #26): the right panel is closed by default and the
// inspector opens it on the first tree selection and closes it when the selection clears, so the class,
// the aria state and the resize dispatch stay in one place whichever side drives them.
const digiTypologyLayout = (function () {
    'use strict';

    const layout = document.querySelector('.typology-layout');
    const toggle = document.getElementById('typology-panel-toggle');

    function isRightPanelCollapsed() {
        return layout !== null && layout.classList.contains('typology-panel-collapsed');
    }

    function setRightPanelCollapsed(collapsed) {
        if (layout === null || toggle === null) {
            return;
        }

        layout.classList.toggle('typology-panel-collapsed', collapsed);
        const label = collapsed ? 'Show panel' : 'Hide panel';
        toggle.setAttribute('aria-expanded', collapsed ? 'false' : 'true');
        toggle.setAttribute('aria-label', label);
        toggle.title = label;

        window.dispatchEvent(new Event('resize'));
    }

    if (toggle !== null) {
        toggle.addEventListener('click', function () {
            setRightPanelCollapsed(!isRightPanelCollapsed());
        });
    }

    return {
        isRightPanelCollapsed: isRightPanelCollapsed,
        setRightPanelCollapsed: setRightPanelCollapsed
    };
})();

// Left side panel fold/unfold toggle. The same symmetric logic as the right side.
(function initLeftPanelToggle() {
    const layout = document.querySelector('.typology-layout');
    const toggle = document.getElementById('typology-left-panel-toggle');
    if (!layout || !toggle) {
        return;
    }

    toggle.addEventListener('click', () => {
        const collapsed = layout.classList.toggle('typology-left-panel-collapsed');
        const label = collapsed ? 'Show panel' : 'Hide panel';
        toggle.setAttribute('aria-expanded', collapsed ? 'false' : 'true');
        toggle.setAttribute('aria-label', label);
        toggle.title = label;

        window.dispatchEvent(new Event('resize'));
    });
})();

// Page orchestration (issue #27): the single owner of the area view's page state. It fires the three
// fetches (outline, centroids, the solve) in parallel, routes each answer to its pure renderer - the
// map, the left panel and the inspector - and owns the selection's cross-cutting controls: the clear
// button and the Escape chain (error modal first, then the typology selection). The panels stay
// renderers: they draw the data handed to them and react to the one 'typology:selectionchange' event.
// This replaces issue #24's deliberately-minimal bootstrap; its window.digiTypologyView global is
// retired with it (no reader outside this file).
const digiTypologyView = (function () {
    'use strict';

    const shell = document.querySelector('.typology-shell');
    if (shell === null || typeof digiTypologyPanel === 'undefined') {
        return;
    }

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

    let activePath = null;
    let modalOpen = false;
    let errorModal = null;

    // ----- helpers -----

    function element(id) {
        return document.getElementById(id);
    }

    function baseUrl() {
        return (window.AppBaseUrl || '/').replace(/\/$/, '');
    }

    function escapeHtml(text) {
        return String(text)
            .replace(/&/g, '&amp;')
            .replace(/</g, '&lt;')
            .replace(/>/g, '&gt;')
            .replace(/"/g, '&quot;')
            .replace(/'/g, '&#39;');
    }

    // Never rejects: a network failure or a non-JSON body resolves to a null body, so the caller routes
    // on {ok, status} and a failed fetch degrades to its surface's "unavailable" line, not a console error.
    function fetchJson(url, options) {
        return fetch(url, options)
            .then(function (response) {
                return Promise.resolve(response.json())
                    .then(
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

    function showErrorModal(lines) {
        const modal = ensureErrorModal();
        modal.querySelector('#typology-solve-error-title').textContent = 'The typology could not be solved';
        let html = '';
        for (let i = 0; i < lines.length; i++) {
            html += '<li>' + escapeHtml(lines[i]) + '</li>';
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

    digiTypologyPanel.setSelectionCallback(function (path, node) {
        activePath = path === null ? null : path;
    });

    // One Escape chain, owned here: the error modal closes first, then the typology selection. The grid
    // claims the key itself only while a building is selected (typology-inspector.js), and the panel's
    // own branch is retired, so nothing else on the page answers Escape.
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
                    showErrorModal(messagesFromBody(result.body, 'The typology could not be solved.'));
                    return;
                }
                if (result.status === 503) {
                    showErrorModal(['The building data service is unavailable. The area outline is shown; the typology could not be solved.']);
                    return;
                }
                showErrorModal(['The typology could not be solved (HTTP ' + result.status + ').']);
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
