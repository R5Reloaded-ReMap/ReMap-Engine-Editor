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
        internal static void OnGUISettingsTab()
        {
            GUILayout.BeginVertical();
            CodeViewsWindow.scrollSettings = GUILayout.BeginScrollView( CodeViewsWindow.scrollSettings, false, false );

            CodeViewsWindow.ShowSquirrelFunction();
            if ( CodeViewsWindow.ShowFunction )
            {
                CodeViewsWindow.Space( 4 );
                CodeViewsWindow.OptionalFunctionName();

                CodeViewsWindow.Space( 6 );
                CodeViewsWindow.Separator();
            } else CodeViewsWindow.Space( 10 );

            CodeViewsWindow.OptionalUseOffset();
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

            if ( CodeViewsWindow.ShowFunction )
            {
                code += $"void function {CodeViewsWindow.functionName}()\n";
                code += "{\n";
                code += Helper.ReMapCredit();
            }

            code += Helper.ShouldAddStartingOrg( StartingOriginType.SquirrelFunction, CodeViewsWindow.StartingOffset.x, CodeViewsWindow.StartingOffset.y, CodeViewsWindow.StartingOffset.z );

            Helper.ForceHideBoolToGenerateObjects( new ObjectType[0] );
            
            code += await Helper.BuildMapCode( BuildType.Script, CodeViewsWindow.EnableSelection );

            if ( CodeViewsWindow.ShowFunction ) code += "}";

            return code;
        }
    }
}