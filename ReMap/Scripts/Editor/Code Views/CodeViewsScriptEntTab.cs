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
        internal static void OnGUITab()
        {
            GUILayout.BeginHorizontal( "box" );
                CodeViewsWindow.OptionalUseOffset();
                if ( Helper.UseStartingOffset ) CodeViewsWindow.OptionalOffsetField();
                GUILayout.FlexibleSpace();
                CodeViewsWindow.OptionalAdvancedOption();
            GUILayout.EndHorizontal();

            //if ( CodeViewsWindow.ShowAdvancedMenu ) CodeViewsWindow.AdvancedOptionMenu();
        }

        internal static string GenerateCode( bool copy )
        {
            string code = "";

            if ( CodeViewsWindow.ShowEntFunction ) code += $"ENTITIES02 num_models={CodeViewsWindow.EntFileID}";

            if ( Helper.GetBoolFromGenerateObjects( ObjectType.Prop ) ) code += BuildObjectsWithEnum( ObjectType.Prop, BuildType.EntFile );
            if ( Helper.GetBoolFromGenerateObjects( ObjectType.VerticalZipLine ) ) code += BuildObjectsWithEnum( ObjectType.VerticalZipLine, BuildType.EntFile );
            if ( Helper.GetBoolFromGenerateObjects( ObjectType.NonVerticalZipLine ) ) code += BuildObjectsWithEnum( ObjectType.NonVerticalZipLine, BuildType.EntFile );

            if ( CodeViewsWindow.ShowEntFunction ) code += "\u0000";

            return code;
        }
    }
}