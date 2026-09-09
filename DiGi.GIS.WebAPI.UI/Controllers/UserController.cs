using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace DiGi.GIS.WebAPI.UI.Controllers
{
    /// <summary>
    /// Controller providing the sign-in page and the session endpoints this application's pages use, relaying each of them to the user authentication service (DiGi.User.WebAPI).
    /// <para>The session token never reaches the browser. It is written into an HttpOnly cookie by <see cref="LoginAsync"/>, read back out of that cookie here on every later call, and presented to the service as a bearer token - so a script on the page can act as the visitor but can never read, copy or forward the credential that lets it.</para>
    /// <para>Nothing in this application is gated on being signed in. These endpoints add a session; which content requires one is decided feature by feature as gated features arrive.</para>
    /// </summary>
    [Route("[controller]")]
    public class UserController : Controller
    {
        private readonly IHttpClientFactory httpClientFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserController"/> class.
        /// </summary>
        /// <param name="httpClientFactory">The HTTP client factory used to create <see cref="HttpClient"/> instances.</param>
        public UserController(IHttpClientFactory httpClientFactory)
        {
            this.httpClientFactory = httpClientFactory;
        }

        // This action will trigger for: gis.digiproject.uk/login
        /// <summary>
        /// Returns the sign-in page.
        /// <para>Routed at the site root rather than under this controller, because it is a page a visitor is sent to and reads as an address, not one of the session endpoints below.</para>
        /// </summary>
        /// <param name="returnUrl">The page to return to once signed in.</param>
        /// <returns>An <see cref="IActionResult"/> holding the sign-in view.</returns>
        [HttpGet("/login")]
        public IActionResult Login([FromQuery(Name = "returnUrl")] string? returnUrl = null)
        {
            // Only an address inside this application is ever returned to. Url.IsLocalUrl rejects an absolute URL,
            // so a crafted returnUrl cannot turn the sign-in page into a redirector to somebody else's site.
            ViewData["ReturnUrl"] = Url.IsLocalUrl(returnUrl) ? returnUrl : Url.Content("~/");

            return View();
        }

        /// <summary>
        /// Exchanges a set of credentials for a session, storing the issued token in this application's session cookie.
        /// <para>Nothing about which part of the credential was wrong survives this method. An unknown email, an account with no stored credential and a wrong password are one 401 to the service and stay one 401 here, so the page can only ever say that signing in failed.</para>
        /// <para>The submitted password exists in the request body and in the relayed body, and nowhere else: it is never logged, never returned and never written into a view. Do not add request logging to this action.</para>
        /// </summary>
        /// <param name="userLoginParameter">The submitted credentials.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by the caller to cancel the asynchronous operation.</param>
        /// <returns>An <see cref="IActionResult"/> that is empty on success, the session being carried in a cookie rather than in the body.</returns>
        [HttpPost("login")]
        public async Task<IActionResult> LoginAsync([FromBody] Classes.UserLoginParameter? userLoginParameter, CancellationToken cancellationToken = default)
        {
            // Refused here rather than relayed, and refused the way the service refuses a wrong password: an
            // incomplete form is not a different outcome to the visitor, and there is nothing to ask the service.
            if (userLoginParameter is null || string.IsNullOrWhiteSpace(userLoginParameter.Email) || string.IsNullOrWhiteSpace(userLoginParameter.Password))
            {
                return Unauthorized();
            }

            HttpClient httpClient = httpClientFactory.CreateClient();

            Classes.WebAPIResponse? webAPIResponse = await httpClient.ResponseAsync(HttpMethod.Post, Constants.Default.UserLoginUri, null, userLoginParameter, cancellationToken);
            if (webAPIResponse is null)
            {
                // The service answered nothing at all. Reported as an outage rather than as a refused credential,
                // so the page can say the service is unavailable instead of blaming the visitor's password.
                return StatusCode(StatusCodes.Status503ServiceUnavailable);
            }

            string? tokenString = webAPIResponse.StatusCode == StatusCodes.Status200OK ? webAPIResponse.Json.TokenString() : null;
            if (string.IsNullOrWhiteSpace(tokenString))
            {
                // A refusal is passed through as it stands - an upstream 500 is a server fault and must not be
                // reported as a bad request, which would blame the caller for it. A 200 carrying no usable token is
                // a broken contract rather than a refusal, so it is reported as a bad gateway.
                return StatusCode(webAPIResponse.StatusCode == StatusCodes.Status200OK ? StatusCodes.Status502BadGateway : webAPIResponse.StatusCode);
            }

            Response.Cookies.Append(Constants.Default.UserTokenCookieName, tokenString, Create.UserTokenCookieOptions());

            // Deliberately no body. The token stays on this side of the browser.
            return Ok();
        }

        /// <summary>
        /// Reads the stored record of the signed-in visitor, for the name and email the header shows.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token that can be used by the caller to cancel the asynchronous operation.</param>
        /// <returns>An <see cref="IActionResult"/> holding the user record, or an empty result when nobody is signed in.</returns>
        [HttpGet("me")]
        public async Task<IActionResult> GetMeAsync(CancellationToken cancellationToken = default)
        {
            string? tokenString = Request.Cookies[Constants.Default.UserTokenCookieName];
            if (string.IsNullOrWhiteSpace(tokenString))
            {
                // Being signed out is not a failure. Answered the way every other absence in this application is,
                // so the header hides its panel rather than reporting an error on a page that is working fine.
                return NoContent();
            }

            HttpClient httpClient = httpClientFactory.CreateClient();

            Classes.WebAPIResponse? webAPIResponse = await httpClient.ResponseAsync(HttpMethod.Get, Constants.Default.UserSecureDataUri, tokenString, cancellationToken);

            return Relay(webAPIResponse);
        }

        /// <summary>
        /// Introspects the current session, returning the identity and the token's issue and expiry times.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token that can be used by the caller to cancel the asynchronous operation.</param>
        /// <returns>An <see cref="IActionResult"/> holding the session information, or an empty result when nobody is signed in.</returns>
        [HttpGet("session")]
        public async Task<IActionResult> GetSessionAsync(CancellationToken cancellationToken = default)
        {
            string? tokenString = Request.Cookies[Constants.Default.UserTokenCookieName];
            if (string.IsNullOrWhiteSpace(tokenString))
            {
                return NoContent();
            }

            HttpClient httpClient = httpClientFactory.CreateClient();

            Classes.WebAPIResponse? webAPIResponse = await httpClient.ResponseAsync(HttpMethod.Get, Constants.Default.UserSessionUri, tokenString, cancellationToken);

            return Relay(webAPIResponse);
        }

        /// <summary>
        /// Carries the session forward by exchanging the presented token for a new one.
        /// <para>This works only while the presented token is still valid: the service issues the new token for the identity carried by the old one, so a session that has already expired cannot be recovered and ends in signing out.</para>
        /// </summary>
        /// <param name="cancellationToken">A cancellation token that can be used by the caller to cancel the asynchronous operation.</param>
        /// <returns>An <see cref="IActionResult"/> that is empty on success, the renewed session being carried in the cookie.</returns>
        [HttpPost("session/refresh")]
        public async Task<IActionResult> RefreshAsync(CancellationToken cancellationToken = default)
        {
            string? tokenString = Request.Cookies[Constants.Default.UserTokenCookieName];
            if (string.IsNullOrWhiteSpace(tokenString))
            {
                return Unauthorized();
            }

            HttpClient httpClient = httpClientFactory.CreateClient();

            Classes.WebAPIResponse? webAPIResponse = await httpClient.ResponseAsync(HttpMethod.Post, Constants.Default.UserRefreshUri, tokenString, cancellationToken);
            if (webAPIResponse is null)
            {
                // The session is left standing: the service being unreachable for a moment says nothing about
                // whether the token the visitor holds is still good.
                return StatusCode(StatusCodes.Status503ServiceUnavailable);
            }

            string? tokenString_New = webAPIResponse.StatusCode == StatusCodes.Status200OK ? webAPIResponse.Json.TokenString() : null;
            if (string.IsNullOrWhiteSpace(tokenString_New))
            {
                // There is nothing to carry the session forward with, so it ends here rather than leaving behind a
                // cookie that every later call would be refused for while the header still claims a session.
                Response.Cookies.Delete(Constants.Default.UserTokenCookieName, Create.UserTokenCookieOptions());

                return StatusCode(webAPIResponse.StatusCode == StatusCodes.Status200OK ? StatusCodes.Status502BadGateway : webAPIResponse.StatusCode);
            }

            Response.Cookies.Append(Constants.Default.UserTokenCookieName, tokenString_New, Create.UserTokenCookieOptions());

            return Ok();
        }

        /// <summary>
        /// Ends the session, clearing this application's cookie and asking the service to revoke the token.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token that can be used by the caller to cancel the asynchronous operation.</param>
        /// <returns>An <see cref="IActionResult"/> confirming that the session has ended.</returns>
        [HttpPost("logout")]
        public async Task<IActionResult> LogoutAsync(CancellationToken cancellationToken = default)
        {
            string? tokenString = Request.Cookies[Constants.Default.UserTokenCookieName];

            // Cleared first, and whatever the service goes on to answer. A cookie left behind by a failed relay is
            // a visitor who still looks signed in and cannot sign out, which is the worse of the two failures; the
            // token it held expires on its own within the hour either way.
            Response.Cookies.Delete(Constants.Default.UserTokenCookieName, Create.UserTokenCookieOptions());

            if (!string.IsNullOrWhiteSpace(tokenString))
            {
                HttpClient httpClient = httpClientFactory.CreateClient();

                // The outcome is deliberately not inspected. Revocation is the service's business, and the session
                // is over on this side regardless of what it says about a token it may already have forgotten.
                await httpClient.ResponseAsync(HttpMethod.Post, Constants.Default.UserLogoutUri, tokenString, cancellationToken);
            }

            return Ok();
        }

        /// <summary>
        /// Turns a relayed response into the result this application answers with.
        /// <para>The upstream status is mirrored rather than reinterpreted: a 500 from the authentication service is a server fault, and answering it as a bad request would blame the caller for it.</para>
        /// <para>Only a successful body is passed on. A failure body carries the service's own diagnostics - a trace identifier and its build number - which are of no use to the page and need not be published to whoever asked.</para>
        /// <para>A 401 is relayed with the cookie left alone, deliberately. It means the presented token was not accepted, but not yet that the session is over: the caller is expected to try <see cref="RefreshAsync"/> once, and clearing the cookie here would delete the very token that refresh has to present, turning every expiry into an immediate sign-out. Ending the session is <see cref="RefreshAsync"/>'s to do, when the refresh itself is refused.</para>
        /// </summary>
        /// <param name="webAPIResponse">The relayed response. This value can be null.</param>
        /// <returns>The result to answer with.</returns>
        private IActionResult Relay(Classes.WebAPIResponse? webAPIResponse)
        {
            if (webAPIResponse is null)
            {
                return StatusCode(StatusCodes.Status503ServiceUnavailable);
            }

            if (webAPIResponse.StatusCode == StatusCodes.Status200OK && !string.IsNullOrWhiteSpace(webAPIResponse.Json))
            {
                return Content(webAPIResponse.Json, "application/json");
            }

            return StatusCode(webAPIResponse.StatusCode);
        }
    }
}
