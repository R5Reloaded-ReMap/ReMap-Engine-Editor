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
        static FunctionRef[] EntMenu = new FunctionRef[]
        {
            () => CodeViewsMenu.OptionalTextField( ref CodeViewsWindow.functionName, "File Name", "Change the name of the file" ),
            () => CodeViewsMenu.OptionalIntField( ref CodeViewsWindow.EntFileID, "Ent ID", "Set the map ID" ),
            () => CodeViewsMenu.OptionalTextInfo( "Info Player Start", "Settings of where to spawn the player" ),
            () => CodeViewsMenu.OptionalVector3Field( ref CodeViewsWindow.InfoPlayerStartOrigin, "- Origin", "Set origin to \"Info Player Start\"" ),
            () => CodeViewsMenu.OptionalVector3Field( ref CodeViewsWindow.InfoPlayerStartAngles, "- Angles", "Set angles to \"Info Player Start\"" )
        };

        static FunctionRef[] OffsetMenu = new FunctionRef[]
        {
            () => CodeViewsMenu.OptionalVector3Field( ref CodeViewsWindow.StartingOffset, "Starting Origin", "Change origins in \"vector startingorg = < 0, 0, 0 >\"" )
        };

        static FunctionRef[] SelectionMenu = new FunctionRef[0];

        static FunctionRef[] AdvancedMenu = new FunctionRef[]
        {
            () => CodeViewsMenu.OptionalAdvancedOption()
        };

        internal static void OnGUISettingsTab()
        {
            GUILayout.BeginVertical();
            CodeViewsWindow.scrollSettings = GUILayout.BeginScrollView( CodeViewsWindow.scrollSettings, false, false );

            CodeViewsMenu.CreateMenu( EntMenu, "Hide Full File", "Show Full File", "If true, display the code as ent file", ref CodeViewsWindow.ShowEntFunction );

            CodeViewsMenu.CreateMenu( OffsetMenu, "Disable Origin Offset", "Enable Origin Offset", "If true, add a position offset to objects", ref Helper.UseStartingOffset );

            CodeViewsMenu.CreateMenu( SelectionMenu, "Disable Selection Only", "Enable Selection Only", "If true, generates the code of the selection only", ref CodeViewsWindow.EnableSelection );

            CodeViewsMenu.CreateMenu( AdvancedMenu, "Hide Advanced Options", "Show Advanced Options", "Choose the objects you want to\ngenerate or not", ref CodeViewsWindow.ShowAdvancedMenu );

            #if ReMapDev
            CodeViewsMenu.CreateMenu( CodeViewsMenu.DevMenu, "Dev Menu", "Dev Menu", "", ref CodeViewsMenu.ShowDevMenu );
            #endif
            
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