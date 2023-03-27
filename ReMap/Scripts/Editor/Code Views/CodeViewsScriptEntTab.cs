using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

using Build;
using static Build.Build;

namespace CodeViewsWindow
{
    public class ScriptEntTab
    {
        internal static void OnGUISettingsTab()
        {
            GUILayout.BeginVertical( "box" );

                CodeViewsWindow.ShowSettings = EditorGUILayout.Foldout( CodeViewsWindow.ShowSettings, "Options", true );

                if ( CodeViewsWindow.ShowSettings )
                {
                    GUILayout.BeginHorizontal();
                        CodeViewsWindow.ShowSquirrelEntFunction();
                    GUILayout.EndHorizontal();

                    GUILayout.Box( "", GUILayout.ExpandWidth( true ), GUILayout.Height( 2 ) );

                    GUILayout.BeginHorizontal();
                        CodeViewsWindow.OptionalUseOffset();
                        if ( Helper.UseStartingOffset ) CodeViewsWindow.OptionalOffsetField();
                        GUILayout.FlexibleSpace();
                    GUILayout.EndHorizontal();

                    GUILayout.Box( "", GUILayout.ExpandWidth( true ), GUILayout.Height( 2 ) );

                    GUILayout.BeginHorizontal();
                        CodeViewsWindow.OptionalSelection();
                    GUILayout.EndHorizontal();

                    GUILayout.Box( "", GUILayout.ExpandWidth( true ), GUILayout.Height( 2 ) );

                    GUILayout.BeginHorizontal();
                        CodeViewsWindow.OptionalAdvancedOption();
                    GUILayout.EndHorizontal();
                }
            GUILayout.EndVertical();
        }

        internal static string GenerateCode( bool copy )
        {
            string code = "";

            if ( CodeViewsWindow.ShowEntFunction )
            {
                code += $"ENTITIES02 num_models={CodeViewsWindow.EntFileID}\n";
                code +=  "{\n";
                code +=  "\"spawnflags\" \"0\"\n";
                code +=  "\"scale\" \"1\"\n";
                code +=  "\"angles\" \"0 0 0\"\n";
                code +=  "\"origin\" \"0 0 0\"\n";
                code +=  "\"classname\" \"info_player_start\"\n";
                code +=  "}\n";
            }

            ObjectType[] showOnly = new ObjectType[]
            {
                ObjectType.Prop,
                ObjectType.VerticalZipLine,
                ObjectType.NonVerticalZipLine,
                ObjectType.SingleDoor,
                ObjectType.DoubleDoor,
                ObjectType.HorzDoor,
                ObjectType.VerticalDoor,
                ObjectType.LootBin,
                ObjectType.FuncWindowHint
            };

            Helper.ForceHideBoolToGenerateObjects( showOnly, true );

            code += Helper.BuildMapCode( BuildType.EntFile, CodeViewsWindow.EnableSelection );

            if ( CodeViewsWindow.ShowEntFunction ) code += "\u0000";

            return code;
        }
    }
}