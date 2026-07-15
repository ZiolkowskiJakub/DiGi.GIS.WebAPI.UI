// DiGi.GIS.WebAPI.UI — building "Selection" panel for the 3D viewers.
// When exactly one building is selected in the 3D view (single click or marquee), a "Selection" card
// appears in the side panel with a "Details" button linking to the standalone building
// details page (building2D/detailsbyreference) and an "Export" button showing the selected
// object's data as formatted JSON in a modal (OK closes, Copy puts the text on the clipboard).
// The details page renders only the details side of the references master-detail layout
// (Building2DDetailsView) and loads the _Building2DView partial through the same AJAX pipeline,
// so all of its scripts and styles work correctly.
// The building data is partitioned per county, so the details link also carries the selected
// building's world centroid: the server resolves the county from that point when needed.
//
// On the communication scene the card is shared with the antennas: communication-tools.js
// dispatches 'communication-antennaselectionchanged' when an antenna is picked in the 3D view and
// the card then shows "Edit" (wired by communication-tools.js) and "Export" (same modal as for
// buildings, filled with the antenna data); the "Details" link stays hidden. It also dispatches
// 'communication-resultselectionchanged' when a scattering polyline is picked: the polyline data
// fills the card's #communication-selection block (owned by communication-tools.js) and all the
// card buttons stay hidden.

import * as THREE from 'three';

const container = document.getElementById('gltf-viewer-container');
const card = document.getElementById('building-details-card');
const showLink = document.getElementById('building-details-show-link');
const exportButton = document.getElementById('building-export-button');
const antennaEditButton = document.getElementById('communication-antenna-edit-button');

let referencePoint = { X: 0, Y: 0, Z: 0 };
let antennaSelected = false;
let resultSelected = false;

// Computes the world (DiGi) X/Y centroid of the selected object from its geometry. Batched
// payloads store each object as a contiguous vertex range of a merged mesh; legacy payloads have
// one mesh per object. The scene is rendered around a local origin (three.js Y-up), so the result
// is mapped back through the reference point: world X = x + ref.X, world Y = -z + ref.Y.
function worldCentroidOf(reference) {
    const viewer = window.gltfViewer;
    const entry = viewer?.objects?.find((object) => object.reference === reference);
    if (!entry || !entry.mesh) {
        return null;
    }

    const center = new THREE.Vector3();
    if (Number.isInteger(entry.vertexStart) && entry.vertexCount > 0) {
        const position = entry.mesh.geometry.getAttribute('position');
        const vertex = new THREE.Vector3();
        for (let index = entry.vertexStart; index < entry.vertexStart + entry.vertexCount; index++) {
            center.add(vertex.fromBufferAttribute(position, index));
        }
        center.divideScalar(entry.vertexCount);
        entry.mesh.localToWorld(center);
    } else {
        new THREE.Box3().setFromObject(entry.mesh).getCenter(center);
    }

    return { x: center.x + referencePoint.X, y: -center.z + referencePoint.Y };
}

// --- Export modal --------------------------------------------------------------------------------
// Built lazily on first use, so both viewer pages get the same modal without duplicating markup.
// The payload is the selected object's reference plus the domain data attached to the glTF extras
// (the same data the Properties panel shows), formatted as indented JSON.

let exportData = null;
let exportModal = null;
let exportPre = null;

function ensureExportModal() {
    if (exportModal) {
        return;
    }

    exportModal = document.createElement('div');
    exportModal.className = 'gis-modal-overlay';
    exportModal.style.display = 'none';
    exportModal.innerHTML =
        '<div class="gis-card gis-modal gis-modal-wide">' +
        '<h3 class="gltf-card-title">Export</h3>' +
        '<pre class="gis-modal-pre"></pre>' +
        '<div class="gis-modal-buttons">' +
        '<button type="button" id="building-export-close-button" class="gis-button">Close</button>' +
        '<button type="button" id="building-export-copy-button" class="gis-button">Copy</button>' +
        '</div>' +
        '</div>';
    document.body.appendChild(exportModal);

    exportPre = exportModal.querySelector('.gis-modal-pre');
    const closeButton = exportModal.querySelector('#building-export-close-button');
    const copyButton = exportModal.querySelector('#building-export-copy-button');

    closeButton.addEventListener('click', () => {
        exportModal.style.display = 'none';
    });

    copyButton.addEventListener('click', async () => {
        const text = exportPre.textContent;
        try {
            await navigator.clipboard.writeText(text);
        } catch {
            // The Clipboard API needs a secure context; fall back to a hidden textarea selection.
            const textarea = document.createElement('textarea');
            textarea.value = text;
            document.body.appendChild(textarea);
            textarea.select();
            document.execCommand('copy');
            textarea.remove();
        }

        // Brief label change as feedback that the payload is on the clipboard.
        copyButton.textContent = 'Copied';
        setTimeout(() => {
            copyButton.textContent = 'Copy';
        }, 1200);
    });

    document.addEventListener('keydown', (event) => {
        if (event.key === 'Escape' && exportModal.style.display !== 'none') {
            exportModal.style.display = 'none';
        }
    });
}

