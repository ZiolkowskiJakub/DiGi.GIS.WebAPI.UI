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

// Right side panel fold/unfold toggle. Collapsing the panel widens the centre viewport; the class
// toggle plus a resize dispatch is all that is needed (there is no 3D engine here, so the refit is
// purely the CSS layout reacting). Wired independently so it works regardless of panel content.
(function initRightPanelToggle() {
    const layout = document.querySelector('.typology-layout');
    const toggle = document.getElementById('typology-panel-toggle');
    if (!layout || !toggle) {
        return;
    }

    toggle.addEventListener('click', () => {
        const collapsed = layout.classList.toggle('typology-panel-collapsed');
        const label = collapsed ? 'Show panel' : 'Hide panel';
        toggle.setAttribute('aria-expanded', collapsed ? 'false' : 'true');
        toggle.setAttribute('aria-label', label);
        toggle.title = label;

        window.dispatchEvent(new Event('resize'));
    });
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
