// DiGi.GIS.WebAPI.UI — communication analysis tooling layered on top of the generic
// DiGi.GLTF.WebAPI viewer engine (gltf-viewer.js owns the base panels; this module owns the
// antenna toolbar, the antenna edit/erase modes, the antenna Selection card pick (clicking an
// antenna shows it in the shared Selection card with "Edit" + "Export"; see
// building-details-panel.js), the calculation modal/request and the delay based results
// rendering: the propagation ellipsoids, the scattering polylines and the angular power
// distribution vectors).
//
// Calculation flow: the "Calculate" button opens a modal collecting the multipath power delay
// profile. The request goes to ~/communication/calculate; the server fetches the analyzed area
// buildings, packages everything into a GeometricalPropagationModel and solves the propagation in
// process (ScatteringSolver + AngularPowerDistributionSolver). The response carries world
// coordinates only, grouped by delay (ascending): one entry per delay holding the propagation
// ellipsoid(s), the scattering polylines (one per ScatteringPointGroup) and the angular power
// distribution vectors. The Results panel drives which delay is rendered and the vector scale
// factor; a selected polyline shows its semi-transparent auxiliary polylines and its data in the
// shared Selection card (see renderDelayResults).
//
// AI-NOTE (mocked remaining inputs): the antenna radiation characteristics are hardcoded server
// side, mirroring the reference xUnit fact ToPropagation_PropagationModel_TypicalUrban; extend the
// calculation modal once they become user configurable.

import * as THREE from 'three';
import { reportStatus, updateLastStatus, formatElapsed } from 'gltf-viewer-core';

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
const DEFAULT_VECTOR_SCALE = 10;         // default stretch applied to the angular power vectors (matches the input value in the Results panel)
const RAY_DECIBEL_WINDOW = 30;           // dB range mapped onto the ray length scale
const CLICK_THRESHOLD = 5;               // pixels of pointer travel before a click becomes a drag
const DEFAULT_ANTENNA_HEIGHT = 15;       // default Z coordinate for the antenna modal and live preview
const DEFAULT_FREQUENCY_MHZ = 5000;      // default frequency of the calculation modal in MHz (matches the input value)
const ANTENNA_PREVIEW_OPACITY = 0.5;     // semi-transparent live preview during add mode

const container = document.getElementById('gltf-viewer-container');

const addButton = document.getElementById('communication-add-antenna-button');
const removeButton = document.getElementById('communication-remove-antenna-button');
const calculateButton = document.getElementById('communication-calculate-button');
const clearButton = document.getElementById('communication-clear-button');
const calculationLoader = document.getElementById('communication-calculation-loader');

const modal = document.getElementById('communication-antenna-modal');
const modalTitle = document.getElementById('communication-antenna-modal-title');
const modalX = document.getElementById('communication-antenna-x');
const modalY = document.getElementById('communication-antenna-y');
const modalZ = document.getElementById('communication-antenna-z');
const modalOkButton = document.getElementById('communication-antenna-ok-button');
const modalCancelButton = document.getElementById('communication-antenna-cancel-button');

const antennaEditButton = document.getElementById('communication-antenna-edit-button');

const calculationModal = document.getElementById('communication-calculation-modal');
const calculationProfileSelect = document.getElementById('communication-calculation-profile');
const calculationFrequencyInput = document.getElementById('communication-calculation-frequency');
const calculationOkButton = document.getElementById('communication-calculation-ok-button');
const calculationCancelButton = document.getElementById('communication-calculation-cancel-button');

const resultsCard = document.getElementById('communication-results-card');
const resultsPanel = document.getElementById('communication-results');
const selectionPanel = document.getElementById('communication-selection');

const delayControls = document.getElementById('communication-delay-controls');
const delaySlider = document.getElementById('communication-delay-slider');
const delayValueLabel = document.getElementById('communication-delay-value');
const vectorScaleInput = document.getElementById('communication-vector-scale');

const resultsDetailsButton = document.getElementById('communication-results-details-button');

const matrixModal = document.getElementById('communication-scattering-matrix-modal');
const matrixTitle = document.getElementById('communication-scattering-matrix-title');
const matrixDelaySelect = document.getElementById('communication-scattering-matrix-delay');
const matrixPanel = document.getElementById('communication-scattering-matrix');
const matrixCloseButton = document.getElementById('communication-scattering-matrix-close-button');

const hitsModal = document.getElementById('communication-scattering-hits-modal');
const hitsTitle = document.getElementById('communication-scattering-hits-title');
const hitsPanel = document.getElementById('communication-scattering-hits');
const hitsCloseButton = document.getElementById('communication-scattering-hits-close-button');

let viewer = null;                 // GltfViewer instance exposed by gltf-viewer.js
let referencePoint = { X: 0, Y: 0, Z: 0 };
let sceneRadius = 10;
let mode = null;                   // null | 'add' | 'erase'
let calculating = false;
let pointerDownPosition = null;

const antennas = [];               // { group, meshes, selected, data: { x, y, z, functions } }
const resultObjects = [];          // three.js objects added by "Calculate"
const resultMeshes = [];           // selectable subset of resultObjects (ellipsoid + polyline + vector meshes)
let selectedResultMesh = null;     // currently highlighted result mesh
let resultPickCandidate = null;    // result mesh hit on pointerdown, resolved on pointerup
let antennaPickCandidate = null;   // antenna hit on pointerdown, resolved on pointerup
let selectedAntenna = null;        // antenna shown in the Selection card (normal mode click)
let editingAntenna = null;         // antenna being edited through the antenna modal (null = add flow)
let polylineSelected = false;      // Selection card currently shows a scattering polyline
let suppressResultClick = false;   // swallows the click event that follows a result pick
let antennaPreview = null;         // semi-transparent antenna preview shown during add mode

