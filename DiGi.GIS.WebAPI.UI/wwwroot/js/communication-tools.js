// DiGi.GIS.WebAPI.UI — communication analysis tooling layered on top of the generic
// DiGi.GLTF.WebAPI viewer engine (gltf-viewer.js owns the base panels; this module owns the
// antenna toolbar, the antenna edit/erase modes, the calculation modal/request and the results
// rendering: the propagation ellipsoid, the arrival rays and the results panel).
//
// Calculation flow: the "Calculate" button opens a modal collecting the propagation inputs that
// are not part of the geometrical model (frequency, polarization, material properties). The
// request goes to ~/communication/calculate; the server rebuilds the analyzed area on the fly
// (Building -> Mesh3D -> ScatteringObject), packages everything into a GeometricalPropagationModel
// and runs the multi-ellipsoidal propagation cascade through DiGi.Communication.WebAPI. The
// response carries world coordinates only: the dominant propagation ellipsoid, the arrival rays
// with their corrected powers and the per delay component summary (see renderResults).
//
// [TEMPORARY A/B TESTING] The v1 calculate endpoint returns a delay grouped payload instead
// (discriminated by its delays array): one entry per delay carrying the propagation ellipsoid(s),
// the scattering polylines (one per ScatteringPointGroup) and the angular power distribution
// vectors. The General panel drives which delay is rendered and the vector scale factor; a
// selected polyline shows its semi-transparent auxiliary polylines (see renderDelayResults).
//
// AI-NOTE (mocked remaining inputs): the multipath power delay profile (TypicalUrban preset) and
// the antenna radiation characteristics are hardcoded server side, mirroring the reference xUnit
// fact ToPropagation_PropagationModel_TypicalUrban; extend the calculation modal once they become
// user configurable.

import * as THREE from 'three';

const MAX_ANTENNAS = 2;
const ANTENNA_COLOR = 0xd13438;          // red mast + dot
const ANTENNA_SELECTED_COLOR = 0xffa500; // orange tint in erase mode
const RAY_COLOR = 0x2ecc40;              // green propagation rays (requirement)
const RAY_SELECTED_COLOR = 0x9dff57;     // lighter green tint of the selected ray
const ELLIPSOID_COLOR = 0x4da6ff;        // semi-transparent propagation ellipsoid
const ELLIPSOID_OPACITY = 0.18;
const ELLIPSOID_SELECTED_OPACITY = 0.32;
const SCATTERING_COLOR = 0xff8c00;       // single color for every scattering polyline (was a cycled palette)
const SCATTERING_SELECTED_COLOR = 0xffffff; // tint of the selected scattering polyline
const AUXILIARY_OPACITY = 0.35;          // auxiliary polylines of the selected scattering polyline
const VECTOR_COLOR = 0x2ecc40;           // angular power distribution vectors (green like the rays)
const SCATTERING_RADIUS_FACTOR = 0.15;   // scattering polyline tube radius vs the antenna dot
const VECTOR_RADIUS_FACTOR = 0.1;        // angular power vector tube radius vs the antenna dot
const DEFAULT_VECTOR_SCALE = 1000;       // default stretch applied to the angular power vectors
const RAY_DECIBEL_WINDOW = 30;           // dB range mapped onto the ray length scale
const CLICK_THRESHOLD = 5;               // pixels of pointer travel before a click becomes a drag
const DEFAULT_ANTENNA_HEIGHT = 30;       // default Z coordinate for the antenna modal and live preview
const ANTENNA_PREVIEW_OPACITY = 0.5;     // semi-transparent live preview during add mode

const container = document.getElementById('gltf-viewer-container');

const addButton = document.getElementById('communication-add-antenna-button');
const removeButton = document.getElementById('communication-remove-antenna-button');
const calculateButton = document.getElementById('communication-calculate-button');
const clearButton = document.getElementById('communication-clear-button');
const modeHint = document.getElementById('communication-mode-hint');

const modal = document.getElementById('communication-antenna-modal');
const modalX = document.getElementById('communication-antenna-x');
const modalY = document.getElementById('communication-antenna-y');
const modalZ = document.getElementById('communication-antenna-z');
const modalOkButton = document.getElementById('communication-antenna-ok-button');
const modalCancelButton = document.getElementById('communication-antenna-cancel-button');

const calculationModal = document.getElementById('communication-calculation-modal');
const calculationFrequencyInput = document.getElementById('communication-calculation-frequency');
const calculationPolarizationSelect = document.getElementById('communication-calculation-polarization');
const calculationPermittivityInput = document.getElementById('communication-calculation-permittivity');
const calculationConductivityInput = document.getElementById('communication-calculation-conductivity');
const calculationProfileSelect = document.getElementById('communication-calculation-profile');
const calculationOkButton = document.getElementById('communication-calculation-ok-button');
const calculationCancelButton = document.getElementById('communication-calculation-cancel-button');

const resultsCard = document.getElementById('communication-results-card');
const resultsPanel = document.getElementById('communication-results');

const generalCard = document.getElementById('communication-general-card');
const delaySlider = document.getElementById('communication-delay-slider');
const delayValueLabel = document.getElementById('communication-delay-value');
const vectorScaleInput = document.getElementById('communication-vector-scale');

let viewer = null;                 // GltfViewer instance exposed by gltf-viewer.js
let referencePoint = { X: 0, Y: 0, Z: 0 };
let sceneRadius = 10;
let mode = null;                   // null | 'add' | 'erase'
let calculating = false;
let pointerDownPosition = null;

