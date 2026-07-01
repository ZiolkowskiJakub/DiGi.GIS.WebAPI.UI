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

            if (textElement && barElement && values[index] !== undefined) {
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

                // Reset color styling in case of previous errors
                textElement.style.color = "";
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