using DiGi.Analytical.Building.Classes;
using DiGi.GIS.WebAPI.UI.Enums;
using System;
using System.Collections.Generic;

namespace DiGi.GIS.WebAPI.UI.Classes
{
    /// <summary>
    /// A background solar radiation job: the request it answers, the calculation prepared for it, its state and, once calculated, its results.
    /// <para>Held in memory by a <see cref="SolarJobQueue"/> and never persisted. Its state changes only through <c>Modify.TryEnqueue</c>, <c>Modify.Cancel</c> and <c>Modify.SolveAsync</c>, under the lock of the queue; read it through <c>Query.SolarJob</c> or <c>Create.SolarJobViewModel</c>.</para>
    /// </summary>
    public class SolarJob
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SolarJob"/> class in the <see cref="SolarJobStatus.Queued"/> state.
        /// </summary>
        /// <param name="id">The unique identifier of the job.</param>
        /// <param name="buildingModelId">The unique identifier of the building.</param>
        /// <param name="countyId">The optional unique identifier of the county associated with the building.</param>
        /// <param name="radius">The neighbour radius in metres.</param>
        /// <param name="receiverCount">The number of receiving surfaces (external walls and roofs) of the building.</param>
        /// <param name="buildingModel">The building, kept to build the coloured scene from the results.</param>
        /// <param name="buildingModels_Surrounding">The neighbours, kept to build the coloured scene from the results.</param>
        /// <param name="calculation">The calculation, prepared with every input it needs; it is released once it has run.</param>
        public SolarJob(
            Guid id,
            long buildingModelId,
            int? countyId,
            double radius,
            int receiverCount,
            BuildingModel? buildingModel,
            List<BuildingModel>? buildingModels_Surrounding,
            Func<List<SurfaceSolarRadiationResult>?>? calculation)
        {
            Id = id;
            BuildingModelId = buildingModelId;
            CountyId = countyId;
            Radius = radius;
            ReceiverCount = receiverCount;
            BuildingModel = buildingModel;
            BuildingModels_Surrounding = buildingModels_Surrounding;
            Calculation = calculation;
        }

        /// <summary> Gets the building, kept to build the coloured scene from the results; null once the job failed or was cancelled. </summary>
        public BuildingModel? BuildingModel { get; internal set; }

        /// <summary> Gets the unique identifier of the building. </summary>
        public long BuildingModelId { get; }

        /// <summary> Gets the neighbours, kept to build the coloured scene from the results; null once the job failed or was cancelled. </summary>
        public List<BuildingModel>? BuildingModels_Surrounding { get; internal set; }

        /// <summary> Gets the prepared calculation, or null once it has run or the job was cancelled. </summary>
        public Func<List<SurfaceSolarRadiationResult>?>? Calculation { get; internal set; }

        /// <summary> Gets the optional unique identifier of the county associated with the building. </summary>
        public int? CountyId { get; }

        /// <summary> Gets the moment the job was queued. </summary>
        public DateTimeOffset CreatedAt { get; internal set; }

        /// <summary> Gets the error text of a failed job. </summary>
        public string? Error { get; internal set; }

        /// <summary> Gets the moment the job completed, failed or was cancelled. </summary>
        public DateTimeOffset? FinishedAt { get; internal set; }

        /// <summary> Gets the unique identifier of the job. </summary>
        public Guid Id { get; }

        /// <summary> Gets the order in which the job was queued, from 1. </summary>
        public long Index { get; internal set; }

        /// <summary> Gets the neighbour radius in metres. </summary>
        public double Radius { get; }

        /// <summary> Gets the number of receiving surfaces (external walls and roofs) of the building. </summary>
        public int ReceiverCount { get; }

        /// <summary> Gets the moment the calculation started. </summary>
        public DateTimeOffset? StartedAt { get; internal set; }

        /// <summary> Gets the state of the job. </summary>
        public SolarJobStatus Status { get; internal set; } = SolarJobStatus.Queued;

        /// <summary> Gets the results of a completed job. </summary>
        public List<SurfaceSolarRadiationResult>? SurfaceSolarRadiationResults { get; internal set; }
    }
}
