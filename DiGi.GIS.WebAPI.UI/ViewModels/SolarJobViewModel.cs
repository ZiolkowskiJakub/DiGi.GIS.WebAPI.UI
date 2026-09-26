namespace DiGi.GIS.WebAPI.UI.ViewModels
{
    /// <summary>
    /// Represents the state of a background solar radiation job as the <c>solar/jobs</c> routes answer it and the solar radiation viewer polls it.
    /// </summary>
    public class SolarJobViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SolarJobViewModel"/> class.
        /// </summary>
        /// <param name="jobId">The unique identifier of the job.</param>
        /// <param name="status">The state of the job, the name of an <see cref="Enums.SolarJobStatus"/> member.</param>
        /// <param name="queuePosition">The place of the job in the queue (0 while it runs), or null once it has finished.</param>
        /// <param name="elapsedSeconds">The seconds spent in the current state: waiting while queued, calculating while running, and the calculation time once finished.</param>
        /// <param name="receiverCount">The number of receiving surfaces (external walls and roofs) the job calculates.</param>
        /// <param name="error">The error text of a failed job.</param>
        public SolarJobViewModel(string jobId, string status, int? queuePosition, double elapsedSeconds, int receiverCount, string? error)
        {
            JobId = jobId;
            Status = status;
            QueuePosition = queuePosition;
            ElapsedSeconds = elapsedSeconds;
            ReceiverCount = receiverCount;
            Error = error;
        }

        /// <summary> Gets the seconds spent in the current state: waiting while queued, calculating while running, and the calculation time once finished. </summary>
        public double ElapsedSeconds { get; }

        /// <summary> Gets the error text of a failed job. </summary>
        public string? Error { get; }

        /// <summary> Gets the unique identifier of the job. </summary>
        public string JobId { get; }

        /// <summary> Gets the place of the job in the queue (0 while it runs), or null once it has finished. </summary>
        public int? QueuePosition { get; }

        /// <summary> Gets the number of receiving surfaces (external walls and roofs) the job calculates. </summary>
        public int ReceiverCount { get; }

        /// <summary> Gets the state of the job, the name of an <see cref="Enums.SolarJobStatus"/> member; a name rather than the number the JSON serializer would write for the enum. </summary>
        public string Status { get; }
    }
}
