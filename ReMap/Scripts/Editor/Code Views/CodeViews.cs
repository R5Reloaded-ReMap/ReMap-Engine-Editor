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

public class CodeViews : EditorWindow
{
    static CodeViews windowInstance;
    static string code_text = "";
    static Vector2 scroll;
    static Vector2 windowSize;
    static int subDivToggle = 5;
    int tab = 0;
    int tab_temp = 0;

    bool OnlyExportMap = true;
    bool OnlyExportMap_temp = true;
    bool UseStartingOffset = false;
    bool UseStartingOffsetTemp = false;
    static bool ShowStartingOffset = false;
    static bool ShowStartingOffset_temp = false;
    bool ShowAdvanced = false;
    public static bool UseOriginOffset = false;
    public static Vector3 OriginOffset;

    bool UseStartingOffsetLocPair = false;
    bool UseStartingOffsetLocPair_temp = false;

    // Gen Settings
    public static Dictionary< string, bool > GenerateObjects = Helper.ObjectGenerateDictionaryInit();
    public static Dictionary< string, bool > GenerateObjects_temp = new Dictionary< string, bool >( GenerateObjects );

    //Counts
    int mapcodecount = 0;
    int datatablecount = 0;
    int scriptentcount = 0;
    int soundentcount = 0;

    [MenuItem("ReMap/Code Views", false, 25)]
    public static void Init()
    {
        TagHelper.CheckAndCreateTags();

        CodeViews window = ( CodeViews )GetWindow( typeof( CodeViews ), false, "Code Views" );
        window.minSize = new Vector2( 1100, 500 );
        window.Show();
        windowInstance = window;
    }

    void OnEnable()
    {
        EditorSceneManager.sceneOpened += EditorSceneManager_sceneOpened;
        EditorSceneManager.sceneSaved += EditorSceneManager_sceneSaved;

        GetLatestCounts();
        GenerateCorrectCode();
    }

    private void EditorSceneManager_sceneSaved(UnityEngine.SceneManagement.Scene arg0)
    {
        GetLatestCounts();
        GenerateCorrectCode();
    }

    private void EditorSceneManager_sceneOpened(UnityEngine.SceneManagement.Scene arg0, OpenSceneMode mode)
    {
        GetLatestCounts();
        GenerateCorrectCode();
    }

    void GetLatestCounts()
    {
        mapcodecount = UnityInfo.GetAllCount();
        datatablecount = UnityInfo.GetSpecificObjectCount( ObjectType.Prop );
        scriptentcount = UnityInfo.GetSpecificObjectCount( ObjectType.Prop );
        soundentcount = UnityInfo.GetSpecificObjectCount( ObjectType.Sound );
    }

    void GenerateCorrectCode()
    {
        switch (tab) {
            case 0: //Map Code
                GenerateMap(OnlyExportMap, false);
                break;
            case 1: //DataTable Code
                GenerateDataTable(false);
                break;
            case 2: //Precache Code
                GeneratePrecacheCode(false);
                break;
            case 3: //Script.ent Code
                GenerateEntCode(false);
                break;
            case 4: //Sound.ent Code
                GenerateSoundEntCode(false);
                break;
            case 5: //NewLocPair Code
                GenerateNewLocPairCode(false);
                break;
        }
    }

