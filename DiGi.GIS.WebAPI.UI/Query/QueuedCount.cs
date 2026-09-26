using DiGi.GIS.WebAPI.UI.Classes;
using DiGi.GIS.WebAPI.UI.Enums;

namespace DiGi.GIS.WebAPI.UI
{
    public static partial class Query
    {
        /// <summary>
        /// Gets the number of background solar radiation jobs waiting in the queue: queued and not cancelled, not counting the one running. This is the count <see cref="Constants.Default.SolarJobQueueLengthMax"/> limits.
        /// </summary>
        /// <param name="solarJobQueue">The queue of jobs.</param>
        /// <returns>The number of waiting jobs; 0 for a null queue.</returns>
        public static int QueuedCount(this SolarJobQueue? solarJobQueue)
        {
            if (solarJobQueue is null)
            {
                return 0;
            }

            lock (solarJobQueue.Lock)
            {
                int result = 0;
                foreach (SolarJob solarJob in solarJobQueue.SolarJobs.Values)
                {
                    if (solarJob.Status == SolarJobStatus.Queued)
                    {
                        result++;
                    }
                }

                return result;
            }
        }
    }
}