if (container && card && showLink) {
    container.addEventListener('gltf-ready', (event) => {
        referencePoint = event.detail.referencePoint ?? { X: 0, Y: 0, Z: 0 };

        // Single building: populate General Details button url using scene centroid
        const generalCard = document.getElementById('building-general-card');
        const generalLink = document.getElementById('building-general-details-link');
        const buildingDetailsUrl = container.dataset.buildingDetailsUrl;
        if (generalCard && generalLink && buildingDetailsUrl && window.gltfViewer) {
            const buildingId = generalCard.dataset.buildingId;
            if (buildingId) {
                const urlParams = new URLSearchParams(window.location.search);
                let x = parseFloat(urlParams.get('x'));
                let y = parseFloat(urlParams.get('y'));
                if (isNaN(x) || isNaN(y)) {
                    const center = new THREE.Vector3();
                    new THREE.Box3().setFromObject(window.gltfViewer.scene).getCenter(center);
                    x = center.x + referencePoint.X;
                    y = -center.z + referencePoint.Y;
                }

                let href = `${buildingDetailsUrl}?reference=${encodeURIComponent(buildingId)}&x=${x.toFixed(3)}&y=${y.toFixed(3)}`;
                const countyId = urlParams.get('countyid');
                if (countyId) {
                    href += `&countyid=${encodeURIComponent(countyId)}`;
                }

                generalLink.href = href;
                generalLink.title = `Building Reference ${buildingId}`;
                generalCard.style.display = '';
            }
        }
    });

    container.addEventListener('gltf-selectionchanged', (event) => {
        // A building selection change always ends any antenna or polyline entry of the shared card.
        antennaSelected = false;
        resultSelected = false;
        if (antennaEditButton) {
            antennaEditButton.style.display = 'none';
        }
        if (exportButton) {
            exportButton.style.display = '';
        }

        const references = event.detail.references;
        if (!references || references.length !== 1 || !references[0]) {
            card.style.display = 'none';
            exportData = null;
            return;
        }

        const reference = references[0];

        // Show Export for any selected object; Details link only for buildings.
        const type = window.gltfViewer?.getUserData(reference)?._type;
        const isBuilding2D = String(type ?? '').includes('Building2D');
        const isBuildingModel = String(type ?? '').includes('BuildingModel');

        if (isBuilding2D) {
            let href = `${container.dataset.buildingDetailsUrl}?reference=${encodeURIComponent(reference)}`;
            const centroid = worldCentroidOf(reference);
            if (centroid) {
                href += `&x=${centroid.x.toFixed(3)}&y=${centroid.y.toFixed(3)}`;
            }
            showLink.href = href;
            showLink.title = reference;
            showLink.style.display = '';
        } else if (isBuildingModel) {
            const buildingModelUrl = container.dataset.buildingmodelDetailsUrl;
            if (buildingModelUrl) {
                let href = `${buildingModelUrl}?reference=${encodeURIComponent(reference)}`;
                const centroid = worldCentroidOf(reference);
                if (centroid) {
                    href += `&x=${centroid.x.toFixed(3)}&y=${centroid.y.toFixed(3)}`;
                }
                showLink.href = href;
                showLink.title = reference;
                showLink.style.display = '';
            } else {
                showLink.style.display = 'none';
            }
        } else {
            showLink.style.display = 'none';
        }

        exportData = { Reference: reference, ...(window.gltfViewer?.getUserData(reference) ?? {}) };
        card.style.display = '';
    });

    // Antenna selection (communication scene): the card shows "Edit" + "Export" for the picked
    // antenna; a null antenna drops the entry again (unless the card meanwhile shows a building).
    container.addEventListener('communication-antennaselectionchanged', (event) => {
        const antenna = event.detail?.antenna ?? null;

        if (!antenna) {
            if (!antennaSelected) {
                return;
            }

            antennaSelected = false;
            if (antennaEditButton) {
                antennaEditButton.style.display = 'none';
            }
            card.style.display = 'none';
            exportData = null;
            return;
        }

        antennaSelected = true;
        resultSelected = false;
        showLink.style.display = 'none';
        if (antennaEditButton) {
            antennaEditButton.style.display = '';
        }
        if (exportButton) {
            exportButton.style.display = '';
        }

        exportData = { Type: 'Antenna', X: antenna.x, Y: antenna.y, Z: antenna.z, Functions: antenna.functions ?? [] };
        card.style.display = '';
    });

    // Scattering polyline selection (communication scene): the card only frames the polyline data
    // block filled by communication-tools.js — the building/antenna buttons stay hidden.
    container.addEventListener('communication-resultselectionchanged', (event) => {
        const selected = event.detail?.selected === true;

        if (!selected) {
            if (!resultSelected) {
                return;
            }

            resultSelected = false;
            if (exportButton) {
                exportButton.style.display = '';
            }
            card.style.display = 'none';
            return;
        }

        resultSelected = true;
        antennaSelected = false;
        showLink.style.display = 'none';
        if (antennaEditButton) {
            antennaEditButton.style.display = 'none';
        }
        if (exportButton) {
            exportButton.style.display = 'none';
        }
        exportData = null;
        card.style.display = '';
    });

    exportButton?.addEventListener('click', () => {
        if (!exportData) {
            return;
        }

        ensureExportModal();
        exportPre.textContent = JSON.stringify(exportData, null, 2);
        exportModal.style.display = 'flex';
    });
}