    void OnGUI()
    {
        GUILayout.BeginVertical("box");
        GUILayout.BeginHorizontal("box");
        tab = GUILayout.Toolbar (tab, new string[] {"Map Code", "DataTable Code", "Precache Code", "Script.ent Code", "Sound.ent Code", "NewLocPair Code"});
        if (GUILayout.Button("Reload Page", GUILayout.Width(100) )) GenerateCorrectCode();
        GUILayout.EndHorizontal();
        GUILayout.EndVertical();

        if(tab != tab_temp)
            GetLatestCounts();

        GUI.contentColor = Color.white;

        switch (tab) {
            case 0: //Map Code
                MapCodeGUI();
                if (tab != tab_temp)
                    GenerateMap(OnlyExportMap, false);
                tab_temp = tab;
                break;
            case 1: //DataTable Code
                DataTableGUI();
                if(tab != tab_temp)
                    GenerateDataTable(false);
                tab_temp = tab;
                break;
            case 2: //Precache Code
                PrecacheGUI();
                if(tab != tab_temp)
                    GeneratePrecacheCode(false);
                tab_temp = tab;
                break;
            case 3: //Script.ent Code
                EntCodeGUI();
                if (tab != tab_temp)
                    GenerateEntCode(false);
                tab_temp = tab;
                break;
            case 4: //Sound.ent Code
                SoundEntGUI();
                if (tab != tab_temp)
                    GenerateSoundEntCode(false);
                tab_temp = tab;
                break;
            case 5: //NewLocPair Code
                NewLocPairGUI();
                if (tab != tab_temp)
                    GenerateNewLocPairCode(false);
                tab_temp = tab;
                break;
        }
    }

    void SetCorrectColor(int count)
    {
        if(count < CodeViewsWindow.CodeViewsWindow.greenPropCount)
            GUI.contentColor = Color.green;
        else if((count < CodeViewsWindow.CodeViewsWindow.yellowPropCount))
            GUI.contentColor = Color.yellow;
        else
            GUI.contentColor = Color.red;
    }

    void SetCorrectEntityLabel(int count)
    {
        if(count < CodeViewsWindow.CodeViewsWindow.greenPropCount)
            GUILayout.Label("Status: Safe");
        else if((count < CodeViewsWindow.CodeViewsWindow.yellowPropCount))
            GUILayout.Label("Status: Safe");
        else
            GUILayout.Label("Status: Warning! Game could crash!");
    }

    void MapCodeGUI()
    {
        GUILayout.BeginVertical("box");
        SetCorrectColor(mapcodecount);
        GUILayout.Label("Entity Count: " + mapcodecount, EditorStyles.boldLabel);
        SetCorrectEntityLabel(mapcodecount);
        GUILayout.EndVertical();

        GetEditorWindowSize();
        
        GUILayout.BeginVertical("box");
            GUILayout.BeginHorizontal();
            OnlyExportMap = EditorGUILayout.Toggle("Only Show Map Code", OnlyExportMap);
            if(OnlyExportMap != OnlyExportMap_temp) {
                OnlyExportMap_temp = OnlyExportMap;
                GenerateMap(OnlyExportMap, false);
            }
            UseStartingOffset = EditorGUILayout.Toggle("Use Map Origin Offset", UseStartingOffset);
            if(UseStartingOffset != UseStartingOffsetTemp) {
                UseStartingOffsetTemp = UseStartingOffset;
                GenerateMap(OnlyExportMap, false);
            }
            ShowStartingOffset = EditorGUILayout.Toggle("Hide Starting Vector", ShowStartingOffset);
            if(ShowStartingOffset != ShowStartingOffset_temp) {
                ShowStartingOffset_temp = ShowStartingOffset;
                GenerateMap(OnlyExportMap, false);
            }
            ShowAdvanced = EditorGUILayout.Toggle("Show Advanced Options", ShowAdvanced);
            GUILayout.EndHorizontal();
        GUILayout.EndVertical();

            if (ShowAdvanced)
            {
                GUILayout.BeginVertical("box");

                    int idx = 0;
                    GUILayout.BeginHorizontal();
                        foreach ( string key in GenerateObjects.Keys )
                        {
                            if ( IsIgnored( key ) ) continue;
                            ObjectType? type = Helper.GetObjectTypeByObjName( key );
                            if ( type != null && UnityInfo.GetSpecificObjectCount( ( ObjectType ) type ) == 0 ) continue;

                            CodeViewsWindow.CodeViewsWindow.GenerateObjects[key] = EditorGUILayout.Toggle( $"Build {key}", CodeViewsWindow.CodeViewsWindow.GenerateObjects[key], GUILayout.Width( windowSize.x / subDivToggle ) );

                            if ( CodeViewsWindow.CodeViewsWindow.GenerateObjects[key] != GenerateObjects_temp[key] )
                            {
                                GenerateObjects_temp[key] = CodeViewsWindow.CodeViewsWindow.GenerateObjects[key];
                                GenerateMap(OnlyExportMap, false);
                            }
                            
                            if ( idx == subDivToggle )
                            {
                                GUILayout.EndHorizontal();
                                GUILayout.BeginHorizontal();
                                idx = 0;
                            } else idx++;
                        }
                    GUILayout.EndHorizontal();

                GUILayout.EndVertical();

                foreach (string key in GenerateObjects_temp.Keys)
                {
                    GenerateObjects[key] = GenerateObjects_temp[key];
                }
            }

        GUILayout.BeginVertical("box");
        scroll = EditorGUILayout.BeginScrollView(scroll);
        GUILayout.TextArea(code_text, GUILayout.ExpandHeight(true));
        EditorGUILayout.EndScrollView();
        GUILayout.EndVertical();

        GUILayout.BeginVertical("box");
        
        if (GUILayout.Button("Copy To Clipboard"))
            GenerateMap(OnlyExportMap, true);
        GUILayout.EndVertical();
    }

