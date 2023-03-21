using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEngine;

public enum StringType
{
    ObjectRef = 0,
    TagName = 1,
    Name = 2
}

public enum ObjectType
{
    // Order of importance
    Prop,
    ZipLine,
    LinkedZipline,
    VerticalZipLine,
    NonVerticalZipLine,
    SingleDoor,
    DoubleDoor,
    HorzDoor,
    VerticalDoor,
    Button,
    Jumppad,
    LootBin,
    WeaponRack,
    Trigger,
    BubbleShield,
    SpawnPoint,
    TextInfoPanel,
    FuncWindowHint,
    Sound
}

public class Helper
{
    public static int maxBuildLength = 75000;
    public static int greenPropCount = 1500;
    public static int yellowPropCount = 3000;
    public static bool Is_Using_Starting_Offset = false;
    public static bool DisableStartingOffsetString = false;

    private static readonly Dictionary< ObjectType, ObjectTypeData > _objectTypeData = new Dictionary< ObjectType, ObjectTypeData >
    {
        { ObjectType.BubbleShield,       new ObjectTypeData( new string[] { "mdl#fx#bb_shield",             "BubbleShield",       "Bubble Shield"        }, typeof( BubbleScript ) ) },
        { ObjectType.Button,             new ObjectTypeData( new string[] { "custom_button",                "Button",             "Button"               }, typeof( ButtonScripting ) ) },
        { ObjectType.DoubleDoor,         new ObjectTypeData( new string[] { "custom_double_door",           "DoubleDoor",         "Double Door"          }, typeof( DoorScript ) ) },
        { ObjectType.FuncWindowHint,     new ObjectTypeData( new string[] { "custom_window_hint",           "FuncWindowHint",     "Window Hint"          }, typeof( WindowHintScript ) ) },
        { ObjectType.HorzDoor,           new ObjectTypeData( new string[] { "custom_sliding_door",          "HorzDoor",           "Horizontal Door"      }, typeof( HorzDoorScript ) ) },
        { ObjectType.Jumppad,            new ObjectTypeData( new string[] { "custom_jumppad",               "Jumppad",            "Jump Pad"             }, typeof( PropScript ) ) },
        { ObjectType.LinkedZipline,      new ObjectTypeData( new string[] { "custom_linked_zipline",        "LinkedZipline",      "Linked Zipline"       }, typeof( LinkedZiplineScript ) ) },
        { ObjectType.LootBin,            new ObjectTypeData( new string[] { "custom_lootbin",               "LootBin",            "Loot Bin"             }, typeof( LootBinScript ) ) },
        { ObjectType.NonVerticalZipLine, new ObjectTypeData( new string[] { "_non_vertical_zipline",        "NonVerticalZipLine", "Non Vertical ZipLine" }, typeof( DrawNonVerticalZipline ) ) },
        { ObjectType.Prop,               new ObjectTypeData( new string[] { "mdl",                          "Prop",               "Prop"                 }, typeof( PropScript ) ) },
        { ObjectType.SingleDoor,         new ObjectTypeData( new string[] { "custom_single_door",           "SingleDoor",         "Single Door"          }, typeof( DoorScript ) ) },
        { ObjectType.Sound,              new ObjectTypeData( new string[] { "custom_sound",                 "Sound",              "Sound"                }, typeof( SoundScript ) ) },
        { ObjectType.SpawnPoint,         new ObjectTypeData( new string[] { "custom_info_spawnpoint_human", "SpawnPoint",         "Spawn Point"          }, typeof( SpawnPointScript ) ) },
        { ObjectType.TextInfoPanel,      new ObjectTypeData( new string[] { "custom_text_info_panel",       "TextInfoPanel",      "Text Info Panel"      }, typeof( TextInfoPanelScript ) ) },
        { ObjectType.Trigger,            new ObjectTypeData( new string[] { "trigger_cylinder",             "Trigger",            "Trigger"              }, typeof( TriggerScripting ) ) },
        { ObjectType.VerticalDoor,       new ObjectTypeData( new string[] { "custom_vertical_door",         "VerticalDoor",       "Vertical Door"        }, typeof( VerticalDoorScript ) ) },
        { ObjectType.VerticalZipLine,    new ObjectTypeData( new string[] { "_vertical_zipline",            "VerticalZipLine",    "Vertical ZipLine"     }, typeof( DrawVerticalZipline ) ) },
        { ObjectType.WeaponRack,         new ObjectTypeData( new string[] { "custom_weaponrack",            "WeaponRack",         "Weapon Rack"          }, typeof( WeaponRackScript ) ) },
        { ObjectType.ZipLine,            new ObjectTypeData( new string[] { "custom_zipline",               "ZipLine",            "ZipLine"              }, typeof( DrawZipline ) ) }
    };

