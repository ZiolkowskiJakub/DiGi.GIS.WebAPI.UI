using DiGi.Geometry.Planar.Classes;
using DiGi.Geometry.Spatial.Classes;
using DiGi.GLTF.Classes;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace DiGi.GIS.WebAPI.UI
{
    public static partial class Create
    {
        /// <summary>
        /// Asynchronously creates the <see cref="GLTFNode"/> holding the ground surface of an area, read from the GIS Web API terrain service.
        /// <para>The surface keeps the elevations it is stored with. Nothing is shifted onto another datum here, so a scene mixing terrain with other geometry only lines up when that geometry carries real elevations too - see the TERRAIN note on <see cref="Constants.Default.TerrainEnabled"/>.</para>
        /// <para><see langword="null"/> is returned whenever there is no surface to show, for any reason (see <see cref="Query.TerrainJsonAsync(HttpClient, string, CancellationToken)"/>). A caller adds the node when it gets one and carries on unchanged when it does not.</para>
        /// </summary>
        /// <param name="httpClient">The HTTP client used for the request. This value can be null.</param>
        /// <param name="requestUri">The terrain service URL to read the surface from, as built by <see cref="Query.TerrainRequestUri(Circle2D, double?)"/>. This value can be null.</param>
        /// <param name="name">The name given to the node. This value can be null.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by the caller to cancel the asynchronous operation.</param>
        /// <returns>The terrain node, or <see langword="null"/> when the area has no surface to show.</returns>
        public static async Task<GLTFNode?> TerrainGLTFNodeAsync(this HttpClient? httpClient, string? requestUri, string? name = null, CancellationToken cancellationToken = default)
        {
            string? json = await httpClient.TerrainJsonAsync(requestUri, cancellationToken);
            if (json is null)
            {
                return null;
            }

            Mesh3D? mesh3D = Core.Convert.ToDiGi<Mesh3D>(json)?.FirstOrDefault();
            if (mesh3D is null)
            {
                return null;
            }

            return new GLTFNode(name ?? Constants.Default.TerrainName, null, mesh3D, new Core.Classes.Color(255, 138, 128, 102), 1, null);
        }

        /// <summary>
        /// Asynchronously creates the <see cref="GLTFNode"/> holding the ground surface of a circular area, read from the GIS Web API terrain service.
        /// </summary>
        /// <param name="httpClient">The HTTP client used for the request. This value can be null.</param>
        /// <param name="circle2D">The area to show the terrain surface of, in PL-1992 (EPSG:2180) metres. This value can be null.</param>
        /// <param name="name">The name given to the node. This value can be null.</param>
        /// <param name="tolerance">An optional tolerance for the spatial query, in metres. When omitted the terrain service applies its own default.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by the caller to cancel the asynchronous operation.</param>
        /// <returns>The terrain node, or <see langword="null"/> when the area has no surface to show.</returns>
        public static async Task<GLTFNode?> TerrainGLTFNodeAsync(this HttpClient? httpClient, Circle2D? circle2D, string? name = null, double? tolerance = null, CancellationToken cancellationToken = default)
        {
            return await httpClient.TerrainGLTFNodeAsync(circle2D.TerrainRequestUri(tolerance), name, cancellationToken);
        }
    }
}
