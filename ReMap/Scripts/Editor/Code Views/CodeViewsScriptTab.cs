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
    public class ScriptTab
    {
        static FunctionRef[] SquirrelMenu = new FunctionRef[]
        {
            () => CodeViewsMenu.OptionalTextField( ref CodeViewsWindow.functionName, "Function Name", "Change the name of the function" )
        };

        static FunctionRef[] OffsetMenu = new FunctionRef[]
        {
            () => CodeViewsMenu.OptionalToggle( ref Helper.ShowStartingOffset, ref Helper.ShowStartingOffsetTemp, "Show Origin Offset", "Show/Hide \"vector startingorg = < 0, 0, 0 >\"" ),
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

            CodeViewsMenu.CreateMenu( SquirrelMenu, "Hide Squirrel Function", "Show Squirrel Function", "If true, display the code as a function", ref CodeViewsWindow.ShowFunction );

            CodeViewsMenu.CreateMenu( OffsetMenu, "Disable Origin Offset", "Enable Origin Offset", "If true, add a position offset to objects", ref Helper.UseStartingOffset );

            CodeViewsMenu.CreateMenu( SelectionMenu, "Disable Selection Only", "Enable Selection Only", "If true, generates the code of the selection only", ref CodeViewsWindow.EnableSelection );

            CodeViewsMenu.CreateMenu( AdvancedMenu, "Hide Advanced Options", "Show Advanced Options", "Choose the objects you want to\ngenerate or not", ref CodeViewsWindow.ShowAdvancedMenu );

            /* CodeViewsWindow.OptionalUseOffset();
            if ( Helper.UseStartingOffset )
            {
                CodeViewsWindow.Space( 4 );
                CodeViewsWindow.OptionalShowOffset();
                if ( Helper.ShowStartingOffset )
                {
                    CodeViewsWindow.Space( 4 );
                    CodeViewsWindow.OptionalOffsetField();
                }

                CodeViewsWindow.Space( 6 );
                CodeViewsWindow.Separator();
            } else CodeViewsWindow.Space( 10 );*/

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

            code += Helper.ShouldAddStartingOrg( StartingOriginType.SquirrelFunction, CodeViewsWindow.StartingOffset.x, CodeViewsWindow.StartingOffset.y, CodeViewsWindow.StartingOffset.z );

            Helper.ForceHideBoolToGenerateObjects( new ObjectType[] { ObjectType.Sound, ObjectType.SpawnPoint } );
            
            code += await Helper.BuildMapCode( BuildType.Script, CodeViewsWindow.EnableSelection );

            if ( CodeViewsWindow.ShowFunction ) code += "}";

            return code;
        }
    }
}