const antennas = [];               // { group, meshes, selected, data: { x, y, z, functions } }
const resultObjects = [];          // three.js objects added by "Calculate"
const resultMeshes = [];           // selectable subset of resultObjects (ellipsoid + ray cylinders)
let activePayload = null;          // last successful calculation payload (world coordinates)
let selectedResultMesh = null;     // currently highlighted result mesh
let resultPickCandidate = null;    // result mesh hit on pointerdown, resolved on pointerup
let suppressResultClick = false;   // swallows the click event that follows a result pick
let antennaPreview = null;         // semi-transparent antenna preview shown during add mode

let delayPayload = null;           // last successful delay based (v1) calculation payload
const delayObjects = [];           // three.js objects of the currently rendered delay frame
let auxiliaryObject = null;        // auxiliary polylines of the selected scattering polyline

const raycaster = new THREE.Raycaster();
const groundPlane = new THREE.Plane(new THREE.Vector3(0, 1, 0), 0); // DiGi Z = 0 plane in three.js Y-up world

// DiGi world coordinates <-> three.js scene coordinates. The scene geometry is translated to a
// local origin (the removed world offset is the reference point) and DiGi is Z-up while three.js
// is Y-up: DiGi local (x, y, z) maps to three.js (x, z, -y).
function toScene(x, y, z) {
    return new THREE.Vector3(x - referencePoint.X, z - (referencePoint.Z ?? 0), -(y - referencePoint.Y));
}

function toWorld(vector3) {
    return {
        x: vector3.x + referencePoint.X,
        y: -vector3.z + referencePoint.Y,
        z: vector3.y + (referencePoint.Z ?? 0)
    };
}

// Direction vectors only need the Z-up -> Y-up rotation (no reference point translation).
function toSceneDirection(direction) {
    return new THREE.Vector3(direction.x, direction.z, -direction.y).normalize();
}

function setHint(text) {
    if (modeHint) {
        modeHint.textContent = text ?? '';
    }
}

function setMode(value) {
    mode = value;
    container.style.cursor = value === 'add' ? 'crosshair' : value === 'erase' ? 'pointer' : '';

    if (value === 'add') {
        setHint('Click a point on the ground plane to place the antenna. Press Esc to cancel.');
    } else if (value === 'erase') {
        setHint('Click antennas to select them. Press Enter to remove the selected antennas, Esc to cancel.');
    } else {
        setHint('');
        removeAntennaPreview();
    }

    updateToolbar();
}

function updateToolbar() {
    const ready = viewer !== null;
    addButton.disabled = !ready || antennas.length >= MAX_ANTENNAS || mode !== null;
    removeButton.disabled = !ready || antennas.length === 0 || mode !== null;
    calculateButton.disabled = !ready || antennas.length !== MAX_ANTENNAS || mode !== null || calculating;
}

// ---------------------------------------------------------------------------------------------
// Antenna rendering: a thin vertical mast with a red dot at the top (the specified height). The
// mast is a cylinder instead of a THREE.Line so erase-mode raycasting works reliably.
// ---------------------------------------------------------------------------------------------

function antennaDotRadius() {
    return Math.max(0.6, sceneRadius * 0.006);
}

function addAntennaObject(data) {
    const dotRadius = antennaDotRadius();
    const base = toScene(data.x, data.y, 0);
    const top = toScene(data.x, data.y, data.z);

    const group = new THREE.Group();
    const meshes = [];

    if (data.z > 0) {
        const mastGeometry = new THREE.CylinderGeometry(dotRadius * 0.12, dotRadius * 0.12, data.z, 8);
        const mast = new THREE.Mesh(mastGeometry, new THREE.MeshBasicMaterial({ color: ANTENNA_COLOR }));
        mast.position.set(base.x, base.y + data.z / 2, base.z);
        group.add(mast);
        meshes.push(mast);
    }

    const dot = new THREE.Mesh(new THREE.SphereGeometry(dotRadius, 16, 12), new THREE.MeshBasicMaterial({ color: ANTENNA_COLOR }));
    dot.position.copy(top);
    group.add(dot);
    meshes.push(dot);

    viewer.scene.add(group);

    antennas.push({ group, meshes, selected: false, data });
    updateToolbar();
}

function removeAntennaObject(antenna) {
    viewer.scene.remove(antenna.group);
    for (const mesh of antenna.meshes) {
        mesh.geometry.dispose();
        mesh.material.dispose();
    }
}

function setAntennaSelected(antenna, selected) {
    antenna.selected = selected;
    for (const mesh of antenna.meshes) {
        mesh.material.color.setHex(selected ? ANTENNA_SELECTED_COLOR : ANTENNA_COLOR);
    }
}

// ---------------------------------------------------------------------------------------------
// Antenna preview: a semi-transparent antenna symbol that follows the cursor during add mode.
// ---------------------------------------------------------------------------------------------

function createAntennaPreview() {
    if (antennaPreview || !viewer) {
        return;
    }

    const dotRadius = antennaDotRadius();
    const group = new THREE.Group();

    if (DEFAULT_ANTENNA_HEIGHT > 0) {
        const mastGeometry = new THREE.CylinderGeometry(dotRadius * 0.12, dotRadius * 0.12, DEFAULT_ANTENNA_HEIGHT, 8);
        const mast = new THREE.Mesh(mastGeometry, new THREE.MeshBasicMaterial({ color: ANTENNA_COLOR, transparent: true, opacity: ANTENNA_PREVIEW_OPACITY, depthWrite: false }));
        mast.position.set(0, DEFAULT_ANTENNA_HEIGHT / 2, 0);
        group.add(mast);
    }

    const dot = new THREE.Mesh(new THREE.SphereGeometry(dotRadius, 16, 12), new THREE.MeshBasicMaterial({ color: ANTENNA_COLOR, transparent: true, opacity: ANTENNA_PREVIEW_OPACITY, depthWrite: false }));
    dot.position.set(0, DEFAULT_ANTENNA_HEIGHT, 0);
    group.add(dot);

    group.visible = false;
    viewer.scene.add(group);
    antennaPreview = group;
}

