using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

using WindowUtility;

namespace CodeViewsWindow
{
    public static class LiveMap
    {
        private enum ConfirmationType
        {
            ERROR,
            OPTIMAL
        }

        public static List< String > Commands;
        public static bool IsSending = false;
        static IntPtr m_hEngine;

        public static int CommandDelay = 50;

        [ DllImport( "user32.dll" ) ]
        public static extern int SendMessage( IntPtr hWnd, int wMsg, IntPtr wParam, IntPtr lParam );

        [ DllImport( "user32.dll", SetLastError = true ) ]
        static extern IntPtr FindWindow( string lpClassName, string lpWindowName );

        [ StructLayout( LayoutKind.Sequential ) ]
        public struct COPYDATASTRUCT
        {
            public IntPtr dwData;
            public int cbData;
            public IntPtr lpData;
        }

        const int WM_COPYDATA = 0x4A;


        public static IntPtr FindApexWindow()
        { 
            return FindWindow("Respawn001", "Apex Legends"); 
        }

        public static bool ApexProcessIsActive()
        {
            return m_hEngine != IntPtr.Zero;
        }

        public static void SendCommandToApex(string command)
        {
            if ( !ApexProcessIsActive() ) return;
                
            string m_pCommand = command;

            COPYDATASTRUCT m_cData;
            m_cData.cbData = m_pCommand.Length + 1;
            m_cData.dwData = IntPtr.Zero;
            m_cData.lpData = Marshal.StringToHGlobalAnsi(m_pCommand);

            IntPtr ptrCopyData = Marshal.AllocCoTaskMem(Marshal.SizeOf(m_cData));
            Marshal.StructureToPtr(m_cData, ptrCopyData, false);

            SendMessage(m_hEngine, WM_COPYDATA, IntPtr.Zero, ptrCopyData);
        }

        public static async void Send()
        {
            if( IsSending )
                return;

            // Delete the button until the message is displayed and to prevent sending too many commands to apex
            FunctionRef[] newLiveCode = new FunctionRef[ ScriptTab.LiveCode.Length - 1 ];
            Array.Copy( ScriptTab.LiveCode, 1, newLiveCode, 0, ScriptTab.LiveCode.Length - 1 );
            ScriptTab.LiveCode = newLiveCode;
            
            IsSending = true;
            //find and set window once
            m_hEngine = FindApexWindow();
            if ( !ApexProcessIsActive() )
            {
                IsSending = false;
                CodeViewsWindow.EnableAutoLiveMapCode = false;
                SendConfirmation( ConfirmationType.ERROR );
                return;
            }

            Commands = new List< string >();

            AddToGameQueue( $"sv_cheats 1" );
            AddToGameQueue( $"sv_quota_stringCmdsPerSecond 9999999" );
            AddToGameQueue( $"cl_quota_stringCmdsPerSecond 9999999" );
            AddToGameQueue( $"script MapEditor_RemoveAllEnts()" );

            CodeViewsWindow.SendedEntityCount = 0;

            await Helper.BuildMapCode( Build.BuildType.LiveMap, CodeViewsWindow.EnableSelection );

            if ( CodeViewsWindow.EnableTeleportPlayerToMap ) RespawnPlayers();

            SendConfirmation( ConfirmationType.OPTIMAL );

            IsSending = false;
        }

        public static void AddToGameQueue( string command )
        {
            Commands.Add( command );
        }

        private static void SendCommands()
        {
            foreach ( string command in Commands )
            {
                SendCommandToApex( command );

                for (int i = 0; i < CommandDelay * 100000; i++) 
                {
                    ;;;;;
                }
            }
        }

