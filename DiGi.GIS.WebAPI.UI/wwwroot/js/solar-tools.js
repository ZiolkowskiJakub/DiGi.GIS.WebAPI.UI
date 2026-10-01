// DiGi.GIS.WebAPI.UI — "Solar radiation" panel of the Building Viewer (issue #66).
// Calculate runs the annual solar radiation of the page's building (solar/viewbybuildingmodelid) and
// colours its receiving walls and roofs in the scene already loaded: each surface takes its irradiation
// colour and its SurfaceSolarRadiationResult as properties (GltfViewer.setObjectAppearance), and the
// scene switches to unlit colours, so a surface reads exactly its legend colour. Nothing else of the
// scene changes - camera, settings, terrain, surrounding elements and the object count stay as they are.
// A building above the synchronous limits (413), or a calculation refused while the solve gate is busy
// (503), is offered a background job (solar-job.js); its id is kept in the page address (?job=), so a
// reload resumes it. Clear restores the original colours, properties, lighting and shade.
// Loaded only on pages whose viewer container carries data-solar-view-url.

import { reportStatus, updateLastStatus, formatElapsed } from 'gltf-viewer-core';
import { refusalMessage } from 'http-refusal';

const LIGHTING_INPUT_IDS = ['gltf-sun-date', 'gltf-sun-hour', 'gltf-sun-azimuth', 'gltf-sun-altitude', 'gltf-sun-intensity', 'gltf-ambient-intensity'];

const container = document.getElementById('gltf-viewer-container');
const calculateButton = document.getElementById('solar-calculate-button');
const clearButton = document.getElementById('solar-clear-button');
const messageArea = document.getElementById('solar-message');
const calculationLoader = document.getElementById('solar-calculation-loader');
const resultsCard = document.getElementById('gltf-results-card');
const resultsSection = document.getElementById('solar-results-section');
const resultsRadius = document.getElementById('solar-results-radius');
const resultsStation = document.getElementById('solar-results-station');
const flatCheckbox = document.getElementById('solar-flat-checkbox');

const viewUrl = container?.dataset.solarViewUrl ?? '';
const jobsUrl = container?.dataset.solarJobsUrl ?? '';
const query = container?.dataset.solarQuery ?? '';

let viewer = null;          // GltfViewer instance exposed by gltf-viewer.js
let running = false;        // a calculation (synchronous or background) is in progress
let applied = false;        // results are shown in the scene
let lightingNote = null;    // note shown in the Lighting card while the unlit colours are on

function updateToolbar() {
    if (calculateButton) {
        calculateButton.disabled = !viewer || running;
    }
    if (clearButton) {
        clearButton.disabled = running || !applied;
    }
}

// Unfolds a card through its own toggle, which keeps aria-expanded in step (cards fold by default).
function expandCard(element) {
    const card = element?.closest('.gltf-card');
    if (card && card.classList.contains('gltf-card-collapsed')) {
        card.querySelector('.gltf-card-toggle')?.click();
    }
}

// Shows a message in the Solar radiation card, followed by any action buttons on a row of their own.
function showMessage(message, buttons = []) {
    if (!messageArea) {
        return;
    }
    const span = document.createElement('span');
    span.textContent = message;
    messageArea.replaceChildren(span);
    if (buttons.length > 0) {
        const row = document.createElement('div');
        row.className = 'solar-message-actions';
        row.append(...buttons);
        messageArea.appendChild(row);
    }
    messageArea.style.display = '';
    expandCard(messageArea);
}

function clearMessage() {
    if (messageArea) {
        messageArea.replaceChildren();
        messageArea.style.display = 'none';
    }
}

function createButton(text, onClick) {
    const button = document.createElement('button');
    button.type = 'button';
    button.className = 'gis-button';
    button.textContent = text;
    button.addEventListener('click', () => {
        button.disabled = true;
        onClick();
    });
    return button;
}

function setLoaderVisible(visible) {
    if (calculationLoader) {
        calculationLoader.style.display = visible ? '' : 'none';
    }
}