function updateAntennaPreview(position) {
    if (!antennaPreview) {
        createAntennaPreview();
    }
    if (antennaPreview) {
        antennaPreview.position.copy(position);
        antennaPreview.visible = true;
    }
}

function hideAntennaPreview() {
    if (antennaPreview) {
        antennaPreview.visible = false;
    }
}

function removeAntennaPreview() {
    if (!antennaPreview) {
        return;
    }

    viewer.scene.remove(antennaPreview);
    antennaPreview.traverse((child) => {
        child.geometry?.dispose();
        if (Array.isArray(child.material)) {
            child.material.forEach((m) => m.dispose());
        } else {
            child.material?.dispose();
        }
    });
    antennaPreview = null;
}

// ---------------------------------------------------------------------------------------------
// Pointer handling: capture-phase listeners intercept the left mouse button while an edit mode is
// active, so the generic viewer never sees those clicks (no accidental building selection).
// Middle mouse navigation keeps working because only button 0 is intercepted.
// ---------------------------------------------------------------------------------------------

function pointerNdc(event) {
    const rectangle = container.getBoundingClientRect();
    return new THREE.Vector2(
        ((event.clientX - rectangle.left) / rectangle.width) * 2 - 1,
        -((event.clientY - rectangle.top) / rectangle.height) * 2 + 1);
}

function onPointerDown(event) {
    if (event.button !== 0) {
        return;
    }

    // Result selection: while calculation results are displayed (and no edit mode is active), a
    // press on the ellipsoid or on a ray is intercepted before the generic viewer sees it, exactly
    // like the antenna edit modes — otherwise the viewer would start a marquee/building selection.
    if (mode === null) {
        resultPickCandidate = pickResultMesh(event);
        if (resultPickCandidate === null) {
            return;
        }
    }

    event.stopPropagation();
    pointerDownPosition = { x: event.clientX, y: event.clientY };
}

function onPointerUp(event) {
    if (event.button !== 0) {
        return;
    }

    if (mode === null) {
        if (resultPickCandidate === null) {
            return;
        }

        event.stopPropagation();
        suppressResultClick = true;

        const resultMesh = resultPickCandidate;
        resultPickCandidate = null;

        if (!pointerDownPosition) {
            return;
        }
        const travel_Result = Math.hypot(event.clientX - pointerDownPosition.x, event.clientY - pointerDownPosition.y);
        pointerDownPosition = null;
        if (travel_Result > CLICK_THRESHOLD) {
            return;
        }

        selectResultMesh(resultMesh);
        return;
    }

    event.stopPropagation();

    if (!pointerDownPosition) {
        return;
    }
    const travel = Math.hypot(event.clientX - pointerDownPosition.x, event.clientY - pointerDownPosition.y);
    pointerDownPosition = null;
    if (travel > CLICK_THRESHOLD) {
        return;
    }

    if (mode === 'add') {
        handleAddClick(event);
    } else if (mode === 'erase') {
        handleEraseClick(event);
    }
}

function onClickCapture(event) {
    if (event.button !== 0) {
        return;
    }

    if (suppressResultClick) {
        suppressResultClick = false;
        event.stopPropagation();
        return;
    }

    if (mode !== null) {
        event.stopPropagation();
    }
}

container.addEventListener('pointerdown', onPointerDown, true);
container.addEventListener('pointerup', onPointerUp, true);
container.addEventListener('click', onClickCapture, true);

function onPointerMove(event) {
    if (mode !== 'add' || !viewer) {
        return;
    }

    raycaster.setFromCamera(pointerNdc(event), viewer.camera);
    const intersection = new THREE.Vector3();
    if (raycaster.ray.intersectPlane(groundPlane, intersection)) {
        updateAntennaPreview(intersection);
    } else {
        hideAntennaPreview();
    }
}

container.addEventListener('pointermove', onPointerMove);

// ---------------------------------------------------------------------------------------------
// Add antenna: click on the ground plane (restricted to Z = 0) -> modal with editable values.
// ---------------------------------------------------------------------------------------------

function handleAddClick(event) {
    raycaster.setFromCamera(pointerNdc(event), viewer.camera);

    const intersection = new THREE.Vector3();
    if (!raycaster.ray.intersectPlane(groundPlane, intersection)) {
        return;
    }

    const world = toWorld(intersection);

    modalX.value = world.x.toFixed(2);
    modalY.value = world.y.toFixed(2);
    modalZ.value = String(DEFAULT_ANTENNA_HEIGHT);
    for (const checkbox of modal.querySelectorAll('.communication-antenna-function')) {
        checkbox.checked = antennas.length === 0 ? checkbox.value === 'Transmitter'
            : antennas.length === 1 ? checkbox.value === 'Receiver'
            : false;
    }

    setMode(null);
    modal.style.display = 'flex';
    modalZ.focus();
}

modalOkButton.addEventListener('click', () => {
    const x = parseFloat(modalX.value);
    const y = parseFloat(modalY.value);
    const z = parseFloat(modalZ.value);
    if (!isFinite(x) || !isFinite(y) || !isFinite(z) || z < 0) {
        return;
    }

    const functions = [...modal.querySelectorAll('.communication-antenna-function')]
        .filter((checkbox) => checkbox.checked)
        .map((checkbox) => checkbox.value);

    modal.style.display = 'none';
    addAntennaObject({ x, y, z, functions });
});

