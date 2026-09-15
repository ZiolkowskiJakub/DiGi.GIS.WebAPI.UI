/**
 * Typology area view - the 3-column shell (issue #23): the 3D viewer's resizer/splitter and fold/unfold
 * toggle logic ported from gltf-viewer.js, bound to the typology-* ids/classes and its own localStorage
 * keys, written once for either side. It has no module imports, so it is loaded as a classic script
 * (Views/Typology/View.cshtml), before the view script that orchestrates the page.
 *
 * Inverted defaults vs the 3D viewer are set in the markup (typology-panel-collapsed on the row, no
 * typology-left-panel-collapsed), not here: the code below only toggles a class and reads the resulting
 * state, so it is symmetric for either starting state. The right panel's collapsed state is exposed
 * (isRightPanelCollapsed / setRightPanelCollapsed) because the inspector (#26) opens the panel on the first
 * tree selection and closes it when the selection clears, so the class, the aria state and the resize
 * dispatch stay in one place whichever side drives them.
 */
const digiTypologyLayout = (function () {
    'use strict';

    const layout = document.querySelector('.typology-layout');

    // A docked panel's splitter: dragging changes the panel's flex basis within [minimum, maximum], every move
    // dispatches resize so the centre viewport (and the header/footer collapse) refits instantly, and the final
    // width is persisted under the panel's own key so it does not share state with the 3D viewer. The direction
    // is +1 for a panel docked on the left (dragging right widens it) and -1 for one on the right.
    function setupResizer(resizerId, panelId, storageKey, minimum, maximum, direction) {
        const resizer = document.getElementById(resizerId);
        const panel = document.getElementById(panelId);
        if (resizer === null || panel === null) {
            return;
        }

        // Load the saved width on start; anything outside the clamp is ignored, not clamped.
        let savedWidth = null;
        try {
            savedWidth = localStorage.getItem(storageKey);
        } catch (error) {
            savedWidth = null;
        }
        if (savedWidth !== null) {
            const width = parseInt(savedWidth, 10);
            if (width >= minimum && width <= maximum) {
                panel.style.flex = '0 0 ' + width + 'px';
            }
        }

        resizer.addEventListener('mousedown', function (mouseDownEvent) {
            mouseDownEvent.preventDefault();
            resizer.classList.add('is-dragging');

            const startX = mouseDownEvent.clientX;
            const startWidth = panel.getBoundingClientRect().width;

            function onMouseMove(mouseMoveEvent) {
                const deltaX = (mouseMoveEvent.clientX - startX) * direction;
                const width = Math.max(minimum, Math.min(maximum, startWidth + deltaX));
                panel.style.flex = '0 0 ' + width + 'px';
                window.dispatchEvent(new Event('resize'));
            }

            function onMouseUp() {
                resizer.classList.remove('is-dragging');
                document.removeEventListener('mousemove', onMouseMove);
                document.removeEventListener('mouseup', onMouseUp);

                try {
                    localStorage.setItem(storageKey, String(panel.getBoundingClientRect().width));
                } catch (error) {
                    // A blocked store only loses the preference.
                }

                window.dispatchEvent(new Event('resize'));
            }

            document.addEventListener('mousemove', onMouseMove);
            document.addEventListener('mouseup', onMouseUp);
        });
    }

    // A corner toggle folds its panel by toggling the collapsed class on the row; the aria state and the title
    // follow, and a resize dispatch lets the CSS layout refit the centre (there is no 3D engine here).
    function setupToggle(toggleId, collapsedClass) {
        const toggle = document.getElementById(toggleId);
        if (layout === null || toggle === null) {
            return null;
        }

        function isCollapsed() {
            return layout.classList.contains(collapsedClass);
        }

        function setCollapsed(collapsed) {
            layout.classList.toggle(collapsedClass, collapsed);
            const label = collapsed ? 'Show panel' : 'Hide panel';
            toggle.setAttribute('aria-expanded', collapsed ? 'false' : 'true');
            toggle.setAttribute('aria-label', label);
            toggle.title = label;
            window.dispatchEvent(new Event('resize'));
        }

        toggle.addEventListener('click', function () {
            setCollapsed(!isCollapsed());
        });

        return { isCollapsed: isCollapsed, setCollapsed: setCollapsed };
    }

    // Right panel: docked on the right (dragging left widens it), clamped to 200-600.
    setupResizer('typology-resizer', 'typology-side-panel', 'typology-side-panel-width', 200, 600, -1);
    // Left panel: docked on the left (dragging right widens it), clamped to 150-500.
    setupResizer('typology-left-resizer', 'typology-left-panel', 'typology-left-panel-width', 150, 500, 1);

    const rightToggle = setupToggle('typology-panel-toggle', 'typology-panel-collapsed');
    setupToggle('typology-left-panel-toggle', 'typology-left-panel-collapsed');

    function isRightPanelCollapsed() {
        return rightToggle !== null && rightToggle.isCollapsed();
    }

    function setRightPanelCollapsed(collapsed) {
        if (rightToggle !== null) {
            rightToggle.setCollapsed(collapsed);
        }
    }

    return {
        isRightPanelCollapsed: isRightPanelCollapsed,
        setRightPanelCollapsed: setRightPanelCollapsed
    };
})();
