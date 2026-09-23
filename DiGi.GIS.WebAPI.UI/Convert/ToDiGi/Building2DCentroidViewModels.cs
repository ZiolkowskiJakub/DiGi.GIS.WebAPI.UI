using DiGi.GIS.WebAPI.UI.ViewModels;
using System.Collections.Generic;
using System.Text.Json;

namespace DiGi.GIS.WebAPI.UI
{
    public static partial class Convert
    {
        /// <summary>
        /// Reads the compact, columnar answer of the GIS Web API's <c>gis/building2D/centroidsbyadministrativeareal2Did</c> (DiGi.GIS.WebAPI#40) - <c>{"References":[…],"CountyIds":[…],"X":[…],"Y":[…]}</c>, the i-th entry of every array describing one building - straight into the view models the Typology area view draws its dots from.
        /// <para>Parsed with <see cref="JsonDocument"/>, with no DiGi object in between: the full answer made the relay rebuild every row as a <c>Building2DCentroid</c> through <c>Core.Convert.ToDiGi</c> only to keep these four values, 155 307 times for county 1465 (DiGi.GIS.WebAPI.UI#29).</para>
        /// <para>A body that is not that shape - not JSON, an array missing, arrays of unequal length, an entry of the wrong kind - is refused as a whole rather than read in part: a partial read would draw an area with buildings silently missing.</para>
        /// </summary>
        /// <param name="json">The response body. This value can be null.</param>
        /// <returns>The centroids in body order, an empty list for four empty arrays, or <see langword="null"/> when the body is missing or not the compact shape.</returns>
        public static List<Building2DCentroidViewModel>? ToDiGi_Building2DCentroidViewModels(string? json)
        {
            if (string.IsNullOrWhiteSpace(json))
            {
                return null;
            }

            try
            {
                using JsonDocument jsonDocument = JsonDocument.Parse(json);
                JsonElement jsonElement_Root = jsonDocument.RootElement;
                if (jsonElement_Root.ValueKind != JsonValueKind.Object
                    || !jsonElement_Root.TryGetProperty("References", out JsonElement references)
                    || !jsonElement_Root.TryGetProperty("CountyIds", out JsonElement countyIds)
                    || !jsonElement_Root.TryGetProperty("X", out JsonElement xs)
                    || !jsonElement_Root.TryGetProperty("Y", out JsonElement ys)
                    || references.ValueKind != JsonValueKind.Array
                    || countyIds.ValueKind != JsonValueKind.Array
                    || xs.ValueKind != JsonValueKind.Array
                    || ys.ValueKind != JsonValueKind.Array)
                {
                    return null;
                }

                int count = references.GetArrayLength();
                if (countyIds.GetArrayLength() != count || xs.GetArrayLength() != count || ys.GetArrayLength() != count)
                {
                    return null;
                }

                List<Building2DCentroidViewModel> result = new(count);
                for (int i = 0; i < count; i++)
                {
                    JsonElement reference = references[i];
                    JsonElement countyId = countyIds[i];
                    JsonElement x = xs[i];
                    JsonElement y = ys[i];

                    if (reference.ValueKind != JsonValueKind.String || !countyId.TryGetInt32(out int countyId_Value) || !x.TryGetDouble(out double x_Value) || !y.TryGetDouble(out double y_Value))
                    {
                        return null;
                    }

                    string? reference_Value = reference.GetString();
                    if (string.IsNullOrWhiteSpace(reference_Value))
                    {
                        return null;
                    }

                    result.Add(new Building2DCentroidViewModel(reference_Value, countyId_Value, x_Value, y_Value));
                }

                return result;
            }
            catch (JsonException)
            {
                return null;
            }
            catch (System.InvalidOperationException)
            {
                // TryGetInt32 / TryGetDouble on an element that is not a number.
                return null;
            }
        }
    }
}