    public static Dictionary<string, string> ObjectToTag = ObjectToTagDictionaryInit();

    // Gen Settings
    public static Dictionary<string, bool> GenerateObjects = ObjectGenerateDictionaryInit();
    public static string[] GenerateIgnore = new string[] { GetObjNameWithEnum( ObjectType.SpawnPoint ), GetObjNameWithEnum( ObjectType.FuncWindowHint ), GetObjNameWithEnum( ObjectType.Sound ) };

    public enum ExportType
    {
        WholeScriptOffset,
        MapOnlyOffset,
        WholeScript,
        MapOnly
    }

    public struct NewDataTable {
        public string Type;
        public Vector3 Origin;
        public Vector3 Angles;
        public float Scale;
        public string FadeDistance;
        public string canMantle;
        public string isVisible;
        public string Model;
        public string Collection;
    }

    /// <summary>
    /// Should add starting origin to object location
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static string ShouldAddStartingOrg(int type = 0)
    {
        if(!Is_Using_Starting_Offset)
            return "";

        if(type == 0)
            return " + startingorg";

        if(DisableStartingOffsetString)
            return "";

        return "    //Starting Origin, Change this to a origin in a map \n    vector startingorg = <0,0,0>" + "\n\n";
    }

    /// <summary>
    /// Builds correct angles from gameobject
    /// </summary>
    /// <param name="go">Prop Object</param>
    /// <returns></returns>
    public static string BuildAngles(GameObject go, bool isEntFile = false)
    {
        string x = (-WrapAngle(go.transform.eulerAngles.x)).ToString("F4").Replace(",", ".");
        string y = (-WrapAngle(go.transform.eulerAngles.y)).ToString("F4").Replace(",", ".");
        string z = (WrapAngle(go.transform.eulerAngles.z)).ToString("F4").Replace(",", ".");

        if ( x.Contains( ".0000" ) ) x = x.Replace( ".0000", "" );
        if ( y.Contains( ".0000" ) ) y = y.Replace( ".0000", "" );
        if ( z.Contains( ".0000" ) ) z = z.Replace( ".0000", "" );
                    
        string angles = $"< {x}, {y}, {z} >";

        if( isEntFile )
            angles = $"{x} {y} {z}";

        return angles;
    }

    /// <summary>
    /// Wraps Angles that are above 180
    /// </summary>
    /// <param name="angle">Angle to wrap</param>
    /// <returns></returns>
    public static float WrapAngle(float angle)
    {
        angle%=360;
        if(angle >180)
            return angle - 360;
 
        return angle;
    }

    /// <summary>
    /// Builds correct ingame origin from gameobject
    /// </summary>
    /// <param name="go">Prop Object</param>
    /// <returns></returns>
    public static string BuildOrigin(GameObject go, bool isEntFile = false)
    {
        float xOffset = 0;
        float yOffset = 0;
        float zOffset = 0;

        if (CodeViews.UseOriginOffset)
        {
            xOffset = CodeViews.OriginOffset.x;
            yOffset = CodeViews.OriginOffset.y;
            zOffset = CodeViews.OriginOffset.z;
        }

        string x = (-go.transform.position.z + zOffset).ToString("F4").Replace(",", ".");
        string y = (go.transform.position.x + xOffset).ToString("F4").Replace(",", ".");
        string z = (go.transform.position.y + yOffset).ToString("F4").Replace(",", ".");

        if ( x.Contains( ".0000" ) ) x = x.Replace( ".0000", "" );
        if ( y.Contains( ".0000" ) ) y = y.Replace( ".0000", "" );
        if ( z.Contains( ".0000" ) ) z = z.Replace( ".0000", "" );

        string origin = $"< {x}, {y}, {z} >";

        if( isEntFile )
            origin = $"{x} {y} {z}";

        return origin;
    }

