/**
 * Building 2D Feature logic (Paging, AJAX details loading, SVG visualization)
 */

/**
 * Resolves a root-relative path relative to the application's base URL.
 */
function getResolvedUrl(path) {
    const baseUrl = window.AppBaseUrl || '/';
    const cleanBase = baseUrl.endsWith('/') ? baseUrl.slice(0, -1) : baseUrl;
    return `${cleanBase}${path.startsWith('/') ? path : '/' + path}`;
}

/**
 * Initializes table pagination for building references.
 * Exposes revealNext and revealAll functions globally.
 *
 * @param {number} initialLimit - Initial record count shown.
 * @param {number} totalRecords - Total record count.
 */
window.initTablePager = function(initialLimit, totalRecords) {
    let displayedCount = initialLimit;

    window.revealNext = function(batchSize) {
        const hiddenRows = document.querySelectorAll('#mainBuildingsTable tbody tr.row-hidden');
        const toShow = Math.min(batchSize, hiddenRows.length);

        for (let i = 0; i < toShow; i++) {
            hiddenRows[i].classList.remove('row-hidden');
            displayedCount++;
        }
        updateUI();
    };

    window.revealAll = function() {
        document.querySelectorAll('#mainBuildingsTable tbody tr.row-hidden').forEach(r => {
            r.classList.remove('row-hidden');
        });
        displayedCount = totalRecords;
        updateUI();
    };

    function updateUI() {
        const shownVal = document.getElementById('shownVal');
        if (shownVal) shownVal.innerText = displayedCount;
        if (displayedCount >= totalRecords) {
            const btns = document.getElementById('paginationButtons');
            if(btns) btns.style.display = 'none';
        }
    }
};

/**
 * Loads building details via AJAX and executes any scripts inside the returned HTML content.
 *
 * @param {string} url - Detail endpoint URL.
 * @param {HTMLElement} clickedElement - Anchor link element that triggered the fetch.
 */
async function loadBuildingDetails(url, clickedElement) {
    const contentContainer = document.getElementById('detailsContent');
    const loader = document.getElementById('detailsLoader');

    if (!contentContainer || !loader) return;

    contentContainer.style.opacity = '0.3';
    loader.style.display = 'block';

    try {
        const response = await fetch(url);
        if (!response.ok) throw new Error('API Error');

        const html = await response.text();

        // 1. Inject HTML
        contentContainer.innerHTML = html;

        // 2. Manually execute scripts in the injected view
        const scripts = contentContainer.querySelectorAll("script");
        scripts.forEach((oldScript) => {
            const newScript = document.createElement("script");
            Array.from(oldScript.attributes).forEach(attr => newScript.setAttribute(attr.name, attr.value));
            newScript.appendChild(document.createTextNode(oldScript.innerHTML));
            oldScript.parentNode.replaceChild(newScript, oldScript);
        });

        // 3. Execute the drawing initialization function defined in the injected view script
        if (typeof window.initializeBuildingGeometry === "function") {
            await window.initializeBuildingGeometry();
        }

        // Highlight the active row in list
        if (clickedElement) {
            document.querySelectorAll('.building-link').forEach(link => {
                const row = link.closest('tr');
                if (row) row.classList.remove('active-selection');
            });
            const currentRow = clickedElement.closest('tr');
            if (currentRow) currentRow.classList.add('active-selection');
        }
    } catch (error) {
        console.error('Error:', error);
        contentContainer.innerHTML = `<div style="padding: 20px; color: #d9534f; text-align: center;"><strong>Error:</strong> Could not load building details.</div>`;
    } finally {
        contentContainer.style.opacity = '1';
        loader.style.display = 'none';
    }
}

/**
 * Initializes event delegation for the building list and auto-loads the first item.
 */
function initBuildingReferencesView() {
    const listSide = document.getElementById('listSide');
    if (listSide) {
        listSide.addEventListener('click', function(e) {
            const link = e.target.closest('.building-link');
            if (link) {
                e.preventDefault();
                const url = link.getAttribute('href');
                loadBuildingDetails(url, link);
            }
        });
    }

    // AUTO-START: Load details of the first building upon page entry
    const firstLink = document.querySelector('.building-link');
    if (firstLink) {
        // Small delay to ensure everything is rendered
        setTimeout(() => firstLink.click(), 100);
    }
}

/**
 * Fetches and draws building polygon geometry onto the SVG canvas.
 *
 * @param {string|number} countyId - The associated county ID.
 * @param {string|number} id - The ID of the building.
 */
