using DiGi.GIS.WebAPI.UI.Classes;
using DiGi.GIS.WebAPI.UI.Enums;
using System;

namespace DiGi.GIS.WebAPI.UI
{
    public static partial class Modify
    {
        /// <summary>
        /// Cancels a background solar radiation job. A queued job leaves the queue at once and is never calculated. A running calculation cannot be interrupted (<c>ShadingSolver.Solve</c> takes no cancellation token), so it runs to its end and its results are discarded. A finished job is left as it is.
        /// <para>The job stays readable, in the <see cref="SolarJobStatus.Cancelled"/> state, until it expires.</para>
        /// </summary>
        /// <param name="solarJobQueue">The queue of jobs.</param>
        /// <param name="id">The unique identifier of the job.</param>
        /// <returns>True when the job exists (whatever its state); false when it is unknown or has expired.</returns>
        public static bool Cancel(this SolarJobQueue? solarJobQueue, Guid id)
        {
            if (solarJobQueue is null)
            {
                return false;
            }

            solarJobQueue.RemoveExpired();

            lock (solarJobQueue.Lock)
            {
                if (!solarJobQueue.SolarJobs.TryGetValue(id, out SolarJob? solarJob))
                {
                    return false;
                }

                if (solarJob.Status == SolarJobStatus.Queued)
                {
                    // Nothing of it will be used again: the calculation and the models go at once.
                    solarJob.Status = SolarJobStatus.Cancelled;
                    solarJob.Calculation = null;
                    solarJob.BuildingModel = null;
                    solarJob.BuildingModels_Surrounding = null;
                    solarJob.FinishedAt = solarJobQueue.TimeProvider.GetUtcNow();
                }
                else if (solarJob.Status == SolarJobStatus.Running)
                {
                    // FinishedAt is stamped when the calculation returns, so the job does not expire while it still runs.
                    solarJob.Status = SolarJobStatus.Cancelled;
                }

                return true;
            }
        }
    }
}
