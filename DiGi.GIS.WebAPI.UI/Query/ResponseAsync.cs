using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace DiGi.GIS.WebAPI.UI
{
    public static partial class Query
    {
        /// <summary>
        /// Asynchronously relays a request carrying no body to a Web API, and reads back the status it answered with together with the body it returned.
        /// <para>The authentication counterpart of <see cref="JsonAsync(HttpClient, string, CancellationToken)"/>. It differs in the one way that matters for signing in: a failure status is reported rather than collapsed into <see langword="null"/> - see <see cref="Classes.WebAPIResponse"/> for why absence and refusal must not be the same answer here.</para>
        /// <para><see langword="null"/> is still returned for the failures that carry no status at all: no client, no URL, the service unreachable, or the caller cancelling. Those are the cases where nothing was answered, as opposed to something being refused.</para>
        /// <para>The token is attached as <c>Authorization: Bearer</c> when one is given, and omitted entirely when it is not, so the same method serves an anonymous sign-in and an authenticated read.</para>
        /// </summary>
        /// <param name="httpClient">The HTTP client used for the request. This value can be null.</param>
        /// <param name="httpMethod">The HTTP method to use. This value can be null.</param>
        /// <param name="requestUri">The Web API URL to relay to. This value can be null.</param>
        /// <param name="bearerToken">The session token to present, or null to make the request anonymously.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by the caller to cancel the asynchronous operation.</param>
        /// <returns>The status and body the Web API answered with, or <see langword="null"/> when it answered nothing at all.</returns>
        public static async Task<Classes.WebAPIResponse?> ResponseAsync(this HttpClient? httpClient, HttpMethod? httpMethod, string? requestUri, string? bearerToken, CancellationToken cancellationToken = default)
        {
            // A call with no body binds here rather than to the overload below with T inferred as CancellationToken:
            // where both are applicable C# prefers the non-generic method. Spelled out because the two signatures
            // differ only in a parameter that the generic one does not default.
            return await httpClient.ResponseAsync<object?>(httpMethod, requestUri, bearerToken, null, cancellationToken);
        }

        /// <summary>
        /// Asynchronously relays a request carrying a JSON body to a Web API, and reads back the status it answered with together with the body it returned.
        /// <para>Behaves exactly as the overload above; see it for how failures are reported.</para>
        /// <para>The body is serialized as the declared type <typeparamref name="T"/>, so call this with the concrete type rather than through a variable typed as <see cref="object"/> - the latter serializes as an empty object without complaining.</para>
        /// </summary>
        /// <typeparam name="T">The type of the request body.</typeparam>
        /// <param name="httpClient">The HTTP client used for the request. This value can be null.</param>
        /// <param name="httpMethod">The HTTP method to use. This value can be null.</param>
        /// <param name="requestUri">The Web API URL to relay to. This value can be null.</param>
        /// <param name="bearerToken">The session token to present, or null to make the request anonymously.</param>
        /// <param name="value">The request body, serialized as JSON. A null value sends no body at all.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by the caller to cancel the asynchronous operation.</param>
        /// <returns>The status and body the Web API answered with, or <see langword="null"/> when it answered nothing at all.</returns>
        public static async Task<Classes.WebAPIResponse?> ResponseAsync<T>(this HttpClient? httpClient, HttpMethod? httpMethod, string? requestUri, string? bearerToken, T? value, CancellationToken cancellationToken = default)
        {
            if (httpClient is null || httpMethod is null || string.IsNullOrWhiteSpace(requestUri))
            {
                return null;
            }

            try
            {
                using HttpRequestMessage httpRequestMessage = new(httpMethod, requestUri);

                if (!string.IsNullOrWhiteSpace(bearerToken))
                {
                    httpRequestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);
                }

                if (value is not null)
                {
                    // JsonSerializerOptions.Default, not the System.Net.Http.Json default. That default is
                    // JsonSerializerDefaults.Web, which renames every property to camelCase on the way out,
                    // so UserLoginParameter went on the wire as {"email","password"} while the contract, and
                    // the service's own UserLogin, say Email and Password. It worked only because ASP.NET Core
                    // binds case-insensitively; the moment a receiver stops doing so, every login fails as a
                    // refused credential. Sending the declared names removes that dependency.
                    httpRequestMessage.Content = JsonContent.Create(value, options: JsonSerializerOptions.Default);
                }

                using HttpResponseMessage httpResponseMessage = await httpClient.SendAsync(httpRequestMessage, cancellationToken);

                string json = await httpResponseMessage.Content.ReadAsStringAsync(cancellationToken);

                return new Classes.WebAPIResponse((int)httpResponseMessage.StatusCode, string.IsNullOrWhiteSpace(json) ? null : json);
            }
            catch
            {
                return null;
            }
        }
    }
}