modalCancelButton.addEventListener('click', () => {
    modal.style.display = 'none';
    updateToolbar();
});

// ---------------------------------------------------------------------------------------------
// Remove antenna: erase mode selects antennas, Enter removes them, Esc cancels without removing.
// ---------------------------------------------------------------------------------------------

function handleEraseClick(event) {
    raycaster.setFromCamera(pointerNdc(event), viewer.camera);

    for (const antenna of antennas) {
        const intersections = raycaster.intersectObjects(antenna.meshes, false);
        if (intersections.length > 0) {
            setAntennaSelected(antenna, !antenna.selected);
            return;
        }
    }
}

function finishErase(remove) {
    for (let index = antennas.length - 1; index >= 0; index--) {
        const antenna = antennas[index];
        if (remove && antenna.selected) {
            removeAntennaObject(antenna);
            antennas.splice(index, 1);
        } else {
            setAntennaSelected(antenna, false);
        }
    }

    setMode(null); // also re-enables "Add antenna" when applicable (see updateToolbar)
}

window.addEventListener('keydown', (event) => {
    if (calculationModal.style.display !== 'none' && event.key === 'Escape') {
        calculationModal.style.display = 'none';
        return;
    }

    if (modal.style.display !== 'none' && event.key === 'Escape') {
        modal.style.display = 'none';
        updateToolbar();
        return;
    }

    if (mode === 'erase') {
        if (event.key === 'Enter') {
            finishErase(true);
        } else if (event.key === 'Escape') {
            finishErase(false);
        }
    } else if (mode === 'add' && event.key === 'Escape') {
        setMode(null);
    }
});

// ---------------------------------------------------------------------------------------------
// Calculate: collect the propagation inputs in the modal, send the analyzed area, the antennas and
// the inputs to the server and render the returned propagation results (ellipsoid + rays + panel).
// ---------------------------------------------------------------------------------------------

function formatPower(value) {
    return value >= 0.001 ? value.toFixed(4) : value.toExponential(3);
}

function formatDelay(delay) {
    return `${(delay * 1e6).toFixed(2)} µs`;
}

function formatAngle(radians) {
    return `${(radians * 180 / Math.PI).toFixed(1)}°`;
}

function appendResultRow(table, key, value) {
    const row = table.insertRow();
    row.insertCell().textContent = key;
    row.insertCell().textContent = value;
}

function pickResultMesh(event) {
    if (resultMeshes.length === 0 || viewer === null) {
        return null;
    }

    raycaster.setFromCamera(pointerNdc(event), viewer.camera);

    // Scattering polylines win over the ellipsoid (like the rays below). They are now thick
    // cylinder meshes, so they raycast directly — no THREE.Line threshold needed.
    const polylineMeshes = resultMeshes.filter((mesh) => mesh.userData.communication?.type === 'polyline');
    if (polylineMeshes.length > 0) {
        const polylineIntersections = raycaster.intersectObjects(polylineMeshes, false);
        if (polylineIntersections.length > 0) {
            return polylineIntersections[0].object;
        }
    }

    // Rays win over the ellipsoid: the semi-transparent ellipsoid usually encloses the receiver,
    // so a nearest-hit-only strategy would make the rays inside it unselectable.
    const rayMeshes = resultMeshes.filter((mesh) => mesh.userData.communication?.type === 'ray');
    const rayIntersections = raycaster.intersectObjects(rayMeshes, false);
    if (rayIntersections.length > 0) {
        return rayIntersections[0].object;
    }

    const ellipsoidMeshes = resultMeshes.filter((mesh) => mesh.userData.communication?.type === 'ellipsoid');
    const ellipsoidIntersections = raycaster.intersectObjects(ellipsoidMeshes, false);
    return ellipsoidIntersections.length > 0 ? ellipsoidIntersections[0].object : null;
}

function setResultMeshHighlighted(mesh, highlighted) {
    const communication = mesh.userData.communication;
    if (communication.type === 'ray') {
        mesh.material.color.setHex(highlighted ? RAY_SELECTED_COLOR : RAY_COLOR);
    } else if (communication.type === 'polyline') {
        mesh.material.color.setHex(highlighted ? SCATTERING_SELECTED_COLOR : communication.baseColor);
    } else {
        mesh.material.opacity = highlighted ? ELLIPSOID_SELECTED_OPACITY : ELLIPSOID_OPACITY;
    }
}

// Selecting the ellipsoid shows the general calculation results (the delay summary in the delay
// based flow); selecting a ray shows the ray specific values; selecting a scattering polyline
// shows its semi-transparent auxiliary polylines and the group specific values.
function selectResultMesh(mesh) {
    if (selectedResultMesh !== null) {
        setResultMeshHighlighted(selectedResultMesh, false);
    }

    selectedResultMesh = mesh;
    setResultMeshHighlighted(mesh, true);

    const communication = mesh.userData.communication;
    if (communication.type === 'ray') {
        showRayResults(communication.data, communication.frequencyResult);
    } else if (communication.type === 'polyline') {
        showAuxiliaryPolylines(communication);
        showPolylineResults(communication.data, communication.delayResult);
    } else if (communication.delayResult) {
        removeAuxiliaryPolylines();
        showDelayResults(delayPayload, communication.delayResult);
    } else {
        showGeneralResults(activePayload, communication.frequencyResult);
    }
}

