// DiGi.GIS.WebAPI.UI — the text of a refused request, shared by the viewer host (gltf-viewer.js) and the
// analysis panels (solar-tools.js, solar-job.js). The controllers answer a refusal as a JSON list of
// messages, a problem document or plain text.

export async function refusalMessage(response) {
    let text = '';
    try {
        text = (await response.text()).trim();
    } catch {
        text = '';
    }
    try {
        const json = JSON.parse(text);
        if (Array.isArray(json) && json.length > 0) {
            return json.join(' ');
        }
        if (typeof json === 'string' && json) {
            return json;
        }
        if (json && (json.detail || json.title)) {
            return json.detail || json.title;
        }
    } catch {
        // Not JSON: plain text below.
    }
    return text ? text.slice(0, 500) : `The request failed (HTTP ${response.status}).`;
}
