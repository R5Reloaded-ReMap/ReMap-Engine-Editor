
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

using ImportExport.Shared;
using static ImportExport.Shared.SharedFunction;
using Build;
using static Build.Build;
using static CodeViewsWindow.CodeViewsWindow;

public enum StartingOriginType
{
    SquirrelFunction = 0,
    Function = 1
}

public enum StringType
{
    ObjectRef = 0,
    TagName = 1,
    Name = 2
}

public enum PathType
{
    Path = 0,
    Name = 1,
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
    JumpTower,
    Button,
    Jumppad,
    LootBin,
    WeaponRack,
    Trigger,
    BubbleShield,
    NewLocPair,
    SpawnPoint,
    TextInfoPanel,
    FuncWindowHint,
    Sound,
    CameraPath,

    // Unity Only
    LiveMapCodePlayerSpawn
}

public class Helper
{
    public static int maxBuildLength = 75000;

    private static readonly Dictionary< ObjectType, ObjectTypeData > _objectTypeData = new Dictionary< ObjectType, ObjectTypeData >
    {
        { ObjectType.BubbleShield,           new ObjectTypeData( new[] { "mdl#fx#bb_shield",             "BubbleShield",       "Bubble Shield"        }, typeof( BubbleScript ),           typeof( BubbleShieldClassData ) ) },
        { ObjectType.Button,                 new ObjectTypeData( new[] { "custom_button",                "Button",             "Button"               }, typeof( ButtonScripting ),        typeof( ButtonClassData ) ) },
        { ObjectType.CameraPath,             new ObjectTypeData( new[] { "custom_camera_path",           "CameraPath",         "Camera Path"          }, typeof( PathScript ),             typeof( CameraPathClassData ) ) },
        { ObjectType.DoubleDoor,             new ObjectTypeData( new[] { "custom_double_door",           "DoubleDoor",         "Double Door"          }, typeof( DoorScript ),             typeof( DoubleDoorClassData ) ) },
        { ObjectType.FuncWindowHint,         new ObjectTypeData( new[] { "custom_window_hint",           "FuncWindowHint",     "Window Hint"          }, typeof( WindowHintScript ),       typeof( FuncWindowHintClassData ) ) },
        { ObjectType.HorzDoor,               new ObjectTypeData( new[] { "custom_sliding_door",          "HorzDoor",           "Horizontal Door"      }, typeof( HorzDoorScript ),         typeof( HorzDoorClassData ) ) },
        { ObjectType.Jumppad,                new ObjectTypeData( new[] { "custom_jumppad",               "Jumppad",            "Jump Pad"             }, typeof( PropScript ),             typeof( JumppadClassData ) ) },
        { ObjectType.JumpTower,              new ObjectTypeData( new[] { "custom_jump_tower",            "JumpTower",          "Jump Tower"           }, typeof( JumpTowerScript ),        typeof( JumpTowerClassData ) ) },
        { ObjectType.LinkedZipline,          new ObjectTypeData( new[] { "custom_linked_zipline",        "LinkedZipline",      "Linked Zipline"       }, typeof( LinkedZiplineScript ),    typeof( LinkedZipLinesClassData ) ) },
        { ObjectType.LootBin,                new ObjectTypeData( new[] { "custom_lootbin",               "LootBin",            "Loot Bin"             }, typeof( LootBinScript ),          typeof( LootBinClassData ) ) },
        { ObjectType.NewLocPair,             new ObjectTypeData( new[] { "custom_new_loc_pair",          "NewLocPair",         "New Loc Pair"         }, typeof( NewLocPairScript ),       typeof( NewLocPairClassData ) ) },
        { ObjectType.NonVerticalZipLine,     new ObjectTypeData( new[] { "_non_vertical_zipline",        "NonVerticalZipLine", "Non Vertical ZipLine" }, typeof( DrawNonVerticalZipline ), typeof( NonVerticalZipLineClassData ) ) },
        { ObjectType.Prop,                   new ObjectTypeData( new[] { "mdl",                          "Prop",               "Prop"                 }, typeof( PropScript ),             typeof( PropClassData ) ) },
        { ObjectType.SingleDoor,             new ObjectTypeData( new[] { "custom_single_door",           "SingleDoor",         "Single Door"          }, typeof( DoorScript ),             typeof( SingleDoorClassData ) ) },
        { ObjectType.Sound,                  new ObjectTypeData( new[] { "custom_sound",                 "Sound",              "Sound"                }, typeof( SoundScript ),            typeof( SoundClassData ) ) },
        { ObjectType.SpawnPoint,             new ObjectTypeData( new[] { "custom_info_spawnpoint_human", "SpawnPoint",         "Spawn Point"          }, typeof( SpawnPointScript ),       typeof( SpawnPointClassData ) ) },
        { ObjectType.TextInfoPanel,          new ObjectTypeData( new[] { "custom_text_info_panel",       "TextInfoPanel",      "Text Info Panel"      }, typeof( TextInfoPanelScript ),    typeof( TextInfoPanelClassData ) ) },
        { ObjectType.Trigger,                new ObjectTypeData( new[] { "trigger_cylinder",             "Trigger",            "Trigger"              }, typeof( TriggerScripting ),       typeof( TriggerClassData ) ) },
        { ObjectType.VerticalDoor,           new ObjectTypeData( new[] { "custom_vertical_door",         "VerticalDoor",       "Vertical Door"        }, typeof( VerticalDoorScript ),     typeof( VerticalDoorClassData ) ) },
        { ObjectType.VerticalZipLine,        new ObjectTypeData( new[] { "_vertical_zipline",            "VerticalZipLine",    "Vertical ZipLine"     }, typeof( DrawVerticalZipline ),    typeof( VerticalZipLineClassData ) ) },
        { ObjectType.WeaponRack,             new ObjectTypeData( new[] { "custom_weaponrack",            "WeaponRack",         "Weapon Rack"          }, typeof( WeaponRackScript ),       typeof( WeaponRackClassData ) ) },
        { ObjectType.ZipLine,                new ObjectTypeData( new[] { "custom_zipline",               "ZipLine",            "ZipLine"              }, typeof( DrawZipline ),            typeof( ZipLineClassData ) ) },

        // Unity Only
        { ObjectType.LiveMapCodePlayerSpawn, new ObjectTypeData( new[] { "unityonly_player_spawn",       "LMCPlayerSpawn",     "Player Spawn ( UO )"  }, typeof( EmptyScript ),            typeof( UOPlayerSpawnClassData ) ) }
    };

