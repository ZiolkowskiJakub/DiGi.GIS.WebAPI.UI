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

// Solve bootstrap (issue #24): the definition the Load modal filed in sessionStorage is posted with the
// area from the shell's data-* attributes to POST /typology/buildings, and the answer is handed to the
// left panel renderer (typology-panel.js), the map (typology-map.js, issue #25) and the right panel
// inspector (typology-inspector.js, issue #26). The map's own loads (outline + centroids) start first and
// do not need a definition: a direct visit still shows the area outline with neutral dots. Deliberately
// minimal - the loading/error/empty-state UI and the shared page state belong to the orchestration
// sub-issue (#27); until then the panel's status line carries the outcome, and window.digiTypologyView
// holds the DTO and the selection for the other panels.
(function initSolve() {
    const shell = document.querySelector('.typology-shell');
    if (!shell || typeof digiTypologyPanel === 'undefined') {
        return;
    }

    window.digiTypologyView = { viewModel: null, selection: null };

    if (typeof digiTypologyMap !== 'undefined') {
        digiTypologyMap.load(shell.getAttribute('data-area-id'));
    }

    // The same key typology.js writes in confirmLoadSelection. A blocked store reads as no definition.
    let definition = null;
    try {
        definition = JSON.parse(window.sessionStorage.getItem('digiTypology.definition'));
    } catch (error) {
        definition = null;
    }

    if (definition === null || typeof definition !== 'object' || !Array.isArray(definition.levels) || definition.levels.length === 0) {
        digiTypologyPanel.showStatus('No definition was carried to this page. Go back to the definition page and press Load.');
        return;
    }

    const base = (window.AppBaseUrl || '/').replace(/\/$/, '');
    const areaId = parseInt(shell.getAttribute('data-area-id'), 10);
    const areaType = parseInt(shell.getAttribute('data-area-type'), 10);
    const areaCode = shell.getAttribute('data-area-code') || null;

    function statusText(status) {
        if (status === 404) {
            return 'The area has no buildings.';
        }
        if (status === 503) {
            return 'The building data service is unavailable.';
        }
        return 'The typology could not be solved (HTTP ' + status + ').';
    }

    fetch(base + '/typology/buildings', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ definition: definition, id: areaId, code: areaCode, administrativeArealType: areaType })
    })
        .then(function (response) {
            if (response.ok) {
                return response.json().then(function (viewModel) {
                    window.digiTypologyView.viewModel = viewModel;
                    digiTypologyPanel.setSelectionCallback(function (path, node) {
                        window.digiTypologyView.selection = path === null ? null : { path: path, node: node };
                    });
                    digiTypologyPanel.render(viewModel);
                    if (typeof digiTypologyMap !== 'undefined') {
                        digiTypologyMap.render(viewModel);
                    }
                    if (typeof digiTypologyInspector !== 'undefined') {
                        digiTypologyInspector.render(viewModel, definition);
                    }
                });
            }

            // 400 and 413 answer a list of messages; anything else is described by its status.
            return response.json()
                .catch(function () { return null; })
                .then(function (body) {
                    digiTypologyPanel.showStatus(Array.isArray(body) && body.length > 0 ? body.join(' ') : statusText(response.status));
                });
        })
        .catch(function () {
            digiTypologyPanel.showStatus('The request could not be sent.');
        });
})();
