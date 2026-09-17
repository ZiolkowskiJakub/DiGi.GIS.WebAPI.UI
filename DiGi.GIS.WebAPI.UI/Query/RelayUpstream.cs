using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DiGi.GIS.WebAPI.UI
{
    public static partial class Query
    {
        /// <summary>
        /// Maps the relayed outcome of a status-preserving relay onto the page's contract: an empty result (the upstream 404) stays the 204 the page reads as "no data"; a 200 with a body passes through byte-identical; an answered failure is a 502; a service that answered nothing at all is a 503.
        /// <para>One implementation for every verbatim relay in this application — the Typology value loads, the county-parts relay and the area search — so the controllers call this rather than each spelling the same mapping (Coding - WebAPI Contracts, section 3).</para>
        /// <para>A refusal is never a 400: the caller asked nothing wrong, and blaming it would turn a service fault into a visitor error (issue #40).</para>
        /// </summary>
        /// <param name="webAPIResponse">The relayed response. This value can be null.</param>
        /// <returns>The result to answer with.</returns>
        public static IActionResult RelayUpstream(Classes.WebAPIResponse? webAPIResponse)
        {
            if (webAPIResponse is null)
            {
                return new StatusCodeResult(StatusCodes.Status503ServiceUnavailable);
            }

            if (webAPIResponse.StatusCode == StatusCodes.Status404NotFound)
            {
                return new NoContentResult();
            }

            if (webAPIResponse.StatusCode == StatusCodes.Status200OK && !string.IsNullOrWhiteSpace(webAPIResponse.Json))
            {
                return new ContentResult() { Content = webAPIResponse.Json, ContentType = "application/json" };
            }

            return new StatusCodeResult(StatusCodes.Status502BadGateway);
        }
    }
}
