
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace LibrarySorter
{
    public class Materials
    {
        MaterialData MaterialData;

        private static string MaterialDataJsonPath = $"{UnityInfo.currentDirectoryPath}/{UnityInfo.relativePathTextureDataList}";

        internal static MaterialData GetMaterialData()
        {
            MaterialData materialData;

            if ( !File.Exists( MaterialDataJsonPath ) )
            {
                materialData = new MaterialData();
                materialData.MaterialList = new List< MaterialClass >();
            }
            else
            {
                string jsonData = System.IO.File.ReadAllText( MaterialDataJsonPath );
                materialData = JsonUtility.FromJson< MaterialData >( jsonData );
            }

            return materialData;
        }

        internal static async Task CreateMaterialData()
        {
            MaterialData materialData = new MaterialData();
            materialData.MaterialList = new List< MaterialClass >();

            string[] modeltextureGUID = AssetDatabase.FindAssets( "t:model", new [] { UnityInfo.relativePathModel, UnityInfo.relativePathDevLods } );

            int min = 1; int max = modeltextureGUID.Length; float progress = 0.0f;
            
            foreach ( var guid in modeltextureGUID )
            {
                EditorUtility.DisplayProgressBar( $"MaterialData", $"Processing... ({min}/{max})", progress );

                string assetPath = AssetDatabase.GUIDToAssetPath( guid );
                GameObject obj = Helper.CreateGameObject( "", assetPath );

                foreach ( Renderer renderer in obj.GetComponentsInChildren< Renderer >() )
                {
                    if ( !Helper.IsValid( renderer ) )
                        continue;

                    foreach ( Material mat in renderer.sharedMaterials )
                    {
                        if ( !Helper.IsValid( mat ) || !mat.HasProperty( "_MainTex" ) )
                            continue;

                        string name = mat.name;

                        if ( materialData.MaterialList.Any( material => material.Name == name ) )
                            continue;

                        string texturePath = AssetDatabase.GetAssetPath( mat.mainTexture );

                        if ( !string.IsNullOrEmpty( texturePath ) )
                        {
                            materialData.MaterialList.Add
                            (
                                new MaterialClass()
                                {
                                    Name = name,
                                    Path = texturePath
                                }
                            );
                        }
                    }
                }

                UnityEngine.Object.DestroyImmediate( obj );

                progress += 1.0f / max; min++;
            }

            EditorUtility.ClearProgressBar();

            Helper.CreateDirectory( UnityInfo.relativePathTextureData );

            string jsonData = JsonUtility.ToJson( materialData );
            System.IO.File.WriteAllText( UnityInfo.relativePathTextureDataList, jsonData );
            
            await Helper.Wait();
        }
    }
}
