using DiGi.GIS.PostgreSQL.Classes;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace DiGi.GIS.WebAPI.UI
{
    public static partial class Query
    {
        /// <summary>
        /// Asynchronously resolves a building known by its reference and a point inside it to its <see cref="Building2DReference"/> (identifier and county), as the 3D viewers know a building.
        /// <para>A viewer node reference may be a ComplexReference carrying the building reference together with its county (see <c>PostgreSQL.Create.Reference</c>); it is unwrapped first, and its county is used when <paramref name="countyId"/> is not given.</para>
        /// <para>Without a county, every polygon part of the county the point falls in is tried rather than one being chosen (<see cref="CountyIdsAsync(HttpClient?, double?, double?, CancellationToken)"/>): a county whose territory is disconnected is stored as one row per part and the building is filed under one of them, which is not necessarily the part covering its own coordinates - for code 3020 the data sits under a part that covers none of the buildings at all (ZiolkowskiJakub/DiGi.GIS.PostgreSQL#64). When no part answers, or the point names no county, the reference alone is looked up.</para>
        /// </summary>
        /// <param name="httpClient">The HTTP client used for the requests. This value can be null.</param>
        /// <param name="reference">The reference of the building, plain or as a viewer node reference. This value can be null.</param>
        /// <param name="countyId">The optional unique identifier of the county associated with the building.</param>
        /// <param name="x">The X coordinate of a point inside the building, in PL-1992 (EPSG:2180) metres. This value can be null.</param>
        /// <param name="y">The Y coordinate of a point inside the building, in PL-1992 (EPSG:2180) metres. This value can be null.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by the caller to cancel the asynchronous operation.</param>
        /// <returns>The <see cref="Building2DReference"/>, or <see langword="null"/> when the building could not be resolved.</returns>
        public static async Task<Building2DReference?> Building2DReferenceByPointAsync(this HttpClient? httpClient, string? reference, int? countyId, double? x, double? y, CancellationToken cancellationToken = default)
        {
            if (httpClient is null || string.IsNullOrWhiteSpace(reference))
            {
                return null;
            }

            if (PostgreSQL.Query.TryParse(reference, out string buildingModelReference, out int? countyId_Reference, out _))
            {
                if (!string.IsNullOrWhiteSpace(buildingModelReference))
                {
                    reference = buildingModelReference;
                }

                countyId ??= countyId_Reference;
            }

            if (countyId.HasValue)
            {
                return await httpClient.Building2DReferenceAsync(reference, countyId, cancellationToken);
            }

            List<int>? countyIds = await httpClient.CountyIdsAsync(x, y, cancellationToken);
            if (countyIds is not null)
            {
                foreach (int countyId_Part in countyIds)
                {
                    Building2DReference? building2DReference = await httpClient.Building2DReferenceAsync(reference, countyId_Part, cancellationToken);
                    if (building2DReference is not null)
                    {
                        return building2DReference;
                    }
                }
            }

            return await httpClient.Building2DReferenceAsync(reference, null, cancellationToken);
        }
    }
}