    void DataTableGUI()
    {
        GUILayout.BeginVertical("box");
        SetCorrectColor(datatablecount);
        GUILayout.Label("Entity Count: " + datatablecount, EditorStyles.boldLabel);
        SetCorrectEntityLabel(datatablecount);
        GUILayout.EndVertical();

        GUI.contentColor = Color.white;

        GUILayout.BeginVertical("box");
        scroll = EditorGUILayout.BeginScrollView(scroll);
        GUILayout.TextArea(code_text, GUILayout.ExpandHeight(true));
        EditorGUILayout.EndScrollView();
        GUILayout.EndVertical();

        GUILayout.BeginVertical("box");
        if (GUILayout.Button("Copy To Clipboard"))
            GenerateDataTable(true);
        GUILayout.EndVertical();
    }

    void PrecacheGUI()
    {
        GUILayout.BeginVertical("box");
        scroll = EditorGUILayout.BeginScrollView(scroll);
        GUILayout.TextArea(code_text, GUILayout.ExpandHeight(true));
        EditorGUILayout.EndScrollView();
        GUILayout.EndVertical();

        GUILayout.BeginVertical("box");
        if (GUILayout.Button("Copy To Clipboard"))
            GeneratePrecacheCode(true);
        GUILayout.EndVertical();
    }

    void EntCodeGUI()
    {
        GUILayout.BeginVertical("box");
        SetCorrectColor(scriptentcount);
        GUILayout.Label("Entity Count: " + scriptentcount, EditorStyles.boldLabel);
        SetCorrectEntityLabel(scriptentcount);
        GUILayout.EndVertical();

        GUI.contentColor = Color.white;

        GUILayout.BeginVertical("box");
        scroll = EditorGUILayout.BeginScrollView(scroll);
        GUILayout.TextArea(code_text, GUILayout.ExpandHeight(true));
        EditorGUILayout.EndScrollView();
        GUILayout.EndVertical();

        GUILayout.BeginVertical("box");
        if (GUILayout.Button("Copy To Clipboard"))
            GenerateEntCode(true);
        GUILayout.EndVertical();
    }