let delayPayload = null;           // last successful delay based (v1) calculation payload
const delayObjects = [];           // three.js objects of the currently rendered delay frame
let auxiliaryObject = null;        // auxiliary polylines of the selected scattering polyline
let pendingGroundZ = 0;            // snapped ground Z for the pending antenna add

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

function setMode(value) {
    mode = value;
    container.style.cursor = value === 'add' ? 'crosshair' : value === 'erase' ? 'pointer' : '';

    if (value !== null) {
        // The edit modes reuse the orange antenna highlight (erase) and the ground plane clicks
        // (add), so the Selection card antenna pick is dropped before the mode starts.
        deselectAntennaObject();
    }

    if (value === 'add') {
        reportStatus('Click a point on the ground plane to place the antenna. Press Esc to cancel.');
    } else if (value === 'erase') {
        reportStatus('Click antennas to select them. Press Enter to remove the selected antennas, Esc to cancel.');
    } else {
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
    return Math.max(0.6, sceneRadius * 0.006) * 0.5;
}

function buildAntennaVisual(data) {
    const dotRadius = antennaDotRadius();
    const baseZ = data.groundZ !== undefined ? data.groundZ : (data.z > DEFAULT_ANTENNA_HEIGHT ? data.z - DEFAULT_ANTENNA_HEIGHT : 0);
    const base = toScene(data.x, data.y, baseZ);
    const top = toScene(data.x, data.y, data.z);

    const group = new THREE.Group();
    const meshes = [];

    const mastHeight = Math.max(0, top.y - base.y);
    if (mastHeight > 0) {
        const mastGeometry = new THREE.CylinderGeometry(dotRadius * 0.12, dotRadius * 0.12, mastHeight, 8);
        const mast = new THREE.Mesh(mastGeometry, new THREE.MeshBasicMaterial({ color: ANTENNA_COLOR }));
        mast.position.set(base.x, base.y + mastHeight / 2, base.z);
        group.add(mast);
        meshes.push(mast);
    }

    const dot = new THREE.Mesh(new THREE.SphereGeometry(dotRadius, 16, 12), new THREE.MeshBasicMaterial({ color: ANTENNA_COLOR }));
    dot.position.copy(top);
    group.add(dot);
    meshes.push(dot);

    return { group, meshes };
}

function addAntennaObject(data) {
    const { group, meshes } = buildAntennaVisual(data);
    viewer.scene.add(group);

    antennas.push({ group, meshes, selected: false, data });
    updateToolbar();
}

// Rebuilds the antenna visual in place (same antennas array entry, so the array order and the
// Selection card pick survive the edit) and refreshes the Selection card payload.
function updateAntennaObject(antenna, data) {
    removeAntennaObject(antenna);

    const { group, meshes } = buildAntennaVisual(data);
    viewer.scene.add(group);

    antenna.group = group;
    antenna.meshes = meshes;
    antenna.data = data;

    if (antenna.selected) {
        setAntennaSelected(antenna, true);
    }

    if (selectedAntenna === antenna) {
        notifyAntennaSelection(antenna);
    }
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
// Antenna selection (normal mode): clicking an antenna shows it in the shared "Selection" card
// (building-details-panel.js listens to the custom event below and drives the card: "Edit" reopens
// the antenna modal, "Export" shows the antenna data like for buildings). The pick replaces any
// building selection; any building selection change drops the antenna pick again.
// ---------------------------------------------------------------------------------------------

function notifyAntennaSelection(antenna) {
    container.dispatchEvent(new CustomEvent('communication-antennaselectionchanged', {
        detail: {
            antenna: antenna === null ? null : {
                x: antenna.data.x,
                y: antenna.data.y,
                z: antenna.data.z,
                functions: [...(antenna.data.functions ?? [])]
            }
        }
    }));
}

function pickAntennaObject(event) {
    if (antennas.length === 0 || viewer === null) {
        return null;
    }

    raycaster.setFromCamera(pointerNdc(event), viewer.camera);
    for (const antenna of antennas) {
        if (raycaster.intersectObjects(antenna.meshes, false).length > 0) {
            return antenna;
        }
    }

    return null;
}

function selectAntennaObject(antenna) {
    // Clearing the viewer selection first: the resulting 'gltf-selectionchanged' event hides the
    // building entries of the Selection card before the antenna entry is shown (and would otherwise
    // drop the antenna pick made below).
    viewer.clearSelection();

    if (selectedAntenna !== null && selectedAntenna !== antenna) {
        setAntennaSelected(selectedAntenna, false);
    }

    selectedAntenna = antenna;
    setAntennaSelected(antenna, true);
    notifyAntennaSelection(antenna);
}

function deselectAntennaObject() {
    if (selectedAntenna === null) {
        return;
    }

    setAntennaSelected(selectedAntenna, false);
    selectedAntenna = null;
    notifyAntennaSelection(null);
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

    // Result/antenna selection: while no edit mode is active, a press on an antenna, on the
    // ellipsoid or on a ray is intercepted before the generic viewer sees it, exactly like the
    // antenna edit modes — otherwise the viewer would start a marquee/building selection. Antennas
    // win over the result meshes (they are the smaller targets).
    if (mode === null) {
        antennaPickCandidate = pickAntennaObject(event);
        resultPickCandidate = antennaPickCandidate !== null ? null : pickResultMesh(event);
        if (antennaPickCandidate === null && resultPickCandidate === null) {
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
        if (antennaPickCandidate === null && resultPickCandidate === null) {
            return;
        }

        event.stopPropagation();
        suppressResultClick = true;

        const antenna = antennaPickCandidate;
        const resultMesh = resultPickCandidate;
        antennaPickCandidate = null;
        resultPickCandidate = null;

        if (!pointerDownPosition) {
            return;
        }
        const travel_Result = Math.hypot(event.clientX - pointerDownPosition.x, event.clientY - pointerDownPosition.y);
        pointerDownPosition = null;
        if (travel_Result > CLICK_THRESHOLD) {
            return;
        }

        if (antenna !== null) {
            selectAntennaObject(antenna);
        } else {
            selectResultMesh(resultMesh);
        }
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

function raycastGroundPoint(event) {
    if (!viewer) {
        return null;
    }

    raycaster.setFromCamera(pointerNdc(event), viewer.camera);

    const pickables = viewer.batchMeshes?.length > 0 ? viewer.batchMeshes : (viewer.objects?.map((o) => o.mesh).filter(Boolean) ?? []);
    if (pickables.length > 0) {
        const intersections = raycaster.intersectObjects(pickables, false);
        if (intersections.length > 0) {
            return intersections[0].point;
        }
    }

    const intersection = new THREE.Vector3();
    if (raycaster.ray.intersectPlane(groundPlane, intersection)) {
        return intersection;
    }

    return null;
}

function onPointerMove(event) {
    if (mode !== 'add' || !viewer) {
        return;
    }

    const point = raycastGroundPoint(event);
    if (point) {
        updateAntennaPreview(point);
    } else {
        hideAntennaPreview();
    }
}

container.addEventListener('pointermove', onPointerMove);

// ---------------------------------------------------------------------------------------------
// Add antenna: click on the terrain / ground plane -> modal with editable values.
// ---------------------------------------------------------------------------------------------

function handleAddClick(event) {
    const point = raycastGroundPoint(event);
    if (!point) {
        return;
    }

    const world = toWorld(point);
    pendingGroundZ = world.z;

    editingAntenna = null;
    modalTitle.textContent = 'Add antenna';
    modalX.value = world.x.toFixed(2);
    modalY.value = world.y.toFixed(2);
    modalZ.value = (world.z + DEFAULT_ANTENNA_HEIGHT).toFixed(2);
    for (const checkbox of modal.querySelectorAll('.communication-antenna-function')) {
        checkbox.checked = antennas.length === 0 ? checkbox.value === 'Transmitter'
            : antennas.length === 1 ? checkbox.value === 'Receiver'
                : false;
    }

    setMode(null);
    modal.style.display = 'flex';
    modalZ.focus();
}

// "Edit" (Selection card): reopens the antenna modal pre-filled with the selected antenna values;
// OK applies the changes to the antenna, Cancel (and Esc) closes without touching it.
antennaEditButton?.addEventListener('click', () => {
    if (selectedAntenna === null) {
        return;
    }

    editingAntenna = selectedAntenna;
    modalTitle.textContent = 'Edit antenna';
    modalX.value = String(editingAntenna.data.x);
    modalY.value = String(editingAntenna.data.y);
    modalZ.value = String(editingAntenna.data.z);
    for (const checkbox of modal.querySelectorAll('.communication-antenna-function')) {
        checkbox.checked = (editingAntenna.data.functions ?? []).includes(checkbox.value);
    }

    modal.style.display = 'flex';
    modalZ.focus();
});

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

    if (editingAntenna !== null) {
        const antenna = editingAntenna;
        editingAntenna = null;

        const data = antenna.data;
        const changed = data.x !== x || data.y !== y || data.z !== z
            || (data.functions ?? []).length !== functions.length
            || (data.functions ?? []).some((value, index) => value !== functions[index]);
        if (!changed) {
            reportStatus('Antenna unchanged.');
            return;
        }

        updateAntennaObject(antenna, { x, y, z, groundZ: data.groundZ, functions });
        reportStatus('Antenna updated.');
        return;
    }

    addAntennaObject({ x, y, z, groundZ: pendingGroundZ, functions });
    reportStatus('Antenna added.');
});

modalCancelButton.addEventListener('click', () => {
    modal.style.display = 'none';
    if (editingAntenna !== null) {
        editingAntenna = null;
        reportStatus('Antenna edit cancelled.');
        return;
    }
    reportStatus('Antenna placement cancelled.');
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
    let removedCount = 0;
    for (let index = antennas.length - 1; index >= 0; index--) {
        const antenna = antennas[index];
        if (remove && antenna.selected) {
            removeAntennaObject(antenna);
            antennas.splice(index, 1);
            removedCount++;
        } else {
            setAntennaSelected(antenna, false);
        }
    }

    setMode(null); // also re-enables "Add antenna" when applicable (see updateToolbar)
    if (remove) {
        if (removedCount > 0) {
            reportStatus(`${removedCount} antenna(s) removed.`);
        } else {
            reportStatus('No antennas selected for removal.');
        }
    } else {
        reportStatus('Antenna removal cancelled.');
    }
}

window.addEventListener('keydown', (event) => {
    // The scattering popups are checked first so Escape peels them off one at a time: the hits table
    // sits on top of the matrix, which in turn sits on top of the page.
    if (hitsModal.style.display !== 'none' && event.key === 'Escape') {
        hitsModal.style.display = 'none';
        return;
    }

    if (matrixModal.style.display !== 'none' && event.key === 'Escape') {
        matrixModal.style.display = 'none';
        return;
    }

    if (calculationModal.style.display !== 'none' && event.key === 'Escape') {
        calculationModal.style.display = 'none';
        return;
    }

    if (modal.style.display !== 'none' && event.key === 'Escape') {
        modal.style.display = 'none';
        if (editingAntenna !== null) {
            editingAntenna = null;
            reportStatus('Antenna edit cancelled.');
            return;
        }
        reportStatus('Antenna placement cancelled.');
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
        reportStatus('Antenna placement cancelled.');
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

// Seconds -> microseconds, the unit every delay is presented in. Kept separate from formatDelay so a
// caller whose label already carries the unit (the delay selector of the scattering matrix modal) can
// render the bare number.
function formatDelayValue(delay) {
    return (delay * 1e6).toFixed(2);
}

function formatDelay(delay) {
    return `${formatDelayValue(delay)} µs`;
}

function formatAngle(radians) {
    return radians === null || radians === undefined ? '—' : `${(radians * 180 / Math.PI).toFixed(1)}°`;
}

// Strict "(X;Y;Z)" form of a payload coordinate (a Point3DResult or a Vector3DResult).
function formatCoordinate(coordinate, digits) {
    if (!coordinate) {
        return '—';
    }

    return `(${coordinate.x.toFixed(digits)};${coordinate.y.toFixed(digits)};${coordinate.z.toFixed(digits)})`;
}

// Payload numbers the server sends as null in place of a double.NaN it could not derive: NaN is not
// valid JSON, so the absent value arrives as null rather than as a number.
function formatValue(value, digits) {
    return value === null || value === undefined ? '—' : value.toFixed(digits);
}

// The tolerance the server renders a reflection coefficient with, mirrored here so a coefficient this
// side derives reads exactly like one the payload carries already rendered.
const REFLECTION_TOLERANCE = 0.001;

// Twin of DiGi.Core.Convert.ToSystem_String(Complex, tolerance, tolerance): each component is rounded
// to a multiple of the tolerance away from zero and the pair is rendered as "{real}{+|-}j{|imaginary|}".
// Needed because an averaged coefficient never passes through the server - the payload bins are finer
// than the buckets the matrix displays, so the averaging happens here.
function formatComplex(real, imaginary) {
    const roundedReal = roundToTolerance(real);
    const roundedImaginary = roundToTolerance(imaginary);

    return `${roundedReal}${roundedImaginary < 0 ? '-' : '+'}j${Math.abs(roundedImaginary)}`;
}

// Math.round breaks a .5 tie upwards rather than away from zero, which would round a negative
// component the opposite way to the server, so the magnitude is rounded and the sign reapplied. The
// toFixed pass drops the floating point noise the division and multiplication leave behind, which the
// server avoids by rounding in decimal.
function roundToTolerance(value) {
    const rounded = Math.sign(value) * Math.round(Math.abs(value) / REFLECTION_TOLERANCE) * REFLECTION_TOLERANCE;

    return parseFloat(rounded.toFixed(3));
}

// Mean reflection coefficient of a group of hits, rendered. Hits whose coefficient could not be
// derived carry null and are left out rather than poisoning the mean; null when none of them has one.
function formatAverageVerticalPolarizationReflection(hits) {
    let real = 0;
    let imaginary = 0;
    let count = 0;

    for (const hit of hits) {
        const value = hit.verticalPolarizationReflectionValue;
        if (!value) {
            continue;
        }

        real += value.real;
        imaginary += value.imaginary;
        count++;
    }

    return count === 0 ? null : formatComplex(real / count, imaginary / count);
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

// Selecting the ellipsoid shows the delay summary; selecting a scattering polyline shows its
// semi-transparent auxiliary polylines and its data in the shared Selection card.
function selectResultMesh(mesh) {
    const communication = mesh.userData.communication;

    if (communication.type === 'polyline') {
        // The polyline data goes to the shared Selection card: drop any building entry first (the
        // resulting 'gltf-selectionchanged' event also drops any antenna or previous polyline
        // entry before the new one is shown below).
        viewer.clearSelection();
    }

    if (selectedResultMesh !== null) {
        setResultMeshHighlighted(selectedResultMesh, false);
    }

    selectedResultMesh = mesh;
    setResultMeshHighlighted(mesh, true);

    if (communication.type === 'polyline') {
        showAuxiliaryPolylines(communication);
        showPolylineSelection(communication.data, communication.delayResult);
    } else if (communication.delayResult) {
        deselectPolylineObject();
        removeAuxiliaryPolylines();
        showDelayResults(delayPayload, communication.delayResult);
    }
}

// ---------------------------------------------------------------------------------------------
// Polyline selection: the data of the selected scattering polyline is shown in the shared
// "Selection" card (the #communication-selection block owned by this module;
// building-details-panel.js listens to the custom event below and drives the card visibility and
// its building/antenna buttons, which stay hidden while a polyline is shown).
// ---------------------------------------------------------------------------------------------

function notifyPolylineSelection(selected) {
    container.dispatchEvent(new CustomEvent('communication-resultselectionchanged', { detail: { selected } }));
}

function showPolylineSelection(polyline, delayResult) {
    selectionPanel.innerHTML = '';

    const table = document.createElement('table');
    appendResultRow(table, 'Selected', 'Scattering polyline');
    appendResultRow(table, 'Reference', polyline.reference ?? '-');
    appendResultRow(table, 'Delay', formatDelay(delayResult.delay));
    appendResultRow(table, 'Points', String(polyline.points?.length ?? 0));
    selectionPanel.appendChild(table);

    const note = document.createElement('div');
    note.className = 'gltf-muted';
    note.style.marginTop = '6px';
    note.textContent = 'The semi-transparent auxiliary polylines connect each scattering point with the profile locations. Click the ellipsoid to deselect the polyline.';
    selectionPanel.appendChild(note);

    selectionPanel.style.display = '';
    polylineSelected = true;
    notifyPolylineSelection(true);
}

function deselectPolylineObject() {
    if (!polylineSelected) {
        return;
    }

    polylineSelected = false;

    if (selectedResultMesh !== null && selectedResultMesh.userData.communication?.type === 'polyline') {
        setResultMeshHighlighted(selectedResultMesh, false);
        selectedResultMesh = null;
    }
    removeAuxiliaryPolylines();

    selectionPanel.innerHTML = '';
    selectionPanel.style.display = 'none';
    notifyPolylineSelection(false);
}

// ---------------------------------------------------------------------------------------------
// Delay based (v1) results: the payload carries one entry per delay (ascending). The delay slider
// in the Results panel drives which delay is rendered — the propagation ellipsoid(s), the
// scattering polylines (one per ScatteringPointGroup) and the angular power distribution vectors
// (scaled by the user provided factor). Selecting a polyline shows its semi-transparent auxiliary
// polylines (location 1 -> scattering point -> location 2, one per point) and its data in the
// shared Selection card.
// ---------------------------------------------------------------------------------------------

function vectorScale() {
    const scale = parseFloat(vectorScaleInput.value);
    return isFinite(scale) && scale > 0 ? scale : DEFAULT_VECTOR_SCALE;
}

function renderDelayResults(payload) {
    clearResults();
    delayPayload = payload;

    delaySlider.min = 0;
    delaySlider.max = payload.results.length - 1;

    // Start on the first delay above 0. Index 0 is the direct path (delay 0) which carries no
    // scattering/propagation results, so defaulting to it leaves the view empty; jump to the first
    // ascending entry with a positive delay so results are visible immediately. Falls back to 0 if
    // no entry qualifies.
    const initialDelayIndex = Math.max(0, payload.results.findIndex(result => result.delay > 0));
    delaySlider.value = initialDelayIndex;
    delayControls.style.display = '';

    renderDelayFrame(initialDelayIndex);
}

function clearDelayFrame() {
    deselectPolylineObject();
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

    // The scattering matrix belongs to one delay; a delay change closes it rather than leaving a
    // matrix of the previous delay on screen. It is reopened from the Details button.
    closeScatteringModals();

    const delayResult = delayPayload.results[index];
    delayValueLabel.textContent = formatDelay(delayResult.delay);

    addDelayEllipsoids(delayResult);
    addScatteringPolylines(delayResult);
    addVectorGroups(delayResult);
    showDelayResults(delayPayload, delayResult);
}

function addDelayEllipsoids(delayResult) {
    for (const ellipsoid of delayResult.ellipsoids ?? []) {
        const material = new THREE.MeshBasicMaterial({ color: ELLIPSOID_COLOR, transparent: true, opacity: ELLIPSOID_OPACITY, side: THREE.DoubleSide, depthWrite: false });

        let mesh;
        if (ellipsoid.mesh?.vertices?.length && ellipsoid.mesh?.indices?.length) {
            // Server-cut ellipsoid part (split by the ground plane): the payload carries the
            // triangulated mesh in world coordinates, so each vertex only needs the DiGi (Z-up)
            // -> three.js (Y-up) scene conversion — no position/scale/rotation is applied.
            const vertices = ellipsoid.mesh.vertices;
            const positions = new Float32Array(vertices.length);
            for (let i = 0; i < vertices.length; i += 3) {
                const scenePoint = toScene(vertices[i], vertices[i + 1], vertices[i + 2]);
                positions[i] = scenePoint.x;
                positions[i + 1] = scenePoint.y;
                positions[i + 2] = scenePoint.z;
            }

            const geometry = new THREE.BufferGeometry();
            geometry.setAttribute('position', new THREE.BufferAttribute(positions, 3));
            geometry.setIndex(ellipsoid.mesh.indices);

            mesh = new THREE.Mesh(geometry, material);
        } else {
            // Fallback (no mesh in the payload): unit sphere scaled to the semi axes — the local X
            // axis carries the semi-major axis and is rotated onto the profile axis (the ellipsoid
            // is rotationally symmetric around it).
            mesh = new THREE.Mesh(new THREE.SphereGeometry(1, 48, 32), material);
            mesh.position.copy(toScene(ellipsoid.center.x, ellipsoid.center.y, ellipsoid.center.z));
            mesh.scale.set(ellipsoid.semiMajorAxis, ellipsoid.semiMinorAxis, ellipsoid.semiMinorAxis);
            mesh.quaternion.setFromUnitVectors(new THREE.Vector3(1, 0, 0), toSceneDirection(ellipsoid.axis));
        }

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

    // Shown for a delay without hits of its own as well, as long as the payload carries a combined
    // distribution: the modal then opens on Combined, which is otherwise unreachable from that delay.
    resultsDetailsButton.style.display = hasScatteringHits(delayResult) || hasCombinedScatteringHits(payload) ? '' : 'none';

    resultsCard.style.display = '';
}

// ---------------------------------------------------------------------------------------------
// Scattering hits, a two step drill-down mirroring the nesting of the payload: "Details" opens the
// azimuth/elevation matrix of the selected delay, and a matrix cell opens the individual hits of that
// bin (cell.hits). The delay selector of the matrix closes with a Combined entry, which loads the same
// matrix built from the hits of all delays at once (payload.combined).
// ---------------------------------------------------------------------------------------------

// Value of the Combined entry of the delay selector. Every other option carries an index into
// payload.results, so the sentinel is deliberately not a number: parseInt would otherwise swallow it.
const COMBINED_MATRIX_OPTION = 'combined';

function hasScatteringHits(delayResult) {
    return (delayResult?.angularDistributions ?? []).some(angularDistribution => (angularDistribution.cells ?? []).length > 0);
}

// The combined distributions carry the hits of all delays at once, one per angular power distribution
// profile, so they are shaped exactly like the angularDistributions of a delay and render unchanged.
function hasCombinedScatteringHits(payload) {
    return (payload?.combined ?? []).some(angularDistribution => (angularDistribution.cells ?? []).length > 0);
}

// The collection bins at half a degree, which is finer than the matrix needs and splits a whole
// degree across two columns. The matrix therefore merges each pair of half degree bins into the
// whole degree bucket that contains them: 41.5°–42.0° and 42.0°–42.5° both become 42°, and 0°
// spans 359.5°–0.5° by pairing the last bin of the circle with the first.
// A bin mid always lands on .25 or .75 of a degree, so rounding it picks out the owning degree
// without any half way ambiguity; 359.75° rounds to 360 and wraps back to 0.
function bucketDegree(range) {
    return Math.round((range.min + range.max) / 2 * 180 / Math.PI) % 360;
}

function formatBucketDegree(degree) {
    return `${degree}°`;
}

function formatBucketSpan(degree) {
    return `${((degree + 359.5) % 360).toFixed(1)}° – ${((degree + 0.5) % 360).toFixed(1)}°`;
}

// Groups half degree ranges into whole degree buckets, keeping the position of each source range so
// the sparse cells can be folded onto the merged grid.
function toBuckets(ranges) {
    const bucketsByDegree = new Map();
    ranges.forEach((range, index) => {
        const degree = bucketDegree(range);
        let bucket = bucketsByDegree.get(degree);
        if (!bucket) {
            bucket = { degree, indices: [] };
            bucketsByDegree.set(degree, bucket);
        }
        bucket.indices.push(index);
    });

    // Sorted by degree rather than by source order: the 359.5° bin arrives last but belongs to 0°.
    const buckets = [...bucketsByDegree.values()].sort((bucket_1, bucket_2) => bucket_1.degree - bucket_2.degree);

    const positionByIndex = new Map();
    buckets.forEach((bucket, position) => {
        for (const index of bucket.indices) {
            positionByIndex.set(index, position);
        }
    });

    return { buckets, positionByIndex };
}

// The delay selector of the modal offers every delay of the payload, so its options are indices into
// payload.results — the delay value itself never has to be matched back on change. Combined closes the
// list, after the delays it aggregates. Built into a fragment and swapped in as a whole, which drops
// the options of the previous open in one go.
function populateMatrixDelaySelect(selectedValue) {
    const fragment = document.createDocumentFragment();
    delayPayload.results.forEach((delayResult, index) => {
        const option = document.createElement('option');
        option.value = String(index);
        option.textContent = formatDelayValue(delayResult.delay);
        fragment.appendChild(option);
    });

    if (hasCombinedScatteringHits(delayPayload)) {
        const option = document.createElement('option');
        option.value = COMBINED_MATRIX_OPTION;
        option.textContent = 'Combined';
        fragment.appendChild(option);
    }

    matrixDelaySelect.replaceChildren(fragment);
    matrixDelaySelect.value = selectedValue;
}

function openScatteringMatrix() {
    if (delayPayload === null) {
        return;
    }

    // Opens on the delay the Results panel is showing, falling back to Combined for a delay that
    // carries no hits of its own — which is the only reason the Details button is visible there.
    const index = parseInt(delaySlider.value, 10);
    const selectedValue = hasScatteringHits(delayPayload.results[index]) ? String(index) : COMBINED_MATRIX_OPTION;
    if (selectedValue === COMBINED_MATRIX_OPTION && !hasCombinedScatteringHits(delayPayload)) {
        return;
    }

    populateMatrixDelaySelect(selectedValue);
    showScatteringMatrixSelection(selectedValue);
    matrixModal.style.display = 'flex';
}

// Loads one selection into the open modal: the title and the matrix below the delay selector. The
// Results panel, the delay slider and the 3D frame are untouched — the selector drives this modal only.
function showScatteringMatrixSelection(selectedValue) {
    if (selectedValue === COMBINED_MATRIX_OPTION) {
        // Shaped as a delay result, which is all renderScatteringMatrix reads of one: the combined
        // distributions are the same per profile matrices, aggregated over the delays instead of
        // belonging to one.
        matrixTitle.textContent = 'Scattering hits – all delays combined';
        renderScatteringMatrix({ angularDistributions: delayPayload.combined });
        return;
    }

    const delayResult = delayPayload.results[parseInt(selectedValue, 10)];
    matrixTitle.textContent = `Scattering hits – delay ${formatDelay(delayResult.delay)}`;
    renderScatteringMatrix(delayResult);
}

function renderScatteringMatrix(delayResult) {
    matrixPanel.innerHTML = '';

    // The hits belong to the delay that was on screen, so a reload closes the drill-down rather than
    // leaving the hits of the previous delay open on top of the new matrix.
    hitsModal.style.display = 'none';
    hitsPanel.innerHTML = '';

    // One distribution per angular power distribution profile (one per receiving antenna).
    const angularDistributions = (delayResult.angularDistributions ?? []).filter(angularDistribution => (angularDistribution.cells ?? []).length > 0);

    // The delay selector lists every delay, including the ones the Details button stays hidden for, so
    // an empty matrix is a normal selection rather than the unreachable state it used to be.
    if (angularDistributions.length === 0) {
        const note = document.createElement('div');
        note.className = 'gltf-muted';
        note.textContent = 'No scattering hits for this delay.';
        matrixPanel.appendChild(note);
        return;
    }

    for (const angularDistribution of angularDistributions) {
        if (angularDistributions.length > 1) {
            const caption = document.createElement('div');
            caption.className = 'communication-matrix-caption';
            caption.textContent = `Location ${angularDistribution.location.x.toFixed(2)}, ${angularDistribution.location.y.toFixed(2)}, ${angularDistribution.location.z.toFixed(2)}`;
            matrixPanel.appendChild(caption);
        }

        // The half degree bins of the payload are merged into whole degree buckets, so both axes are
        // bucketed first and every sparse cell is folded onto the bucket pair that contains it. Two
        // azimuth bins by two elevation bins can land on the same bucket cell, so the merged cell holds
        // the concatenated hits of all of them.
        const azimuth = toBuckets(angularDistribution.azimuthRanges ?? []);
        const elevation = toBuckets(angularDistribution.elevationRanges ?? []);

        const hitsByBucket = new Map();
        for (const cell of angularDistribution.cells) {
            const bucketKey = `${azimuth.positionByIndex.get(cell.azimuthIndex)}:${elevation.positionByIndex.get(cell.elevationIndex)}`;
            let hits = hitsByBucket.get(bucketKey);
            if (!hits) {
                hits = [];
                hitsByBucket.set(bucketKey, hits);
            }

            hits.push(...(cell.hits ?? []));
        }

        const table = document.createElement('table');
        table.className = 'gis-data-table communication-matrix-table';

        const headerRow = table.createTHead().insertRow();
        const cornerCell = document.createElement('th');
        cornerCell.textContent = 'Elevation \\ Azimuth';
        headerRow.appendChild(cornerCell);
        for (const azimuthBucket of azimuth.buckets) {
            const headerCell = document.createElement('th');
            headerCell.textContent = formatBucketDegree(azimuthBucket.degree);
            headerCell.title = formatBucketSpan(azimuthBucket.degree);
            headerRow.appendChild(headerCell);
        }

        const body = table.createTBody();
        for (let elevationPosition = 0; elevationPosition < elevation.buckets.length; elevationPosition++) {
            const elevationBucket = elevation.buckets[elevationPosition];

            const row = body.insertRow();
            const labelCell = document.createElement('th');
            labelCell.textContent = formatBucketDegree(elevationBucket.degree);
            labelCell.title = formatBucketSpan(elevationBucket.degree);
            row.appendChild(labelCell);

            for (let azimuthPosition = 0; azimuthPosition < azimuth.buckets.length; azimuthPosition++) {
                const cell = row.insertCell();
                const hits = hitsByBucket.get(`${azimuthPosition}:${elevationPosition}`);
                if (!hits || hits.length === 0) {
                    continue;
                }

                // The cell carries the mean reflection coefficient of the bin, which is what the next
                // step lists one hit at a time; the hit count it used to carry is the hover text.
                const azimuthBucket = azimuth.buckets[azimuthPosition];
                const button = document.createElement('button');
                button.type = 'button';
                button.className = 'gis-button communication-matrix-cell';
                button.textContent = formatAverageVerticalPolarizationReflection(hits) ?? '—';
                button.title = `${hits.length} ${hits.length === 1 ? 'hit' : 'hits'}`;
                button.addEventListener('click', () => openScatteringHits(hits, azimuthBucket.degree, elevationBucket.degree));
                cell.appendChild(button);
            }
        }

        const scrollBox = document.createElement('div');
        scrollBox.className = 'gis-scroll-box';
        scrollBox.appendChild(table);
        matrixPanel.appendChild(scrollBox);
    }
}

function formatBinTitle(azimuthDegree, elevationDegree) {
    return `azimuth ${formatBucketDegree(azimuthDegree)} (${formatBucketSpan(azimuthDegree)}), elevation ${formatBucketDegree(elevationDegree)} (${formatBucketSpan(elevationDegree)})`;
}

function openScatteringHits(hits, azimuthDegree, elevationDegree) {
    hitsTitle.textContent = `Scattering hits – ${formatBinTitle(azimuthDegree, elevationDegree)}`;
    renderScatteringHits(hits);
    hitsModal.style.display = 'flex';
}

function renderScatteringHits(hits) {
    hitsPanel.innerHTML = '';

    const table = document.createElement('table');
    table.className = 'gis-data-table communication-hits-table';

    const headerRow = table.createTHead().insertRow();
    for (const columnName of ['Location', 'Reference', 'Name', 'A', 'B', 'C', 'D', 'Conductivity', 'Relative permittivity', 'Reflection angle', 'Grazing angle', 'Receiver vector', 'Transmitter vector', 'Normal', 'Vertical polarization reflection']) {
        const headerCell = document.createElement('th');
        headerCell.textContent = columnName;
        headerRow.appendChild(headerCell);
    }

    // Every value of a row is read off the hit itself, electrical properties included: a hit carries
    // its own material, so the table no longer depends on the bin it was opened from.
    const body = table.createTBody();
    for (const hit of hits) {
        const electricalProperties = hit.electricalProperties;

        const row = body.insertRow();
        appendHitCell(row, formatCoordinate(hit.location, 2), 'nowrap');

        // A scattering object reference runs to a few hundred characters, so the cell shows the last
        // step of the chain (payload displayReference, falling back to the full string when the
        // reference could not be parsed) and carries the full one as its hover text. Set as title
        // rather than as markup, so no escaping is involved.
        const referenceCell = row.insertCell();
        referenceCell.className = 'communication-hit-reference';
        referenceCell.textContent = hit.displayReference ?? hit.reference ?? '—';
        referenceCell.title = hit.reference ?? '';

        appendHitCell(row, electricalProperties?.name ?? '—', 'nowrap');
        appendHitCell(row, electricalProperties ? String(electricalProperties.a) : '—', 'num nowrap');
        appendHitCell(row, electricalProperties ? String(electricalProperties.b) : '—', 'num nowrap');
        appendHitCell(row, electricalProperties ? String(electricalProperties.c) : '—', 'num nowrap');
        appendHitCell(row, electricalProperties ? String(electricalProperties.d) : '—', 'num nowrap');

        appendHitCell(row, formatValue(hit.conductivity, 2), 'num nowrap');
        appendHitCell(row, formatValue(hit.relativePermittivity, 2), 'num nowrap');

        // The angles travel in radians like every other angle of the payload.
        appendHitCell(row, formatAngle(hit.reflectionAngle), 'num nowrap');
        appendHitCell(row, formatAngle(hit.grazingAngle), 'num nowrap');

        appendHitCell(row, formatCoordinate(hit.vectorReceiver, 4), 'nowrap');
        appendHitCell(row, formatCoordinate(hit.vectorTransmitter, 4), 'nowrap');
        appendHitCell(row, formatCoordinate(hit.normal, 4), 'nowrap');

        // Complex, so the payload carries it already rendered rather than as a number to format here.
        appendHitCell(row, hit.verticalPolarizationReflection ?? '—', 'num nowrap');
    }

    const scrollBox = document.createElement('div');
    scrollBox.className = 'gis-scroll-box';
    scrollBox.appendChild(table);
    hitsPanel.appendChild(scrollBox);
}

function appendHitCell(row, value, className) {
    const cell = row.insertCell();
    cell.textContent = value;
    cell.className = className;
}

function closeScatteringModals() {
    hitsModal.style.display = 'none';
    matrixModal.style.display = 'none';
    hitsPanel.innerHTML = '';
    matrixPanel.innerHTML = '';

    // The options are rebuilt from the payload on every open, so they are released with the matrix DOM.
    matrixDelaySelect.replaceChildren();
}

function clearResults() {
    // Closed first: the drill-down is opened from delayPayload and its popups hold slices of it, which
    // is dropped below.
    closeScatteringModals();
    resultsDetailsButton.style.display = 'none';

    deselectPolylineObject();
    removeAuxiliaryPolylines();
    for (const object of delayObjects) {
        viewer.scene.remove(object);
        object.geometry?.dispose();
        object.material?.dispose();
    }
    delayObjects.length = 0;
    delayPayload = null;
    delayControls.style.display = 'none';

    for (const object of resultObjects) {
        viewer.scene.remove(object);
        object.geometry?.dispose();
        object.material?.dispose();
    }
    resultObjects.length = 0;
    resultMeshes.length = 0;
    selectedResultMesh = null;
    resultPickCandidate = null;

    resultsPanel.innerHTML = '';
    resultsCard.style.display = 'none';
}

async function calculate(calculationParameters) {
    if (calculating || antennas.length !== MAX_ANTENNAS) {
        return;
    }

    calculating = true;
    updateToolbar();
    reportStatus('Calculating propagation…');
    if (calculationLoader) {
        calculationLoader.style.display = 'flex';
    }

    const calculationStart = performance.now();
    const calculationTimer = setInterval(() => updateLastStatus(`Calculating propagation… (${formatElapsed(calculationStart)})`), 200);

    try {
        const body = {
            centerX: parseFloat(container.dataset.centerX),
            centerY: parseFloat(container.dataset.centerY),
            radius: parseFloat(container.dataset.radius),
            antennas: antennas.map((antenna) => ({
                x: antenna.data.x,
                y: antenna.data.y,
                z: antenna.data.z,
                functions: antenna.data.functions
            }))
        };

        body.defaultSimpleMultipathPowerDelayProfile = calculationParameters.defaultSimpleMultipathPowerDelayProfile;
        body.frequency = calculationParameters.frequency;

        console.log('Calculate URL:', container.dataset.calculateUrl);
        console.log('Calculate body:', body);

        const response = await fetch(container.dataset.calculateUrl, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(body)
        });

        if (!response.ok || response.status === 204) {
            let detail = '';
            try {
                detail = (await response.text()).trim();
            } catch {
                detail = '';
            }
            reportStatus(detail ? `Calculation failed (${response.status}): ${detail}` : `Calculation failed (${response.status}).`);
            return;
        }

        const payload = await response.json();
        if (!payload || !Array.isArray(payload.results) || payload.results.length === 0) {
            reportStatus('Calculation failed (empty results).');
            return;
        }

        renderDelayResults(payload);
        reportStatus(`Calculation completed in ${formatElapsed(calculationStart)}`);
    } catch (error) {
        console.error('Calculate error:', error);
        reportStatus('Calculation failed (network).');
    } finally {
        clearInterval(calculationTimer);
        calculating = false;
        updateToolbar();
        if (calculationLoader) {
            calculationLoader.style.display = 'none';
        }
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
    if (calculationFrequencyInput) {
        calculationFrequencyInput.value = String(DEFAULT_FREQUENCY_MHZ);
    }

    if (calculationProfileSelect) {
        calculationProfileSelect.focus();
    }
});

calculationOkButton.addEventListener('click', () => {
    calculationModal.style.display = 'none';

    // The modal collects MHz; everything downstream (request body, solver options) is in Hz.
    const frequencyMHz = parseFloat(calculationFrequencyInput?.value);

    calculate({
        defaultSimpleMultipathPowerDelayProfile: calculationProfileSelect?.value ?? 'TypicalUrban',
        frequency: (isFinite(frequencyMHz) && frequencyMHz > 0 ? frequencyMHz : DEFAULT_FREQUENCY_MHZ) * 1e6
    });
});

calculationCancelButton.addEventListener('click', () => {
    calculationModal.style.display = 'none';
});

// Delay controls wiring (Results panel): the delay slider walks payload.results (ascending
// delays); the vector scale re-renders the current frame so the vectors stretch immediately.
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

// Scattering hit drill-down wiring: Details -> matrix of the selected delay -> hits of one bin.
resultsDetailsButton.addEventListener('click', openScatteringMatrix);

// The delay selector at the top of the matrix modal reloads the matrix in place.
matrixDelaySelect.addEventListener('change', () => {
    showScatteringMatrixSelection(matrixDelaySelect.value);
});

matrixCloseButton.addEventListener('click', () => {
    matrixModal.style.display = 'none';
});

hitsCloseButton.addEventListener('click', () => {
    hitsModal.style.display = 'none';
});

function clearAntennas() {
    deselectAntennaObject();
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
        reportStatus('Antennas and results cleared.');
    }
});

if (container) {
    container.addEventListener('gltf-ready', (event) => {
        viewer = window.gltfViewer;
        referencePoint = event.detail.referencePoint ?? { X: 0, Y: 0, Z: 0 };
        sceneRadius = viewer?.radius ?? 10;
        updateToolbar();
    });

    // Any building selection change (click, marquee, context menu clear) replaces the antenna or
    // polyline entry of the shared Selection card. Registered before building-details-panel.js
    // listens, so those entries are dropped before the card is rebuilt for the buildings.
    container.addEventListener('gltf-selectionchanged', () => {
        deselectAntennaObject();
        deselectPolylineObject();
    });
}