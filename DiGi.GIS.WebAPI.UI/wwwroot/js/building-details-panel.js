// DiGi.GIS.WebAPI.UI — building "Selection" panel for the 3D viewers.
// When exactly one building is selected in the 3D view (single click or marquee), a "Selection" card
// appears in the side panel with a "Details" button linking to the standalone building
// details page (building2D/detailsbyreference). That page renders only the details side of the
// references master-detail layout (Building2DDetailsView) and loads the _Building2DView partial
// through the same AJAX pipeline, so all of its scripts and styles work correctly.
// The building data is partitioned per county, so the details link also carries the selected
// building's world centroid: the server resolves the county from that point when needed.

import * as THREE from 'three';

const container = document.getElementById('gltf-viewer-container');
const card = document.getElementById('building-details-card');
const showLink = document.getElementById('building-details-show-link');

let referencePoint = { X: 0, Y: 0, Z: 0 };

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

if (container && card && showLink) {
    container.addEventListener('gltf-ready', (event) => {
        referencePoint = event.detail.referencePoint ?? { X: 0, Y: 0, Z: 0 };
    });

    container.addEventListener('gltf-selectionchanged', (event) => {
        const references = event.detail.references;
        if (!references || references.length !== 1 || !references[0]) {
            card.style.display = 'none';
            return;
        }

        const reference = references[0];

        // Only buildings expose the details page; any other selectable type keeps the card hidden.
        const type = window.gltfViewer?.getUserData(reference)?._type;
        if (type && !String(type).includes('Building2D')) {
            card.style.display = 'none';
            return;
        }

        let href = `${container.dataset.buildingDetailsUrl}?reference=${encodeURIComponent(reference)}`;
        const centroid = worldCentroidOf(reference);
        if (centroid) {
            href += `&x=${centroid.x.toFixed(3)}&y=${centroid.y.toFixed(3)}`;
        }

        showLink.href = href;
        showLink.title = reference;
        card.style.display = '';
    });
}