function showGeneralResults(payload, frequencyResult) {
    resultsPanel.innerHTML = '';

    const table = document.createElement('table');
    appendResultRow(table, 'Frequency', `${frequencyResult.frequency} MHz`);
    appendResultRow(table, 'Polarization', frequencyResult.polarization);
    appendResultRow(table, 'Distance', `${payload.distance.toFixed(2)} m`);
    appendResultRow(table, 'Total power P', formatPower(frequencyResult.totalPower));
    appendResultRow(table, 'Directional power P₀', formatPower(frequencyResult.directionalPower));
    appendResultRow(table, 'Rays', String(frequencyResult.rays?.length ?? 0));
    if (frequencyResult.ellipsoid) {
        appendResultRow(table, 'Dominant delay', formatDelay(frequencyResult.ellipsoid.delay));
    }
    resultsPanel.appendChild(table);

    // Per delay ellipsoid components P_n, collapsed by default.
    const components = frequencyResult.ellipsoidComponents ?? [];
    if (components.length > 0) {
        const details = document.createElement('details');
        const summary = document.createElement('summary');
        summary.textContent = `Ellipsoid components [${components.length}]`;
        details.appendChild(summary);

        const componentsTable = document.createElement('table');
        for (const component of components) {
            appendResultRow(componentsTable, formatDelay(component.delay), `p'ₙ = ${formatPower(component.measuredFractionalPower)}`);
        }
        details.appendChild(componentsTable);
        resultsPanel.appendChild(details);
    }

    const note = document.createElement('div');
    note.className = 'gltf-muted';
    note.style.marginTop = '6px';
    note.textContent = 'Click a green ray in the 3D view for the ray specific values; click the ellipsoid to return to this summary.';
    resultsPanel.appendChild(note);

    resultsCard.style.display = '';
}

function showRayResults(ray, frequencyResult) {
    resultsPanel.innerHTML = '';

    const table = document.createElement('table');
    appendResultRow(table, 'Selected', 'Ray');
    appendResultRow(table, 'Frequency', `${frequencyResult.frequency} MHz`);
    appendResultRow(table, 'Delay', formatDelay(ray.delay));
    appendResultRow(table, 'Theta', formatAngle(ray.theta));
    appendResultRow(table, 'Phi', formatAngle(ray.phi));
    appendResultRow(table, 'Power pₙₖₗ', formatPower(ray.power));
    appendResultRow(table, 'Relative power', `${(10 * Math.log10(ray.power)).toFixed(1)} dB`);
    resultsPanel.appendChild(table);

    const note = document.createElement('div');
    note.className = 'gltf-muted';
    note.style.marginTop = '6px';
    note.textContent = 'Click the ellipsoid to return to the general results.';
    resultsPanel.appendChild(note);

    resultsCard.style.display = '';
}

function addEllipsoidObject(payload, frequencyResult) {
    const ellipsoid = frequencyResult.ellipsoid;
    if (!ellipsoid) {
        return;
    }

    // Unit sphere scaled to the semi axes: the local X axis carries the semi-major axis and is
    // rotated onto the transmitter-receiver direction (the ellipsoid is rotationally symmetric
    // around it, so no further orientation is needed).
    const mesh = new THREE.Mesh(
        new THREE.SphereGeometry(1, 48, 32),
        new THREE.MeshBasicMaterial({ color: ELLIPSOID_COLOR, transparent: true, opacity: ELLIPSOID_OPACITY, side: THREE.DoubleSide, depthWrite: false }));

    mesh.position.copy(toScene(ellipsoid.center.x, ellipsoid.center.y, ellipsoid.center.z));
    mesh.scale.set(ellipsoid.semiMajorAxis, ellipsoid.semiMinorAxis, ellipsoid.semiMinorAxis);
    mesh.quaternion.setFromUnitVectors(new THREE.Vector3(1, 0, 0), toSceneDirection(ellipsoid.axis));
    mesh.userData.communication = { type: 'ellipsoid', data: ellipsoid, frequencyResult };

    viewer.scene.add(mesh);
    resultObjects.push(mesh);
    resultMeshes.push(mesh);
}

function addRayObjects(payload, frequencyResult) {
    const rays = frequencyResult.rays ?? [];

    let maximumPower = 0;
    for (const ray of rays) {
        maximumPower = Math.max(maximumPower, ray.power);
    }
    if (maximumPower <= 0) {
        return;
    }

    const origin = toScene(payload.receiver.x, payload.receiver.y, payload.receiver.z);
    const radius = Math.max(0.15, antennaDotRadius() * 0.3);

    for (const ray of rays) {
        if (ray.power <= 0) {
            continue;
        }

        // Ray lengths follow a dB scale relative to the strongest ray: the corrected powers span
        // several orders of magnitude, so linear scaling would hide all but the dominant ray.
        const attenuation = 10 * Math.log10(ray.power / maximumPower); // <= 0
        const factor = Math.max(0, 1 + (attenuation / RAY_DECIBEL_WINDOW));
        const length = payload.distance * (0.08 + (0.32 * factor));

        const direction = toSceneDirection(ray.direction);

        // Thin cylinders instead of THREE.Line so the rays raycast (select) reliably.
        const mesh = new THREE.Mesh(
            new THREE.CylinderGeometry(radius, radius, length, 8),
            new THREE.MeshBasicMaterial({ color: RAY_COLOR }));
        mesh.position.copy(origin.clone().add(direction.clone().multiplyScalar(length / 2)));
        mesh.quaternion.setFromUnitVectors(new THREE.Vector3(0, 1, 0), direction);
        mesh.userData.communication = { type: 'ray', data: ray, frequencyResult };

        viewer.scene.add(mesh);
        resultObjects.push(mesh);
        resultMeshes.push(mesh);
    }
}