    /// <summary>
    /// Builds correct ingame origin from vector3
    /// </summary>
    /// <param name="go">Prop Object</param>
    /// <returns></returns>
    public static string BuildOriginVector(Vector3 vec, bool isEntFile = false)
    {
        float xOffset = 0;
        float yOffset = 0;
        float zOffset = 0;

        if (CodeViews.UseOriginOffset)
        {
            xOffset = CodeViews.OriginOffset.x;
            yOffset = CodeViews.OriginOffset.y;
            zOffset = CodeViews.OriginOffset.z;
        }

        string x = (-vec.z + zOffset).ToString("F4").Replace(",", ".");
        string y = (vec.x + xOffset).ToString("F4").Replace(",", ".");
        string z = (vec.y + yOffset).ToString("F4").Replace(",", ".");

        if ( x.Contains( ".0000" ) ) x = x.Replace( ".0000", "" );
        if ( y.Contains( ".0000" ) ) y = y.Replace( ".0000", "" );
        if ( z.Contains( ".0000" ) ) z = z.Replace( ".0000", "" );

        string origin = $"< {x}, {y}, {z} >";

        if( isEntFile )
            origin = $"( {x} {y} {z} )";

        return origin;
    }

    /// <summary>
    /// Tags Custom Prefabs so users cant wrongly tag a item
    /// </summary>
    public static void FixPropTags()
    {
        //Retag All Objects
        foreach ( GameObject go in UnityInfo.GetAllGameObjectInScene() )
        {
            go.tag = "Untagged";

            foreach (string key in ObjectToTag.Keys)
                if (go.name.Contains(key))
                    go.tag = ObjectToTag[key];
        }
    }

    public static NewDataTable BuildDataTable(string item)
    {
        string[] items = item.Replace("\"", "").Split(char.Parse(","));

        NewDataTable dt = new NewDataTable();
        dt.Type = items[0];
        dt.Origin = new Vector3(float.Parse(items[2]), float.Parse(items[3].Replace(">", "")), -(float.Parse(items[1].Replace("<", ""))));
        dt.Angles = new Vector3(-(float.Parse(items[4].Replace("<", ""))), -(float.Parse(items[5])), float.Parse(items[6].Replace(">", "")));
        dt.Scale = float.Parse(items[7]);
        dt.FadeDistance = items[8];
        dt.canMantle = items[9];
        dt.isVisible = items[10];
        dt.Model = items[11].Replace("/", "#").Replace(".rmdl", "").Replace("\"", "").Replace("\n", "").Replace("\r", "");
        dt.Collection = items[12].Replace("\"", "");

        return dt;
    }

    public static void CreateDataTableItem(NewDataTable dt, UnityEngine.Object loadedPrefabResource)
    {
        GameObject obj = PrefabUtility.InstantiatePrefab(loadedPrefabResource as GameObject) as GameObject;
        obj.transform.position = dt.Origin;
        obj.transform.eulerAngles = dt.Angles;
        obj.name = dt.Model;
        obj.gameObject.transform.localScale = new Vector3(dt.Scale, dt.Scale, dt.Scale);
        obj.SetActive(dt.isVisible == "true");

        PropScript script = obj.GetComponent<PropScript>();
        script.FadeDistance = float.Parse(dt.FadeDistance);
        script.AllowMantle = dt.canMantle == "true";

        if (dt.Collection == "")
            return;

        GameObject parent = GameObject.Find(dt.Collection);
        if (parent != null)
            obj.gameObject.transform.parent = parent.transform;
    }

    public static List<String> BuildCollectionList(string[] items)
    {
        List<String> collectionList = new List<String>();
        foreach (string item in items)
        {
            string[] itemsplit = item.Replace("\"", "").Split(char.Parse(","));

            if (itemsplit.Length < 12)
                continue;

            string collection = itemsplit[12].Replace("\"", "");

            if (collection == "")
                continue;

            if (!collectionList.Contains(collection))
                collectionList.Add(collection);
        }

        return collectionList;
    }

