// DiGi.GIS.WebAPI.UI — host integration for the generic DiGi.GLTF.WebAPI viewer engine.
// The core engine (gltf-viewer-core.js) renders the scene and handles navigation and
// selection; this script owns the GIS-specific UI around it: the lighting sliders,
// the properties panel (filled from the domain data attached to glTF extras when
// exactly one object is selected) and the scene information panel.

import { GltfViewer, GltfStatusTerminal, readSceneData, readGlbBytes, fetchGlbBytes, reportStatus } from 'gltf-viewer-core';

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

    const changeModal = document.getElementById('scene-change-modal');
    if (changeModal) {
        const changeButton = document.createElement('button');
        changeButton.type = 'button';
        changeButton.className = 'gis-button';
        changeButton.textContent = 'Change';
        changeButton.title = 'Change the analyzed circular area';
        changeButton.addEventListener('click', () => {
            const container = document.getElementById('gltf-viewer-container');
            if (!container) {
                return;
            }
            document.getElementById('scene-change-x').value = container.dataset.centerX || '';
            document.getElementById('scene-change-y').value = container.dataset.centerY || '';
            document.getElementById('scene-change-radius').value = container.dataset.radius || '';
            document.getElementById('scene-change-storey-height').value = container.dataset.storeyHeight || '';
            changeModal.style.display = 'flex';
        });
        panel.appendChild(changeButton);
    }

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

// Date/time driven sun position. The date picker and the hour slider send the scene's world
// location (the glTF reference point) and the selected moment to the sun-direction endpoint;
// the solar math and the GIS coordinate conversion live server side (DiGi.Solar / DiGi.GIS).
// The returned azimuth/altitude animate the two lighting sliders, which drive the scene light
// through the existing initLightingPanel wiring (synthetic 'input' events), so this function
// never touches the viewer directly.
const SUN_CLOCK_DEBOUNCE_MS = 150;   // one request per drag pause instead of one per tick
const SUN_CLOCK_TWEEN_MS = 500;      // smooth slider/light sweep to the fetched angles

