using DiGi.GIS.WebAPI.UI.Classes;
using System;
using System.Collections.Generic;

namespace DiGi.GIS.WebAPI.UI
{
    public static partial class Modify
    {
        /// <summary>
        /// Removes every background solar radiation job that finished, failed or was cancelled more than <see cref="Constants.Default.SolarJobResultRetentionMinutes"/> minutes ago, together with its results. Queued and running jobs never expire.
        /// <para>Called by every read and write of the queue, so no timer is needed.</para>
        /// </summary>
        /// <param name="solarJobQueue">The queue of jobs.</param>
        /// <returns>The number of jobs removed.</returns>
        public static int RemoveExpired(this SolarJobQueue? solarJobQueue)
        {
            if (solarJobQueue is null)
            {
                return 0;
            }

            DateTimeOffset dateTimeOffset = solarJobQueue.TimeProvider.GetUtcNow() - TimeSpan.FromMinutes(Constants.Default.SolarJobResultRetentionMinutes);

            lock (solarJobQueue.Lock)
            {
                List<Guid> ids = [];
                foreach (SolarJob solarJob in solarJobQueue.SolarJobs.Values)
                {
                    if (solarJob.FinishedAt is DateTimeOffset finishedAt && finishedAt < dateTimeOffset)
                    {
                        ids.Add(solarJob.Id);
                    }
                }

                foreach (Guid id in ids)
                {
                    solarJobQueue.SolarJobs.Remove(id);
                }

                return ids.Count;
            }
        }
    }
}