function renderResults(payload) {
    clearResults();
    activePayload = payload;

    // AI-NOTE (multi-frequency rendering): payload.results holds one entry per calculated
    // frequency; only the first entry is rendered today. To expose the remaining entries, iterate
    // the array here, keep the created meshes grouped per frequency and bind per frequency
    // visibility toggles in the results panel.
    const frequencyResult = payload.results[0];

    addEllipsoidObject(payload, frequencyResult);
    addRayObjects(payload, frequencyResult);
    showGeneralResults(payload, frequencyResult);
}

// ---------------------------------------------------------------------------------------------
// Delay based (v1) results: the payload carries one entry per delay (ascending). The General
// panel drives which delay is rendered — the propagation ellipsoid(s), the scattering polylines
// (one per ScatteringPointGroup) and the angular power distribution vectors (scaled by the user
// provided factor). Selecting a polyline shows its semi-transparent auxiliary polylines
// (location 1 -> scattering point -> location 2, one per point).
// ---------------------------------------------------------------------------------------------

function vectorScale() {
    const scale = parseFloat(vectorScaleInput.value);
    return isFinite(scale) && scale > 0 ? scale : DEFAULT_VECTOR_SCALE;
}

function renderDelayResults(payload) {
    clearResults();
    activePayload = payload;
    delayPayload = payload;

    delaySlider.min = 0;
    delaySlider.max = payload.results.length - 1;

    // Start on the first delay above 0. Index 0 is the direct path (delay 0) which carries no
    // scattering/propagation results, so defaulting to it leaves the view empty; jump to the first
    // ascending entry with a positive delay so results are visible immediately. Falls back to 0 if
    // no entry qualifies.
    const initialDelayIndex = Math.max(0, payload.results.findIndex(result => result.delay > 0));
    delaySlider.value = initialDelayIndex;
    generalCard.style.display = '';

    renderDelayFrame(initialDelayIndex);
}

function clearDelayFrame() {
    removeAuxiliaryPolylines();
    selectedResultMesh = null;
    resultPickCandidate = null;

    for (const object of delayObjects) {
        viewer.scene.remove(object);
        object.geometry?.dispose();
        object.material?.dispose();
    }
    delayObjects.length = 0;

    // In the delay based flow every pickable result mesh belongs to the rendered delay frame.
    resultMeshes.length = 0;
}

function renderDelayFrame(index) {
    clearDelayFrame();

    const delayResult = delayPayload.results[index];
    delayValueLabel.textContent = formatDelay(delayResult.delay);

    addDelayEllipsoids(delayResult);
    addScatteringPolylines(delayResult);
    addVectorGroups(delayResult);
    showDelayResults(delayPayload, delayResult);
}

function addDelayEllipsoids(delayResult) {
    for (const ellipsoid of delayResult.ellipsoids ?? []) {
        // Unit sphere scaled to the semi axes, exactly like addEllipsoidObject: the local X axis
        // carries the semi-major axis and is rotated onto the profile axis (the ellipsoid is
        // rotationally symmetric around it).
        const mesh = new THREE.Mesh(
            new THREE.SphereGeometry(1, 48, 32),
            new THREE.MeshBasicMaterial({ color: ELLIPSOID_COLOR, transparent: true, opacity: ELLIPSOID_OPACITY, side: THREE.DoubleSide, depthWrite: false }));

        mesh.position.copy(toScene(ellipsoid.center.x, ellipsoid.center.y, ellipsoid.center.z));
        mesh.scale.set(ellipsoid.semiMajorAxis, ellipsoid.semiMinorAxis, ellipsoid.semiMinorAxis);
        mesh.quaternion.setFromUnitVectors(new THREE.Vector3(1, 0, 0), toSceneDirection(ellipsoid.axis));
        mesh.userData.communication = { type: 'ellipsoid', data: ellipsoid, delayResult };

        viewer.scene.add(mesh);
        delayObjects.push(mesh);
        resultMeshes.push(mesh);
    }
}

// A thick straight tube between two scene points, built like the ray cylinders (a Mesh, so it
// raycasts reliably). Callers pass a shared material so a multi-segment polyline recolors as one.
function cylinderBetween(start, end, radius, material) {
    const direction = new THREE.Vector3().subVectors(end, start);
    const length = direction.length();

    const mesh = new THREE.Mesh(new THREE.CylinderGeometry(radius, radius, length, 8), material);
    mesh.position.copy(start).add(end).multiplyScalar(0.5);
    if (length > 1e-9) {
        mesh.quaternion.setFromUnitVectors(new THREE.Vector3(0, 1, 0), direction.divideScalar(length));
    }
    return mesh;
}

function addScatteringPolylines(delayResult) {
    const radius = Math.max(0.25, antennaDotRadius() * SCATTERING_RADIUS_FACTOR);
    const polylines = delayResult.polylines ?? [];
    for (let index = 0; index < polylines.length; index++) {
        const polyline = polylines[index];
        const points = (polyline.points ?? []).map((point) => toScene(point.x, point.y, point.z));

        // A scattering point group can arrive as a closed ring (its last point duplicating the
        // first). Drop the duplicated tail so the polyline keeps a distinct start and end.
        if (points.length > 2 && points[0].distanceToSquared(points[points.length - 1]) < 1e-6) {
            points.pop();
        }

        // A polyline needs at least two points. Single-point groups are no longer drawn — the old
        // "big dot" sphere is intentionally hidden.
        if (points.length < 2) {
            continue;
        }

        // One shared material and userData per polyline so every segment/joint highlights together
        // on selection and resolves to the same scattering group when picked.
        const communication = { type: 'polyline', data: polyline, delayResult, baseColor: SCATTERING_COLOR };
        const material = new THREE.MeshBasicMaterial({ color: SCATTERING_COLOR });

        for (let i = 0; i < points.length - 1; i++) {
            const segment = cylinderBetween(points[i], points[i + 1], radius, material);
            segment.userData.communication = communication;
            viewer.scene.add(segment);
            delayObjects.push(segment);
            resultMeshes.push(segment);

            // Fill the interior joints (not the two ends) so corners have no wedge gap.
            if (i > 0) {
                const joint = new THREE.Mesh(new THREE.SphereGeometry(radius, 8, 6), material);
                joint.position.copy(points[i]);
                joint.userData.communication = communication;
                viewer.scene.add(joint);
                delayObjects.push(joint);
                resultMeshes.push(joint);
            }
        }
    }
}

