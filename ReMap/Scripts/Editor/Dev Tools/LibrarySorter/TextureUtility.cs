using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace DevLibrarySorter
{
    public class TextureUtility
    {
        public static void DeleteNotUsedTexture()
        {
            List<string> texturesList = new List<string>();

            string[] modeltextureGUID = AssetDatabase.FindAssets("t:model", new [] {"Assets/ReMap/Lods - Dont use these/Models"});

            foreach (var guid in modeltextureGUID)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                string[] dependencie = AssetDatabase.GetDependencies(assetPath);
                foreach( string dependencies in dependencie )
                {
                    string fileName = Path.GetFileNameWithoutExtension(dependencies);
                    if ( Path.GetExtension(dependencies) == ".dds" && !texturesList.Contains(fileName))
                    {
                        texturesList.Add(fileName);
                    }
                }
            }

            string[] usedTextures = texturesList.ToArray();

            string[] defaultAssetGUID = AssetDatabase.FindAssets("t:defaultAsset", new [] {"Assets/ReMap/Lods - Dont use these/Materials"});
            int j = 0;
            foreach (var guid in defaultAssetGUID)
            {
                string defaultAssetPath = AssetDatabase.GUIDToAssetPath(guid);

                if ( Path.GetExtension(defaultAssetPath) == ".dds")
                {
                    File.Delete(defaultAssetPath);
                    File.Delete(defaultAssetPath + ".meta");
                    j++;
                }
            }

            string[] textureGUID = AssetDatabase.FindAssets("t:texture", new [] {"Assets/ReMap/Lods - Dont use these/Materials"});
            int i = 0;
            foreach (var guid in textureGUID)
            {
                string texturePath = AssetDatabase.GUIDToAssetPath(guid);

                if( !usedTextures.Contains(Path.GetFileNameWithoutExtension(texturePath)) )
                {
                    File.Delete(texturePath);
                    File.Delete(texturePath + ".meta");
                    i++;
                }
            }

            ReMapConsole.Log($"{i} textures not used have been deleted", ReMapConsole.LogType.Success);
            ReMapConsole.Log($"{j} native assets have been deleted", ReMapConsole.LogType.Success);
            ReMapConsole.Log($"Total used textures: {usedTextures.Length} for {modeltextureGUID.Length} models", ReMapConsole.LogType.Info);
        }
    }
}