
using System.IO;
using UnityEngine;
using UnityEditor;

namespace Sandbox
{
    public class FixPrefabs
    {
        [MenuItem("ReMap/Sandbox/Fix Prefabs", false, 100)]
        public static void FixPrefabsInit()
        {
            FixPrefabsInScene();
        }

        internal static void FixPrefabsInScene()
        {
            foreach ( GameObject go in Helper.GetAllObjectTypeInScene() )
            {
                GameObject origin = PrefabUtility.GetCorrespondingObjectFromSource( go ) as GameObject;
                string filePath = $"{UnityInfo.currentDirectoryPath}/{UnityInfo.relativePathPrefabs}/all_models/{UnityInfo.GetUnityModelName( go, true )}";

                if ( !Helper.IsValid( origin ) && File.Exists( filePath ) )
                {
                    PrefabUtility.SaveAsPrefabAssetAndConnect( go, filePath, InteractionMode.UserAction );
                }
            }
        }
    }
}
