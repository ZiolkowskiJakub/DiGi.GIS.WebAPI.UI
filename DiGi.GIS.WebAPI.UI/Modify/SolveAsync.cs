using DiGi.GIS.WebAPI.UI.Classes;
using DiGi.GIS.WebAPI.UI.Enums;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;

namespace DiGi.GIS.WebAPI.UI
{
    public static partial class Modify
    {
        /// <summary>
        /// Runs a queued background solar radiation job behind the solve gate it shares with the synchronous requests: waits for the gate, marks the job <see cref="SolarJobStatus.Running"/>, runs its prepared calculation and marks it <see cref="SolarJobStatus.Completed"/> with the results, or <see cref="SolarJobStatus.Failed"/> with the error text when the calculation throws or gives no result.
        /// <para>A job cancelled before it starts is skipped without taking the gate; one cancelled while it runs keeps the <see cref="SolarJobStatus.Cancelled"/> state and its results are discarded. Either way the calculation and the inputs it holds are released, and only a completed job keeps its building and neighbours, for its scene, until it expires. The job stays <see cref="SolarJobStatus.Queued"/> while it waits for the gate. Expired jobs are removed after each job, so memory is returned even when nobody reads the queue.</para>
        /// </summary>
        /// <param name="solarJobQueue">The queue the job belongs to.</param>
        /// <param name="solarJob">The job.</param>
        /// <param name="semaphoreSlim">The gate shared by every solar radiation solve on this host, registered under <see cref="Constants.Default.SolarSolveGateKey"/>.</param>
        /// <param name="log">Receives one line per job that ran, or null for none.</param>
        /// <param name="cancellationToken">A cancellation token that stops the wait for the gate; the calculation itself cannot be interrupted.</param>
        /// <returns>A task that completes when the job has run or was skipped.</returns>
        public static async Task SolveAsync(this SolarJobQueue? solarJobQueue, SolarJob? solarJob, SemaphoreSlim? semaphoreSlim, Action<string>? log = null, CancellationToken cancellationToken = default)
        {
            if (solarJobQueue is null || solarJob is null || semaphoreSlim is null)
            {
                return;
            }

            lock (solarJobQueue.Lock)
            {
                if (solarJob.Status != SolarJobStatus.Queued)
                {
                    return;
                }
            }

            await semaphoreSlim.WaitAsync(cancellationToken);
            try
            {
                Func<List<SurfaceSolarRadiationResult>?>? calculation;
                DateTimeOffset startedAt = solarJobQueue.TimeProvider.GetUtcNow();

                lock (solarJobQueue.Lock)
                {
                    // Cancelled while it waited for the gate.
                    if (solarJob.Status != SolarJobStatus.Queued)
                    {
                        return;
                    }

                    solarJob.Status = SolarJobStatus.Running;
                    solarJob.StartedAt = startedAt;
                    calculation = solarJob.Calculation;
                    solarJob.Calculation = null;
                }

                List<SurfaceSolarRadiationResult>? surfaceSolarRadiationResults = null;
                string? error = null;
                try
                {
                    surfaceSolarRadiationResults = calculation?.Invoke();
                    if (surfaceSolarRadiationResults is null)
                    {
                        error = $"The solar radiation of building {solarJob.BuildingModelId} could not be calculated from its model and weather file.";
                    }
                }
                catch (Exception exception)
                {
                    error = $"{exception.GetType().Name}: {exception.Message}";
                }

                DateTimeOffset finishedAt = solarJobQueue.TimeProvider.GetUtcNow();

                SolarJobStatus solarJobStatus;
                lock (solarJobQueue.Lock)
                {
                    solarJob.FinishedAt = finishedAt;
                    if (solarJob.Status != SolarJobStatus.Cancelled)
                    {
                        if (surfaceSolarRadiationResults is null)
                        {
                            solarJob.Status = SolarJobStatus.Failed;
                            solarJob.Error = error;
                        }
                        else
                        {
                            solarJob.Status = SolarJobStatus.Completed;
                            solarJob.SurfaceSolarRadiationResults = surfaceSolarRadiationResults;
                        }
                    }

                    solarJobStatus = solarJob.Status;
                    if (solarJobStatus != SolarJobStatus.Completed)
                    {
                        solarJob.BuildingModel = null;
                        solarJob.BuildingModels_Surrounding = null;
                    }
                }

                solarJobQueue.RemoveExpired();

                log?.Invoke(string.Format(CultureInfo.InvariantCulture, "Solar radiation job {0} for building {1} (county {2}): {3}, {4} receivers, waited {5:F0} ms, calculation {6:F0} ms{7}.", solarJob.Id, solarJob.BuildingModelId, solarJob.CountyId, solarJobStatus, solarJob.ReceiverCount, (startedAt - solarJob.CreatedAt).TotalMilliseconds, (finishedAt - startedAt).TotalMilliseconds, error is null ? string.Empty : $", error: {error}"));
            }
            finally
            {
                semaphoreSlim.Release();
            }
        }
    }
}
