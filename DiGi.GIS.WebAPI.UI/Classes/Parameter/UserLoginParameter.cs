namespace DiGi.GIS.WebAPI.UI.Classes
{
    /// <summary>
    /// The credentials a visitor submits on the sign-in page, relayed to the user authentication service.
    /// <para>These two property names are the wire contract of <c>POST /user/login</c> and must match <c>DiGi.User.Classes.UserLogin</c>. This application reaches that service over HTTP only, so nothing checks them at compile time and a rename on either side fails silently - diff them by hand whenever either moves (Coding - WebAPI Contracts, section 5).</para>
    /// <para><see cref="Password"/> is a secret in transit. It is never logged, never echoed back to the browser and never written into a view.</para>
    /// </summary>
    public class UserLoginParameter
    {
        /// <summary> Gets or sets the email address identifying the account. </summary>
        public string? Email { get; set; }

        /// <summary> Gets or sets the password submitted for the account. </summary>
        public string? Password { get; set; }
    }
}