async function drawPolygon(countyId, id) {
    if (!id || !countyId) return;

    try {
        const response = await fetch(getResolvedUrl(`/building2D/svg/polygonbyid?id=${id}&countyid=${countyId}`));
        if (!response.ok) return;

        const rawData = await response.text();
        const coords = rawData.trim().split(/\s+/).map(Number);
        if (coords.length < 4) return;

        // 1. Calculate bounding box
        let minX = Infinity, maxX = -Infinity, minY = Infinity, maxY = -Infinity;
        for (let i = 0; i < coords.length; i += 2) {
            let x = coords[i];
            let y = coords[i+1];
            if (x < minX) minX = x; if (x > maxX) maxX = x;
            if (y < minY) minY = y; if (y > maxY) maxY = y;
        }

        const width = maxX - minX;
        const height = maxY - minY;
        const canvasSize = 500;
        const margin = 40; // Safe margin from edge
        const drawingArea = canvasSize - (2 * margin);

        // 2. Scale preserving aspect ratio
        const scale = Math.min(drawingArea / width, drawingArea / height);

        // 3. Center building in SVG canvas
        const scaledWidth = width * scale;
        const scaledHeight = height * scale;
        const offsetX = (canvasSize - scaledWidth) / 2;
        const offsetY = (canvasSize - scaledHeight) / 2;

        let pointsArray = [];
        for (let i = 0; i < coords.length; i += 2) {
            let screenX = (coords[i] - minX) * scale + offsetX;
            // Invert orientation since SVG Y increases downwards
            let screenY = canvasSize - ((coords[i+1] - minY) * scale + offsetY);
            pointsArray.push(`${screenX.toFixed(2)},${screenY.toFixed(2)}`);
        }

        const pointsAttr = pointsArray.join(" ");
        const svg = document.getElementById("mainCanvas");

        if (svg) {
            // Clear existing elements in SVG canvas
            while (svg.firstChild) {
                svg.removeChild(svg.firstChild);
            }

            const polygon = document.createElementNS("http://www.w3.org/2000/svg", "polygon");
            polygon.setAttribute("points", pointsAttr);
            polygon.style.fill = "rgba(0, 120, 212, 0.2)";
            polygon.style.stroke = "#0078d4";
            polygon.style.strokeWidth = "2";
            svg.appendChild(polygon);
        }
    } catch (error) {
        console.error("Polygon drawing/normalization error:", error);
    }
}

/**
 * Fetches and displays year-built details partial view.
 */
async function loadYearBuiltData(reference, countyId) {
    if (!reference) return;

    const section = document.getElementById("year-built-section");
    const container = document.getElementById("year-builts-container");
    if (!section || !container) return;

    const url = getResolvedUrl(`/yearbuiltdata/itembyreference?reference=${encodeURIComponent(reference)}&countyid=${countyId}`);

    try {
        const response = await fetch(url);

        if (response.status === 204 || response.status === 404) {
            section.style.display = "none";
            return;
        }

        if (response.ok) {
            const htmlContent = await response.text();
            if (!htmlContent.trim()) {
                section.style.display = "none";
                return;
            }
            container.innerHTML = htmlContent;
            section.style.display = "block";
        } else {
            section.style.display = "none";
        }
    } catch (error) {
        console.error("AJAX Error during fetching Year Builts:", error);
        section.style.display = "none";
    }
}

/**
 * Fetches and displays orto coverage factors details partial view.
 */
async function loadOrtoDatas(reference, countyId) {
    if (!reference) return;

    const section = document.getElementById("orto-data-section");
    const container = document.getElementById("orto-datas-container");
    if (!section || !container) return;

    const url = getResolvedUrl(`/ortodatas/itembyreference?reference=${encodeURIComponent(reference)}&countyid=${countyId}`);

    try {
        const response = await fetch(url);

        if (response.status === 204) {
            section.style.display = "none";
            return;
        }

        if (response.ok) {
            const htmlContent = await response.text();
            if (htmlContent.trim().length > 0) {
                container.innerHTML = htmlContent;
                section.style.display = "block";
            } else {
                section.style.display = "none";
            }
        }
    } catch (error) {
        console.error("Error loading Orto Data:", error);
        section.style.display = "none";
    }
}

/**
 * Fetches and displays building occupancy details partial view.
 */
async function loadOccupancyData(reference, countyId) {
    if (!reference) return;

    const section = document.getElementById("occupancy-data-section");
    const container = document.getElementById("occupancy-data-container");
    if (!section || !container) return;

    const url = getResolvedUrl(`/occupancydata/building2d/itembyreference?reference=${encodeURIComponent(reference)}&countyid=${countyId}`);

    try {
        const response = await fetch(url);

        if (response.status === 204) {
            section.style.display = "none";
            return;
        }

        if (response.ok) {
            const htmlContent = await response.text();
            if (htmlContent.trim().length > 0) {
                container.innerHTML = htmlContent;
                section.style.display = "block";
            } else {
                section.style.display = "none";
            }
        }
    } catch (error) {
        console.error("Error loading Occupancy Data:", error);
        section.style.display = "none";
    }
}

/**
 * Fetches and displays regulated heat transfer coefficients details partial view.
 */
async function loadRegulatedHeatTransferCoefficients(reference, countyId) {
    if (!reference) return;

    const section = document.getElementById("regulated-heat-transfer-coefficients-section");
    const container = document.getElementById("regulated-heat-transfer-coefficients-container");
    if (!section || !container) return;

    const url = getResolvedUrl(`/heattransfercoefficient/regulatedheattransfercoefficientsbyreference?reference=${encodeURIComponent(reference)}&countyid=${countyId}`);

    try {
        const response = await fetch(url);

        if (response.status === 204) {
            section.style.display = "none";
            return;
        }

        if (response.ok) {
            const htmlContent = await response.text();
            if (htmlContent.trim().length > 0) {
                container.innerHTML = htmlContent;
                section.style.display = "block";
            } else {
                section.style.display = "none";
            }
        }
    } catch (error) {
        console.error("Error loading Regulated Heat Transfer Coefficients Data:", error);
        section.style.display = "none";
    }
}