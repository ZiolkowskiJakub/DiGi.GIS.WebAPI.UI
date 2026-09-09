using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace DiGi.GIS.WebAPI.UI
{
    public static partial class Query
    {
        /// <summary>
        /// Asynchronously posts a request body to the GIS Web API and reads the response body.
        /// <para>The counterpart of <see cref="JsonAsync(HttpClient, string, CancellationToken)"/> for the endpoints that take their criteria in the body rather than in the query string, and it answers absence the same way - see that method for why.</para>
        /// <para>The HTTP verb is kept in the name deliberately, matching <c>DiGi.WebAPI.Query.GetAsync</c>: the verb is the only thing separating this method from its sibling, so nothing else can name it.</para>
        /// </summary>
        /// <typeparam name="T">The type of the request body.</typeparam>
        /// <param name="httpClient">The HTTP client used for the request. This value can be null.</param>
        /// <param name="requestUri">The GIS Web API URL to post to. This value can be null.</param>
        /// <param name="value">The request body, serialized as JSON.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by the caller to cancel the asynchronous operation.</param>
        /// <returns>The response body, or <see langword="null"/> when there is none.</returns>
        public static async Task<string?> PostJsonAsync<T>(this HttpClient? httpClient, string? requestUri, T value, CancellationToken cancellationToken = default)
        {
            if (httpClient is null || string.IsNullOrWhiteSpace(requestUri))
            {
                return null;
            }

            try
            {
                // JsonSerializerOptions.Default rather than what System.Net.Http.Json would pick on its own,
                // which is JsonSerializerDefaults.Web - a camelCase naming policy that renames every property
                // of the body on the way out. Nothing sent through here today carries property names (a string,
                // a collection of ints), so this changes no call that exists; it is what stops the first caller
                // that posts a parameter object from silently depending on the receiver binding names
                // case-insensitively. See Coding - WebAPI Contracts, section 2.
                using HttpResponseMessage httpResponseMessage = await httpClient.PostAsJsonAsync(requestUri, value, JsonSerializerOptions.Default, cancellationToken);
                if (!httpResponseMessage.IsSuccessStatusCode)
                {
                    return null;
                }

                string json = await httpResponseMessage.Content.ReadAsStringAsync(cancellationToken);
                if (string.IsNullOrWhiteSpace(json))
                {
                    return null;
                }

                return json;
            }
            catch
            {
                return null;
            }
        }
    }
}
