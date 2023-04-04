using UnityEngine;
using UnityEditor;
using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

public class LiveUpdate : EditorWindow
{
    bool IsSending = false;
    IntPtr m_hEngine;

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
                    if(!IsSending)
                        SendMap();

            if(IsSending)
                GUILayout.Label("Sending Map");
            GUILayout.EndHorizontal();
        GUILayout.EndVertical();
    }

    public IntPtr FindApexWindow()
    { 
        return FindWindow("Respawn001", "Apex Legends"); 
    }

    public void SendCommandToApex(string command)
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

    public void SendMap()
    {
        IsSending = true;
        Debug.Log("Find Window");
        //find and set window once
        m_hEngine = FindApexWindow();
        if(m_hEngine == null) {
            IsSending = false;
            Debug.Log("Window Not Found");
            return;
        }

        SendCommandToApex($"sv_cheats 1");
        SendCommandToApex($"sv_quota_stringCmdsPerSecond 9999999");
        SendCommandToApex($"cl_quota_stringCmdsPerSecond 9999999");

        SendProps();
        SendZiplines();
        SendLinkedZiplines();

        IsSending = false;
    }

    public void SendProps()
    {
        GameObject[] PropObjects = GameObject.FindGameObjectsWithTag( Helper.GetObjTagNameWithEnum( ObjectType.Prop ) );

        foreach ( GameObject obj in PropObjects )
        {
            PropScript script = ( PropScript ) Helper.GetComponentByEnum( obj, ObjectType.Prop );
            if ( script == null ) continue;

            string model = UnityInfo.GetApexModelName( UnityInfo.GetObjName( obj ), true );
            string scale = Helper.ReplaceComma( obj.transform.localScale.x );

            SendCommandToApex($"script MapEditor_CreateProp( $\"{model}\", {Helper.BuildOrigin(obj) + Helper.ShouldAddStartingOrg()}, {Helper.BuildAngles(obj)}, {Helper.BoolToLower( script.AllowMantle )}, {Helper.ReplaceComma( script.FadeDistance )}, {script.RealmID}, {scale} )");
            
            //Custom delay function
            //using any other wait is accurate down to the miliseconds
            //10 seems to be the lowest otherwise it sends to many commands
            DelayInMS(10);
        }
    }

    void DelayInMS(int ms) // Stops the code for milliseconds and then resumes it (Basically It's delay)
        {
            for (int i = 0; i < ms * 100000; i++) 
            {
                ;
                ;
                ;
                ;
                ;
            }
        }

    public void SendZiplines()
    {
        GameObject[] Ziplines = GameObject.FindGameObjectsWithTag( Helper.GetObjTagNameWithEnum( ObjectType.ZipLine ) );

        // Build the code
        foreach ( GameObject obj in Ziplines )
        {
            DrawZipline script = ( DrawZipline ) Helper.GetComponentByEnum( obj, ObjectType.ZipLine );
            if ( script == null ) continue;

            //string model = "custom_zipline" );
            string ziplinestart = "";
            string ziplineend = "";

            foreach ( Transform child in obj.transform )
            {
                if ( child.name == "zipline_start" ) ziplinestart = Helper.BuildOrigin( child.gameObject );
                if ( child.name == "zipline_end" ) ziplineend = Helper.BuildOrigin( child.gameObject );
            }

            SendCommandToApex($"script CreateZipline( {ziplinestart + Helper.ShouldAddStartingOrg()}, {ziplineend + Helper.ShouldAddStartingOrg()} )");

            //Custom delay function
            //using any other wait is accurate down to the miliseconds
            //10 seems to be the lowest otherwise it sends to many commands
            DelayInMS(10);
        }
    }

    public void SendLinkedZiplines()
    {
        GameObject[] Ziplines = GameObject.FindGameObjectsWithTag( Helper.GetObjTagNameWithEnum( ObjectType.LinkedZipline ) );

        // Build the code
        foreach ( GameObject obj in Ziplines )
        {
            LinkedZiplineScript script = ( LinkedZiplineScript ) Helper.GetComponentByEnum( obj, ObjectType.LinkedZipline );
            if ( script == null ) continue;

            string function = "";
            string smoothType = script.SmoothType ? "GetAllPointsOnBezier" : "GetBezierOfPath";
            string nodes = MakeLinkedZiplineNodeArray( obj );

            if ( script.EnableSmoothing ) function = $"{smoothType}( {nodes}, {script.SmoothAmount} )";
            else function = $"{nodes}";

            SendCommandToApex($"script MapEditor_CreateLinkedZipline( {function} )");

            //Custom delay function
            //using any other wait is accurate down to the miliseconds
            //10 seems to be the lowest otherwise it sends to many commands
            DelayInMS(10);
        }
    }

    private static string MakeLinkedZiplineNodeArray( GameObject obj )
        {
            bool first = true;

            string nodes = "[ ";
            foreach ( Transform child in obj.transform )
            {
                if (!first)
                    nodes += ", ";

                nodes += Helper.BuildOrigin( child.gameObject );

                    first = false;
            }
            nodes += " ]";

            return nodes;
        }
}