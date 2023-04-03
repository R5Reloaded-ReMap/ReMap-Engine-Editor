using UnityEngine;
using UnityEditor;
using System;
using System.Runtime.InteropServices;

public class LiveUpdate : EditorWindow
{
    bool IsSending = false;
    #if ReMapDev
    [MenuItem("ReMap/LiveMapTesting", false, 0)]
    static void Init()
    {
        LiveUpdate window = (LiveUpdate)EditorWindow.GetWindow(typeof(LiveUpdate), false, "Testing");
        window.minSize = new Vector2(678, 290);
        window.maxSize = new Vector2(678, 290);
        window.Show();
    }
    #endif

    private Texture2D m_Logo = null;
    void OnEnable()
    {
        m_Logo = (Texture2D)Resources.Load("Images/logo",typeof(Texture2D));
    }

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

    void OnGUI()
    {
        GUI.contentColor = Color.white;
        GUILayout.BeginVertical("box");
            GUILayout.BeginHorizontal();
                if (GUILayout.Button("Send Map"))
                {
                    if(IsSending)
                        return;
                    
                    IsSending = true;
                    SendProps();
                }
            GUILayout.EndHorizontal();
        GUILayout.EndVertical();
    }

    public void SendCommandToApex(string command)
    {
        string m_pTestCommand = "script " + command;

        IntPtr m_hEngine = FindWindow("Respawn001", "Apex Legends");

        COPYDATASTRUCT m_cData;
        m_cData.cbData = m_pTestCommand.Length + 1;
        m_cData.dwData = IntPtr.Zero;
        m_cData.lpData = Marshal.StringToHGlobalAnsi(m_pTestCommand);

        // Allocate memory for the data and copy
        IntPtr ptrCopyData = Marshal.AllocCoTaskMem(Marshal.SizeOf(m_cData));
        Marshal.StructureToPtr(m_cData, ptrCopyData, false);

        SendMessage(m_hEngine, WM_COPYDATA, IntPtr.Zero, ptrCopyData);
    }

    public void SendProps()
    {
        GameObject[] props = GameObject.FindGameObjectsWithTag("Prop");
            // Build the code
        foreach ( GameObject obj in props )
        {
            PropScript script = ( PropScript ) Helper.GetComponentByEnum( obj, ObjectType.Prop );
            if ( script == null ) continue;

            string model = UnityInfo.GetApexModelName( UnityInfo.GetObjName( obj ), true );
            string scale = Helper.ReplaceComma( obj.transform.localScale.x );

            SendCommandToApex($"MapEditor_CreateProp( $\"{model}\", {Helper.BuildOrigin(obj) + Helper.ShouldAddStartingOrg()}, {Helper.BuildAngles(obj)}, {Helper.BoolToLower( script.AllowMantle )}, {Helper.ReplaceComma( script.FadeDistance )}, {script.RealmID}, {scale} )");
            //await System.Threading.Tasks.Task.Delay(TimeSpan.FromSeconds(0.001));
        }

        IsSending = false;
    }
}