function removeJobParameter() {
    const url = new URL(window.location.href);
    if (url.searchParams.has('job')) {
        url.searchParams.delete('job');
        window.history.replaceState(window.history.state, '', url);
    }
}

// The Lighting card has no effect while the unlit colours are on, so its controls are locked with a note.
function setLightingLocked(locked) {
    for (const id of LIGHTING_INPUT_IDS) {
        const input = document.getElementById(id);
        if (input) {
            input.disabled = locked;
        }
    }
    if (locked && !lightingNote) {
        const anchor = document.getElementById('gltf-sun-azimuth');
        const content = anchor?.closest('.gltf-card-content') ?? anchor?.closest('.gltf-card');
        if (content) {
            lightingNote = document.createElement('div');
            lightingNote.className = 'gltf-muted solar-lighting-note';
            lightingNote.textContent = 'Flat result colours are on, so lighting does not apply. Uncheck "Flat result colours" in Results to use it.';
            content.prepend(lightingNote);
        }
    } else if (!locked && lightingNote) {
        lightingNote.remove();
        lightingNote = null;
    }
}

function setUnlit(enabled) {
    viewer?.setUnlitColors(enabled);
    setLightingLocked(enabled);
    if (flatCheckbox) {
        flatCheckbox.checked = enabled;
    }
}

// The Properties and Selection panels show the data of the current selection; they are refreshed after the
// payload of the selected objects changed, through the same event a selection change sends.
function refreshSelection() {
    if (!viewer) {
        return;
    }
    const references = [...viewer.selectedIds].map((id) => viewer.objects[id]?.reference).filter((reference) => reference);
    container.dispatchEvent(new CustomEvent('gltf-selectionchanged', { detail: { references } }));
}

function parseProperties(text) {
    if (!text) {
        return null;
    }
    try {
        return JSON.parse(text);
    } catch {
        return null;
    }
}

function formatMeters(value) {
    return `${Number(value).toLocaleString('en-US', { maximumFractionDigits: 1 })} m`;
}

function fillResults(radius, stationName, stationUrl) {
    if (resultsRadius) {
        const requested = Number(container.dataset.solarRadiusRequested);
        resultsRadius.textContent = Number.isFinite(requested) && requested > Number(radius)
            ? `${formatMeters(radius)} (limited from ${formatMeters(requested)})`
            : formatMeters(radius);
    }
    if (resultsStation) {
        const name = stationName || 'EPW';
        if (stationUrl) {
            const link = document.createElement('a');
            link.href = stationUrl;
            link.target = '_blank';
            link.rel = 'noopener';
            link.textContent = name;
            resultsStation.replaceChildren(link);
        } else {
            resultsStation.textContent = name;
        }
    }
}

// Applies a solar radiation view (SolarRadiationViewModel; ASP.NET answers camelCase, PascalCase is tolerated).
function applyView(view) {
    const surfaces = view?.surfaces ?? view?.Surfaces ?? [];

    // A second calculation replaces the first: start from the original appearance.
    viewer.resetObjectAppearance();

    let matched = 0;
    for (const surface of surfaces) {
        const reference = surface.reference ?? surface.Reference;
        const color = surface.color ?? surface.Color;
        const properties = parseProperties(surface.properties ?? surface.Properties);
        if (viewer.setObjectAppearance(reference, { color, properties: properties ?? undefined })) {
            matched++;
        }
    }

    if (matched === 0) {
        viewer.resetObjectAppearance();
        const message = 'The results could not be matched to the walls and roofs of this scene.';
        reportStatus(message);
        showMessage(message);
        return false;
    }

    setUnlit(true);
    fillResults(view.radius ?? view.Radius, view.stationName ?? view.StationName, view.stationUrl ?? view.StationUrl);

    if (resultsSection) {
        resultsSection.style.display = '';
    }
    if (resultsCard) {
        resultsCard.style.display = '';
        expandCard(resultsSection);
    }

    applied = true;
    refreshSelection();

    if (matched < surfaces.length) {
        reportStatus(`${surfaces.length - matched} of ${surfaces.length} results have no matching object in this scene.`);
    }
    return true;
}

