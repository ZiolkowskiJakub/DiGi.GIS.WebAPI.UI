using DiGi.GIS.WebAPI.UI.Classes;
using System;

namespace DiGi.GIS.WebAPI.UI
{
    public static partial class Query
    {
        /// <summary>
        /// Gets a background solar radiation job by its identifier, after removing the expired jobs.
        /// <para>The job's state keeps changing while its consumer runs it; read a consistent view of it through <see cref="Create.SolarJobViewModel(SolarJobQueue?, Guid)"/>. Its building, neighbours and results do not change once it has completed.</para>
        /// </summary>
        /// <param name="solarJobQueue">The queue of jobs.</param>
        /// <param name="id">The unique identifier of the job.</param>
        /// <returns>The job, or null when it is unknown or has expired.</returns>
        public static SolarJob? SolarJob(this SolarJobQueue? solarJobQueue, Guid id)
        {
            if (solarJobQueue is null)
            {
                return null;
            }

            solarJobQueue.RemoveExpired();

            lock (solarJobQueue.Lock)
            {
                return solarJobQueue.SolarJobs.TryGetValue(id, out SolarJob? solarJob) ? solarJob : null;
            }
        }
    }
}
