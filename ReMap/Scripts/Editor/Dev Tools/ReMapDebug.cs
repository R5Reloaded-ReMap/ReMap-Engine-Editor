using System.IO;
using CodeViews;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

public class ReMapDebug
{
    internal static string outputFolder = $"{UnityInfo.currentDirectoryPath}/{UnityInfo.relativeRMAPDEVfolder}";
    internal static string output = $"{outputFolder}/WriteFile.txt";

    public static async void Debug_FileWrite()
    {
        if ( !Directory.Exists( outputFolder ) ) Directory.CreateDirectory( outputFolder );

        if ( !File.Exists( output ) ) File.Create( output );

        string file = await LiveMap.BuildScriptFile();

        File.WriteAllText( output, file );
    }

    public static void Debug_ClearProgressBar()
    {
        EditorUtility.ClearProgressBar();
    }

    public static void DeleteObjectsWithSpecificMaterial()
    {
        // Define the material name
        string[] materialNames =
        {
            "toolstrigger", "toolsskybox", "toolsfogvolume",
            "toolsblack", "toolsclip", "toolsblocklight"
        };

        // Iterate over root objects and their descendants
        foreach ( var obj in UnityInfo.GetAllGameObjectInScene() )
        {
            var childs = obj.GetComponentsInChildren< Transform >( true );
            int min = 0;
            int max = childs.Length;
            float progress = 0.0f;
            foreach ( var child in childs )
            {
                EditorUtility.DisplayProgressBar( "Tools Trigger Remover", $"Processing... ({min++}/{max})", progress );

                // Access the MeshRenderer component in the child GameObject
                var renderer = child.GetComponent< MeshRenderer >();

                progress += 1.0f / max;

                // Continue if there is no MeshRenderer component
                if ( renderer == null ) continue;

                // Get the materials of the MeshRenderer
                var materials = renderer.sharedMaterials;

                // Check if the first material is the one we are looking for
                foreach ( string materialName in materialNames )
                    if ( !Helper.IsEmpty( materials ) && materials[0].name == materialName )
                    {
                        // If so, destroy the GameObject
                        Object.DestroyImmediate( child.gameObject );
                        // Exit the loop since the GameObject no longer exists
                        break;
                    }
            }
        }

        EditorUtility.ClearProgressBar();
    }

    public static void DeleteMultipleFormats()
    {
        foreach ( string file in Directory.GetFiles( $"{UnityInfo.currentDirectoryPath}/{UnityInfo.relativePathMaterials}" ) )
            if ( file.Contains( ".dds.dds" ) )
                File.Delete( file );
    }

    public static async void ResizeAllTextures()
    {
        await TextureConverter.ResizeTextures();
    }
}