/**
 * Shared GIS frontend helper functions
 */

/**
 * Performs a bulk request to estimate coverage factors for a list of administrative area IDs,
 * then updates the corresponding UI progress bars and text labels.
 *
 * @param {Array<number>} ids - The list of administrative area reference IDs to load.
 */
async function loadAllOrtoCoverages(ids) {
    const baseUrl = window.AppBaseUrl || '/';
    const cleanBase = baseUrl.endsWith('/') ? baseUrl.slice(0, -1) : baseUrl;
    const apiUrl = `${cleanBase}/ortodatas/estimatedcoveragefactors`;

    try {
        const response = await fetch(apiUrl, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(ids)
        });

        if (!response.ok) throw new Error('Bulk API Error');

        const values = await response.json();

        // Update the UI for each element
        ids.forEach((id, index) => {
            const textElement = document.getElementById(`text-coverage-${id}`);
            const barElement = document.getElementById(`bar-coverage-${id}`);

            if (!textElement || !barElement || values[index] === undefined) {
                return;
            }

            // Reset color styling in case of previous errors
            textElement.style.color = "";

            // null is not zero. The API answers null where it could not measure the coverage at all - a
            // county nothing has ever been downloaded for - and drawing that as an empty red bar would
            // report it as uncovered. parseFloat(null) is NaN, so this has to be caught before the maths.
            if (values[index] === null) {
                textElement.innerText = "N/A";
                barElement.style.width = "0%";
                barElement.style.backgroundColor = "";
                return;
            }

            const percentage = (parseFloat(values[index]) * 100).toFixed(1);

            textElement.innerText = `${percentage}%`;
            const safeWidth = Math.min(Math.max(percentage, 0), 100);
            barElement.style.width = `${safeWidth}%`;

            // Logic-based coloring
            if (safeWidth > 80) {
                barElement.style.backgroundColor = "#28a745";
            } else if (safeWidth < 30) {
                barElement.style.backgroundColor = "#dc3545";
            } else {
                barElement.style.backgroundColor = ""; // Reset inline color to CSS default if middle range
            }
        });
    } catch (error) {
        console.error('Error loading bulk coverage:', error);
        ids.forEach(id => {
            const el = document.getElementById(`text-coverage-${id}`);
            if (el) {
                el.innerText = "N/A";
                el.style.color = "red";
            }
        });
    }
}

/**
 * Displays a theme-consistent modal dialog (warning or error) for display area validations.
 *
 * @param {Object} options - Configuration options for the dialog.
 * @param {'warning'|'error'} [options.type='warning'] - Dialog type.
 * @param {string} options.title - Dialog title.
 * @param {string} options.message - Dialog message.
 * @param {string} [options.confirmText='OK'] - Label for the confirm button.
 * @param {string} [options.cancelText='Cancel'] - Label for the cancel button.
 * @param {boolean} [options.showCancel=false] - Whether to show the cancel button.
 * @param {Function} [options.onConfirm] - Callback executed when confirm button is clicked.
 * @param {Function} [options.onCancel] - Callback executed when cancel button or dismiss is clicked.
 */
