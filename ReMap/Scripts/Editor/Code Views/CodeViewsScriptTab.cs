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
        internal static void OnGUIScriptTab()
        {
            GUILayout.BeginVertical( "box" );

                GUILayout.BeginHorizontal( "box" );
                    CodeViewsWindow.ObjectCount();
                    GUILayout.FlexibleSpace();
                    CodeViewsWindow.ExportButton();
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal( "box" );
                    CodeViewsWindow.ShowSquirrelFunction();

                    CodeViewsWindow.OptionalUseOffset();

                    if ( Helper.UseStartingOffset ) CodeViewsWindow.OptionalShowOffset();
                    if ( Helper.UseStartingOffset && Helper.ShowStartingOffset ) CodeViewsWindow.OptionalOffsetField();
                    
                    GUILayout.FlexibleSpace();

                    CodeViewsWindow.OptionalAdvancedOption();
                GUILayout.EndHorizontal();

                if ( CodeViewsWindow.ShowFunction )
                {
                    GUILayout.BeginHorizontal( "box" );
                        CodeViewsWindow.OptionalFunctionName();
                    GUILayout.EndHorizontal();
                }

            if ( CodeViewsWindow.ShowAdvancedMenu ) CodeViewsWindow.AdvancedOptionMenu();

            CodeViewsWindow.CodeOutput();
        
                if (GUILayout.Button( "Copy To Clipboard" ) ) GenerateCode( true );

            GUILayout.EndVertical();
        }

        internal static string GenerateCode( bool copy )
        {
            string code = "";

            if ( CodeViewsWindow.ShowFunction )
            {
                code += $"void function {CodeViewsWindow.functionName}()"; PageBreak( ref code );
                code += "{"; PageBreak( ref code );
                code += Helper.ReMapCredit();
            }

            code += Helper.ShouldAddStartingOrg( StartingOriginType.SquirrelFunction, CodeViewsWindow.StartingOffset.x, CodeViewsWindow.StartingOffset.y, CodeViewsWindow.StartingOffset.z );

            code += Helper.BuildMapCode( BuildType.Script,
            Helper.GetBoolFromGenerateObjects( ObjectType.Prop ), Helper.GetBoolFromGenerateObjects( ObjectType.ZipLine ), Helper.GetBoolFromGenerateObjects( ObjectType.LinkedZipline ), Helper.GetBoolFromGenerateObjects( ObjectType.VerticalZipLine ), Helper.GetBoolFromGenerateObjects( ObjectType.NonVerticalZipLine ),
            Helper.GetBoolFromGenerateObjects( ObjectType.SingleDoor ), Helper.GetBoolFromGenerateObjects( ObjectType.DoubleDoor ), Helper.GetBoolFromGenerateObjects( ObjectType.HorzDoor ), Helper.GetBoolFromGenerateObjects( ObjectType.VerticalDoor ), Helper.GetBoolFromGenerateObjects( ObjectType.Button ),
            Helper.GetBoolFromGenerateObjects( ObjectType.Jumppad ), Helper.GetBoolFromGenerateObjects( ObjectType.LootBin ), Helper.GetBoolFromGenerateObjects( ObjectType.WeaponRack ), Helper.GetBoolFromGenerateObjects( ObjectType.Trigger ), Helper.GetBoolFromGenerateObjects( ObjectType.BubbleShield ),
            Helper.GetBoolFromGenerateObjects( ObjectType.SpawnPoint ), Helper.GetBoolFromGenerateObjects( ObjectType.TextInfoPanel ), Helper.GetBoolFromGenerateObjects( ObjectType.FuncWindowHint ), Helper.GetBoolFromGenerateObjects( ObjectType.Sound ) );

            if ( CodeViewsWindow.ShowFunction ) code += "}";

            return code;
        }
    }
}