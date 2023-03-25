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
        private static Vector3 startingOffset = Vector3.zero;

        private static int GUILayoutToggleSize = 180;
        private static int GUILayoutVector3FieldSize = 210;
        private static int GUILayoutLabelSize = 40;

        internal static void OnGUIScriptTab()
        {
            GUILayout.BeginVertical( "box" );

                CodeViewsWindow.ObjectCount();

                GUILayout.BeginHorizontal( "box" );
                    CodeViewsWindow.ShowFunction = EditorGUILayout.Toggle( "Show Squirrel Function", CodeViewsWindow.ShowFunction, GUILayout.MaxWidth( GUILayoutToggleSize ) );
                    if( CodeViewsWindow.ShowFunction != CodeViewsWindow.ShowFunctionTemp )
                    {
                        CodeViewsWindow.ShowFunctionTemp = CodeViewsWindow.ShowFunction;
                        CodeViewsWindow.GenerateCorrectCode();
                    }

                    Helper.UseStartingOffset = EditorGUILayout.Toggle( "Use Map Origin Offset", Helper.UseStartingOffset, GUILayout.MaxWidth( GUILayoutToggleSize ) );
                    if( Helper.UseStartingOffset != Helper.UseStartingOffsetTemp )
                    {
                        Helper.UseStartingOffsetTemp = Helper.UseStartingOffset;
                        CodeViewsWindow.GenerateCorrectCode();
                    }

                    if ( Helper.UseStartingOffset )
                    Helper.ShowStartingOffset = EditorGUILayout.Toggle( "Show Origin Offset", Helper.ShowStartingOffset, GUILayout.MaxWidth( GUILayoutToggleSize ) );
                    if( Helper.ShowStartingOffset != Helper.ShowStartingOffsetTemp )
                    {
                        Helper.ShowStartingOffsetTemp = Helper.ShowStartingOffset;
                        CodeViewsWindow.GenerateCorrectCode();

                    }

                    if ( Helper.UseStartingOffset && Helper.ShowStartingOffset )
                    {
                        EditorGUILayout.LabelField( "Offset", GUILayout.MaxWidth( GUILayoutLabelSize ) );
                        startingOffset = EditorGUILayout.Vector3Field( "", startingOffset, GUILayout.MaxWidth( GUILayoutVector3FieldSize ) );
                    }
                    
                    GUILayout.FlexibleSpace();

                    CodeViewsWindow.ShowAdvanced = EditorGUILayout.Toggle( "Show Advanced Options", CodeViewsWindow.ShowAdvanced, GUILayout.MaxWidth( GUILayoutToggleSize ) );
                GUILayout.EndHorizontal();
            GUILayout.EndVertical();

            if ( CodeViewsWindow.ShowAdvanced ) CodeViewsWindow.OptionalOption();

            GUILayout.BeginVertical( "box" );
                CodeViewsWindow.scroll = EditorGUILayout.BeginScrollView( CodeViewsWindow.scroll );

                    GUILayout.TextArea( CodeViewsWindow.code, GUILayout.ExpandHeight( true ) );

                EditorGUILayout.EndScrollView();
            GUILayout.EndVertical();

            GUILayout.BeginVertical( "box" );
        
                if (GUILayout.Button( "Copy To Clipboard" ) ) GenerateCode( true, CodeViewsWindow.ShowFunction );

            GUILayout.EndVertical();
        }

        internal static string GenerateCode( bool copy, bool ShowFunction )
        {
            string code = ""; PageBreak( ref code );

            if ( ShowFunction )
            {
                code += Helper.GetSquirrelSceneNameFunction(); PageBreak( ref code );
                code += "{"; PageBreak( ref code );
                code += Helper.ReMapCredit();
            }

            code += Helper.ShouldAddStartingOrg( StartingOriginType.SquirrelFunction, startingOffset.x, startingOffset.y, startingOffset.z );

            code += Helper.BuildMapCode( BuildType.Script,
            Helper.GetBoolFromGenerateObjects( ObjectType.Prop ), Helper.GetBoolFromGenerateObjects( ObjectType.ZipLine ), Helper.GetBoolFromGenerateObjects( ObjectType.LinkedZipline ), Helper.GetBoolFromGenerateObjects( ObjectType.VerticalZipLine ), Helper.GetBoolFromGenerateObjects( ObjectType.NonVerticalZipLine ),
            Helper.GetBoolFromGenerateObjects( ObjectType.SingleDoor ), Helper.GetBoolFromGenerateObjects( ObjectType.DoubleDoor ), Helper.GetBoolFromGenerateObjects( ObjectType.HorzDoor ), Helper.GetBoolFromGenerateObjects( ObjectType.VerticalDoor ), Helper.GetBoolFromGenerateObjects( ObjectType.Button ),
            Helper.GetBoolFromGenerateObjects( ObjectType.Jumppad ), Helper.GetBoolFromGenerateObjects( ObjectType.LootBin ), Helper.GetBoolFromGenerateObjects( ObjectType.WeaponRack ), Helper.GetBoolFromGenerateObjects( ObjectType.Trigger ), Helper.GetBoolFromGenerateObjects( ObjectType.BubbleShield ),
            Helper.GetBoolFromGenerateObjects( ObjectType.SpawnPoint ), Helper.GetBoolFromGenerateObjects( ObjectType.TextInfoPanel ), Helper.GetBoolFromGenerateObjects( ObjectType.FuncWindowHint ), Helper.GetBoolFromGenerateObjects( ObjectType.Sound ) );

            if ( ShowFunction ) code += "}";

            return code;
        }
    }
}