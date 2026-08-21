using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace DiGi.GIS.WebAPI.UI
{
    public static partial class Query
    {
        /// <summary>
        /// Asynchronously reads a GIS Web API response and deserializes the first DiGi object it carries.
        /// <para>The GIS Web API serializes a single object as a one element collection, so a by-identifier read and a by-criteria read are deserialized the same way and differ only in how many elements are worth looking at.</para>
        /// <para>An absent body, a body that cannot be deserialized and an empty collection are all answered with <see langword="null"/>, for the reasons given on <see cref="JsonAsync(HttpClient, string, CancellationToken)"/>.</para>
        /// </summary>
        /// <typeparam name="T">The type of the object carried by the response.</typeparam>
        /// <param name="httpClient">The HTTP client used for the request. This value can be null.</param>
        /// <param name="requestUri">The GIS Web API URL to read. This value can be null.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by the caller to cancel the asynchronous operation.</param>
        /// <returns>The first object carried by the response, or <see langword="null"/> when there is none.</returns>
        public static async Task<T?> ItemAsync<T>(this HttpClient? httpClient, string? requestUri, CancellationToken cancellationToken = default) where T : Core.Interfaces.ISerializableObject
        {
            List<T>? items = await httpClient.ItemsAsync<T>(requestUri, cancellationToken);
            if (items is null || items.Count == 0)
            {
                return default;
            }

            return items[0];
        }
    }
}
