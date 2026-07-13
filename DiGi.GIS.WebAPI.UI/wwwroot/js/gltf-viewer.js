// DiGi.GIS.WebAPI.UI — host integration for the generic DiGi.GLTF.WebAPI viewer engine.
// The core engine (gltf-viewer-core.js) renders the scene and handles navigation and
// selection; this script owns the GIS-specific UI around it: the lighting sliders,
// the properties panel (filled from the domain data attached to glTF extras when
// exactly one object is selected) and the scene information panel.

import { GltfViewer, readSceneData, readGlbBytes, fetchGlbBytes } from 'gltf-viewer-core';

const DEFAULT_PROPERTIES_HINT = 'Click an object or drag a selection rectangle in the 3D view. Properties are displayed when exactly one object is selected.';

function appendPropertyRow(table, key, value) {
    const row = table.insertRow();
    row.insertCell().textContent = key;

    const cell = row.insertCell();
    if (value !== null && typeof value === 'object') {
        const details = document.createElement('details');
        const summary = document.createElement('summary');
        summary.textContent = Array.isArray(value) ? `[${value.length}]` : '{…}';
        details.appendChild(summary);
        const pre = document.createElement('pre');
        pre.style.whiteSpace = 'pre-wrap';
        pre.style.margin = '4px 0 0 0';
        pre.textContent = JSON.stringify(value, null, 1);
        details.appendChild(pre);
        cell.appendChild(details);
    } else {
        cell.textContent = value === null || value === undefined ? '-' : String(value);
    }
}

// The properties panel shows details only for exactly one selected object;
// empty and multiple selections both reset it to the default state.
function fillProperties(viewer, references) {
    const panel = document.getElementById('gltf-properties');
    if (!panel) {
        return;
    }

    if (!references || references.length !== 1) {
        panel.innerHTML = `<span class="gltf-muted">${DEFAULT_PROPERTIES_HINT}</span>`;
        return;
    }

    const reference = references[0];
    const userData = viewer.getUserData(reference);

    const table = document.createElement('table');
    appendPropertyRow(table, 'Reference', reference || '-');

    if (userData) {
        for (const [key, value] of Object.entries(userData)) {
            if (key === '_type') {
                appendPropertyRow(table, 'Type', String(value).split(',')[0].split('.').pop());
                continue;
            }
            appendPropertyRow(table, key, value);
        }
    }

    panel.innerHTML = '';
    panel.appendChild(table);
}

function fillSceneInfo(referencePoint, objectCount) {
    const panel = document.getElementById('gltf-scene-info');
    if (!panel) {
        return;
    }

    const table = document.createElement('table');

    if (referencePoint) {
        appendPropertyRow(table, 'Reference point X', referencePoint.X);
        appendPropertyRow(table, 'Reference point Y', referencePoint.Y);
        appendPropertyRow(table, 'Reference point Z', referencePoint.Z);
    }

    appendPropertyRow(table, 'Objects', objectCount);

    panel.innerHTML = '';
    panel.appendChild(table);

    const note = document.createElement('div');
    note.className = 'gltf-muted';
    note.style.marginTop = '6px';
    note.textContent = 'Geometry is rendered around a local origin; the reference point stores the original world offset.';
    panel.appendChild(note);
}

function initLightingPanel(viewer) {
    const azimuthInput = document.getElementById('gltf-sun-azimuth');
    const altitudeInput = document.getElementById('gltf-sun-altitude');
    const sunIntensityInput = document.getElementById('gltf-sun-intensity');
    const ambientIntensityInput = document.getElementById('gltf-ambient-intensity');
    if (!azimuthInput || !altitudeInput || !sunIntensityInput || !ambientIntensityInput) {
        return;
    }

    const sunState = viewer.getSunState();
    azimuthInput.value = Math.round(sunState.azimuth);
    altitudeInput.value = Math.round(sunState.altitude);
    sunIntensityInput.value = sunState.intensity;
    ambientIntensityInput.value = sunState.ambientIntensity;

    const apply = () => {
        viewer.setSun(parseFloat(azimuthInput.value), parseFloat(altitudeInput.value));
        viewer.setSunIntensity(parseFloat(sunIntensityInput.value));
        viewer.setAmbientIntensity(parseFloat(ambientIntensityInput.value));

        document.getElementById('gltf-sun-azimuth-value').textContent = `${azimuthInput.value}°`;
        document.getElementById('gltf-sun-altitude-value').textContent = `${altitudeInput.value}°`;
        document.getElementById('gltf-sun-intensity-value').textContent = sunIntensityInput.value;
        document.getElementById('gltf-ambient-intensity-value').textContent = ambientIntensityInput.value;
    };

    for (const input of [azimuthInput, altitudeInput, sunIntensityInput, ambientIntensityInput]) {
        input.addEventListener('input', apply);
    }

    apply();
}