    void SoundEntGUI()
    {
        GUILayout.BeginVertical("box");
        SetCorrectColor(soundentcount);
        GUILayout.Label("Entity Count: " + soundentcount, EditorStyles.boldLabel);
        SetCorrectEntityLabel(soundentcount);
        GUILayout.EndVertical();

        GUI.contentColor = Color.white;

        GUILayout.BeginVertical("box");
        UseOriginOffset = EditorGUILayout.Toggle("Add a origin offset", UseOriginOffset);
        if(UseOriginOffset) OriginOffset = EditorGUILayout.Vector3Field("Origin Offset", OriginOffset);
        GUILayout.EndVertical();

        GUILayout.BeginVertical("box");
        scroll = EditorGUILayout.BeginScrollView(scroll);
        GUILayout.TextArea(code_text, GUILayout.ExpandHeight(true));
        EditorGUILayout.EndScrollView();
        GUILayout.EndVertical();

        GUILayout.BeginVertical("box");
        if (GUILayout.Button("Copy To Clipboard"))
            GenerateSoundEntCode(true);
        GUILayout.EndVertical();
    }

    void NewLocPairGUI()
    {
        Helper.UseStartingOffset = UseStartingOffsetLocPair;

        GUI.contentColor = Color.white;

        GUILayout.BeginVertical("box");
        GUILayout.BeginHorizontal();
        UseOriginOffset = EditorGUILayout.Toggle("Add a origin offset", UseOriginOffset);
        if(UseOriginOffset) OriginOffset = EditorGUILayout.Vector3Field("Origin Offset", OriginOffset);

        UseStartingOffsetLocPair = EditorGUILayout.Toggle("Use Map Origin Offset", UseStartingOffsetLocPair);
        if(UseStartingOffsetLocPair != UseStartingOffsetLocPair_temp) {
            UseStartingOffsetLocPair_temp = UseStartingOffsetLocPair;
            GenerateNewLocPairCode(false);
        }

        GUILayout.EndHorizontal();
        GUILayout.EndVertical();

        GUILayout.BeginVertical("box");
        scroll = EditorGUILayout.BeginScrollView(scroll);
        GUILayout.TextArea(code_text, GUILayout.ExpandHeight(true));
        EditorGUILayout.EndScrollView();
        GUILayout.EndVertical();

        GUILayout.BeginVertical("box");
        if (GUILayout.Button("Copy To Clipboard"))
            GenerateNewLocPairCode(true);
        GUILayout.EndVertical();
    }

    void GenerateMap(bool onlyMapCode, bool copyCode)
    {
        Helper.FixPropTags();

        Helper.UseStartingOffset = UseStartingOffset;
        Helper.ShowStartingOffset = ShowStartingOffset;

        string mapcode = Helper.ReMapCredit() + "\n" + $"void function {SceneManager.GetActiveScene().name.Replace(" ", "_")}()" + "\n{\n" +  Helper.ShouldAddStartingOrg( StartingOriginType.SquirrelFunction );
        if(onlyMapCode)
            mapcode = Helper.ShouldAddStartingOrg( StartingOriginType.SquirrelFunction );

        //Build Map Code
        mapcode += Helper.BuildMapCode( BuildType.Script,
            Helper.GetBoolFromGenerateObjects( ObjectType.Prop ), Helper.GetBoolFromGenerateObjects( ObjectType.ZipLine ), Helper.GetBoolFromGenerateObjects( ObjectType.LinkedZipline ), Helper.GetBoolFromGenerateObjects( ObjectType.VerticalZipLine ), Helper.GetBoolFromGenerateObjects( ObjectType.NonVerticalZipLine ),
            Helper.GetBoolFromGenerateObjects( ObjectType.SingleDoor ), Helper.GetBoolFromGenerateObjects( ObjectType.DoubleDoor ), Helper.GetBoolFromGenerateObjects( ObjectType.HorzDoor ), Helper.GetBoolFromGenerateObjects( ObjectType.VerticalDoor ), Helper.GetBoolFromGenerateObjects( ObjectType.Button ),
            Helper.GetBoolFromGenerateObjects( ObjectType.Jumppad ), Helper.GetBoolFromGenerateObjects( ObjectType.LootBin ), Helper.GetBoolFromGenerateObjects( ObjectType.WeaponRack ), Helper.GetBoolFromGenerateObjects( ObjectType.Trigger ), Helper.GetBoolFromGenerateObjects( ObjectType.BubbleShield ),
            Helper.GetBoolFromGenerateObjects( ObjectType.SpawnPoint ), Helper.GetBoolFromGenerateObjects( ObjectType.TextInfoPanel ), Helper.GetBoolFromGenerateObjects( ObjectType.FuncWindowHint ), Helper.GetBoolFromGenerateObjects( ObjectType.Sound )
        );

        if(!onlyMapCode)
            mapcode += "}";
     
        if(copyCode) {
            GUIUtility.systemCopyBuffer = mapcode;
            mapcode = "";
            return;
        }

        code_text = mapcode;
        mapcode = "";

        ReMapConsole.Log("[Code Views] Map Code Generated", ReMapConsole.LogType.Success);
    }

