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
using WindowUtility;

namespace CodeViewsWindow
{
    public class SoundEntTab
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

        internal static void OnGUISettingsTab()
        {
            GUILayout.BeginVertical();
            CodeViewsWindow.scrollSettings = GUILayout.BeginScrollView( CodeViewsWindow.scrollSettings, false, false );

            //CodeViewsMenu.CreateMenu( EntMenu, "Hide Full File", "Show Full File", "If true, display the code as ent file", ref CodeViewsWindow.ShowEntFunction );

            //CodeViewsMenu.CreateMenu( OffsetMenu, "Disable Origin Offset", "Enable Origin Offset", "If true, add a position offset to objects", ref Helper.UseStartingOffset );

            //CodeViewsMenu.CreateMenu( SelectionMenu, "Disable Selection Only", "Enable Selection Only", "If true, generates the code of the selection only", ref CodeViewsWindow.EnableSelection );

            CodeViewsMenu.SharedFunctions();
            
            GUILayout.EndScrollView();
            GUILayout.EndVertical();
        }

        internal static async Task< string > GenerateCode()
        {
            string code = "";

            if ( CodeViewsWindow.ShowEntFunction )
            {
                code += $"ENTITIES02 num_models={CodeViewsWindow.EntFileID}\n";
            }

            Helper.ForceHideBoolToGenerateObjects( new ObjectType[] { ObjectType.Sound }, true );

            code += await Helper.BuildMapCode( BuildType.EntFile, MenuInit.IsEnable( CodeViewsWindow.SelectionMenu ) );

            if ( CodeViewsWindow.ShowEntFunction ) code += "\u0000";

            return code;
        }
    }
}