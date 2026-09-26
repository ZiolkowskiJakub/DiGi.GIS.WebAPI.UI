using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Channels;

namespace DiGi.GIS.WebAPI.UI.Classes
{
    /// <summary>
    /// The background solar radiation jobs of this host: every job by its identifier until it expires, and the channel through which queued jobs reach their single consumer (<c>SolarJobHostedService</c>). Registered as a singleton.
    /// <para>The channel is unbounded and only carries jobs to the consumer: the queue length (<see cref="Constants.Default.SolarJobQueueLengthMax"/>) is counted over the queued jobs under <see cref="Lock"/>, so that a cancelled job frees its place at once instead of holding a channel slot until the consumer reaches it (DiGi.GIS.WebAPI.UI#60).</para>
    /// </summary>
    public class SolarJobQueue
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SolarJobQueue"/> class.
        /// </summary>
        /// <param name="timeProvider">The clock that stamps the jobs and expires them.</param>
        public SolarJobQueue(TimeProvider timeProvider)
        {
            TimeProvider = timeProvider;
            Channel = System.Threading.Channels.Channel.CreateUnbounded<SolarJob>(new UnboundedChannelOptions() { SingleReader = true });
        }

        /// <summary> Gets the channel through which queued jobs reach their consumer. </summary>
        internal Channel<SolarJob> Channel { get; }

        /// <summary> Gets the number of jobs queued so far, the source of <see cref="SolarJob.Index"/>. </summary>
        internal long Count { get; set; }

        /// <summary> Gets the lock guarding <see cref="SolarJobs"/>, <see cref="Count"/> and the state of every job. </summary>
        internal Lock Lock { get; } = new();

        /// <summary> Gets every job that has not expired, by its identifier. </summary>
        internal Dictionary<Guid, SolarJob> SolarJobs { get; } = [];

        /// <summary> Gets the clock that stamps the jobs and expires them. </summary>
        public TimeProvider TimeProvider { get; }
    }
}