// Right side panel fold/unfold toggle. Collapsing the panel widens the viewer container; the
// engine's ResizeObserver then refits the 3D canvas automatically, so only the layout class is
// toggled here. Wired independently of the viewer so it works before the glTF payload finishes
// loading.
(function initPanelToggle() {
    const layout = document.querySelector('.gltf-layout');
    const toggle = document.getElementById('gltf-panel-toggle');
    if (!layout || !toggle) {
        return;
    }

    toggle.addEventListener('click', () => {
        const collapsed = layout.classList.toggle('gltf-panel-collapsed');
        const label = collapsed ? 'Show panel' : 'Hide panel';
        toggle.setAttribute('aria-expanded', collapsed ? 'false' : 'true');
        toggle.setAttribute('aria-label', label);
        toggle.title = label;
    });
})();

// Per-card fold/unfold. Every card in the right side panel becomes independently collapsible with a
// chevron toggle in its title, and all cards start folded. Each card's content is moved into a
// wrapper so a single class toggle shows or hides it; element ids inside the cards are preserved, so
// the viewer, lighting, properties and results code keeps finding them unchanged.
(function initCardFolding() {
    const panel = document.querySelector('.gltf-side-panel');
    if (!panel) {
        return;
    }

    const cards = panel.querySelectorAll('.gltf-card');
    cards.forEach((card) => {
        const title = card.querySelector('.gltf-card-title');
        if (!title) {
            return;
        }

        const label = title.textContent.trim();

        // Move everything after the title into a collapsible content wrapper.
        const content = document.createElement('div');
        content.className = 'gltf-card-content';
        let node = title.nextSibling;
        while (node) {
            const next = node.nextSibling;
            content.appendChild(node);
            node = next;
        }
        card.appendChild(content);

        const button = document.createElement('button');
        button.type = 'button';
        button.className = 'gltf-card-toggle';
        button.setAttribute('aria-label', 'Expand ' + label);
        button.setAttribute('aria-expanded', 'false');
        button.innerHTML = '<svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" aria-hidden="true"><path d="m6 9 6 6 6-6" /></svg>';
        title.appendChild(button);

        // Folded by default.
        card.classList.add('gltf-card-collapsed');

        function toggle() {
            const collapsed = card.classList.toggle('gltf-card-collapsed');
            button.setAttribute('aria-expanded', collapsed ? 'false' : 'true');
            button.setAttribute('aria-label', (collapsed ? 'Expand ' : 'Collapse ') + label);
        }

        // The whole title acts as the click target; the button stops propagation to avoid toggling twice.
        title.addEventListener('click', toggle);
        button.addEventListener('click', (event) => {
            event.stopPropagation();
            toggle();
        });
    });
})();

const container = document.getElementById('gltf-viewer-container');
if (container) {
    (async () => {
        const loader = document.getElementById('gltf-loader');

        try {
            const sceneData = readSceneData('gltf-scene-data');

            // Streamed delivery is preferred: the binary glTF payload is fetched from the glb endpoint
            // (raw binary, browser-cacheable). The embedded base64 payload is the fallback mode; its
            // decode is asynchronous so multi-megabyte scenes never block the UI thread.
            const glbUrl = container.dataset.glbUrl;
            const glbBuffer = glbUrl ? await fetchGlbBytes(glbUrl) : await readGlbBytes('gltf-glb-base64');

            if (!glbBuffer) {
                if (loader) {
                    loader.style.display = 'none';
                }

                const panel = document.getElementById('gltf-properties');
                if (panel) {
                    panel.innerHTML = '<span class="gltf-muted">No objects were found for this request.</span>';
                }
                return;
            }

            const viewer = new GltfViewer(container, sceneData, glbBuffer);

            // Exposed on window for debugging and UI automation.
            window.gltfViewer = viewer;

            container.addEventListener('gltf-ready', (event) => {
                if (loader) {
                    loader.style.display = 'none';
                }

                initLightingPanel(viewer);
                fillSceneInfo(event.detail.referencePoint, event.detail.objectCount);

                // The page shell does not know the object count upfront in streamed mode.
                const title = document.getElementById('gltf-title');
                if (title && event.detail.objectCount > 0) {
                    title.textContent = `${title.textContent} — ${event.detail.objectCount} objects`;
                }
            });

        container.addEventListener('gltf-selectionchanged', (event) => {
            fillProperties(viewer, event.detail.references);
        });
        } catch {
            if (loader) {
                loader.style.display = 'none';
            }

            const panel = document.getElementById('gltf-properties');
            if (panel) {
                panel.innerHTML = '<span class="gltf-muted">Failed to load the 3D scene.</span>';
            }
        }
    })();
}
