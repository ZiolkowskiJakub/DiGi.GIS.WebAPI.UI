using DiGi.GLTF.Classes;

namespace DiGi.GIS.WebAPI.UI.ViewModels
{
    /// <summary>
    /// Represents a view model for rendering a <see cref="GLTFScene"/> in the 3D glTF viewer.
    /// <para>Two delivery modes are supported: streamed (the view carries only <see cref="GLBUrl"/> and the viewer fetches the binary glTF payload, whose scene extras are fully self-describing) and embedded (the scene JSON and the base64 encoded payload are inlined in the page).</para>
    /// </summary>
    public class GLTFSceneViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GLTFSceneViewModel"/> class.
        /// </summary>
        public GLTFSceneViewModel()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GLTFSceneViewModel"/> class for streamed delivery: the viewer fetches the binary glTF payload from <paramref name="gLBUrl"/> and reads the scene configuration from its extras.
        /// </summary>
        /// <param name="title">The title displayed above the viewer.</param>
        /// <param name="gLBUrl">The application relative URL of the binary glTF (.glb) endpoint.</param>
        public GLTFSceneViewModel(string? title, string? gLBUrl)
        {
            Title = title;
            GLBUrl = gLBUrl;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GLTFSceneViewModel"/> class for embedded delivery.
        /// </summary>
        /// <param name="gLTFScene">The <see cref="GLTFScene"/> to be rendered.</param>
        /// <param name="gLTFSceneJson">The JSON representation of the scene used by the viewer for lights, camera and reference point configuration.</param>
        /// <param name="gLBBase64">The base64 encoded binary glTF (.glb) payload rendered by the viewer.</param>
        /// <param name="title">The title displayed above the viewer.</param>
        public GLTFSceneViewModel(GLTFScene? gLTFScene, string? gLTFSceneJson, string? gLBBase64, string? title)
        {
            GLTFScene = gLTFScene;
            GLTFSceneJson = gLTFSceneJson;
            GLBBase64 = gLBBase64;
            Title = title;
        }

        /// <summary> Gets the base64 encoded binary glTF (.glb) payload rendered by the viewer (embedded delivery only). </summary>
        public string? GLBBase64 { get; }

        /// <summary> Gets the application relative URL of the binary glTF (.glb) endpoint (streamed delivery only). </summary>
        public string? GLBUrl { get; }

        /// <summary> Gets the <see cref="GLTFScene"/> to be rendered (embedded delivery only). </summary>
        public GLTFScene? GLTFScene { get; }

        /// <summary> Gets the JSON representation of the scene used by the viewer for lights, camera and reference point configuration (embedded delivery only). </summary>
        public string? GLTFSceneJson { get; }

        /// <summary> Gets the title displayed above the viewer. </summary>
        public string? Title { get; }
    }
}