    private static readonly List< ObjectType > ObjectToTagPriorities = new List< ObjectType >
    { 
        ObjectType.Prop,
        ObjectType.BubbleShield,
        ObjectType.VerticalZipLine,
        ObjectType.NonVerticalZipLine
    };

    public static readonly Dictionary< string, string > ObjectToTag = ObjectToTagDictionaryInit();

    // Gen Settings
    public static readonly Dictionary< string, bool > GenerateObjects = ObjectGenerateDictionaryInit();
    // Always Hide Unity Only Objects
    public static readonly ObjectType[] GenerateIgnoreStatic = new ObjectType[]
    {
        ObjectType.LiveMapCodePlayerSpawn
    };
    public static ObjectType[] GenerateIgnore = new ObjectType[0];

    public static bool UseStartingOffset()
    {
        return CodeViewsWindow.MenuInit.IsEnable( CodeViewsWindow.CodeViewsWindow.OffsetMenuOffset );
    }
    public static bool ShowStartingOffset()
    {
        return CodeViewsWindow.MenuInit.IsEnable( CodeViewsWindow.CodeViewsWindow.OffsetMenuShowOffset );
    }

    public static void SetUseStartingOffset( bool value )
    {
        CodeViewsWindow.MenuInit.SetBool( CodeViewsWindow.CodeViewsWindow.OffsetMenuOffset, value );
    }
    public static void SetShowStartingOffset( bool value )
    {
        CodeViewsWindow.MenuInit.SetBool( CodeViewsWindow.CodeViewsWindow.OffsetMenuShowOffset, value );
    }

