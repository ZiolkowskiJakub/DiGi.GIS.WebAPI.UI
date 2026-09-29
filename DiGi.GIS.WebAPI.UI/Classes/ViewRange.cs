namespace DiGi.GIS.WebAPI.UI.Classes
{
    /// <summary>
    /// The bounds of the 3D viewer "View range" slider, in metres, bound to a scene: objects further from the scene center than the current value are not rendered.
    /// <para>Built by <see cref="Create.ViewRange(double?)"/> and <see cref="Create.BuildingViewRange(double?)"/>, which validate the values; the constructor only assigns them.</para>
    /// </summary>
    public class ViewRange
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ViewRange"/> class.
        /// </summary>
        /// <param name="minimum">The smallest view range, in metres.</param>
        /// <param name="maximum">The largest view range, in metres.</param>
        /// <param name="defaultValue">The view range the slider starts at, in metres.</param>
        public ViewRange(double minimum, double maximum, double defaultValue)
        {
            Minimum = minimum;
            Maximum = maximum;
            Default = defaultValue;
        }

        /// <summary> Gets the view range the slider starts at, in metres. </summary>
        public double Default { get; }

        /// <summary> Gets the largest view range, in metres. </summary>
        public double Maximum { get; }

        /// <summary> Gets the smallest view range, in metres. </summary>
        public double Minimum { get; }
    }
}
