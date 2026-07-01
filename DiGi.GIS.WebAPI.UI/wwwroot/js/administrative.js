/**
 * Administrative Area Map & Polygon visualization
 */

let globalScaleParams = null;
const activeHovers = new Set();
const controllers = new Map();

/**
 * Handles hover entry over an administrative area row.
 * Triggers drawing the area's polygons.
 *
 * @param {number|string} id - The ID of the administrative area.
 */
async function handleMouseEnter(id) {
    activeHovers.add(id);
    const controller = new AbortController();
    controllers.set(id, controller);

    await drawPolygons(id, false, controller.signal);
}

/**
 * Handles hover exit from an administrative area row.
 * Aborts any pending fetch requests and removes the area's polygons.
 *
 * @param {number|string} id - The ID of the administrative area.
 * @param {number|string} baseId - The ID of the base area which should not be removed.
 */
function handleMouseLeave(id, baseId) {
    activeHovers.delete(id);
    if (controllers.has(id)) {
        controllers.get(id).abort();
        controllers.delete(id);
    }
    removePolygons(id, baseId);
}

/**
 * Fetches and draws polygons for the given administrative area ID onto the SVG canvas.
 *
 * @param {number|string} id - The ID of the area.
 * @param {boolean} isBase - Whether this is the base/reference area.
 * @param {AbortSignal} signal - Optional abort signal.
 */
async function drawPolygons(id, isBase = false, signal = null) {
    if (!id) return;
    // Check if group for this id already exists
    if (document.getElementById(`group-${id}`)) return;

    try {
        const baseUrl = window.AppBaseUrl || '/';
        const cleanBase = baseUrl.endsWith('/') ? baseUrl.slice(0, -1) : baseUrl;
        const response = await fetch(`${cleanBase}/administrativeareal2D/svg/polygonsbyid?id=${id}`, { signal });
        if (!response.ok) return;

        // Data is now a JSON array of arrays: [[x1, y1, x2, y2...], [...]]
        const allPolygonsData = await response.json();

        if (!isBase && !activeHovers.has(id)) return;
        if (!allPolygonsData || allPolygonsData.length === 0) return;

        // Calculate bounding box for ALL polygons to set scale
        if (isBase || !globalScaleParams) {
            let minX = Infinity, maxX = -Infinity, minY = Infinity, maxY = -Infinity;

            allPolygonsData.forEach(coords => {
                for (let i = 0; i < coords.length; i += 2) {
                    let x = coords[i]; let y = coords[i + 1];
                    if (x < minX) minX = x; if (x > maxX) maxX = x;
                    if (y < minY) minY = y; if (y > maxY) maxY = y;
                }
            });

            const scale = Math.min((450) / (maxX - minX), (450) / (maxY - minY));
            globalScaleParams = { minX, minY, scale, padding: 25, canvasSize: 500 };
        }

        const { minX, minY, scale, padding, canvasSize } = globalScaleParams;

        // Create a group to hold multiple polygons for this ID
        const group = document.createElementNS("http://www.w3.org/2000/svg", "g");
        group.setAttribute("id", `group-${id}`);

        allPolygonsData.forEach((coords, index) => {
            if (coords.length < 4) return;

            const pointsString = [];
            for (let i = 0; i < coords.length; i += 2) {
                const px = (coords[i] - minX) * scale + padding;
                const py = canvasSize - ((coords[i + 1] - minY) * scale + padding);
                pointsString.push(`${px.toFixed(2)},${py.toFixed(2)}`);
            }

            const polygon = document.createElementNS("http://www.w3.org/2000/svg", "polygon");
            polygon.setAttribute("id", `poly-${id}-${index}`);
            polygon.setAttribute("points", pointsString.join(" "));

            // Styling
            polygon.style.fill = isBase ? "rgba(0, 120, 212, 0.1)" : "rgba(0, 120, 212, 0.4)";
            polygon.style.stroke = "#0078d4";
            polygon.style.strokeWidth = isBase ? "1" : "2";
            polygon.style.pointerEvents = "none";

            group.appendChild(polygon);
        });

        const mainCanvas = document.getElementById("mainCanvas");
        if (mainCanvas) {
            mainCanvas.appendChild(group);
        }
    } catch (e) {
        if (e.name !== 'AbortError') console.error(e);
    }
}

/**
 * Removes drawn polygons for the given ID from the SVG canvas.
 *
 * @param {number|string} id - The ID of the area.
 * @param {number|string} baseId - The ID of the base area which should not be removed.
 */
function removePolygons(id, baseId) {
    // Check if the ID is the base ID that should not be removed
    if (baseId && id && id.toString() === baseId.toString()) return;

    const group = document.getElementById(`group-${id}`);
    if (group) {
        group.remove();
    }
}