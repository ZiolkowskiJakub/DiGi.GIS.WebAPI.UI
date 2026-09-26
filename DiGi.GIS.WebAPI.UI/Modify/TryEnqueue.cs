using DiGi.GIS.WebAPI.UI.Classes;
using DiGi.GIS.WebAPI.UI.Enums;

namespace DiGi.GIS.WebAPI.UI
{
    public static partial class Modify
    {
        /// <summary>
        /// Queues a background solar radiation job for its consumer, stamping its creation time and queue order, unless <see cref="Constants.Default.SolarJobQueueLengthMax"/> jobs are already waiting.
        /// <para>The queue length counts the queued jobs, not the channel: a cancelled job has left the count although its consumer has not reached it yet (DiGi.GIS.WebAPI.UI#60).</para>
        /// </summary>
        /// <param name="solarJobQueue">The queue of jobs.</param>
        /// <param name="solarJob">The job, in the <see cref="SolarJobStatus.Queued"/> state and not queued before.</param>
        /// <returns>True when the job was queued; false when the queue is full or the arguments are invalid.</returns>
        public static bool TryEnqueue(this SolarJobQueue? solarJobQueue, SolarJob? solarJob)
        {
            if (solarJobQueue is null || solarJob is null || solarJob.Status != SolarJobStatus.Queued)
            {
                return false;
            }

            solarJobQueue.RemoveExpired();

            lock (solarJobQueue.Lock)
            {
                if (solarJobQueue.SolarJobs.ContainsKey(solarJob.Id))
                {
                    return false;
                }

                // The lock is re-entrant, so the count and the insertion below are one step.
                if (solarJobQueue.QueuedCount() >= Constants.Default.SolarJobQueueLengthMax)
                {
                    return false;
                }

                solarJobQueue.Count++;
                solarJob.Index = solarJobQueue.Count;
                solarJob.CreatedAt = solarJobQueue.TimeProvider.GetUtcNow();
                solarJobQueue.SolarJobs[solarJob.Id] = solarJob;

                // Unbounded and never completed, so the write cannot fail.
                solarJobQueue.Channel.Writer.TryWrite(solarJob);
            }

            return true;
        }
    }
}
