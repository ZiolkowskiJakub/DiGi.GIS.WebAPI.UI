using DiGi.Analytical.Building.Classes;
using DiGi.WebAPI.Classes;
using Building2DReference = DiGi.GIS.PostgreSQL.Classes.Building2DReference;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace DiGi.GIS.WebAPI.UI
{
    public static partial class Query
    {
        /// <summary>
        /// Asynchronously retrieves the 3D <see cref="BuildingModel"/> stored in the database for the building with the specified unique identifier.
        /// </summary>
        /// <param name="httpClient">The HTTP client used for the request. This value can be null.</param>
        /// <param name="id">The unique identifier of the building.</param>
        /// <param name="countyId">The optional unique identifier of the county associated with the building.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by the caller to cancel the asynchronous operation.</param>
        /// <returns>The stored <see cref="BuildingModel"/>, or <see langword="null"/> if the building could not be found.</returns>
        public static async Task<BuildingModel?> BuildingModelAsync(this HttpClient? httpClient, long id, int? countyId = null, CancellationToken cancellationToken = default)
        {
            if (httpClient is null)
            {
                return null;
            }

            UrlBuilder urlBuilder = new($"{Constants.Default.GISWebAPIUri}/gis/building2D/building2Dreferencebyid");
            urlBuilder = urlBuilder.AddParameter("id", id);
            if (countyId.HasValue)
            {
                urlBuilder = urlBuilder.AddParameter("countyid", countyId.Value);
            }

            Building2DReference? building2DReference = await httpClient.ItemAsync<Building2DReference>(urlBuilder.ToString(), cancellationToken);
            if (building2DReference is null || string.IsNullOrWhiteSpace(building2DReference.Reference))
            {
                return null;
            }

            return await httpClient.BuildingModelAsync(building2DReference.Reference, building2DReference.CountyId ?? countyId, cancellationToken);
        }

        /// <summary>
        /// Asynchronously retrieves the 3D <see cref="BuildingModel"/> stored in the database for the specified cadastral building reference.
        /// </summary>
        /// <param name="httpClient">The HTTP client used for the request. This value can be null.</param>
        /// <param name="reference">The cadastral reference of the building.</param>
        /// <param name="countyId">The optional unique identifier of the county associated with the building.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by the caller to cancel the asynchronous operation.</param>
        /// <returns>The stored <see cref="BuildingModel"/>, or <see langword="null"/> if the building could not be found.</returns>
        public static async Task<BuildingModel?> BuildingModelAsync(this HttpClient? httpClient, string? reference, int? countyId = null, CancellationToken cancellationToken = default)
        {
            if (httpClient is null || string.IsNullOrWhiteSpace(reference))
            {
                return null;
            }

            UrlBuilder urlBuilder = new($"{Constants.Default.GISWebAPIUri}/gis/buildingmodel/itemsbyreferences");
            urlBuilder = urlBuilder.AddParameter("references", reference);
            if (countyId.HasValue)
            {
                urlBuilder = urlBuilder.AddParameter("countyid", countyId.Value);
            }

            return await httpClient.ItemAsync<BuildingModel>(urlBuilder.ToString(), cancellationToken);
        }
    }
}
