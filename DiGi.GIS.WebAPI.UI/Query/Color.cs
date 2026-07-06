using DiGi.Analytical.Building.Interfaces;
using DiGi.Core.Interfaces;
using DiGi.GIS.Classes;

namespace DiGi.GIS.WebAPI.UI
{
    public static partial class Query
    {
        /// <summary>
        /// Gets the default display <see cref="Core.Classes.Color"/> for the specified domain object.
        /// </summary>
        /// <param name="serializableObject">The domain object to be styled. This value can be null.</param>
        /// <returns>A <see cref="Core.Classes.Color"/> representing the default styling of the object, or null if no default styling is defined.</returns>
        public static Core.Classes.Color? Color(this ISerializableObject? serializableObject)
        {
            switch (serializableObject)
            {
                case IWindow:
                    return new Core.Classes.Color(byte.MaxValue, 135, 206, 235);

                case IDoor:
                    return new Core.Classes.Color(byte.MaxValue, 139, 90, 43);

                case IRoof:
                    return new Core.Classes.Color(byte.MaxValue, 178, 34, 34);

                case IWall:
                    return new Core.Classes.Color(byte.MaxValue, 235, 230, 220);

                case IFloor:
                    return new Core.Classes.Color(byte.MaxValue, 128, 128, 128);

                case IShade:
                    return new Core.Classes.Color(byte.MaxValue, 105, 105, 105);

                case Building2D:
                    return new Core.Classes.Color(byte.MaxValue, 222, 184, 135);

                default:
                    return null;
            }
        }
    }
}
