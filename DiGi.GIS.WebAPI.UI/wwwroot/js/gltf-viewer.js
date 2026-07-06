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

const container = document.getElementById('gltf-viewer-container');
if (container) {
    (async () => {
        const sceneData = readSceneData('gltf-scene-data');

        // Streamed delivery is preferred: the binary glTF payload is fetched from the glb endpoint
        // (raw binary, browser-cacheable). The embedded base64 payload is the fallback mode; its
        // decode is asynchronous so multi-megabyte scenes never block the UI thread.
        const glbUrl = container.dataset.glbUrl;
        const glbBuffer = glbUrl ? await fetchGlbBytes(glbUrl) : await readGlbBytes('gltf-glb-base64');

        if (!glbBuffer) {
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

        document.getElementById('gltf-fit-button')?.addEventListener('click', () => viewer.frameScene());
        document.getElementById('gltf-clear-button')?.addEventListener('click', () => viewer.clearSelection());
    })();
}
