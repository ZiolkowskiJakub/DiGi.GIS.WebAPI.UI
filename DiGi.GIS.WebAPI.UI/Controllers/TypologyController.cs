using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using DiGi.GIS.WebAPI.UI.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace DiGi.GIS.WebAPI.UI.Controllers
{
    /// <summary>
    /// Provides the Typology definition feature: the page where a building typology is defined as a chain of building-data columns with rule types, ranges and colors.
    /// <para>The column data and the solving are owned by the GIS Web API (ZiolkowskiJakub/DiGi.Gis#5); this controller only reads and renders, so the query and rule semantics stay owned by that service and cannot drift here.</para>
    /// </summary>
    [Route("[controller]")]
    public class TypologyController : Controller
    {
        private readonly IHttpClientFactory httpClientFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="TypologyController"/> class.
        /// </summary>
        /// <param name="httpClientFactory">The <see cref="IHttpClientFactory"/> the feature's data actions use to create <see cref="HttpClient"/> instances.</param>
        public TypologyController(IHttpClientFactory httpClientFactory)
        {
            this.httpClientFactory = httpClientFactory;
        }

        // This action will trigger for: gis.digiproject.uk/typology
        /// <summary>
        /// Starts the Typology definition page.
        /// </summary>
        /// <returns>An <see cref="IActionResult"/> result that renders the start view.</returns>
        [HttpGet("")]
        public IActionResult Start()
        {
            return View();
        }

        /// <summary>
        /// Retrieves the building-data columns available for typology grouping, sorted alphabetically by name.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token that can be used by the caller to cancel the asynchronous operation.</param>
        /// <returns>A <see cref="Task{IActionResult}"/> containing the sorted column list, or a 204 No Content response when the upstream service answers nothing.</returns>
        [HttpGet("columns")]
        public async Task<IActionResult> GetColumnsAsync(CancellationToken cancellationToken = default)
        {
            HttpClient httpClient = httpClientFactory.CreateClient();

            List<DiGi.PostgreSQL.Table.Classes.Column>? columns = await httpClient.ItemsAsync<DiGi.PostgreSQL.Table.Classes.Column>(
                $"{Constants.Default.GISWebAPIUri}/gis/BuildingData/columns", cancellationToken);

            if (columns is null || columns.Count == 0)
            {
                return NoContent();
            }

            List<TypologyColumnViewModel> viewModels = columns
                .OrderBy(column => column.Name ?? "", StringComparer.OrdinalIgnoreCase)
                .Select(column => new TypologyColumnViewModel(column))
                .ToList();

            return Ok(viewModels);
        }
    }
}