function initSunClock(viewer, referencePoint) {
    const dateInput = document.getElementById('gltf-sun-date');
    const hourInput = document.getElementById('gltf-sun-hour');
    const hourValue = document.getElementById('gltf-sun-hour-value');
    const azimuthInput = document.getElementById('gltf-sun-azimuth');
    const altitudeInput = document.getElementById('gltf-sun-altitude');
    const container = document.getElementById('gltf-viewer-container');
    if (!dateInput || !hourInput || !azimuthInput || !altitudeInput || !container) {
        return;
    }

    const sunDirectionUrl = container.dataset.sunDirectionUrl;

    // World location of the scene: the glTF reference point holds the original world offset;
    // scene-parameter pages (communication views) carry the analyzed center as a data attribute.
    const x = referencePoint?.X ?? parseFloat(container.dataset.centerX);
    const y = referencePoint?.Y ?? parseFloat(container.dataset.centerY);
    if (!sunDirectionUrl || !isFinite(x) || !isFinite(y)) {
        dateInput.disabled = true;
        hourInput.disabled = true;
        dateInput.title = 'The scene carries no world location - the sun position cannot be computed.';
        return;
    }

    // Defaults: today, now (quarter-hour resolution). Built from local time parts - toISOString
    // would shift the date across the UTC boundary.
    const now = new Date();
    const pad = (value) => String(value).padStart(2, '0');
    dateInput.value = `${now.getFullYear()}-${pad(now.getMonth() + 1)}-${pad(now.getDate())}`;
    hourInput.value = Math.round((now.getHours() + now.getMinutes() / 60) * 4) / 4;

    const formatHour = (value) => {
        const hours = Math.floor(value);
        const minutes = Math.round((value - hours) * 60);
        return `${pad(hours === 24 ? 24 : hours)}:${pad(minutes)}`;
    };

    let debounceTimer = null;
    let abortController = null;
    let tweenTimer = null;
    let applyingTween = false;

    // A manual grab of the azimuth/altitude sliders takes priority over a running sweep. The
    // sweep itself re-dispatches 'input' on the same sliders, so those synthetic events are
    // flagged and ignored here.
    for (const input of [azimuthInput, altitudeInput]) {
        input.addEventListener('input', () => {
            if (!applyingTween && tweenTimer !== null) {
                clearInterval(tweenTimer);
                tweenTimer = null;
            }
        });
    }

    // Sweeps both sliders (and through their 'input' wiring the scene light) to the target
    // angles: azimuth along the shortest arc across the 0/360 wrap, altitude clamped to the
    // slider range (the backend reports negative altitudes at night; the light stays at the
    // slider minimum then). Timer driven so the sweep is independent of the render loop.
    const animateSunTo = (azimuth, altitude) => {
        if (tweenTimer !== null) {
            clearInterval(tweenTimer);
        }

        const startAzimuth = parseFloat(azimuthInput.value);
        const startAltitude = parseFloat(altitudeInput.value);
        const azimuthDelta = ((azimuth - startAzimuth + 540) % 360) - 180;
        const targetAltitude = Math.min(Math.max(altitude, parseFloat(altitudeInput.min)), parseFloat(altitudeInput.max));
        const start = performance.now();

        tweenTimer = setInterval(() => {
            const t = Math.min(1, (performance.now() - start) / SUN_CLOCK_TWEEN_MS);
            const eased = t * t * (3 - 2 * t); // smoothstep ease-in-out

            applyingTween = true;
            azimuthInput.value = ((startAzimuth + azimuthDelta * eased) % 360 + 360) % 360;
            altitudeInput.value = startAltitude + (targetAltitude - startAltitude) * eased;
            azimuthInput.dispatchEvent(new Event('input'));
            applyingTween = false;

            if (t >= 1) {
                clearInterval(tweenTimer);
                tweenTimer = null;
            }
        }, 16);
    };

    const requestSunDirection = () => {
        if (hourValue) {
            hourValue.textContent = formatHour(parseFloat(hourInput.value));
        }
        if (!dateInput.value) {
            return; // cleared date picker
        }

        clearTimeout(debounceTimer);
        debounceTimer = setTimeout(async () => {
            abortController?.abort();
            abortController = new AbortController();
            try {
                const url = `${sunDirectionUrl}?x=${encodeURIComponent(x)}&y=${encodeURIComponent(y)}` +
                    `&date=${encodeURIComponent(dateInput.value)}&hour=${encodeURIComponent(hourInput.value)}`;
                const response = await fetch(url, { signal: abortController.signal });
                if (!response.ok) {
                    return;
                }

                const data = await response.json();
                const azimuth = data.azimuth ?? data.Azimuth;
                const altitude = data.altitude ?? data.Altitude;
                if (!isFinite(azimuth) || !isFinite(altitude)) {
                    return;
                }

                // The endpoint returns the true solar azimuth (0 = north, clockwise). The viewer
                // places the sun with azimuth 0 at three.js +Z, which is geographic south in the
                // Z-up -> Y-up rotated scene, so the compass angle maps to 180 - azimuth.
                animateSunTo((((180 - azimuth) % 360) + 360) % 360, altitude);
            } catch {
                // Aborted by a newer request or the endpoint is unreachable - keep the current sun.
            }
        }, SUN_CLOCK_DEBOUNCE_MS);
    };

    dateInput.addEventListener('change', requestSunDirection);
    hourInput.addEventListener('input', requestSunDirection);

    if (hourValue) {
        hourValue.textContent = formatHour(parseFloat(hourInput.value));
    }
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

// Left side panel fold/unfold toggle. Starts hidden/collapsed by default.
(function initLeftPanelToggle() {
    const layout = document.querySelector('.gltf-layout');
    const toggle = document.getElementById('gltf-left-panel-toggle');
    const panel = document.getElementById('gltf-left-panel');
    const resizer = document.getElementById('gltf-left-resizer');
    if (!layout || !toggle || !panel || !resizer) {
        return;
    }

    toggle.addEventListener('click', () => {
        const collapsed = layout.classList.toggle('gltf-left-panel-collapsed');
        const label = collapsed ? 'Show controls' : 'Hide controls';
        toggle.setAttribute('aria-expanded', collapsed ? 'false' : 'true');
        toggle.setAttribute('aria-label', label);
        toggle.title = label;
        
        window.dispatchEvent(new Event('resize'));
    });
})();

// Per-card fold/unfold. Every card in the right side panel becomes independently collapsible with a
// chevron toggle in its title, and all cards start folded. Each card's content is moved into a
// wrapper so a single class toggle shows or hides it; element ids inside the cards are preserved, so
// the viewer, lighting, properties and results code keeps finding them unchanged.
(function initCardFolding() {
    const panels = document.querySelectorAll('.gltf-side-panel, .gltf-left-panel');
    panels.forEach((panel) => {
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

        // Folded by default unless explicitly expanded.
        if (card.dataset.expanded !== 'true') {
            card.classList.add('gltf-card-collapsed');
        } else {
            button.setAttribute('aria-expanded', 'true');
            button.setAttribute('aria-label', 'Collapse ' + label);
        }

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
    });
})();

const container = document.getElementById('gltf-viewer-container');
if (container) {
    (async () => {
        const loader = document.getElementById('gltf-loader');

        // Status terminal: attached before the payload fetch so the loading task is its first
        // entry; the viewer engine reuses this instance instead of creating a second one.
        GltfStatusTerminal.attach(container);
        reportStatus('Loading...');

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
                reportStatus('No objects were found for this request.');
                return;
            }

            const viewer = new GltfViewer(container, sceneData, glbBuffer);

            // Exposed on window for debugging and UI automation.
            window.gltfViewer = viewer;

            container.addEventListener('gltf-ready', (event) => {
                if (loader) {
                    loader.style.display = 'none';
                }

                reportStatus('Loaded');

                initLightingPanel(viewer);
                initSunClock(viewer, event.detail.referencePoint);
                fillSceneInfo(event.detail.referencePoint, event.detail.objectCount);

                // The page shell does not know the object count upfront in streamed mode.
                const title = document.getElementById('gltf-title');
                if (title && event.detail.objectCount > 0) {
                    title.textContent = `${title.textContent} — ${event.detail.objectCount} objects`;
                }
            });

        container.addEventListener('gltf-selectionchanged', (event) => {
            fillProperties(viewer, event.detail.references);

            const references = (event.detail.references ?? []).filter((reference) => reference);
            reportStatus(references.length === 0
                ? 'Selection cleared'
                : `Selected (${references.length}): ${references.join(', ')}`);
        });
        } catch {
            if (loader) {
                loader.style.display = 'none';
            }

            const panel = document.getElementById('gltf-properties');
            if (panel) {
                panel.innerHTML = '<span class="gltf-muted">Failed to load the 3D scene.</span>';
            }
            reportStatus('Failed to load the 3D scene.');
        }
    })();
}

