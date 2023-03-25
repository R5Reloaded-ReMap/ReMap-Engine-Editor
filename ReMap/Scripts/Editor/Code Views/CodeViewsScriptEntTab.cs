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
        internal static void OnGUIScriptEntTab()
        {
            GUILayout.BeginVertical( "box" );

                CodeViewsWindow.ObjectCount();

            GUILayout.EndVertical();

            GUILayout.BeginVertical( "box" );
                CodeViewsWindow.scroll = EditorGUILayout.BeginScrollView( CodeViewsWindow.scroll );

                    GUILayout.TextArea( CodeViewsWindow.code, GUILayout.ExpandHeight( true ) );

                EditorGUILayout.EndScrollView();
            GUILayout.EndVertical();

            GUILayout.BeginVertical( "box" );
        
                if (GUILayout.Button( "Copy To Clipboard" ) ) GenerateCode( true );

            GUILayout.EndVertical();
        }

        internal static string GenerateCode( bool copy )
        {
            string code = "";

            code += Helper.BuildMapCode( BuildType.EntFile,
            Helper.GetBoolFromGenerateObjects( ObjectType.Prop ), Helper.GetBoolFromGenerateObjects( ObjectType.ZipLine ), Helper.GetBoolFromGenerateObjects( ObjectType.LinkedZipline ), Helper.GetBoolFromGenerateObjects( ObjectType.VerticalZipLine ), Helper.GetBoolFromGenerateObjects( ObjectType.NonVerticalZipLine ),
            Helper.GetBoolFromGenerateObjects( ObjectType.SingleDoor ), Helper.GetBoolFromGenerateObjects( ObjectType.DoubleDoor ), Helper.GetBoolFromGenerateObjects( ObjectType.HorzDoor ), Helper.GetBoolFromGenerateObjects( ObjectType.VerticalDoor ), Helper.GetBoolFromGenerateObjects( ObjectType.Button ),
            Helper.GetBoolFromGenerateObjects( ObjectType.Jumppad ), Helper.GetBoolFromGenerateObjects( ObjectType.LootBin ), Helper.GetBoolFromGenerateObjects( ObjectType.WeaponRack ), Helper.GetBoolFromGenerateObjects( ObjectType.Trigger ), Helper.GetBoolFromGenerateObjects( ObjectType.BubbleShield ),
            Helper.GetBoolFromGenerateObjects( ObjectType.SpawnPoint ), Helper.GetBoolFromGenerateObjects( ObjectType.TextInfoPanel ), Helper.GetBoolFromGenerateObjects( ObjectType.FuncWindowHint ), Helper.GetBoolFromGenerateObjects( ObjectType.Sound ) );

            return code;
        }
    }
}