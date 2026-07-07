// DiGi.GIS.WebAPI.UI — communication analysis tooling layered on top of the generic
// DiGi.GLTF.WebAPI viewer engine (gltf-viewer.js owns the base panels; this module owns the
// antenna toolbar, the antenna edit/erase modes, the calculation request and the results panel).
//
// AI-NOTE (temporary calculation flow): the "Calculate" button currently posts the scene input
// parameters and the two antennas to ~/communication/calculate. The server rebuilds the analyzed
// area on the fly (Building -> Mesh3D -> ScatteringObject), packages everything into a
// GeometricalPropagationModel and calls the temporary DiGi.Communication.WebAPI segment3d
// endpoint, which returns a Segment3D connecting the two antenna tops. When the proper propagation
// calculations are implemented, the response will carry calculation objects (scattering profiles,
// rays, power delay profiles) and this module will render them in the 3D view instead of the
// single line of sight.

import * as THREE from 'three';

const MAX_ANTENNAS = 2;
const ANTENNA_COLOR = 0xd13438;          // red mast + dot
const ANTENNA_SELECTED_COLOR = 0xffa500; // orange tint in erase mode
const RESULT_COLOR = 0xffd700;           // gold line for the temporary calculation result
const CLICK_THRESHOLD = 5;               // pixels of pointer travel before a click becomes a drag

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

const resultsCard = document.getElementById('communication-results-card');
const resultsPanel = document.getElementById('communication-results');

let viewer = null;                 // GltfViewer instance exposed by gltf-viewer.js
let referencePoint = { X: 0, Y: 0, Z: 0 };
let sceneRadius = 10;
let mode = null;                   // null | 'add' | 'erase'
let calculating = false;
let pointerDownPosition = null;

const antennas = [];               // { group, meshes, selected, data: { x, y, z, functions } }
const resultObjects = [];          // three.js objects added by "Calculate"

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

function setHint(text) {
    if (modeHint) {
        modeHint.textContent = text ?? '';
    }
}

function setMode(value) {
    mode = value;
    container.style.cursor = value === 'add' ? 'crosshair' : value === 'erase' ? 'pointer' : '';

    if (value === 'add') {
        setHint('Click a point on the ground plane (Z = 0) to place the antenna. Press Esc to cancel.');
    } else if (value === 'erase') {
        setHint('Click antennas to select them. Press Enter to remove the selected antennas, Esc to cancel.');
    } else {
        setHint('');
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
    if (mode === null || event.button !== 0) {
        return;
    }
    event.stopPropagation();
    pointerDownPosition = { x: event.clientX, y: event.clientY };
}

function onPointerUp(event) {
    if (mode === null || event.button !== 0) {
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
    if (mode !== null && event.button === 0) {
        event.stopPropagation();
    }
}

container.addEventListener('pointerdown', onPointerDown, true);
container.addEventListener('pointerup', onPointerUp, true);
container.addEventListener('click', onClickCapture, true);

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
    modalZ.value = '0';
    for (const checkbox of modal.querySelectorAll('.communication-antenna-function')) {
        checkbox.checked = true; // all Function values are selected by default
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
// Calculate: send the analyzed area and the antennas to the server, render the returned line.
// ---------------------------------------------------------------------------------------------

function showResults(distance) {
    // AI-NOTE (placeholder results): only the distance between the two antenna tops is displayed.
    // This panel will be redesigned to present the proper propagation calculation results.
    resultsPanel.innerHTML = '';

    const table = document.createElement('table');
    const row = table.insertRow();
    row.insertCell().textContent = 'Distance';
    row.insertCell().textContent = `${distance.toFixed(2)} m`;
    resultsPanel.appendChild(table);

    const note = document.createElement('div');
    note.className = 'gltf-muted';
    note.style.marginTop = '6px';
    note.textContent = 'Temporary result: the distance between the two antenna tops. Proper propagation results will be displayed here once implemented.';
    resultsPanel.appendChild(note);

    resultsCard.style.display = '';
}

function clearResults() {
    for (const object of resultObjects) {
        viewer.scene.remove(object);
        object.geometry?.dispose();
        object.material?.dispose();
    }
    resultObjects.length = 0;

    resultsPanel.innerHTML = '';
    resultsCard.style.display = 'none';
}

async function calculate() {
    if (calculating || antennas.length !== MAX_ANTENNAS) {
        return;
    }

    calculating = true;
    updateToolbar();
    setHint('Calculating…');

    try {
        const response = await fetch(container.dataset.calculateUrl, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({
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
            })
        });

        if (!response.ok || response.status === 204) {
            setHint('Calculation failed.');
            return;
        }

        const result = await response.json();
        if (!result || !result.start || !result.end) {
            setHint('Calculation failed.');
            return;
        }

        clearResults();

        // Temporary rendering: one line connecting the two antenna tops. The final implementation
        // will render the calculation objects returned by the propagation calculation.
        const geometry = new THREE.BufferGeometry().setFromPoints([
            toScene(result.start.x, result.start.y, result.start.z),
            toScene(result.end.x, result.end.y, result.end.z)
        ]);
        const line = new THREE.Line(geometry, new THREE.LineBasicMaterial({ color: RESULT_COLOR }));
        viewer.scene.add(line);
        resultObjects.push(line);

        showResults(result.distance);
        setHint('');
    } catch {
        setHint('Calculation failed.');
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

calculateButton.addEventListener('click', calculate);

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
