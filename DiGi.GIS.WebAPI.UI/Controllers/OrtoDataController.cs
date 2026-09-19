using DiGi.GIS.PostgreSQL.Classes;
using DiGi.GIS.WebAPI.UI.Classes;
using DiGi.WebAPI.Classes;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace DiGi.GIS.WebAPI.UI.Controllers
{
    /// <summary>
    /// Provides the Orto Data verification page and the endpoints it uses, each relaying to the GIS Web API with the visitor's session presented as a bearer token.
    /// <para>Distinct from <see cref="OrtoDatasController"/>, which serves the orthophoto coverage panel of the building details page. This controller is the whole of a standalone page: it draws a building that has photos but no user year built answer yet, shows one card per photo year, and records the reviewer's answer.</para>
    /// <para>The page requires a session: the page action redirects an anonymous visitor to the sign-in page, and every other action answers 401, which the page script answers with one refresh attempt and then a sign-out (<c>wwwroot/js/user.js</c>).</para>
    /// </summary>
    [Route("[controller]")]
    public class OrtoDataController : Controller
    {
        private readonly IHttpClientFactory httpClientFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="OrtoDataController"/> class.
        /// </summary>
        /// <param name="httpClientFactory">The <see cref="IHttpClientFactory"/> used to create HTTP clients.</param>
        public OrtoDataController(IHttpClientFactory httpClientFactory)
        {
            this.httpClientFactory = httpClientFactory;
        }

        /// <summary>
        /// Returns the Orto Data verification page.
        /// <para>The page itself carries no building: its script draws one on load, either the next unverified building or the one named by the <c>countyid</c> and <c>reference</c> query values it reads itself. The optional repeated <c>countyids</c> confines the script's draws to those <c>building_2d</c> parts.</para>
        /// </summary>
        /// <returns>An <see cref="IActionResult"/> holding the verification view, or a redirect to the sign-in page for an anonymous visitor.</returns>
        [HttpGet("")]
        public IActionResult Start()
        {
            string? tokenString = Request.Cookies[Constants.Default.UserTokenCookieName];
            if (string.IsNullOrWhiteSpace(tokenString))
            {
                // The sign-in pattern of UserController.Login: only an address inside this application is ever
                // returned to, so a crafted address cannot turn this redirect into an open redirector.
                string returnUrl = HttpContext.Request.Path + HttpContext.Request.QueryString;
                string loginUrl = Url.Content("~/login") + "?returnUrl=" + Uri.EscapeDataString(Url.IsLocalUrl(returnUrl) ? returnUrl : Url.Content("~/"));

                return Redirect(loginUrl);
            }

            return View();
        }

        /// <summary>
        /// Draws the next building to verify and reads the photo years it holds.
        /// </summary>
        /// <param name="countyIds">Optional <c>building_2d</c> part ids confining the draw; omitted or empty draws from every covered part.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by the caller to cancel the asynchronous operation.</param>
        /// <returns>An <see cref="IActionResult"/> holding the drawn building with its years, the empty state when nothing unverified remains, 401 without a session, or the status the service answered with.</returns>
        [HttpGet("random")]
        public async Task<IActionResult> GetRandomAsync([FromQuery(Name = "countyids")] int[]? countyIds, CancellationToken cancellationToken = default)
        {
            string? tokenString = Request.Cookies[Constants.Default.UserTokenCookieName];
            if (string.IsNullOrWhiteSpace(tokenString))
            {
                return Unauthorized();
            }

            // TODO [OrtoDataEndpoints]: remove this gate together with Constants.Default.OrtoDataEndpointsDeployed once the upstream build carrying the endpoint is deployed.
            if (!Constants.Default.OrtoDataEndpointsDeployed)
            {
                return Ok(new OrtoDataBuildingResponse());
            }

            HttpClient httpClient = httpClientFactory.CreateClient();

            UrlBuilder urlBuilder = new(Constants.Default.OrtoDataRandomBuilding2DReferenceUri);
            if (countyIds is { Length: > 0 })
            {
                urlBuilder = urlBuilder.AddParameter("countyids", countyIds);
            }

            WebAPIResponse? webAPIResponse = await httpClient.ResponseAsync(HttpMethod.Get, urlBuilder.ToString(), tokenString, cancellationToken);
            if (webAPIResponse is null)
            {
                return StatusCode(StatusCodes.Status503ServiceUnavailable);
            }

            if (webAPIResponse.StatusCode == StatusCodes.Status404NotFound)
            {
                // Nothing unverified remains. The empty state, not an error: the page celebrates rather than reporting.
                return Ok(new OrtoDataBuildingResponse());
            }

            if (webAPIResponse.StatusCode != StatusCodes.Status200OK)
            {
                return StatusCode(webAPIResponse.StatusCode);
            }

            List<Building2DReference>? building2DReferences = Core.Convert.ToDiGi<Building2DReference>(webAPIResponse.Json);
            Building2DReference? building2DReference = building2DReferences?.FirstOrDefault();
            if (building2DReference is null)
            {
                return Ok(new OrtoDataBuildingResponse());
            }

            List<short> years = await YearsAsync(httpClient, building2DReference.Reference, building2DReference.CountyId, tokenString, cancellationToken) ?? [];

            return Ok(new OrtoDataBuildingResponse { CountyId = building2DReference.CountyId, Reference = building2DReference.Reference, Years = years });
        }

        /// <summary>
        /// Reads a named building with its photo years and the answer already recorded for it.
        /// <para>Direct mode: reached from the query values the page was opened with, and the only mode that can carry an existing answer - a freshly drawn building has none by construction.</para>
        /// </summary>
        /// <param name="countyId">The optional identifier of the county part the building is filed under; omitted, the service resolves the reference to the lowest part holding it.</param>
        /// <param name="reference">The reference of the building to read.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by the caller to cancel the asynchronous operation.</param>
        /// <returns>An <see cref="IActionResult"/> holding the building with its years and any recorded answer, 404 when no such building exists, 400 without a reference, 401 without a session, or the status the service answered with.</returns>
        [HttpGet("building")]
        public async Task<IActionResult> GetBuildingAsync([FromQuery(Name = "countyid")] int? countyId, [FromQuery(Name = "reference")] string? reference, CancellationToken cancellationToken = default)
        {
            string? tokenString = Request.Cookies[Constants.Default.UserTokenCookieName];
            if (string.IsNullOrWhiteSpace(tokenString))
            {
                return Unauthorized();
            }

            // TODO [OrtoDataEndpoints]: remove this gate together with Constants.Default.OrtoDataEndpointsDeployed once the upstream build carrying the endpoint is deployed.
            if (!Constants.Default.OrtoDataEndpointsDeployed)
            {
                return Ok(new OrtoDataBuildingResponse());
            }

            if (string.IsNullOrWhiteSpace(reference))
            {
                return BadRequest();
            }

            HttpClient httpClient = httpClientFactory.CreateClient();

            Building2DReference? building2DReference = await httpClient.Building2DReferenceAsync(reference, countyId, cancellationToken);
            if (building2DReference is null)
            {
                return NotFound();
            }

            List<short> years = await YearsAsync(httpClient, building2DReference.Reference, building2DReference.CountyId, tokenString, cancellationToken) ?? [];

            // The user entry is read from the full year built record; predicted years are never surfaced. Deserialized
            // as the concrete class because the user entry reader lives there, not on IYearBuiltData. The DiGi.GIS
            // class is named in full: DiGi.GIS.PostgreSQL.Classes holds a same-named YearBuiltData (Coding - General, section 1.9).
            UrlBuilder urlBuilder = new($"{Constants.Default.GISWebAPIUri}/gis/yearbuiltdata/itemsbyreference");
            urlBuilder = urlBuilder.AddParameter("reference", building2DReference.Reference);
            if (building2DReference.CountyId.HasValue)
            {
                urlBuilder = urlBuilder.AddParameter("countyid", building2DReference.CountyId.Value);
            }

            List<GIS.Classes.YearBuiltData>? yearBuiltDatas = await httpClient.ItemsAsync<GIS.Classes.YearBuiltData>(urlBuilder.ToString(), cancellationToken);
            GIS.Classes.UserYearBuilt? userYearBuilt = yearBuiltDatas?.FirstOrDefault()?.GetUserYearBuilt();

            OrtoDataBuildingResponse ortoDataBuildingResponse = new() { CountyId = building2DReference.CountyId, Reference = building2DReference.Reference, Years = years };
            if (userYearBuilt is not null)
            {
                ortoDataBuildingResponse.Selected = new OrtoDataBuildingResponse.Selection { Year = userYearBuilt.Year, Relation = (int)userYearBuilt.YearBuiltRelation, UserName = userYearBuilt.UserName, DateTime = userYearBuilt.DateTime };
            }

            return Ok(ortoDataBuildingResponse);
        }

        /// <summary>
        /// Reads the orthophoto image of a building for one year, as JPEG bytes.
        /// <para>The image is requested only for years the years read listed, and a year the service holds no photo for answers 404 here, which the page turns into a hidden card.</para>
        /// </summary>
        /// <param name="countyId">The optional identifier of the county part the building is filed under.</param>
        /// <param name="reference">The reference of the building.</param>
        /// <param name="year">The photo year to read.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by the caller to cancel the asynchronous operation.</param>
        /// <returns>An <see cref="IActionResult"/> holding the image, 404 when there is none, 400 without a reference or year, or 401 without a session.</returns>
        [HttpGet("image")]
        public async Task<IActionResult> GetImageAsync([FromQuery(Name = "countyid")] int? countyId, [FromQuery(Name = "reference")] string? reference, [FromQuery(Name = "year")] short? year, CancellationToken cancellationToken = default)
        {
            string? tokenString = Request.Cookies[Constants.Default.UserTokenCookieName];
            if (string.IsNullOrWhiteSpace(tokenString))
            {
                return Unauthorized();
            }

            // Nullable bindings on purpose: an absent reference or year must be refused rather than read as 0 or an empty string.
            if (string.IsNullOrWhiteSpace(reference) || !year.HasValue)
            {
                return BadRequest();
            }

            HttpClient httpClient = httpClientFactory.CreateClient();

            UrlBuilder urlBuilder = new(Constants.Default.OrtoDataImageByReferenceUri);
            urlBuilder = urlBuilder.AddParameter("reference", reference);
            urlBuilder = urlBuilder.AddParameter("year", year.Value);
            if (countyId.HasValue)
            {
                urlBuilder = urlBuilder.AddParameter("countyid", countyId.Value);
            }
            urlBuilder = urlBuilder.AddParameter("fallbackbyreference", true);

            byte[]? bytes = await httpClient.BytesAsync(urlBuilder.ToString(), cancellationToken);
            if (bytes is null || bytes.Length == 0)
            {
                return NotFound();
            }

            return File(bytes, "image/jpeg");
        }

        /// <summary>
        /// Records a reviewer's year built answer for a building, relaying it to the GIS Web API.
        /// <para>The upstream status is mirrored rather than reinterpreted: 404 is the page's "building not found", 400 the service's own refusal of an incomplete answer, 401 a spent session, and a 500 or 503 a server fault that must not be reported as the caller's mistake.</para>
        /// </summary>
        /// <param name="userYearBuiltParameter">The submitted answer.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by the caller to cancel the asynchronous operation.</param>
        /// <returns>An <see cref="IActionResult"/> that is empty on success or carries the status the service answered with.</returns>
        [HttpPost("useryearbuilt")]
        public async Task<IActionResult> SetUserYearBuiltAsync([FromBody] UserYearBuiltParameter? userYearBuiltParameter, CancellationToken cancellationToken = default)
        {
            string? tokenString = Request.Cookies[Constants.Default.UserTokenCookieName];
            if (string.IsNullOrWhiteSpace(tokenString))
            {
                return Unauthorized();
            }

            // TODO [OrtoDataEndpoints]: remove this gate together with Constants.Default.OrtoDataEndpointsDeployed once the upstream build carrying the endpoint is deployed.
            if (!Constants.Default.OrtoDataEndpointsDeployed)
            {
                return StatusCode(StatusCodes.Status501NotImplemented);
            }

            // Refused the way the service refuses an incomplete answer, so the page cannot distinguish this 400 from its own.
            if (userYearBuiltParameter is null || !userYearBuiltParameter.CountyId.HasValue || string.IsNullOrWhiteSpace(userYearBuiltParameter.Reference) || !userYearBuiltParameter.Year.HasValue)
            {
                return BadRequest();
            }

            HttpClient httpClient = httpClientFactory.CreateClient();

            WebAPIResponse? webAPIResponse = await httpClient.ResponseAsync(HttpMethod.Post, Constants.Default.YearBuiltDataSetUserYearBuiltUri, tokenString, userYearBuiltParameter, cancellationToken);
            if (webAPIResponse is null)
            {
                return StatusCode(StatusCodes.Status503ServiceUnavailable);
            }

            return StatusCode(webAPIResponse.StatusCode);
        }

        /// <summary>
        /// Reads the photo years the GIS Web API holds for a building, presenting the session token the read requires.
        /// <para>Asked with <c>fallbackbyreference</c>, so a reference filed under a different county part than the one named still answers with the part that holds it.</para>
        /// </summary>
        /// <param name="httpClient">The HTTP client used for the request.</param>
        /// <param name="reference">The reference of the building.</param>
        /// <param name="countyId">The identifier of the county part the building is filed under. This value can be null.</param>
        /// <param name="tokenString">The session token to present to the service.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by the caller to cancel the asynchronous operation.</param>
        /// <returns>The sorted years that hold a photo, or <see langword="null"/> when the service answered with none.</returns>
        private static async Task<List<short>?> YearsAsync(HttpClient httpClient, string? reference, int? countyId, string? tokenString, CancellationToken cancellationToken)
        {
            UrlBuilder urlBuilder = new(Constants.Default.OrtoDataYearsByReferenceUri);
            urlBuilder = urlBuilder.AddParameter("reference", reference);
            if (countyId.HasValue)
            {
                urlBuilder = urlBuilder.AddParameter("countyid", countyId.Value);
            }
            urlBuilder = urlBuilder.AddParameter("fallbackbyreference", true);

            WebAPIResponse? webAPIResponse = await httpClient.ResponseAsync(HttpMethod.Get, urlBuilder.ToString(), tokenString, cancellationToken);
            if (webAPIResponse?.StatusCode != StatusCodes.Status200OK || string.IsNullOrWhiteSpace(webAPIResponse.Json))
            {
                return null;
            }

            try
            {
                return JsonSerializer.Deserialize<List<short>>(webAPIResponse.Json);
            }
            catch (JsonException)
            {
                return null;
            }
        }
    }
}
