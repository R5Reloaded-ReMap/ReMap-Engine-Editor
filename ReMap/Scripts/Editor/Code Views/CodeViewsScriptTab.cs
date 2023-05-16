
using System;
using System.Collections.Generic;
using System.Text;
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
    public class ScriptTab
    {
        static FunctionRef[] SquirrelMenu = new FunctionRef[]
        {
            () => CodeViewsMenu.CreateMenu( CodeViewsWindow.SquirrelMenuShowFunction, SquirrelFunction, MenuType.Medium, "Hide Squirrel Function", "Show Squirrel Function", "If true, display the code as a function", true ),
            () => CodeViewsMenu.CreateMenu( CodeViewsWindow.SquirrelMenuShowAdditionalCode, () => CodeViewsMenu.OptionalAdditionalCodeOption(), MenuType.Medium, "Add Additional Code", "Add Additional Code", "", true )
        };

        static FunctionRef[] SquirrelFunction = new FunctionRef[]
        {
            () => CodeViewsMenu.OptionalTextField( ref CodeViewsWindow.functionName, "Function Name", "Change the name of the function", null, MenuType.Small ),
            () => CodeViewsMenu.CreateMenu( CodeViewsWindow.SquirrelMenuShowPrecacheCode, CodeViewsMenu.EmptyFunctionRefArray, MenuType.Medium, "Hide Precache Code", "Show Precache Code", "", true )
        };

        static FunctionRef[] OffsetMenu = new FunctionRef[]
        {
            () => CodeViewsMenu.CreateMenu( CodeViewsWindow.OffsetMenuOffset, OffsetSubMenu, MenuType.Medium, "Disable Origin Offset", "Enable Origin Offset", "If true, add a position offset to objects", true )
        };

        static FunctionRef[] OffsetSubMenu = new FunctionRef[]
        {
            () => CodeViewsMenu.CreateMenu( CodeViewsWindow.OffsetMenuShowOffset, CodeViewsMenu.EmptyFunctionRefArray, MenuType.Medium, "Hide Origin Offset", "Show Origin Offset", "Show/Hide \"vector startingorg = < 0, 0, 0 >\"", true ),
            () => CodeViewsMenu.OptionalTextInfo( "Starting Origin", "Change origins in \"vector startingorg = < 0, 0, 0 >\"", MenuInit.IsEnable( CodeViewsWindow.OffsetMenuShowOffset ), MenuType.Small ),
            () => CodeViewsMenu.OptionalVector3Field( ref CodeViewsWindow.StartingOffset, "- Origin", "Change origins in \"vector startingorg = < 0, 0, 0 >\"", MenuInit.IsEnable( CodeViewsWindow.OffsetMenuShowOffset ), MenuType.Small )
        };

        internal static FunctionRef[] LiveCodeMenu = new FunctionRef[]
        {
            () => CodeViewsMenu.OptionalButton( "Fast Load", "Sends the code live if you have your game on\nNote: Some objects / features do not work,\nuse \"Restart Level And Write Script\" instead", () => LiveMap.Send(), !CodeViewsWindow.SendingObjects, MenuType.Medium ),
            () => CodeViewsMenu.CreateMenu( CodeViewsWindow.LiveCodeMenuRespawn, SubLiveCodeRespawnMenu, MenuType.Medium, "Respawn Menu", "Respawn Menu", "" ),
            () => CodeViewsMenu.CreateMenu( CodeViewsWindow.LiveCodeMenuAutoSend, CodeViewsMenu.EmptyFunctionRefArray, MenuType.Medium, "Disable Auto Send Live Map Code", "Enable Auto Send Live Map Code", "Automaticly sends live map code" ),
            () => CodeViewsMenu.CreateMenu( CodeViewsWindow.LiveCodeMenuAdvanced, SubLiveCodeAdvancedMenu, MenuType.Medium, "Hide Advanced", "Show Advanced", "Restart your game and rewrite\nthe script to spawn your map", false, true, !CodeViewsWindow.SendingObjects )
        };

        internal static FunctionRef[] SubLiveCodeRespawnMenu = new FunctionRef[]
        {
            () => CodeViewsMenu.CreateMenu( CodeViewsWindow.LiveCodeMenuTeleportation, CodeViewsMenu.EmptyFunctionRefArray, MenuType.Small, "Disable Teleport Player To Map", "Enable Teleport Player To Map", "Automaticly teleport all players to the map when sending the code to the game" ),
            () => CodeViewsMenu.OptionalButton( "Respawn Players", "Makes players respawn without regenerating the map", () => LiveMap.ReMapTeleportToMap(), !CodeViewsWindow.SendingObjects, MenuType.Small ),
        };

        internal static FunctionRef[] SubLiveCodeAdvancedMenu = new FunctionRef[]
        {
            () => CodeViewsMenu.OptionalButton( "Restart Level", "Generate your map in mp_rr_remap.nut\nand reload the level\nNote: Works only on the survival_dev playlist", () => LiveMap.ReloadLevel(), null, MenuType.Small ),
            () => CodeViewsMenu.OptionalButton( "Reset Script", "Reset mp_rr_remap.nut", () => LiveMap.ReloadLevel( true ), null, MenuType.Small )
        };

        internal static void OnGUISettingsTab()
        {
            GUILayout.BeginVertical();
            CodeViewsWindow.scrollSettings = GUILayout.BeginScrollView( CodeViewsWindow.scrollSettings, false, false );

            CodeViewsMenu.CreateMenu( CodeViewsWindow.SquirrelMenu, SquirrelMenu, MenuType.Large, "Function Menu", "Function Menu", "" );

            CodeViewsMenu.CreateMenu( CodeViewsWindow.OffsetMenu, OffsetMenu, MenuType.Large, "Offset Menu", "Offset Menu", "" );

            CodeViewsMenu.SelectionMenu();

            CodeViewsMenu.CreateMenu( CodeViewsWindow.LiveCodeMenu, LiveCodeMenu, MenuType.Large, "Live Generation", "Live Generation", "Allows you to send commands to\nspawn prop if your game is open" );

            CodeViewsMenu.CreateMenu( CodeViewsWindow.AdvancedMenu, () => CodeViewsMenu.OptionalAdvancedOption(), MenuType.Large, "Advanced Options", "Advanced Options", "Choose the objects you want to\ngenerate or not" );

            CodeViewsMenu.SharedFunctions();

            GUILayout.EndScrollView();
            GUILayout.EndVertical();
        }

        internal static async Task< string > GenerateCode()
        {
            
            ObjectType[] forceHide = new ObjectType[]
            {
                ObjectType.Sound,
                ObjectType.SpawnPoint,
                ObjectType.CameraPath,
            };

            Helper.ForceHideBoolToGenerateObjects( forceHide );


            StringBuilder code = new StringBuilder();

            CodeViewsWindow.AppendAdditionalCode( AdditionalCodeType.Head, ref code, CodeViewsWindow.additionalCodeHead, ( CodeViewsWindow.ShowFunctionEnable() && CodeViewsWindow.AdditionalCodeEnable() ) );

            if ( CodeViewsWindow.ShowFunctionEnable() && CodeViewsWindow.ShowPrecacheEnable() )
            {
                AppendCode( ref code, $"void function {CodeViewsWindow.functionName}_Init()" );
                AppendCode( ref code, "{" );
                AppendCode( ref code, await Build.Build.BuildObjectsWithEnum( ObjectType.Prop, Build.BuildType.Precache, false ) );
                AppendCode( ref code, $"    AddCallback_EntitiesDidLoad( {CodeViewsWindow.functionName} )" );
                AppendCode( ref code, "}", 2 );
            }

            if ( CodeViewsWindow.ShowFunctionEnable() )
            {
                AppendCode( ref code, $"void function {CodeViewsWindow.functionName}()" );
                AppendCode( ref code, "{" );
                AppendCode( ref code, Helper.ReMapCredit(), 0 );
            }

            AppendCode( ref code, Helper.ShouldAddStartingOrg( StartingOriginType.SquirrelFunction, CodeViewsWindow.StartingOffset.x, CodeViewsWindow.StartingOffset.y, CodeViewsWindow.StartingOffset.z ), 0 );

            AppendCode( ref code, await Helper.BuildMapCode( BuildType.Script, CodeViewsWindow.SelectionEnable() ), 0 );

            CodeViewsWindow.AppendAdditionalCode( AdditionalCodeType.InBlock, ref code, CodeViewsWindow.additionalCodeInBlock, CodeViewsWindow.AdditionalCodeEnable() );

            if ( CodeViewsWindow.ShowFunctionEnable() ) AppendCode( ref code, "}" );

            CodeViewsWindow.AppendAdditionalCode( AdditionalCodeType.Below, ref code, CodeViewsWindow.additionalCodeBelow, ( CodeViewsWindow.ShowFunctionEnable() && CodeViewsWindow.AdditionalCodeEnable() ) );

            if ( CodeViewsWindow.AutoSendEnable() ) LiveMap.Send();

            return code.ToString();
        }
    }
}