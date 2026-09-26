using DiGi.GIS.WebAPI.UI.Classes;
using DiGi.GIS.WebAPI.UI.Enums;
using System;
using System.Globalization;

namespace DiGi.GIS.WebAPI.UI
{
    public static partial class Create
    {
        /// <summary>
        /// Creates the view of a background solar radiation job, read in one step under the lock of its queue so that its state, times and place agree with each other.
        /// </summary>
        /// <param name="solarJobQueue">The queue of jobs.</param>
        /// <param name="id">The unique identifier of the job.</param>
        /// <returns>The view of the job, or null when it is unknown or has expired.</returns>
        public static ViewModels.SolarJobViewModel? SolarJobViewModel(this SolarJobQueue? solarJobQueue, Guid id)
        {
            if (solarJobQueue is null)
            {
                return null;
            }

            solarJobQueue.RemoveExpired();

            DateTimeOffset now = solarJobQueue.TimeProvider.GetUtcNow();

            lock (solarJobQueue.Lock)
            {
                if (!solarJobQueue.SolarJobs.TryGetValue(id, out SolarJob? solarJob))
                {
                    return null;
                }

                TimeSpan timeSpan = solarJob.Status switch
                {
                    SolarJobStatus.Queued => now - solarJob.CreatedAt,
                    SolarJobStatus.Running => now - (solarJob.StartedAt ?? now),
                    _ => (solarJob.FinishedAt ?? now) - (solarJob.StartedAt ?? solarJob.FinishedAt ?? now),
                };

                // A job cancelled while it runs has no FinishedAt yet: it is still calculating.
                if (solarJob.Status == SolarJobStatus.Cancelled && solarJob.FinishedAt is null && solarJob.StartedAt is DateTimeOffset startedAt)
                {
                    timeSpan = now - startedAt;
                }

                return new ViewModels.SolarJobViewModel(solarJob.Id.ToString("D", CultureInfo.InvariantCulture), solarJob.Status.ToString(), solarJobQueue.QueuePosition(id), System.Math.Max(0, timeSpan.TotalSeconds), solarJob.ReceiverCount, solarJob.Error);
            }
        }
    }
}
