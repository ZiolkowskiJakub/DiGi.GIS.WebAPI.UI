namespace DiGi.GIS.WebAPI.UI.ViewModels
{
    /// <summary>
    /// The pre-flight answer of <c>GET /typology/buildingcount</c>: how many buildings a Typology solve of an area reads, known before the solve starts, so the area view can say what it is waiting for (DiGi.GIS.WebAPI.UI#29, B1).
    /// </summary>
    public class TypologyAreaCountViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TypologyAreaCountViewModel"/> class.
        /// </summary>
        public TypologyAreaCountViewModel() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="TypologyAreaCountViewModel"/> class.
        /// </summary>
        /// <param name="count">The number of buildings the solve reads, or null when it is not counted.</param>
        /// <param name="countyPartCount">The number of county parts the solve reads.</param>
        /// <param name="ceiling">The number of buildings above which a solve is refused.</param>
        /// <param name="clipped">A value indicating whether the solve clips its county parts to the area's polygon.</param>
        public TypologyAreaCountViewModel(long? count, int countyPartCount, int ceiling, bool clipped)
        {
            Count = count;
            CountyPartCount = countyPartCount;
            Ceiling = ceiling;
            Clipped = clipped;
        }

        /// <summary>
        /// Gets the number of buildings the solve reads: the sum over the area's county parts. For a clipped area it is the count of the county parts the area lies in, an upper bound of the area's own buildings. Null for a country, which is not counted - it is refused as above the ceiling outright.
        /// </summary>
        public long? Count { get; }

        /// <summary>
        /// Gets the number of county parts the solve reads.
        /// </summary>
        public int CountyPartCount { get; }

        /// <summary>
        /// Gets the number of buildings above which a solve is refused with a 413 (<see cref="Constants.Default.BuildingSolveCeiling"/>).
        /// </summary>
        public int Ceiling { get; }

        /// <summary>
        /// Gets a value indicating whether the area is a municipality or subdivision, whose county parts are clipped to its polygon after the read - so <see cref="Count"/> is an upper bound ("up to") rather than the area's count.
        /// </summary>
        public bool Clipped { get; }
    }
}