    public struct NewDataTable
    {
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
    public static string ShouldAddStartingOrg( StartingOriginType type = StartingOriginType.Function, float x = 0, float y = 0, float z = 0, bool addSpace = true )
    {
        string vector = $"< {ReplaceComma( x )}, {ReplaceComma( y )}, {ReplaceComma( z )} >";
        string space = addSpace ? "    " : "";

        switch ( type )
        {
            case StartingOriginType.SquirrelFunction:
                if ( UseStartingOffset() && ShowStartingOffset() )
                return $"{space}//Starting Origin, Change this to a origin in a map \n{space}vector startingorg = {vector}" + "\n\n";
                break;

            case StartingOriginType.Function:
                if ( UseStartingOffset() )
                return " + startingorg";
                break;

            default: break;
        }

        return "";
    }

    /// <summary>
    /// Builds correct ingame angles from GameObject
    /// </summary>
    public static string BuildAngles( GameObject go, bool isEntFile = false )
    {
        return BuildAngles( go.transform.eulerAngles, isEntFile );
    }

    /// <summary>
    /// Builds correct ingame angles from Vector3
    /// </summary>
    public static string BuildAngles( Vector3 vec, bool isEntFile = false )
    {
        string x = ReplaceComma( -WrapAngle( vec.x ) );
        string y = ReplaceComma( -WrapAngle( vec.y ) );
        string z = ReplaceComma( WrapAngle( vec.z ) );

        string angles = $"< {x}, {y}, {z} >";

        if ( isEntFile ) angles = $"{x} {y} {z}";

        return angles;
    }

    public static string BuildRightVector( Vector3 vec, bool isEntFile = false )
    {
        string x = ReplaceComma( WrapAngle( vec.z ) );
        string y = ReplaceComma( WrapAngle( vec.x ) );
        string z = ReplaceComma( -WrapAngle( vec.y ) );

        string angles = $"< {x}, {y}, {z} >";

        if ( isEntFile ) angles = $"{x} {y} {z}";

        return angles;
    }

    /// <summary>
    /// Wraps Angles that are above 180
    /// </summary>
    /// <param name="angle">Angle to wrap</param>
    /// <returns></returns>
    public static float WrapAngle( float angle )
    {
        angle %= 360;

        if( angle > 180 ) return angle - 360;
 
        return angle;
    }

    /// <summary>
    /// Builds correct ingame origin from GameObject
    /// </summary>
    public static string BuildOrigin( GameObject go, bool isEntFile = false, bool returnWithOffset = false )
    {
        return BuildOrigin( go.transform.position, isEntFile, returnWithOffset );
    }