    /// <summary>
    /// Builds Map Code
    /// </summary>
    /// <returns>built map code string</returns>
    public static string BuildMapCode(
    bool Prop = true,       bool ZipLine = true,       bool LinkedZipline = true,  bool VerticalZipLine = true, bool NonVerticalZipLine = true,
    bool SingleDoor = true, bool DoubleDoor = true,    bool HorzDoor = true,       bool VerticalDoor = true,    bool Button = true,
    bool Jumppad = true,    bool LootBin = true,       bool WeaponRack = true,     bool Trigger = true,         bool BubbleShield = true,
    bool SpawnPoint = true, bool TextInfoPanel = true, bool FuncWindowHint = true, bool Sound = true )
    {
        // Order of importance
        string code = "";
        if( Prop )                code += Build.Props( null, Build.BuildType.Map );
        if( ZipLine )             code += Build.ZipLines();
        if( LinkedZipline )       code += Build.LinkedZipLines();
        if( VerticalZipLine )     code += Build.VerticalZipLines();
        if( NonVerticalZipLine )  code += Build.NonVerticalZipLines();
        if( SingleDoor )          code += Build.SingleDoors();
        if( DoubleDoor )          code += Build.DoubleDoors();
        if( HorzDoor )            code += Build.HorizontalDoors();
        if( VerticalDoor )        code += Build.VertDoors();
        if( Button )              code += Build.Buttons();
        if( Jumppad )             code += Build.Jumpads();
        if( LootBin )             code += Build.LootBins();
        if( WeaponRack )          code += Build.WeaponRacks();
        if( Trigger )             code += Build.Triggers();
        if( BubbleShield )        code += Build.BubbleShields();
        //if( SpawnPoint )          code += Build.;
        if( TextInfoPanel )       code += Build.TextInfoPanel();
        //if( FuncWindowHint )      code += Build.;
        //if( Sound )               code += Build.;

        return code;
    }

    public static void ApplyComponentScriptData<T>(T target, T source) where T : Component
    {
        Type type = typeof(T);
        FieldInfo[] fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance);

        foreach (FieldInfo field in fields)
        {
            object value = field.GetValue(source);
            field.SetValue(target, value);
        }
    }

    /*
    public static void ApplyComponentScriptDataFromJson<T>(T target, T source) where T : Component
    {
        Type type = typeof(T);
        FieldInfo[] fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance);

        foreach (FieldInfo field in fields)
        {
            object value = field.GetValue(source);
            field.SetValue(target, value);
        }
    }
    */

    public static string GetRandomGUIDForEnt()
    {
        return Guid.NewGuid().ToString().Replace("-", "").Substring(0, 16);
    }

    public static GameObject[] GetObjArrayWithEnum( ObjectType objectType )
    {
        return GameObject.FindGameObjectsWithTag( GetObjTagNameWithEnum( objectType ) );
    }

    public static string GetObjRefWithEnum( ObjectType objectType )
    {
        return Internal_GetStringByEnum( objectType, StringType.ObjectRef );
    }

    public static string GetObjTagNameWithEnum( ObjectType objectType )
    {
        return Internal_GetStringByEnum( objectType, StringType.TagName );
    }

    public static string GetObjNameWithEnum( ObjectType objectType )
    {
        return Internal_GetStringByEnum( objectType, StringType.Name );
    }

    private static string Internal_GetStringByEnum( ObjectType objectType, StringType stringType )
    {
        if ( _objectTypeData.TryGetValue( objectType, out ObjectTypeData objectTypeData ) && objectTypeData != null )
        {
            return objectTypeData.StringData[ ( int ) stringType ];
        }

        throw new ArgumentOutOfRangeException( nameof( objectType ), objectType, "This ObjectType does not exist." );
    }

    public static Component GetComponentByEnum( GameObject obj, ObjectType objectType )
    {
        if ( _objectTypeData.TryGetValue( objectType, out ObjectTypeData objectTypeData ) && objectTypeData != null )
        {
            return obj.GetComponent( objectTypeData.ComponentType );
        }

        return null;
    }

    private class ObjectTypeData
    {
        public string[] StringData { get; }
        public System.Type ComponentType { get; }

        public ObjectTypeData( string[] stringData, System.Type componentType )
        {
            StringData = stringData;
            ComponentType = componentType;
        }
    }

    private static Dictionary< string, string > ObjectToTagDictionaryInit()
    {
        Dictionary< string, string > dictionary = new Dictionary< string, string >();

        foreach ( ObjectType objectType in Enum.GetValues( typeof( ObjectType ) ) )
        {
            dictionary.Add( GetObjRefWithEnum( objectType ), GetObjTagNameWithEnum( objectType ) );
        }

        return dictionary;
    }

    public static Dictionary< string, bool > ObjectGenerateDictionaryInit()
    {
        Dictionary< string, bool > dictionary = new Dictionary< string, bool >();

        foreach ( ObjectType objectType in Enum.GetValues( typeof( ObjectType ) ) )
        {
            dictionary.Add( GetObjNameWithEnum( objectType ), true );
        }

        return dictionary;
    }

    public static bool GetBoolFromGenerateObjects( ObjectType objectType )
    {
        return GenerateObjects[ GetObjNameWithEnum( objectType ) ];
    }

    public static string Credits = @"
//Made with Unity Map Editor
//By AyeZee#6969 & Julefox#0050
";
}