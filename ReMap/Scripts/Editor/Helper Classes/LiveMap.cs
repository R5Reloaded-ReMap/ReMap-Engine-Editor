using UnityEngine;
using UnityEditor;
using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

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
            if(IsSending)
                return;
            
            IsSending = true;
            //find and set window once
            m_hEngine = FindApexWindow();
            if(m_hEngine == null) {
                IsSending = false;
                CodeViewsWindow.EnableAutoLiveMapCode = false;
                Debug.Log("Window Not Found");
                return;
            }

            SendCommandToApex($"sv_cheats 1");
            SendCommandToApex($"sv_quota_stringCmdsPerSecond 9999999");
            SendCommandToApex($"cl_quota_stringCmdsPerSecond 9999999");
            SendCommandToApex($"script MapEditor_RemoveAllEnts()");

            await Helper.BuildMapCode(Build.BuildType.LiveMap, CodeViewsWindow.EnableSelection);

            IsSending = false;
        }
    }
}