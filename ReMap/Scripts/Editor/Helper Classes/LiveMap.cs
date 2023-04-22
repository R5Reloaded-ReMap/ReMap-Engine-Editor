using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace CodeViewsWindow
{
    public static class LiveMap
    {
        public static bool IsSending = false;
        static IntPtr m_hEngine;

        public static int BuildWaitMS = 50;

        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int wMsg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [StructLayout(LayoutKind.Sequential)]
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

        public static void SendCommandToApex(string command)
        {
            if(m_hEngine == null)
                return;
                
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
            if( m_hEngine == null )
            {
                IsSending = false;
                CodeViewsWindow.EnableAutoLiveMapCode = false;
                UnityInfo.Printt( "Window Not Found" );
                return;
            }

            SendCommandToApex($"sv_cheats 1");
            Helper.DelayInMS( BuildWaitMS );
            SendCommandToApex( $"sv_quota_stringCmdsPerSecond 9999999" );
            Helper.DelayInMS( BuildWaitMS );
            SendCommandToApex( $"cl_quota_stringCmdsPerSecond 9999999" );
            Helper.DelayInMS( BuildWaitMS );
            SendCommandToApex ($"script MapEditor_RemoveAllEnts()" );
            Helper.DelayInMS( BuildWaitMS );

            CodeViewsWindow.SendedEntityCount = 0;

            await Helper.BuildMapCode( Build.BuildType.LiveMap, CodeViewsWindow.EnableSelection );

            SendConfirmed();

            IsSending = false;
        }

        public static async void ReloadLevel( bool reset = false )
        {
            string processName = "r5apex";
            Process[] processes = Process.GetProcesses();
            bool processFound = false;
            string path = "";

            foreach (Process process in processes)
            {
                if ( process.ProcessName == processName )
                {
                    try
                    {
                        path = Path.GetDirectoryName( process.MainModule.FileName );
                        processFound = true;
                    }
                    catch (System.ComponentModel.Win32Exception)
                    {
                        UnityInfo.Printt("System.ComponentModel.Win32Exception: Process Not Found");
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

            if (!processFound)
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

        private static async void SendConfirmed()
        {
            CodeViewsWindow.SendingObjects = true;

            await Task.Delay( 1000 * 6 ); // Show the message while 6 seconds

            CodeViewsWindow.SendingObjects = false;

            // Add the button
            FunctionRef[] updatedLiveCode = new FunctionRef[ ScriptTab.LiveCode.Length + 1 ];
            updatedLiveCode[0] = ScriptTab.SendMapCodeButton;
            Array.Copy( ScriptTab.LiveCode, 0, updatedLiveCode, 1, ScriptTab.LiveCode.Length );
            ScriptTab.LiveCode = updatedLiveCode;
        }
    }
}