        public static async void ReloadLevel( bool reset = false )
        {
            string processName = "r5apex";
            Process[] processes = Process.GetProcesses();
            bool processFound = false;
            string path = "";

            foreach ( Process process in processes )
            {
                if ( process.ProcessName == processName )
                {
                    try
                    {
                        path = Path.GetDirectoryName( process.MainModule.FileName );
                        processFound = true;
                    }
                    catch ( System.ComponentModel.Win32Exception )
                    {
                        UnityInfo.Printt( "System.ComponentModel.Win32Exception: Process Not Found" );
                    }

                    break;
                }
            }

            m_hEngine = FindApexWindow();

            if( m_hEngine == null ) return;

            if ( path != "" )
            {
                path += "\\platform\\scripts\\vscripts\\mp\\levels\\mp_rr_remap.nut";

                if ( !File.Exists( path ) ) File.Create( path );

                string code = await BuildScriptFile( reset );

                File.WriteAllText( path, code );

                if ( !reset ) SendCommandToApex( $"script GameRules_ChangeMap( GetMapName(), \"survival_dev\" )" );
            }

            if ( !processFound )
            {
                UnityEngine.Debug.Log( "Process Not Found" );
            }
        }

        private static async Task<string> BuildScriptFile( bool reset = false )
        {
            string code = "";

            Build.Build.IgnoreCounter = true;

            code += "\nglobal function ReMapLive_Init\n\n";
            code += Helper.ReMapCredit( true );

            code += "void function ReMapLive_Init()\n";
            code += "{\n";
            
            if ( reset )
            {
                code += "\n}\n";
            }
            else
            {
                code += await Build.Build.BuildObjectsWithEnum( ObjectType.Prop, Build.BuildType.Precache, false );
                code += "\n    AddCallback_EntitiesDidLoad( ReMapLive )\n";
                code += "}\n\n";

                code += "void function ReMapLive()\n";
                code += "{\n";
                code += Helper.ShouldAddStartingOrg( StartingOriginType.SquirrelFunction, CodeViewsWindow.StartingOffset.x, CodeViewsWindow.StartingOffset.y, CodeViewsWindow.StartingOffset.z );
                code += await Helper.BuildMapCode( Build.BuildType.Script, false );
                code += "}\n";
            }

            Build.Build.IgnoreCounter = false;

            return code;
        }

        public static void RespawnPlayers()
        {

            string[] data = GetLiveMapCodePlayerSpawnData();
            AddToGameQueue( $"script ReMapSetRemapArrayVec01( {data[0]} )" );
            AddToGameQueue( $"script ReMapSetRemapArrayVec02( {data[1]} )" );
            AddToGameQueue( $"script ReMapTeleportToMap()" );
        }

        private static string[] GetLiveMapCodePlayerSpawnData()
        {
            GameObject[] obj = Helper.GetObjArrayWithEnum( ObjectType.LiveMapCodePlayerSpawn );

            int objLength = obj.Length;

            if ( objLength == 0 ) return new[] { $"[{ Helper.BuildOriginVector( new Vector3( 0, 40, 0 ), false, true ) }]", $"[{ Helper.BuildAnglesVector( new Vector3( 0, 0, 0 ), false ) }]" };

            StringBuilder origin = new StringBuilder(); int i = 1;
            StringBuilder angles = new StringBuilder();

            foreach ( GameObject playerSpawn in obj )
            {
                origin.Append( Helper.BuildOrigin( playerSpawn, false, true ) );
                angles.Append( Helper.BuildAngles( playerSpawn, false ) );

                if ( i != objLength )
                {
                    origin.Append( "," );
                    angles.Append( "," );
                    i++;
                }
            }

            return new[] { $"[{ origin }]", $"[{ angles }]" };
        }

        private static async void SendConfirmation( ConfirmationType confirmationType )
        {
            CodeViewsWindow.SendingObjects = true;

            switch ( confirmationType )
            {
                case ConfirmationType.ERROR:
                    await Task.Delay( 1000 * 6 ); // Show the message while 6 seconds
                    break;

                case ConfirmationType.OPTIMAL:
                    SendCommands();
                    await Task.Delay( 1000 * 2 ); // Show the message while 2 seconds
                break;
            }

            CodeViewsWindow.SendingObjects = false;

            // Add the button
            FunctionRef[] updatedLiveCode = new FunctionRef[ ScriptTab.LiveCode.Length + 1 ];
            updatedLiveCode[0] = ScriptTab.SendMapCodeButton;
            Array.Copy( ScriptTab.LiveCode, 0, updatedLiveCode, 1, ScriptTab.LiveCode.Length );
            ScriptTab.LiveCode = updatedLiveCode;
        }
    }
}