
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace LibrarySorter
{
    public class MaterialWindowSelector : EditorWindow
    {
        private static MaterialWindowSelector windowInstance;
        private static Dictionary< string, List< Texture2D > > materialList = new Dictionary< string, List< Texture2D > >();
        private static Vector2 scroll = Vector2.zero;
        private static int scale = 256;

        public static void AddNewMaterialSelection( string directoryPath )
        {
            if ( !Helper.IsValid( materialList ) )
            {
                materialList = new Dictionary< string, List< Texture2D > >();
            }

            if ( !materialList.ContainsKey( directoryPath ) )
            {
                materialList.Add( directoryPath, GetTexturesInPath( directoryPath ) );
            }

            if ( !Helper.IsValid( windowInstance ) )
            {
                windowInstance = GetWindow< MaterialWindowSelector >( "Material Selector Window" );
                windowInstance.minSize = new Vector2( 1060, 800 );
                windowInstance.maxSize = new Vector2( 1060, 800 );
            }
        }

        private static List< Texture2D > GetTexturesInPath( string path )
        {
            List< Texture2D > list = new List< Texture2D >();

            var info = new DirectoryInfo( path );
            var fileInfo = info.GetFiles();

            foreach ( var file in fileInfo )
            {
                if ( file.Extension.ToLower() != ".dds" )
                    continue;

                var assetPath = $"{Materials.GetAssetPath( path )}/{file.Name}";
                Texture2D texture = AssetDatabase.LoadAssetAtPath< Texture2D >( assetPath );

                if ( !Helper.IsValid( texture ) )
                    continue;

                list.Add( texture );
            }

            return list;
        }

        private void OnGUI()
        {
            int remainingTexture = materialList.Count;

            GUILayout.BeginVertical();

            WindowUtility.WindowUtility.CreateTextInfoCentered( $"Model Queued: {remainingTexture}" );

            WindowUtility.WindowUtility.GetEditorWindowSize( windowInstance );

            scroll = EditorGUILayout.BeginScrollView( scroll );

            GUIStyle buttonStyle = new GUIStyle( GUI.skin.button );
            buttonStyle.alignment = TextAnchor.MiddleCenter;

            int loopLimit = Math.Min( remainingTexture, 10 ); // Show only 10 maximum textures settings for performances

            for ( int i = 0; i < loopLimit; i++ )
            {
                string path = materialList.Keys.ElementAt( i );

                string textureName = Path.GetFileNameWithoutExtension( path );

                int idx = 0;
                GUILayout.BeginVertical( "box" );

                WindowUtility.WindowUtility.CreateTextInfoCentered( $"{textureName}" );

                GUILayout.BeginHorizontal();

                foreach ( Texture2D texture in materialList[path] )
                {
                    if ( idx % 4 == 0 )
                    {
                        GUILayout.EndHorizontal();
                        GUILayout.BeginHorizontal();
                    }

                    GUIContent buttonContent = new GUIContent( texture );

                    if ( GUILayout.Button( buttonContent, GUILayout.Width( scale ), GUILayout.Height( scale ) ) )
                    {
                        if ( Event.current.button == 1 ) // Right click
                        {
                            MaterialManagerPreview.ShowPreview( texture );
                        }
                        else if ( Event.current.button == 0 ) // Left click
                        {
                            string filePath = $"{UnityInfo.currentDirectoryPath}/{AssetDatabase.GetAssetPath( texture )}";
                            string fileName = Path.GetFileName( filePath );

                            // #TOFIX
                            if ( Helper.MoveFile( filePath, $"{Materials.materialDirectory}/{fileName}", false ) )
                            {
                                Materials.MaterialData.RemoveMaterial( textureName );

                                Materials.MaterialData.Add
                                (
                                    new MaterialClass()
                                    {
                                        Name = textureName,
                                        Path = Materials.GetAssetPath( $"{Materials.materialDirectory}/{fileName}" )
                                    }
                                );

                                Materials.SaveMaterialData();

                                GUILayout.EndHorizontal();
                                GUILayout.EndVertical();
                                EditorGUILayout.EndScrollView();
                                GUILayout.EndVertical();

                                materialList.Remove( path );

                                MaterialManagerWindow.Refresh();

                                //Helper.DeleteDirectory( $"{Path.GetDirectoryName( path ).Replace( "\\", "/" )}/{textureName}" );

                                return;
                            }
                        }
                    }

                    idx++;
                }

                GUILayout.EndHorizontal();

                if ( GUILayout.Button( "Ignore Texture", buttonStyle ) )
                {
                    materialList.Remove( path );
                }

                GUILayout.EndVertical();
            }

            EditorGUILayout.EndScrollView();

            GUILayout.EndVertical();
        }
    }


    public class MaterialManagerWindow : EditorWindow
    {
        static MaterialData materialData = Materials.GetMaterialData();
        static Dictionary< string, Texture2D > texturesDictionary = new Dictionary< string, Texture2D >();
        static Vector2 scroll = Vector2.zero;
        
        static int scale = 256;
        static int page = 0;
        static int itemsPerPage = 20;
        static string search = "";

        public static void Init()
        {
            GetWindow< MaterialManagerWindow >( "Material Manager" );
        }

        void OnEnable()
        {
            Refresh();
        }

        private void OnGUI()
        {
            GUILayout.BeginHorizontal( "box" );

            WindowUtility.WindowUtility.CreateTextField( ref search, 0, 20 );
            WindowUtility.WindowUtility.CreateButton( "Page-", "", () => { if ( page > 0 ) page--; }, 100, 20 );
            WindowUtility.WindowUtility.CreateButton( "Page+", "", () => { if ( ( page * itemsPerPage ) < texturesDictionary.Count ) page++; }, 100, 20 );

            GUILayout.EndHorizontal();

            scroll = EditorGUILayout.BeginScrollView( scroll );

            GUILayout.BeginVertical();

            GUIStyle buttonStyle = new GUIStyle( GUI.skin.button );
            buttonStyle.alignment = TextAnchor.MiddleCenter;

            int loopLimit = Math.Min( texturesDictionary.Count, ( page * itemsPerPage ) + itemsPerPage );

            for ( int i = page * itemsPerPage; i < loopLimit; i++ )
            {
                var texture = texturesDictionary.ElementAt( i );

                GUIContent buttonContent = new GUIContent( texture.Value );

                GUILayout.BeginVertical( "box" );

                string path = AssetDatabase.GetAssetPath( texture.Value );

                WindowUtility.WindowUtility.CreateTextInfoCentered( $"{texture.Key} => {Path.GetFileNameWithoutExtension( path )}", "", 0, 20 );

                GUILayout.BeginHorizontal();

                GUILayout.BeginVertical();

                if ( GUILayout.Button( $"Remove {texture.Key}", buttonStyle, GUILayout.Height( scale / 2 ) ) )
                {
                    Materials.MaterialData.RemoveMaterial( texture.Key );
                    Materials.SaveMaterialData();
                    Refresh();

                    Helper.DeleteFile( path );

                    GUILayout.EndVertical();
                    GUILayout.EndVertical();
                    GUILayout.EndHorizontal();
                    EditorGUILayout.EndScrollView();
                    GUILayout.EndVertical();
                    return;
                }

                if ( GUILayout.Button( $"ReImport {texture.Key}", buttonStyle, GUILayout.Height( scale / 2 ) ) )
                {
                    ReImportMaterial( texture.Key );
                }

                GUILayout.EndVertical();

                if ( GUILayout.Button( buttonContent, GUILayout.Width( scale ), GUILayout.Height( scale ) ) )
                {
                    MaterialManagerPreview.ShowPreview( texture.Value );
                }

                GUILayout.EndVertical();

                GUILayout.EndHorizontal();
            }

            EditorGUILayout.EndScrollView();

            GUILayout.EndVertical();
        }

        private static async void ReImportMaterial( string materialName )
        {
            if ( LegionExporting.GetValidRpakPaths() )
            {
                Helper.Ping( "No valid path for LegionPlus." );
                return;
            }

            List< string > singleMaterialList = new List< string > { materialName };

            EditorUtility.DisplayProgressBar( $"Material Manager", $"Try Exporting \"{materialName}\"...", 0.0f );

            await LegionExporting.ExtractModelFromLegion( materialName );

            EditorUtility.ClearProgressBar();

            Materials.AppendNewTexture( singleMaterialList, true );
        }

        internal static void Refresh()
        {
            texturesDictionary = LoadTextures();
        }

        private static Dictionary< string, Texture2D > LoadTextures()
        {
            Dictionary< string, Texture2D > textureToLoad = new Dictionary< string, Texture2D >();

            Materials.MaterialData = Materials.GetMaterialData();

            int min = 0; int max = Materials.MaterialData.MaterialList.Count; float progress = 0.0f;

            foreach ( MaterialClass material in Materials.MaterialData.MaterialList )
            {
                EditorUtility.DisplayProgressBar( $"Material Manager ({min++}/{max})", $"Processing... {material.Name}", progress );

                Texture2D texture = AssetDatabase.LoadAssetAtPath< Texture2D >( material.Path );

                progress += 1.0f / max;

                if ( !Helper.IsValid( texture ) || textureToLoad.ContainsKey( material.Name ) )
                    continue;

                textureToLoad.Add( material.Name, texture );
            }

            EditorUtility.ClearProgressBar();

            return textureToLoad;
        }
    }

    public class MaterialManagerPreview : EditorWindow
    {
        public static MaterialManagerPreview window;
        public static Texture2D texturePreview;

        public static void ShowPreview( Texture2D texture )
        {
            window = GetWindow< MaterialManagerPreview >( "Material Preview" );

            texturePreview = texture;

            // Set the max size of the window based on the texture size
            window.maxSize = new Vector2( texture.width, texture.height );
        }

        private void OnGUI()
        {
            if( Helper.IsValid( texturePreview ) )
            {
                // Create a GUIStyle that aligns content in the center
                GUIStyle centeredStyle = GUI.skin.GetStyle( "Label" );
                centeredStyle.alignment = TextAnchor.MiddleCenter;

                // Get the window size
                Vector2 windowSize = new Vector2( position.width, position.height );
            
                // Create a GUILayoutOption array with window size
                GUILayoutOption[] options = new GUILayoutOption[]
                {
                    GUILayout.Width( windowSize.x ),
                    GUILayout.Height( windowSize.y )
                };

                GUILayout.BeginHorizontal();
                    GUILayout.FlexibleSpace();
                    // Draw the texture using the centeredStyle and options
                    GUILayout.Label( texturePreview, centeredStyle, options );
                    GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
            }
        }
    }
}
