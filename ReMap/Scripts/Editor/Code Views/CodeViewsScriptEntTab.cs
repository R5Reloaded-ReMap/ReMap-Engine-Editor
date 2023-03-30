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
            GUILayout.BeginVertical();
            CodeViewsWindow.scrollSettings = GUILayout.BeginScrollView( CodeViewsWindow.scrollSettings, false, false );

            CodeViewsWindow.ShowSquirrelEntFunction();
            if ( CodeViewsWindow.ShowEntFunction )
            {
                CodeViewsWindow.Space( 4 );
                CodeViewsWindow.OptionalFunctionName( "File Name", "Change the name of the file" );
                CodeViewsWindow.Space( 4 );
                CodeViewsWindow.OptionalMapID();
                CodeViewsWindow.Space( 4 );
                CodeViewsWindow.OptionalInfoPlayerStart();
                CodeViewsWindow.Space( 6 );
                CodeViewsWindow.Separator();
            } else CodeViewsWindow.Space( 10 );

            CodeViewsWindow.OptionalUseOffset();
            if ( Helper.UseStartingOffset )
            {
                CodeViewsWindow.Space( 4 );
                CodeViewsWindow.OptionalOffsetField();
                CodeViewsWindow.Space( 6 );
                CodeViewsWindow.Separator();
            } else CodeViewsWindow.Space( 10 );

            CodeViewsWindow.OptionalSelection();

            CodeViewsWindow.Space( 10 );

            CodeViewsWindow.OptionalAdvancedOption();

            GUILayout.EndScrollView();
            GUILayout.EndVertical();
        }

        internal static async Task< string > GenerateCode()
        {
            string code = "";

            Vector3 IPSAngles = CodeViewsWindow.InfoPlayerStartAngles;
            Vector3 IPSOrigin = CodeViewsWindow.InfoPlayerStartOrigin;

            if ( CodeViewsWindow.ShowEntFunction )
            {
                code += $"ENTITIES02 num_models={CodeViewsWindow.EntFileID}\n";
                code +=  "{\n";
                code +=  "\"spawnflags\" \"0\"\n";
                code +=  "\"scale\" \"1\"\n";
                code += $"\"angles\" \"{Helper.ReplaceComma( IPSAngles.x )} {Helper.ReplaceComma( IPSAngles.y )} {Helper.ReplaceComma( IPSAngles.z )}\"\n";
                code += $"\"origin\" \"{Helper.ReplaceComma( IPSOrigin.x )} {Helper.ReplaceComma( IPSOrigin.y )} {Helper.ReplaceComma( IPSOrigin.z )}\"\n";
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

            code += await Helper.BuildMapCode( BuildType.EntFile, CodeViewsWindow.EnableSelection );

            if ( CodeViewsWindow.ShowEntFunction ) code += "\u0000";

            return code;
        }
    }
}