    void GenerateDataTable(bool copycode_text)
    {
        Helper.FixPropTags();
        EditorSceneManager.SaveOpenScenes();

        string tableCode = BuildObjectsWithEnum( ObjectType.Prop, BuildType.DataTable );

        if (copycode_text) {
            GUIUtility.systemCopyBuffer = tableCode;
            tableCode = "";
            return;
        }
        
        code_text = tableCode;
        tableCode = "";

        ReMapConsole.Log("[Code Views] Datatable Code Generated", ReMapConsole.LogType.Success);
    }

    void GeneratePrecacheCode(bool copycode_text)
    {
        Helper.FixPropTags();
        EditorSceneManager.SaveOpenScenes();

        string precacheCode = Build_.Props( null, Build_.BuildType.Precache );
     
        if(copycode_text) {
            GUIUtility.systemCopyBuffer = precacheCode;
            precacheCode = "";
            return;
        }

        code_text = precacheCode;
        precacheCode = "";

        ReMapConsole.Log("[Code Views] Precache Code Generated", ReMapConsole.LogType.Success);
    }

    void GenerateEntCode(bool copycode_text)
    {
        Helper.FixPropTags();
        EditorSceneManager.SaveOpenScenes();

        string entCode = Build_.Props( null, Build_.BuildType.Ent );

        if(copycode_text) {
            GUIUtility.systemCopyBuffer = entCode;
            entCode = "";
            return;
        }
        
        code_text = entCode;
        entCode = "";

        ReMapConsole.Log("[Code Views] Script.ent Code Generated", ReMapConsole.LogType.Success);
    }

    void GenerateSoundEntCode(bool copycode_text)
    {
        Helper.FixPropTags();
        EditorSceneManager.SaveOpenScenes();

        string entCode = Build_.Sounds();

        if(copycode_text) {
            GUIUtility.systemCopyBuffer = entCode;
            entCode = "";
            return;
        }
        
        code_text = entCode;
        entCode = "";

        ReMapConsole.Log("[Code Views] Sound.ent Code Generated", ReMapConsole.LogType.Success);
    }

    void GenerateNewLocPairCode(bool copycode_text)
    {
        Helper.FixPropTags();
        EditorSceneManager.SaveOpenScenes();

        string entCode = Build_.NewLocPair();

        if(copycode_text) {
            GUIUtility.systemCopyBuffer = entCode;
            entCode = "";
            return;
        }
        
        code_text = entCode;
        entCode = "";

        ReMapConsole.Log("[Code Views] NewLocPair Code Generated", ReMapConsole.LogType.Success);
    }

    private static bool IsIgnored( string key )
    {
        foreach ( string ignoredObj in CodeViewsWindow.CodeViewsWindow.GenerateIgnore )
        {
            if ( ignoredObj == key ) return true;
        }

        return false;
    }

    private static void GetEditorWindowSize()
    {
        EditorWindow editorWindow = windowInstance;
        if ( editorWindow != null )
        {
            windowSize = new Vector2( editorWindow.position.width, editorWindow.position.height );
        }
    }
}