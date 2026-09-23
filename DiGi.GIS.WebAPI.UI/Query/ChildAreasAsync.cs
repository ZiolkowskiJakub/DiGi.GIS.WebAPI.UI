using DiGi.GIS.PostgreSQL.Classes;
using DiGi.GIS.PostgreSQL.Enums;
using DiGi.GIS.WebAPI.UI.ViewModels;
using DiGi.WebAPI.Classes;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace DiGi.GIS.WebAPI.UI
{
    public static partial class Query
    {
        /// <summary>
        /// Lists the areas one level below an area too large for a Typology solve, with their building counts, so the area view can offer a smaller area instead of an error (DiGi.GIS.WebAPI.UI#29, B2).
        /// <para>A <b>voivodeship</b> lists its counties: the county references whose code starts with the voivodeship's. The parts of a multi-part county are merged by code into one entry, with the lowest part identifier - a solve resolves every part from the code anyway. The count is the exact <c>countbycountyid</c> sum over the parts, one request per part, sequential. A county whose count cannot be read is still listed, with a null count.</para>
        /// <para>The <b>country</b> lists its voivodeships, merged by code the same way and uncounted: each one holds hundreds of thousands of buildings, above the solve ceiling, so only its own county list can be opened.</para>
        /// <para>Entries are sorted by name under Polish collation, so <c>Łódzkie</c> follows <c>Lubuskie</c> rather than ending the list.</para>
        /// </summary>
        /// <param name="httpClient">The HTTP client used for the requests. This value can be null.</param>
        /// <param name="code">The code of the area; required for a voivodeship. This value can be null.</param>
        /// <param name="administrativeArealType">The type of the area; <see cref="AdministrativeArealType.Voivodeship"/> and <see cref="AdministrativeArealType.Country"/> have children to list.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by the caller to cancel the asynchronous operation.</param>
        /// <returns>The child areas, or <see langword="null"/> when the type has no children to list here, the code is missing for a voivodeship, or the upstream answered nothing.</returns>
        public static async Task<List<TypologyChildAreaViewModel>?> ChildAreasAsync(this HttpClient? httpClient, string? code, AdministrativeArealType administrativeArealType, CancellationToken cancellationToken = default)
        {
            if (httpClient is null)
            {
                return null;
            }

            AdministrativeArealType administrativeArealType_Child;
            if (administrativeArealType == AdministrativeArealType.Voivodeship)
            {
                if (string.IsNullOrWhiteSpace(code))
                {
                    return null;
                }

                administrativeArealType_Child = AdministrativeArealType.County;
            }
            else if (administrativeArealType == AdministrativeArealType.Country)
            {
                administrativeArealType_Child = AdministrativeArealType.Voivodeship;
            }
            else
            {
                return null;
            }

            UrlBuilder urlBuilder = new($"{Constants.Default.GISWebAPIUri}/gis/administrativeareal2D/administrativeareal2Dreferencesbyadministrativearealtype");
            urlBuilder = urlBuilder.AddParameter("administrativearealtype", (int)administrativeArealType_Child);

            List<AdministrativeAreal2DReference>? administrativeAreal2DReferences = await httpClient.ItemsAsync<AdministrativeAreal2DReference>(urlBuilder.ToString(), cancellationToken);
            if (administrativeAreal2DReferences is null)
            {
                return null;
            }

            // One entry per code: a multi-part area is one row per polygon part (Coding - GIS Administrative Data, section 1).
            SortedDictionary<string, List<AdministrativeAreal2DReference>> parts_ByCode = new(StringComparer.Ordinal);
            foreach (AdministrativeAreal2DReference administrativeAreal2DReference in administrativeAreal2DReferences)
            {
                string? code_Child = administrativeAreal2DReference?.Code;
                if (administrativeAreal2DReference is null || string.IsNullOrWhiteSpace(code_Child) || administrativeAreal2DReference.Id <= 0)
                {
                    continue;
                }

                if (administrativeArealType == AdministrativeArealType.Voivodeship && !code_Child.StartsWith(code!, StringComparison.Ordinal))
                {
                    continue;
                }

                if (!parts_ByCode.TryGetValue(code_Child, out List<AdministrativeAreal2DReference>? parts))
                {
                    parts = [];
                    parts_ByCode[code_Child] = parts;
                }

                parts.Add(administrativeAreal2DReference);
            }

            List<TypologyChildAreaViewModel> result = new(parts_ByCode.Count);
            foreach (KeyValuePair<string, List<AdministrativeAreal2DReference>> keyValuePair in parts_ByCode)
            {
                List<int> ids = [.. keyValuePair.Value.Select(x => x.Id).Distinct().Order()];
                string? name = keyValuePair.Value.Select(x => x.Name).FirstOrDefault(x => !string.IsNullOrWhiteSpace(x));

                long? count = null;
                if (administrativeArealType_Child == AdministrativeArealType.County)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    count = await httpClient.BuildingDataCountAsync(ids, cancellationToken);
                }

                result.Add(new TypologyChildAreaViewModel(ids[0], keyValuePair.Key, name, (int)administrativeArealType_Child, count));
            }

            StringComparer stringComparer = StringComparer.Create(CultureInfo.GetCultureInfo("pl-PL"), true);
            result.Sort((x, y) => stringComparer.Compare(x.Name ?? x.Code, y.Name ?? y.Code));

            return result;
        }
    }
}