function addVectorGroups(delayResult) {
    const scale = vectorScale();
    const radius = Math.max(0.2, antennaDotRadius() * VECTOR_RADIUS_FACTOR);
    const material = new THREE.MeshBasicMaterial({ color: VECTOR_COLOR });

    for (const vectorGroup of delayResult.vectorGroups ?? []) {
        const origin = toScene(vectorGroup.location.x, vectorGroup.location.y, vectorGroup.location.z);

        for (const vector of vectorGroup.vectors ?? []) {
            // Reversed direction (requirement): negate the (Z-up -> Y-up) vector so it points
            // opposite to the raw sample, extended from the origin and stretched by the user scale
            // factor (default 1000). The length still carries the power; drawn as a thick cylinder.
            const tip = origin.clone().add(new THREE.Vector3(-vector.x, -vector.z, vector.y).multiplyScalar(scale));
            if (origin.distanceToSquared(tip) < 1e-9) {
                continue; // zero-power vector — nothing to draw
            }

            const cylinder = cylinderBetween(origin, tip, radius, material);
            viewer.scene.add(cylinder);
            delayObjects.push(cylinder);
        }
    }
}

function showAuxiliaryPolylines(communication) {
    removeAuxiliaryPolylines();

    const polyline = communication.data;
    const location1 = toScene(polyline.location1.x, polyline.location1.y, polyline.location1.z);
    const location2 = toScene(polyline.location2.x, polyline.location2.y, polyline.location2.z);

    // One auxiliary polyline (location 1 -> scattering point -> location 2) per point, merged
    // into a single LineSegments object so even dense groups stay one draw call.
    const points = [];
    for (const point of polyline.points ?? []) {
        const scenePoint = toScene(point.x, point.y, point.z);
        points.push(location1, scenePoint, scenePoint, location2);
    }

    if (points.length === 0) {
        return;
    }

    auxiliaryObject = new THREE.LineSegments(
        new THREE.BufferGeometry().setFromPoints(points),
        new THREE.LineBasicMaterial({ color: communication.baseColor, transparent: true, opacity: AUXILIARY_OPACITY, depthWrite: false }));

    viewer.scene.add(auxiliaryObject);
}

function removeAuxiliaryPolylines() {
    if (!auxiliaryObject) {
        return;
    }

    viewer.scene.remove(auxiliaryObject);
    auxiliaryObject.geometry.dispose();
    auxiliaryObject.material.dispose();
    auxiliaryObject = null;
}

function showDelayResults(payload, delayResult) {
    resultsPanel.innerHTML = '';

    let vectorCount = 0;
    for (const vectorGroup of delayResult.vectorGroups ?? []) {
        vectorCount += vectorGroup.vectors?.length ?? 0;
    }

    const table = document.createElement('table');
    appendResultRow(table, 'Delay', formatDelay(delayResult.delay));
    appendResultRow(table, 'Distance', `${payload.distance.toFixed(2)} m`);
    appendResultRow(table, 'Scattering groups', String((delayResult.polylines ?? []).length));
    appendResultRow(table, 'Vectors', String(vectorCount));
    resultsPanel.appendChild(table);

    const note = document.createElement('div');
    note.className = 'gltf-muted';
    note.style.marginTop = '6px';
    note.textContent = 'Move the delay slider in the General panel to change the displayed delay. Click a scattering polyline for its auxiliary polylines.';
    resultsPanel.appendChild(note);

    resultsCard.style.display = '';
}

function showPolylineResults(polyline, delayResult) {
    resultsPanel.innerHTML = '';

    const table = document.createElement('table');
    appendResultRow(table, 'Selected', 'Scattering polyline');
    appendResultRow(table, 'Reference', polyline.reference ?? '-');
    appendResultRow(table, 'Delay', formatDelay(delayResult.delay));
    appendResultRow(table, 'Points', String(polyline.points?.length ?? 0));
    resultsPanel.appendChild(table);

    const note = document.createElement('div');
    note.className = 'gltf-muted';
    note.style.marginTop = '6px';
    note.textContent = 'The semi-transparent auxiliary polylines connect each scattering point with the profile locations. Click the ellipsoid to return to the delay summary.';
    resultsPanel.appendChild(note);

    resultsCard.style.display = '';
}

function clearResults() {
    removeAuxiliaryPolylines();
    for (const object of delayObjects) {
        viewer.scene.remove(object);
        object.geometry?.dispose();
        object.material?.dispose();
    }
    delayObjects.length = 0;
    delayPayload = null;
    generalCard.style.display = 'none';

    for (const object of resultObjects) {
        viewer.scene.remove(object);
        object.geometry?.dispose();
        object.material?.dispose();
    }
    resultObjects.length = 0;
    resultMeshes.length = 0;
    selectedResultMesh = null;
    resultPickCandidate = null;
    activePayload = null;

    resultsPanel.innerHTML = '';
    resultsCard.style.display = 'none';
}

