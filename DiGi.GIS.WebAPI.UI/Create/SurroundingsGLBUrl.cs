using System.Globalization;

namespace DiGi.GIS.WebAPI.UI
{
    public static partial class Create
    {
        /// <summary>
        /// Creates the application relative URL of the surrounding elements payload of the Building Viewer (<c>buildingmodel/glb/surroundingsbybuildingid</c>) for a building known by identifier, which the viewer fetches only when the user first checks "Show surrounding elements".
        /// </summary>
        /// <param name="id">The unique identifier of the building.</param>
        /// <param name="countyId">The optional unique identifier of the county associated with the building.</param>
        /// <returns>The application relative URL.</returns>
        public static string SurroundingsGLBUrl(long id, int? countyId)
        {
            string result = $"~/buildingmodel/glb/surroundingsbybuildingid?id={id.ToString(CultureInfo.InvariantCulture)}";
            if (countyId.HasValue)
            {
                result += $"&countyid={countyId.Value.ToString(CultureInfo.InvariantCulture)}";
            }

            return result;
        }
    }
}
