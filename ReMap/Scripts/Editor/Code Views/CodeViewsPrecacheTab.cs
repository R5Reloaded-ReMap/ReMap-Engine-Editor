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
    public class PrecacheTab
    {
        static FunctionRef[] SquirrelMenu = new FunctionRef[]
        {
            () => CodeViewsMenu.OptionalTextField( ref CodeViewsWindow.functionName, "Function Name", "Change the name of the function" )
        };

        static FunctionRef[] SelectionMenu = new FunctionRef[0];

        internal static void OnGUISettingsTab()
        {
            GUILayout.BeginVertical();
            CodeViewsWindow.scrollSettings = GUILayout.BeginScrollView( CodeViewsWindow.scrollSettings, false, false );

            CodeViewsMenu.CreateMenu( SquirrelMenu, "Hide Squirrel Function", "Show Squirrel Function", "If true, display the code as a function", ref CodeViewsWindow.ShowFunction );

            CodeViewsMenu.CreateMenu( SelectionMenu, "Disable Selection Only", "Enable Selection Only", "If true, generates the code of the selection only", ref CodeViewsWindow.EnableSelection );

            #if ReMapDev
            CodeViewsMenu.CreateMenu( CodeViewsMenu.DevMenu, "Dev Menu", "Dev Menu", "", ref CodeViewsMenu.ShowDevMenu );
            #endif
            
            GUILayout.EndScrollView();
            GUILayout.EndVertical();
        }

        internal static async Task< string > GenerateCode()
        {
            string code = "";

            if ( CodeViewsWindow.ShowFunction )
            {
                code += $"void function {CodeViewsWindow.functionName}()\n";
                code += "{\n";
                code += Helper.ReMapCredit();
            }

            Helper.ForceHideBoolToGenerateObjects( new ObjectType[0] );

            code += await BuildObjectsWithEnum( ObjectType.Prop, BuildType.Precache, CodeViewsWindow.EnableSelection );

            if ( CodeViewsWindow.ShowFunction ) code += "}";

            return code;
        }
    }
}