using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace DiGi.GIS.WebAPI.UI
{
    public static partial class Query
    {
        /// <summary>
        /// Asynchronously reads a GIS Web API response and deserializes it into DiGi objects.
        /// <para>An absent body and a body that cannot be deserialized are both answered with <see langword="null"/>, for the reasons given on <see cref="JsonAsync(HttpClient, string, CancellationToken)"/>.</para>
        /// </summary>
        /// <typeparam name="T">The type of the objects carried by the response.</typeparam>
        /// <param name="httpClient">The HTTP client used for the request. This value can be null.</param>
        /// <param name="requestUri">The GIS Web API URL to read. This value can be null.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by the caller to cancel the asynchronous operation.</param>
        /// <returns>The objects carried by the response, or <see langword="null"/> when there are none.</returns>
        public static async Task<List<T>?> ItemsAsync<T>(this HttpClient? httpClient, string? requestUri, CancellationToken cancellationToken = default) where T : Core.Interfaces.ISerializableObject
        {
            string? json = await httpClient.JsonAsync(requestUri, cancellationToken);
            if (string.IsNullOrWhiteSpace(json))
            {
                return null;
            }

            return Core.Convert.ToDiGi<T>(json);
        }
    }
}