// Side panel resizer/splitter logic
(function () {
    const resizer = document.getElementById('gltf-resizer');
    const sidePanel = document.getElementById('gltf-side-panel');
    const layout = document.querySelector('.gltf-layout');
    
    if (!resizer || !sidePanel || !layout) {
        return;
    }
    
    // Load saved width on start
    const savedWidth = localStorage.getItem('gltf-side-panel-width');
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
            
            // Dispatch resize event so Three.js canvas adjusts instantly
            window.dispatchEvent(new Event('resize'));
        }
        
        function onMouseUp() {
            resizer.classList.remove('is-dragging');
            document.removeEventListener('mousemove', onMouseMove);
            document.removeEventListener('mouseup', onMouseUp);
            
            // Save width preference
            const finalWidth = sidePanel.getBoundingClientRect().width;
            localStorage.setItem('gltf-side-panel-width', finalWidth);
            
            // Final resize dispatch
            window.dispatchEvent(new Event('resize'));
        }
        
        document.addEventListener('mousemove', onMouseMove);
        document.addEventListener('mouseup', onMouseUp);
    });
})();

// Left side panel resizer/splitter logic
(function () {
    const resizer = document.getElementById('gltf-left-resizer');
    const leftPanel = document.getElementById('gltf-left-panel');
    const layout = document.querySelector('.gltf-layout');
    
    if (!resizer || !leftPanel || !layout) {
        return;
    }
    
    // Load saved width on start
    const savedWidth = localStorage.getItem('gltf-left-panel-width');
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
        const startWidth = leftPanel.getBClientRect().width;
        
        function onMouseMove(mouseMoveEvent) {
            const deltaX = mouseMoveEvent.clientX - startX;
            // The left panel is on the left, so dragging right (positive deltaX) increases its width.
            const newWidth = Math.max(150, Math.min(500, startWidth + deltaX));
            
            leftPanel.style.flex = `0 0 ${newWidth}px`;
            
            // Dispatch resize event so Three.js canvas adjusts instantly
            window.dispatchEvent(new Event('resize'));
        }
        
        function onMouseUp() {
            resizer.classList.remove('is-dragging');
            document.removeEventListener('mousemove', onMouseMove);
            document.removeEventListener('mouseup', onMouseUp);
            
            // Save width preference
            const finalWidth = leftPanel.getBClientRect().width;
            localStorage.setItem('gltf-left-panel-width', finalWidth);
            
            // Final resize dispatch
            window.dispatchEvent(new Event('resize'));
        }
        
        document.addEventListener('mousemove', onMouseMove);
        document.addEventListener('mouseup', onMouseUp);
    });
})();

(function () {
    const modal = document.getElementById('scene-change-modal');
    if (!modal) {
        return;
    }

    const okButton = document.getElementById('scene-change-ok-button');
    const cancelButton = document.getElementById('scene-change-cancel-button');

    if (cancelButton) {
        cancelButton.addEventListener('click', () => {
            modal.style.display = 'none';
        });
    }

    if (okButton) {
        okButton.addEventListener('click', () => {
            const x = document.getElementById('scene-change-x')?.value;
            const y = document.getElementById('scene-change-y')?.value;
            const radius = document.getElementById('scene-change-radius')?.value;
            const storeyHeight = document.getElementById('scene-change-storey-height')?.value;

            if (!x || !y || !radius) {
                return;
            }

            let url = `/communication/buildingsbyradius?centerX=${encodeURIComponent(x)}&centerY=${encodeURIComponent(y)}&radius=${encodeURIComponent(radius)}`;
            if (storeyHeight) {
                url += `&storeyheight=${encodeURIComponent(storeyHeight)}`;
            }
            window.location.href = url;
        });
    }
})();

