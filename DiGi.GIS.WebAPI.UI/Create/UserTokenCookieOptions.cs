using Microsoft.AspNetCore.Http;

namespace DiGi.GIS.WebAPI.UI
{
    public static partial class Create
    {
        /// <summary>
        /// Creates the options this application writes and deletes its session token cookie with.
        /// <para><c>HttpOnly</c> puts the token out of reach of every script on the page: the browser presents it on each request, but nothing running in the page can read, copy or forward it. <c>Secure</c> keeps it off a plaintext connection - local development is served over https as well, so this holds there too.</para>
        /// <para><c>SameSite=Lax</c> means a cross-site POST does not carry the cookie, which is what lets the relay actions accept a request without an antiforgery token. Relaxing it to <c>None</c> without adding one would open them to cross-site forgery.</para>
        /// <para>No expiry is set, so this is a session cookie. A token issued by the service lives one hour (<c>DiGi.User.WebAPI.Constants.Session.TokenLifetime</c>) and the signing key is regenerated whenever that host restarts, so a longer-lived cookie would only keep a dead token around to be refused.</para>
        /// <para>A cookie is deleted by matching its name, path and domain, so the delete has to be given the same options as the append. That is why they are created here once rather than written out at each call site.</para>
        /// </summary>
        /// <returns>The <see cref="CookieOptions"/> used for the session token cookie.</returns>
        public static CookieOptions UserTokenCookieOptions()
        {
            return new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Lax,
                Path = "/"
            };
        }
    }
}
