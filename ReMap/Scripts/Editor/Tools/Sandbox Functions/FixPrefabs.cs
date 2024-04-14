using System.IO;
using UnityEditor;

namespace Sandbox
{
    public class FixPrefabs
    {
        public static void FixPrefabsInit()
        {
            FixPrefabsInScene();
        }

        internal static void FixPrefabsInScene()
        {
            foreach ( var go in Helper.GetAllObjectTypeInScene() )
            {
                var origin = PrefabUtility.GetCorrespondingObjectFromSource( go );
                string filePath = $"{UnityInfo.currentDirectoryPath}/{UnityInfo.relativePathPrefabs}/all_models/{UnityInfo.GetUnityModelName( go, true )}";

                if ( !Helper.IsValid( origin ) && File.Exists( filePath ) )
                    PrefabUtility.SaveAsPrefabAssetAndConnect( go, filePath, InteractionMode.UserAction );
            }
        }
    }
}