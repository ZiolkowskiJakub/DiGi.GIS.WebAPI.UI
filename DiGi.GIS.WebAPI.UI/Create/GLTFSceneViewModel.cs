using DiGi.Core;
using DiGi.GLTF;
using DiGi.GLTF.Classes;

namespace DiGi.GIS.WebAPI.UI
{
    public static partial class Create
    {
        /// <summary>
        /// Creates a <see cref="ViewModels.GLTFSceneViewModel"/> for the 3D viewer from the specified <see cref="GLTFScene"/> by serializing the scene to JSON and exporting it as a base64 encoded binary glTF (.glb) payload.
        /// </summary>
        /// <param name="gLTFScene">The <see cref="GLTFScene"/> to be rendered. This value can be null.</param>
        /// <param name="title">The title displayed above the viewer. If this value is null, the scene name is used.</param>
        /// <returns>A <see cref="ViewModels.GLTFSceneViewModel"/> ready to be passed to the glTF scene view, or null if the scene is null or could not be exported.</returns>
        public static ViewModels.GLTFSceneViewModel? GLTFSceneViewModel(this GLTFScene? gLTFScene, string? title = null)
        {
            if (gLTFScene is null)
            {
                return null;
            }

            // The page payload carries only the scene configuration (reference point, lights, camera):
            // geometry and object properties travel inside the batched binary glTF payload.
            GLTFScene gLTFScene_Configuration = new(gLTFScene.Name, gLTFScene.ReferencePoint, null, gLTFScene.Lights, gLTFScene.Camera);
            string? gLTFSceneJson = gLTFScene_Configuration.ToSystem_String();

            // Batched export: all objects merge into one draw unit per alpha mode with per-vertex
            // object ids, so thousands of buildings render with one or two WebGL draw calls.
            byte[]? bytes = gLTFScene.ToSystem_Bytes(true);
            if (bytes is null || bytes.Length == 0)
            {
                return null;
            }

            string gLBBase64 = System.Convert.ToBase64String(bytes);

            return new ViewModels.GLTFSceneViewModel(gLTFScene, gLTFSceneJson, gLBBase64, title ?? gLTFScene.Name);
        }
    }
}