function showDisplayAreaDialog(options) {
    const existingModal = document.getElementById('gis-display-area-dialog');
    if (existingModal) {
        existingModal.remove();
    }

    const overlay = document.createElement('div');
    overlay.id = 'gis-display-area-dialog';
    overlay.className = 'gis-modal-overlay';

    const isError = options.type === 'error';
    const titleClass = isError ? 'gis-dialog-title-error' : 'gis-dialog-title-warning';

    const modal = document.createElement('div');
    modal.className = 'gis-card gis-modal gis-dialog-card';
    modal.setAttribute('role', 'dialog');
    modal.setAttribute('aria-modal', 'true');
    modal.setAttribute('aria-labelledby', 'gis-dialog-title');

    modal.innerHTML = `
        <h3 id="gis-dialog-title" class="gltf-card-title ${titleClass}">${options.title}</h3>
        <p class="gis-dialog-message">${options.message}</p>
        <div class="gis-modal-buttons">
            ${options.showCancel ? `<button type="button" id="gis-dialog-cancel-button" class="gis-button gis-button-secondary">${options.cancelText || 'Cancel'}</button>` : ''}
            <button type="button" id="gis-dialog-confirm-button" class="gis-button">${options.confirmText || 'OK'}</button>
        </div>
    `;

    overlay.appendChild(modal);
    document.body.appendChild(overlay);

    const confirmBtn = modal.querySelector('#gis-dialog-confirm-button');
    const cancelBtn = modal.querySelector('#gis-dialog-cancel-button');

    function closeDialog() {
        overlay.remove();
        document.removeEventListener('keydown', handleKeyDown);
    }

    function handleKeyDown(event) {
        if (event.key === 'Escape') {
            closeDialog();
            if (options.onCancel) options.onCancel();
        }
    }

    document.addEventListener('keydown', handleKeyDown);

    if (confirmBtn) {
        confirmBtn.focus();
        confirmBtn.addEventListener('click', function () {
            closeDialog();
            if (options.onConfirm) options.onConfirm();
        });
    }

    if (cancelBtn) {
        cancelBtn.addEventListener('click', function () {
            closeDialog();
            if (options.onCancel) options.onCancel();
        });
    }

    overlay.addEventListener('click', function (event) {
        if (event.target === overlay) {
            closeDialog();
            if (options.onCancel) options.onCancel();
        }
    });
}

/**
 * Sets up client-side 3D display area radius validation and warning prompts on a form.
 * - Zone 1 (r <= 1000m): Allows immediate submission without prompt.
 * - Zone 2 (1000m < r <= 1500m): Displays a warning confirmation dialog before proceeding.
 * - Zone 3 (r > 1500m): Displays an error dialog and blocks submission.
 *
 * @param {string|HTMLFormElement} formSelector - The form element or its CSS selector.
 * @param {string} [radiusInputName="radius"] - The name or selector of the radius input element.
 * @param {number} [fastMax=1000] - Fast loading threshold in meters.
 * @param {number} [max=1500] - Maximum allowable radius in meters.
 */
function setupDisplayAreaRadiusValidation(formSelector, radiusInputName = "radius", fastMax = 1000, max = 1500) {
    const form = typeof formSelector === 'string' ? document.querySelector(formSelector) : formSelector;
    if (!form) return;

    let isConfirmed = false;

    form.addEventListener('submit', function (event) {
        if (isConfirmed) {
            isConfirmed = false;
            return;
        }

        const radiusInput = form.elements[radiusInputName] || form.querySelector(`input[name="${radiusInputName}"]`);
        if (!radiusInput) return;

        const radius = parseFloat(radiusInput.value);
        if (isNaN(radius) || radius <= 0) {
            return;
        }

        // Zone 3: Invalid area (> 1500 m)
        if (radius > max) {
            event.preventDefault();
            showDisplayAreaDialog({
                type: 'error',
                title: 'Display Area Too Large',
                message: `The requested search radius (${radius.toLocaleString()} m) exceeds the maximum allowed display limit of ${max.toLocaleString()} m. Please reduce the radius to proceed.`,
                confirmText: 'OK',
                showCancel: false
            });
            return;
        }

        // Zone 2: Slow loading area (1000 m < r <= 1500 m)
        if (radius > fastMax) {
            event.preventDefault();
            showDisplayAreaDialog({
                type: 'warning',
                title: 'Large Display Area',
                message: `The requested search radius (${radius.toLocaleString()} m) is greater than ${fastMax.toLocaleString()} m. Retrieving and rendering 3D data for a large area may take longer. Do you want to proceed?`,
                confirmText: 'Proceed',
                cancelText: 'Cancel',
                showCancel: true,
                onConfirm: function () {
                    isConfirmed = true;
                    form.requestSubmit ? form.requestSubmit() : form.submit();
                }
            });
            return;
        }

        // Zone 1: Fast loading (r <= 1000 m) -> normal submit proceeds
    });
}