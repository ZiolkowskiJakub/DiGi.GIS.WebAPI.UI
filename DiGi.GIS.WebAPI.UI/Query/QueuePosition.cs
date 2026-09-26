using DiGi.GIS.WebAPI.UI.Classes;
using DiGi.GIS.WebAPI.UI.Enums;
using System;

namespace DiGi.GIS.WebAPI.UI
{
    public static partial class Query
    {
        /// <summary>
        /// Gets the place of a background solar radiation job in the queue: 1 for the next job to run, 2 for the one after it, and so on; 0 for the running job.
        /// <para>The job at place 1 may still be waiting for the solve gate, held by a synchronous request.</para>
        /// </summary>
        /// <param name="solarJobQueue">The queue of jobs.</param>
        /// <param name="id">The unique identifier of the job.</param>
        /// <returns>The place of the job, or null when it is finished, cancelled, unknown or expired.</returns>
        public static int? QueuePosition(this SolarJobQueue? solarJobQueue, Guid id)
        {
            if (solarJobQueue is null)
            {
                return null;
            }

            lock (solarJobQueue.Lock)
            {
                if (!solarJobQueue.SolarJobs.TryGetValue(id, out SolarJob? solarJob))
                {
                    return null;
                }

                if (solarJob.Status == SolarJobStatus.Running)
                {
                    return 0;
                }

                if (solarJob.Status != SolarJobStatus.Queued)
                {
                    return null;
                }

                int result = 1;
                foreach (SolarJob solarJob_Temp in solarJobQueue.SolarJobs.Values)
                {
                    if (solarJob_Temp.Status == SolarJobStatus.Queued && solarJob_Temp.Index < solarJob.Index)
                    {
                        result++;
                    }
                }

                return result;
            }
        }
    }
}
