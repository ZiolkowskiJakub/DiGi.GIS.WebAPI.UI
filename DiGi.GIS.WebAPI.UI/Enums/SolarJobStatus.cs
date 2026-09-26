namespace DiGi.GIS.WebAPI.UI.Enums
{
    /// <summary>
    /// The state of a background solar radiation job (<see cref="Classes.SolarJob"/>).
    /// </summary>
    public enum SolarJobStatus
    {
        /// <summary>
        /// Waiting in the queue, or for the solve gate the job shares with the synchronous requests.
        /// </summary>
        Queued,

        /// <summary>
        /// Being calculated.
        /// </summary>
        Running,

        /// <summary>
        /// Calculated; its results can be fetched until the job expires.
        /// </summary>
        Completed,

        /// <summary>
        /// The calculation threw or produced no result; the job carries the error text.
        /// </summary>
        Failed,

        /// <summary>
        /// Cancelled by the client: a queued job is never calculated, and the results of a running one are discarded.
        /// </summary>
        Cancelled,
    }
}
