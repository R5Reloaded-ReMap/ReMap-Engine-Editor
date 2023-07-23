
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

public class ReMapDebug
{
    internal static string outputFolder = $"{UnityInfo.currentDirectoryPath}/{UnityInfo.relativeRMAPDEVfolder}";
    internal static string output = $"{outputFolder}/WriteFile.txt";

    //#if ReMapDev
        [ MenuItem( "ReMap Dev Tools/File Write Test", false, 100 ) ]
        public static async void Debug_FileWrite()
        {
            if ( !Directory.Exists( outputFolder ) ) Directory.CreateDirectory( outputFolder );

            if ( !File.Exists( output ) ) File.Create( output );

            string file = await CodeViews.LiveMap.BuildScriptFile();

            File.WriteAllText( output, file );
        }

        [ MenuItem( "ReMap Dev Tools/Clear Progress Bar", false, 100 ) ]
        public static void Debug_ClearProgressBar()
        {
            EditorUtility.ClearProgressBar();
        }

        [MenuItem( "ReMap Dev Tools/Remove Tools Material" )]
        private static void DeleteObjectsWithSpecificMaterial()
        {
            // Define the material name
            string[] materialNames = new[]
            {
                "toolstrigger", "toolsskybox", "toolsfogvolume",
                "toolsblack", "toolsclip", "toolsblocklight"
            };

            // Iterate over root objects and their descendants
            foreach ( GameObject obj in UnityInfo.GetAllGameObjectInScene() )
            {
                Transform[] childs = obj.GetComponentsInChildren< Transform >( true );
                int min = 0; int max = childs.Length; float progress = 0.0f;
                foreach ( Transform child in childs )
                {
                    EditorUtility.DisplayProgressBar( $"Tools Trigger Remover", $"Processing... ({min++}/{max})", progress );

                    // Access the MeshRenderer component in the child GameObject
                    MeshRenderer renderer = child.GetComponent< MeshRenderer >();

                    progress += 1.0f / max;

                    // Continue if there is no MeshRenderer component
                    if ( renderer == null ) continue;

                    // Get the materials of the MeshRenderer
                    Material[] materials = renderer.sharedMaterials;

                    // Check if the first material is the one we are looking for
                    foreach ( string materialName in materialNames )
                    {
                        if ( !Helper.IsEmpty( materials ) && materials[0].name == materialName )
                        {
                            // If so, destroy the GameObject
                            GameObject.DestroyImmediate( child.gameObject );
                            // Exit the loop since the GameObject no longer exists
                            break;
                        }
                    }
                }
            }

            EditorUtility.ClearProgressBar();
        }

        [MenuItem( "ReMap Dev Tools/Add New Material" )]
        private static void AddNewMaterial()
        {
            foreach ( GameObject obj in UnityInfo.GetAllGameObjectInScene() )
            {
                Transform[] childs = obj.GetComponentsInChildren< Transform >( true );
                int min = 0; int max = childs.Length; float progress = 0.0f;
                foreach ( Transform child in childs )
                {
                    EditorUtility.DisplayProgressBar( $"Adding New Materials", $"Processing... ({min++}/{max})", progress );

                    // Access the MeshRenderer component in the child GameObject
                    MeshRenderer renderer = child.GetComponent< MeshRenderer >();

                    progress += 1.0f / max;

                    // Continue if there is no MeshRenderer component
                    if ( renderer == null ) continue;

                    // Get the materials of the MeshRenderer
                    Material[] materials = renderer.sharedMaterials;

                    if ( !Helper.IsEmpty( materials ) )
                    {
                        //string[] guids = AssetDatabase.FindAssets( materials[0].name, new[] {$"{UnityInfo.relativePathPrefabs}/all_models"} );

                        //if ( !Helper.IsEmpty( guids ) )
                        //{
                        //    //string assetPath = AssetDatabase.GUIDToAssetPath( guids[0] );
                        //    //materials[0] = AssetDatabase.LoadAssetAtPath< Material >( assetPath );
                        //}
                    }
                }
            }

            EditorUtility.ClearProgressBar();
        }
    //#endif
}
