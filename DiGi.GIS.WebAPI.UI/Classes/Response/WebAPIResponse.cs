namespace DiGi.GIS.WebAPI.UI.Classes
{
    /// <summary>
    /// The outcome of a single request relayed to a Web API: the status it answered with, and the body it carried.
    /// <para>The other <c>Query</c> helpers of this application collapse every failure into <see langword="null"/>, so that a page assembled from several independent requests survives one of them coming back empty. Authentication is the case that rule does not cover: there the status <em>is</em> the answer. A refused credential (401) has to stay distinct from a faulting authentication service (500) and from a service that could not be reached at all, because collapsing them reports an outage to the visitor as a wrong password and hides it from everyone else.</para>
    /// </summary>
    public class WebAPIResponse
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WebAPIResponse"/> class.
        /// </summary>
        /// <param name="statusCode">The HTTP status code the Web API answered with.</param>
        /// <param name="json">The response body, or null when the response carried none.</param>
        public WebAPIResponse(int statusCode, string? json = null)
        {
            StatusCode = statusCode;
            Json = json;
        }

        /// <summary> Gets the response body, or null when the response carried none. </summary>
        public string? Json { get; }

        /// <summary> Gets the HTTP status code the Web API answered with. </summary>
        public int StatusCode { get; }
    }
}