    /// <summary>
    /// Builds correct ingame origin from Vector3
    /// </summary>
    public static string BuildOrigin( Vector3 vec, bool isEntFile = false, bool returnWithOffset = false )
    {
        float xOffset = UseStartingOffset() && returnWithOffset ? 0 : StartingOffset.x;
        float yOffset = UseStartingOffset() && returnWithOffset ? 0 : StartingOffset.y;
        float zOffset = UseStartingOffset() && returnWithOffset ? 0 : StartingOffset.z;

        string x = ReplaceComma( -vec.z + xOffset );
        string y = ReplaceComma( vec.x + yOffset );
        string z = ReplaceComma( vec.y + zOffset );

        string origin = $"< {x}, {y}, {z} >";

        if ( isEntFile ) origin = $"{x} {y} {z}";

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

            foreach ( string key in ObjectToTag.Keys )
            {
                if ( go.name.Contains( key ) ) go.tag = ObjectToTag[key];
            }
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
    /// Build Map Code
    /// </summary>
    /// <returns>Map Code as string</returns>
    public static async Task< string > BuildMapCode( BuildType buildType = BuildType.Script, bool Selection = false )
    {
        // Order of importance
        StringBuilder code = new StringBuilder();

        foreach ( ObjectType objectType in GetAllObjectType() )
        {
            if ( GetBoolFromGenerateObjects( objectType ) ) AppendCode( ref code, await BuildObjectsWithEnum( objectType, buildType, Selection ), 0 );
        }

        return code.ToString();
    }

    public static void ApplyComponentScriptData< T >( T target, T source ) where T : Component
    {
        Type type = typeof( T );
        FieldInfo[] fields = type.GetFields( BindingFlags.Public | BindingFlags.Instance );

        foreach ( FieldInfo field in fields )
        {
            object value = field.GetValue( source );
            field.SetValue( target, value );
        }
    }

    public static string GetRandomGUIDForEnt()
    {
        return Guid.NewGuid().ToString().Replace( "-", "" ).Substring( 0, 16 );
    }

    public static string[] GetAllTags()
    {
        return GetAllObjectType().Select( type => GetObjTagNameWithEnum( type ) ).ToArray();
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

    public static Type GetImportExportClassByEnum( ObjectType objectType )
    {
        if ( _objectTypeData.TryGetValue( objectType, out ObjectTypeData objectTypeData ) && objectTypeData != null )
        {
            return objectTypeData.ImportExportClass;
        }

        return null;
    }

    public static ObjectType? GetObjectTypeByObjName( string searchTerm )
    {
        foreach ( ObjectType objectType in GetAllObjectType() )
        {
            if ( Helper.GetObjNameWithEnum( objectType ) == searchTerm ) return objectType;
        }

        return null;
    }

    private class ObjectTypeData
    {
        public string[] StringData { get; }
        public System.Type ComponentType { get; }
        public Type ImportExportClass { get; }

        public ObjectTypeData( string[] stringData, System.Type componentType, Type importExportClass )
        {
            if ( stringData.Length != 3 )
            {
                throw new ArgumentException( "stringData must have exactly 3 elements", nameof( stringData ) );
            }

            StringData = stringData;
            ComponentType = componentType;
            ImportExportClass = importExportClass;
        }
    }

    private static Dictionary< string, string > ObjectToTagDictionaryInit()
    {
        var dictionary = ObjectToTagPriorities.ToDictionary( objectType => GetObjRefWithEnum( objectType ), objectType => GetObjTagNameWithEnum( objectType ) );

        foreach ( ObjectType objectType in GetAllObjectType() )
        {
            string key = GetObjRefWithEnum( objectType );

            if ( !dictionary.ContainsKey( key ) )
            {
                dictionary.Add( key, GetObjTagNameWithEnum( objectType ) );
            }
        }

        return dictionary;
    }

    private static Dictionary< string, bool > ObjectGenerateDictionaryInit()
    {
        return GetAllObjectType().ToDictionary( objectType => GetObjNameWithEnum( objectType ), objectType => true );
    }

    public static ObjectType[] GetAllObjectType()
    {
        return ( ObjectType[] ) Enum.GetValues( typeof( ObjectType ) );
    }

    public static bool GetBoolFromGenerateObjects( ObjectType objectType )
    {
        return GenerateObjects[ GetObjNameWithEnum( objectType ) ];
    }

    public static void ForceSetBoolToGenerateObjects( ObjectType[] array, bool value )
    {
        foreach ( ObjectType objectType in array )
        {
            GenerateObjects[ GetObjNameWithEnum( objectType ) ] = value;
        }
    }

    /// <summary>
    /// Forces objects not to appear in code, if forceShow is true, this return the opposite of the array specifier
    /// </summary>
    public static void ForceHideBoolToGenerateObjects( ObjectType[] array, bool forceShow = false )
    {
        var objectTypeArray = forceShow ?
        GetAllObjectType().Where( objectType => !array.Contains( objectType ) ).ToList() :
        array.ToList();

        objectTypeArray.AddRange( GenerateIgnoreStatic );
    
        GenerateIgnore = objectTypeArray.ToArray();
    }

    public static GameObject[] GetAllObjectTypeWithEnum( ObjectType[] objectTypes, bool selectionOnly = false )
    {
        List< GameObject > list = new List< GameObject >();

        foreach ( ObjectType objectType in objectTypes )
        {
            list.AddRange( GetAllObjectTypeWithEnum( objectType, selectionOnly ).ToList() );
        }

        return list.ToArray();
    }

    public static GameObject[] GetAllObjectTypeWithEnum( ObjectType objectType, bool selectionOnly = false )
    {
        if ( selectionOnly )
        {
            GameObject[] SelectedObject = Selection.gameObjects
            .Where( obj => obj.CompareTag( Helper.GetObjTagNameWithEnum( objectType ) ) )
            .SelectMany( obj => obj.GetComponentsInChildren< Transform >( true ) )
            .Where( child => child.gameObject.CompareTag( Helper.GetObjTagNameWithEnum( objectType ) ) )
            .Select( child => child.gameObject )
            .Concat( Selection.gameObjects.Where( obj => obj.transform.childCount > 0 )
            .SelectMany( obj => obj.GetComponentsInChildren< Transform >( true ) )
            .Where( child => child.gameObject.CompareTag( Helper.GetObjTagNameWithEnum( objectType ) ) )
            .Select( child => child.gameObject ) )
            .Distinct().ToArray();

            return SelectedObject;
        }

        return UnityInfo.GetAllGameObjectInScene().Where( obj => obj.CompareTag( Helper.GetObjTagNameWithEnum( objectType ) ) ).ToArray();
    }

    public static GameObject[] AppendMultipleObjectType( GameObject[][] objectsArray )
    {
        return objectsArray.SelectMany( objects => objects ).ToArray();
    }

    public static void SelectAllObjectTypeInScene( ObjectType objectType, bool selectionOnly = false )
    {
        ChangeSelection( GetAllObjectTypeWithEnum( objectType, selectionOnly ) );
    }

    public static void ChangeSelection( GameObject[][] array )
    {
        ChangeSelection( AppendMultipleObjectType( array ) );
    }

    public static void ChangeSelection( GameObject[] array )
    {
        Selection.objects = array;
    }

    public static GameObject CreateGameObject( string name = "", string path = "", PathType pathType = PathType.Path )
    {
        GameObject obj = null;
        
        if ( path == "" ) path = UnityInfo.relativePathEmptyPrefab;

        UnityEngine.Object loadedPrefabResource = AssetDatabase.LoadAssetAtPath( $"{path}", typeof( UnityEngine.Object ) ) as GameObject;

        switch ( pathType )
        {
            case PathType.Path:
                loadedPrefabResource = AssetDatabase.LoadAssetAtPath( $"{path}", typeof( UnityEngine.Object ) ) as GameObject;
                break;

            case PathType.Name:
                loadedPrefabResource = UnityInfo.FindPrefabFromName( path );
                break;

            default: return null;
        }

        if ( loadedPrefabResource == null ) return null;
        
        obj = PrefabUtility.InstantiatePrefab( loadedPrefabResource as GameObject ) as GameObject;

        if ( name != "" ) obj.name = name;

        return obj;
    }

    public static void IncrementEntityCount( int value = 1 )
    {
        CodeViewsWindow.CodeViewsWindow.EntityCount += value;
    }

    public static void RemoveEntityCount( int value = 1 )
    {
        CodeViewsWindow.CodeViewsWindow.EntityCount -= value;
    }

    public static void IncrementSendedEntityCount( int value = 1 )
    {
        CodeViewsWindow.CodeViewsWindow.SendedEntityCount += value;
    }

    public static void RemoveSendedEntityCount( int value = 1 )
    {
        CodeViewsWindow.CodeViewsWindow.SendedEntityCount -= value;
    }

    public static bool IsValid< T >( T obj ) where T : class
    {
        return obj != null;
    }

    public static void OverideWindowSize( float x, float y, float size )
    {
        GUILayout.Button( $"{x} x {y}", GUILayout.Width( size ) );
    }

    public static string ReplaceComma( float value, bool forceComma = false )
    {
        string str = value.ToString( "F4" ).TrimEnd( '0' ).Replace( ',', '.' ).TrimEnd( '.' );

        if ( !str.Contains('.') && forceComma ) str += ".0";

        return str;
    }

    public static string BoolToLower( bool value )
    {
        return value.ToString().ToLower();
    }

    public static string GetSceneName()
    {
        if ( SceneManager.GetActiveScene().name == "" ) return "Unnamed";

        return $"{SceneManager.GetActiveScene().name.Replace(" ", "_")}";
    }

    public static string ReMapCredit( bool noSpace = false )
    {
        StringBuilder credit = new StringBuilder();

        string space = noSpace ? "" : "    ";

        AppendCode( ref credit, $"{space}// Generated with Unity ReMap Editor {UnityInfo.ReMapVersion}" );
        AppendCode( ref credit, $"{space}// Made with love by AyeZee#6969 & Julefox#0050 :)", 2 );

        return credit.ToString();
    }
}