function clearResults() {
    if (!viewer) {
        return;
    }
    viewer.resetObjectAppearance();
    setUnlit(false);
    if (resultsSection) {
        resultsSection.style.display = 'none';
    }
    // The card holds one section per analysis; it hides once none of them is shown.
    if (resultsCard && ![...resultsCard.querySelectorAll('.gltf-results-section')].some((section) => section.style.display !== 'none')) {
        resultsCard.style.display = 'none';
    }
    applied = false;
    clearMessage();
    removeJobParameter();
    refreshSelection();
    updateToolbar();
    reportStatus('Solar radiation results cleared.');
}

// Background calculation: posts a new job (jobId null) or resumes one, reports its progress in the card and
// the status terminal, and applies its view once it has completed. The scene stays usable meanwhile.
async function runJob(jobId) {
    running = true;
    updateToolbar();

    const { runSolarJob, cancelSolarJob } = await import('solar-job');

    let currentJobId = jobId;
    const cancelButton = createButton('Cancel calculation', () => cancelSolarJob(jobsUrl, currentJobId));
    let reported = false;
    const result = await runSolarJob({
        jobsUrl,
        query,
        jobId,
        pollSeconds: container.dataset.solarJobPollSeconds,
        refusalMessage,
        onStatus: (text, id, completed) => {
            currentJobId = id;
            showMessage(text, completed || cancelButton.disabled ? [] : [cancelButton]);
            if (reported) {
                updateLastStatus(text);
            } else {
                reportStatus(text);
                reported = true;
            }
        },
    });

    running = false;
    if (result.view) {
        clearMessage();
        if (applyView(result.view)) {
            reportStatus('Background calculation completed: solar radiation shown.');
        }
    } else {
        reportStatus(result.message);
        showMessage(result.message, result.retry ? [createButton('Calculate in background', () => runJob(null))] : []);
    }
    updateToolbar();
}

async function calculate() {
    if (!viewer || running) {
        return;
    }

    running = true;
    clearMessage();
    updateToolbar();
    setLoaderVisible(true);

    const calculationStart = performance.now();
    reportStatus('Calculating solar radiation…');
    const calculationTimer = setInterval(() => updateLastStatus(`Calculating solar radiation… (${formatElapsed(calculationStart)})`), 200);

    let offerBackground = false;
    let message = null;
    let view = null;
    try {
        const response = await fetch(`${viewUrl}?${query}`, { cache: 'no-store' });
        if (response.status === 200) {
            view = await response.json();
        } else if (response.status === 204) {
            message = 'The building or its weather file was not found.';
        } else {
            message = await refusalMessage(response);
            // Above the synchronous limits (413), or while the solve gate is busy (503, typically with a
            // background job running), a background calculation is offered.
            offerBackground = response.status === 413 || response.status === 503;
        }
    } catch {
        message = 'The calculation failed: the server did not answer.';
    } finally {
        clearInterval(calculationTimer);
        setLoaderVisible(false);
        running = false;
    }

    if (view) {
        if (applyView(view)) {
            reportStatus(`Solar radiation calculated in ${formatElapsed(calculationStart)}`);
        }
    } else {
        reportStatus(message);
        showMessage(message, offerBackground ? [createButton('Calculate in background', () => runJob(null))] : []);
    }
    updateToolbar();
}

if (container && viewUrl) {
    calculateButton?.addEventListener('click', () => calculate());
    clearButton?.addEventListener('click', () => clearResults());
    flatCheckbox?.addEventListener('change', () => setUnlit(flatCheckbox.checked));

    const start = () => {
        viewer = window.gltfViewer ?? null;
        updateToolbar();

        // A background job kept in the page address is resumed, so a reload applies its result.
        const jobId = new URLSearchParams(window.location.search).get('job');
        if (viewer && jobId && jobsUrl) {
            runJob(jobId);
        }
    };

    container.addEventListener('gltf-ready', start, { once: true });
    updateToolbar();
}
