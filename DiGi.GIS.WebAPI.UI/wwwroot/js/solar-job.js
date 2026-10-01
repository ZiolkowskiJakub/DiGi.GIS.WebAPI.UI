// DiGi.GIS.WebAPI.UI — background solar radiation jobs (issues #60, #66).
// A building above the synchronous limits is refused with 413; this module posts it as a job, polls
// the job's state and fetches its solar radiation view (solar/jobs/{id}/view) once it has completed.
// The job id is kept in the page address (?job=), so a reload resumes the polling instead of starting
// another calculation. Loaded on demand by solar-tools.js, the Building Viewer's "Solar radiation" panel.

const sleep = (milliseconds) => new Promise((resolve) => setTimeout(resolve, milliseconds));

// "42 s", "7 min 05 s" or "1 h 03 min".
export function formatSeconds(seconds) {
    const total = Math.max(0, Math.round(Number(seconds) || 0));
    const hours = Math.floor(total / 3600);
    const minutes = Math.floor((total % 3600) / 60);
    const rest = total % 60;
    if (hours > 0) {
        return `${hours} h ${String(minutes).padStart(2, '0')} min`;
    }
    if (minutes > 0) {
        return `${minutes} min ${String(rest).padStart(2, '0')} s`;
    }
    return `${rest} s`;
}

// Keeps the job id in the page address, or removes it (null).
export function setJobParameter(jobId) {
    const url = new URL(window.location.href);
    if (jobId) {
        url.searchParams.set('job', jobId);
    } else {
        url.searchParams.delete('job');
    }
    window.history.replaceState(window.history.state, '', url);
}

// The routes answer ASP.NET's camelCase; the PascalCase fallback keeps the reader tolerant.
function readJob(job) {
    return {
        jobId: job.jobId ?? job.JobId,
        status: job.status ?? job.Status,
        queuePosition: job.queuePosition ?? job.QueuePosition,
        elapsedSeconds: job.elapsedSeconds ?? job.ElapsedSeconds ?? 0,
        receiverCount: job.receiverCount ?? job.ReceiverCount,
        error: job.error ?? job.Error,
    };
}

function statusText(job) {
    const elapsed = formatSeconds(job.elapsedSeconds);
    if (job.status === 'Queued') {
        const ahead = (job.queuePosition ?? 1) - 1;
        return ahead > 0
            ? `Background calculation queued: ${ahead} ahead of it · waiting ${elapsed}`
            : `Background calculation queued: next to run · waiting ${elapsed}`;
    }
    if (job.status === 'Running') {
        return `Calculating ${job.receiverCount} walls and roofs in the background · ${elapsed}. The result stays available for an hour, so this page can be closed and reopened.`;
    }
    return `Background calculation: ${job.status}`;
}

// Asks the server to cancel a job: a queued job is never calculated, a running one finishes and its result is discarded.
export async function cancelSolarJob(jobsUrl, jobId) {
    if (!jobId) {
        return;
    }
    try {
        await fetch(`${jobsUrl}/${encodeURIComponent(jobId)}`, { method: 'DELETE' });
    } catch {
        // Unreachable: the next poll reports the job's actual state.
    }
}

// Runs a background job to its end: posts a new one when jobId is null, otherwise resumes polling it.
// onStatus(text, jobId, completed) receives every state change; completed is true once nothing is left to cancel. Resolves with { view } for the
// solar radiation view, or with { view: null, message, retry } where retry says whether starting another job makes sense.
export async function runSolarJob({ jobsUrl, query, jobId, pollSeconds, refusalMessage, onStatus }) {
    const pollMilliseconds = Math.max(1, Number(pollSeconds) || 5) * 1000;

    if (!jobId) {
        let response;
        try {
            response = await fetch(`${jobsUrl}?${query}`, { method: 'POST' });
        } catch {
            return { view: null, message: 'The background calculation could not be started: the server did not answer.', retry: true };
        }
        if (response.status !== 202) {
            // 503: the queue is full; any other refusal (413 above the job limits, 422, ...) will not change on a retry.
            return { view: null, message: await refusalMessage(response), retry: response.status === 503 };
        }
        const job = readJob(await response.json());
        jobId = job.jobId;
        setJobParameter(jobId);
        onStatus(statusText(job), jobId);
    }

    for (;;) {
        let response;
        try {
            response = await fetch(`${jobsUrl}/${encodeURIComponent(jobId)}`, { cache: 'no-store' });
        } catch {
            await sleep(pollMilliseconds); // a transient network failure: keep polling
            continue;
        }

        if (response.status === 404) {
            setJobParameter(null);
            return { view: null, message: 'This background calculation is no longer available: its result expired or the server restarted.', retry: true };
        }
        if (!response.ok) {
            await sleep(pollMilliseconds);
            continue;
        }

        const job = readJob(await response.json());
        if (job.status === 'Completed') {
            onStatus('Background calculation completed. Loading the results…', jobId, true);
            try {
                const viewResponse = await fetch(`${jobsUrl}/${encodeURIComponent(jobId)}/view`, { cache: 'no-store' });
                if (viewResponse.ok && viewResponse.status !== 204) {
                    return { view: await viewResponse.json(), message: null, retry: false };
                }
                if (viewResponse.status === 404) {
                    setJobParameter(null);
                }
                return { view: null, message: viewResponse.ok ? 'The calculation returned no results for the walls and roofs.' : await refusalMessage(viewResponse), retry: viewResponse.status === 404 };
            } catch {
                return { view: null, message: 'The calculated results could not be loaded. Reload the page to try again.', retry: false };
            }
        }
        if (job.status === 'Failed') {
            setJobParameter(null);
            return { view: null, message: `The background calculation failed: ${job.error || 'no details were given.'}`, retry: true };
        }
        if (job.status === 'Cancelled') {
            setJobParameter(null);
            return { view: null, message: 'The background calculation was cancelled.', retry: true };
        }

        onStatus(statusText(job), jobId);
        await sleep(pollMilliseconds);
    }
}
