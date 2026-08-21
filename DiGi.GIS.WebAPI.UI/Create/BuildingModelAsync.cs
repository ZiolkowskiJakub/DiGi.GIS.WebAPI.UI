using DiGi.Analytical.Building.Classes;
using DiGi.WebAPI.Classes;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace DiGi.GIS.WebAPI.UI
{
    public static partial class Create
    {
        /// <summary>
        /// Asynchronously creates a <see cref="BuildingModel"/> for the building with the specified unique identifier by fetching the <see cref="GIS.Classes.Building2D"/> from the GIS Web API and extruding its 2D footprint storey by storey into individual components (walls, floors and roofs).
        /// </summary>
        /// <param name="httpClient">The <see cref="HttpClient"/> used to call the GIS Web API.</param>
        /// <param name="id">The unique identifier of the building.</param>
        /// <param name="countyId">The optional unique identifier of the county associated with the building.</param>
        /// <param name="storeyHeight">The height of a single storey in meters used for the extrusion.</param>
        /// <param name="tolerance">The distance tolerance used during component creation.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by the caller to cancel the asynchronous operation.</param>
        /// <returns>A <see cref="Task{TResult}"/> holding the created <see cref="BuildingModel"/>, or null if the building could not be fetched or converted.</returns>
        public static async Task<BuildingModel?> BuildingModelAsync(this HttpClient? httpClient, long id, int? countyId = null, double storeyHeight = Constants.Default.StoreyHeight, double tolerance = DiGi.Core.Constants.Tolerance.Distance, CancellationToken cancellationToken = default)
        {
            if (httpClient is null)
            {
                return null;
            }

            UrlBuilder urlBuilder = new($"{Constants.Default.GISWebAPIUri}/gis/building2D/itembyid");
            urlBuilder = urlBuilder.AddParameter("id", id);
            if (countyId.HasValue)
            {
                urlBuilder = urlBuilder.AddParameter("countyid", countyId.Value);
            }

            GIS.Classes.Building2D? building2D = await httpClient.ItemAsync<GIS.Classes.Building2D>(urlBuilder.ToString(), cancellationToken);
            if (building2D is null)
            {
                return null;
            }

            // Bound by name - the extruded overload takes the base elevation ahead of the storey height and the tolerance, so passing them positionally would place the model at the storey height and extrude it by the tolerance.
            return Analytical.Create.BuildingModel(building2D, storeyHeight: storeyHeight, tolerance: tolerance);
        }
    }
}