async function calculate(calculationParameters) {
    if (calculating || antennas.length !== MAX_ANTENNAS) {
        return;
    }

    calculating = true;
    updateToolbar();
    setHint('Calculating…');

    try {
        const body = {
            centerX: parseFloat(container.dataset.centerX),
            centerY: parseFloat(container.dataset.centerY),
            radius: parseFloat(container.dataset.radius),
            storeyHeight: parseFloat(container.dataset.storeyHeight),
            antennas: antennas.map((antenna) => ({
                x: antenna.data.x,
                y: antenna.data.y,
                z: antenna.data.z,
                functions: antenna.data.functions
            }))
        };

        if (calculationParameters.defaultSimpleMultipathPowerDelayProfile !== undefined) {
            body.defaultSimpleMultipathPowerDelayProfile = calculationParameters.defaultSimpleMultipathPowerDelayProfile;
        } else {
            body.frequencies = calculationParameters.frequencies;
            body.polarization = calculationParameters.polarization;
            body.relativePermittivity = calculationParameters.relativePermittivity;
            body.conductivity = calculationParameters.conductivity;
        }

        console.log('Calculate URL:', container.dataset.calculateUrl);
        console.log('Calculate body:', body);

        const response = await fetch(container.dataset.calculateUrl, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(body)
        });

        if (!response.ok || response.status === 204) {
            setHint(`Calculation failed (${response.status}).`);
            return;
        }

        const payload = await response.json();
        if (!payload || !Array.isArray(payload.results) || payload.results.length === 0) {
            setHint('Calculation failed (empty results).');
            return;
        }

        // [TEMPORARY A/B TESTING] The v1 endpoint returns the delay grouped payload
        // (discriminated by its delays array); the v2 endpoint the frequency result payload.
        if (Array.isArray(payload.delays)) {
            renderDelayResults(payload);
        } else {
            renderResults(payload);
        }
        setHint('');
    } catch (error) {
        console.error('Calculate error:', error);
        setHint('Calculation failed (network).');
    } finally {
        calculating = false;
        updateToolbar();
    }
}

// ---------------------------------------------------------------------------------------------
// Toolbar wiring.
// ---------------------------------------------------------------------------------------------

addButton.addEventListener('click', () => {
    if (antennas.length < MAX_ANTENNAS) {
        setMode('add');
    }
});

removeButton.addEventListener('click', () => {
    if (antennas.length > 0) {
        setMode('erase');
    }
});

// "Calculate" opens the modal collecting the propagation inputs that are missing from the
// geometrical model; the calculation request is sent when the modal is confirmed.
calculateButton.addEventListener('click', () => {
    if (calculating || antennas.length !== MAX_ANTENNAS) {
        return;
    }

    calculationModal.style.display = 'flex';
    const isV1 = container.dataset.calculateUrl?.includes('/v1/');
    if (isV1 && calculationProfileSelect) {
        calculationProfileSelect.focus();
    } else if (calculationFrequencyInput) {
        calculationFrequencyInput.focus();
    }
});

calculationOkButton.addEventListener('click', () => {
    calculationModal.style.display = 'none';

    const isV1 = container.dataset.calculateUrl?.includes('/v1/');
    if (isV1) {
        calculate({
            defaultSimpleMultipathPowerDelayProfile: calculationProfileSelect?.value ?? 'TypicalUrban'
        });
        return;
    }

    // AI-NOTE (multi-frequency input): the frequency field accepts a comma separated list; every
    // valid value is sent, the backend calculates all of them and the response carries one result
    // entry per frequency (see renderResults for the rendering extensibility point).
    const frequencies = calculationFrequencyInput.value
        .split(',')
        .map((value) => parseFloat(value.trim()))
        .filter((value) => isFinite(value) && value > 0);

    const relativePermittivity = parseFloat(calculationPermittivityInput.value);
    const conductivity = parseFloat(calculationConductivityInput.value);

    if (frequencies.length === 0 || !isFinite(relativePermittivity) || relativePermittivity < 1 || !isFinite(conductivity) || conductivity < 0) {
        return;
    }

    calculate({
        frequencies,
        polarization: calculationPolarizationSelect.value,
        relativePermittivity,
        conductivity
    });
});

calculationCancelButton.addEventListener('click', () => {
    calculationModal.style.display = 'none';
});

// General panel wiring: the delay slider walks payload.results (ascending delays); the vector
// scale re-renders the current frame so the vectors stretch immediately.
delaySlider.addEventListener('input', () => {
    if (delayPayload !== null) {
        renderDelayFrame(parseInt(delaySlider.value, 10));
    }
});

vectorScaleInput.addEventListener('input', () => {
    if (delayPayload !== null) {
        renderDelayFrame(parseInt(delaySlider.value, 10));
    }
});

function clearAntennas() {
    for (const antenna of antennas) {
        removeAntennaObject(antenna);
    }
    antennas.length = 0;
}

clearButton.addEventListener('click', () => {
    if (viewer !== null) {
        clearResults();
        clearAntennas();
        setMode(null); // exits any active edit/erase mode and refreshes the toolbar state
    }
});

if (container) {
    container.addEventListener('gltf-ready', (event) => {
        viewer = window.gltfViewer;
        referencePoint = event.detail.referencePoint ?? { X: 0, Y: 0, Z: 0 };
        sceneRadius = viewer?.radius ?? 10;
        updateToolbar();
    });
}
