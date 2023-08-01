
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

using Build;
using ImportExport;
using static Build.Build;
using static CodeViews.CodeViewsWindow;
using static ImportExport.SharedFunction;

public enum StartingOriginType
{
    SquirrelFunction = 0,
    Function = 1
}

public enum VectorType
{
    Unity = 0,
    Apex = 1
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
    RespawnableHeal,
    SpeedBoost,
    AnimatedCamera,
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
        { ObjectType.AnimatedCamera,         new ObjectTypeData( new[] { "animated_camera",              "AnimatedCamera",     "Animated Camera"      }, typeof( AnimatedCameraScript ),   typeof( AnimatedCameraClassData ) ) },
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
        { ObjectType.RespawnableHeal,        new ObjectTypeData( new[] { "custom_respawnable_heal_",     "RespawnableHeal",    "Respawnable Heal"     }, typeof( RespawnableHealScript ),  typeof( RespawnableHealClassData ) ) },
        { ObjectType.SingleDoor,             new ObjectTypeData( new[] { "custom_single_door",           "SingleDoor",         "Single Door"          }, typeof( DoorScript ),             typeof( SingleDoorClassData ) ) },
        { ObjectType.Sound,                  new ObjectTypeData( new[] { "custom_sound",                 "Sound",              "Sound"                }, typeof( SoundScript ),            typeof( SoundClassData ) ) },
        { ObjectType.SpawnPoint,             new ObjectTypeData( new[] { "custom_info_spawnpoint_human", "SpawnPoint",         "Spawn Point"          }, typeof( SpawnPointScript ),       typeof( SpawnPointClassData ) ) },
        { ObjectType.SpeedBoost,             new ObjectTypeData( new[] { "custom_speed_boost",           "SpeedBoost",         "Speed Boost"          }, typeof( SpeedBoostScript ),       typeof( SpeedBoostClassData ) ) },
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
    public static readonly Dictionary< ObjectType, bool > GenerateObjects = ObjectGenerateDictionaryInit();
    public static readonly Dictionary< ObjectType, bool > ObjectsToHide = new Dictionary< ObjectType, bool >( GenerateObjects );

    // All ObjectType Inside This [] Will Not Be Generated
    public static ObjectType[] GenerateIgnore = new ObjectType[0];

    // Always Hide Unity Only Objects
    public static readonly ObjectType[] GenerateIgnoreStatic = new ObjectType[]
    {
        ObjectType.LiveMapCodePlayerSpawn
    };

    // When Refreshing, Get All GameObjects In Scene
    public static GameObject[] ObjectsInScene = new GameObject[0];

    private static readonly Dictionary< string, string > LocalizedString = new Dictionary< string, string >
    {
        { "#FUNCTION_NAME_PRECACHE", $"{CodeViews.CodeViewsWindow.functionName}_Init" },
        { "#FUNCTION_NAME", CodeViews.CodeViewsWindow.functionName }
    };

    public static readonly Dictionary< string, ( string SearchTerm, Func< GameObject, string > ReplacementFunc ) > LocalizedStringTrigger = new Dictionary< string, ( string, Func< GameObject, string > ) >
    {
        [ "#TRIGGER_H_ORIGIN" ] = ( "#TRIGGER_H_ORIGIN", obj => obj != null && obj.activeSelf ? Helper.BuildOrigin( obj ) : "< 0, 0, 0 >" ),
        [ "#TRIGGER_H_ANGLES" ] = ( "#TRIGGER_H_ANGLES", obj => obj != null && obj.activeSelf ? Helper.BuildAngles( obj ) : "< 0, 0, 0 >" ),
        [ "#TRIGGER_H_OFFSET" ] = ( "#TRIGGER_H_OFFSET", obj => "+ startingorg" )
    };

    public static readonly Dictionary< string, string > BadChars = new Dictionary< string, string >
    {
        { "/", "" }, { "\\", "" }, { "\r", "" }, { "\n", "" }, { "-" , "_" }, { "[", "" }, { "]",  "" }, { "{", "" }, { "}", "" }, { "(", "" },
        { ")", "" }, { "!",  "" }, { "@",  "" }, { "#",  "" }, { "$",  ""  }, { "%", "" }, { "^",  "" }, { "&", "" }, { "*", "" }, { "=", "" },
        { "+", "" }, { "?",  "" }, { "<",  "" }, { ">",  "" }, { ",",  ""  }, { ".", "" }, { "\"", "" }, { "'", "" }, { ";", "" }, { ":", "" },
        { "`", "" }, { "~",  "" }, { "é",  "" }, { "è",  "" }, { "ê",  ""  }, { "á", "" }, { "à",  "" }, { "â", "" }, { "í", "" }, { "ì", "" },
        { "î", "" }
    };

    public static bool UseStartingOffset()
    {
        return CodeViews.MenuInit.IsEnable( CodeViews.CodeViewsWindow.OffsetMenuOffset );
    }
    
    public static bool ShowStartingOffset()
    {
        return CodeViews.MenuInit.IsEnable( CodeViews.CodeViewsWindow.OffsetMenuShowOffset );
    }

    public static void SetUseStartingOffset( bool value )
    {
        CodeViews.MenuInit.SetBool( CodeViews.CodeViewsWindow.OffsetMenuOffset, value );
    }

    public static void SetShowStartingOffset( bool value )
    {
        CodeViews.MenuInit.SetBool( CodeViews.CodeViewsWindow.OffsetMenuShowOffset, value );
    }

    public static void ReplaceLocalizedString( ref StringBuilder code )
    {
        code = LocalizedString.Aggregate( code, ( current, pair ) => current.Replace( pair.Key, pair.Value ) );
    }

    public static void ReplaceLocalizedString( ref string code )
    {
        code = LocalizedString.Aggregate( code, ( current, pair ) => current.Replace( pair.Key, pair.Value ) );
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
        string vector = $"< {ReplaceComma(x)}, {ReplaceComma(y)}, {ReplaceComma(z)} >";
        string space = addSpace ? "    " : "";

        if ( type == StartingOriginType.SquirrelFunction && UseStartingOffset() && ShowStartingOffset() )
            return $"{space}//Starting Origin, Change this to a origin in a map \n{space}vector startingorg = {vector}\n\n";

        if ( type == StartingOriginType.Function && UseStartingOffset() )
            return " + startingorg";

        return "";
    }

    public static string BuildAngles( GameObject go, VectorType vectorType )
    {
        return BuildAngles( go.transform.eulerAngles, false, vectorType );
    }

    /// <summary>
    /// Builds correct ingame angles from GameObject
    /// </summary>
    public static string BuildAngles( GameObject go, bool isEntFile = false, VectorType vectorType = VectorType.Apex )
    {
        return BuildAngles( go.transform.eulerAngles, isEntFile );
    }

    /// <summary>
    /// Builds correct ingame angles from Vector3
    /// </summary>
    public static string BuildAngles( Vector3 vec, bool isEntFile = false, VectorType vectorType = VectorType.Apex )
    {
        bool isUnity = vectorType == VectorType.Unity;

        string x = ReplaceComma( isUnity ? WrapAngle( vec.x ) : -WrapAngle( vec.x ) );
        string y = ReplaceComma( isUnity ? WrapAngle( vec.y ) : -WrapAngle( vec.y ) );
        string z = ReplaceComma( isUnity ? WrapAngle( vec.z ) :  WrapAngle( vec.z ) );

        return isEntFile ? $"{x} {y} {z}" : $"< {x}, {y}, {z} >";
    }

    public static string BuildRightVector( Vector3 vec, bool isEntFile = false, VectorType vectorType = VectorType.Apex )
    {
        string x = ReplaceComma(  WrapAngle( vec.z ) );
        string y = ReplaceComma(  WrapAngle( vec.x ) );
        string z = ReplaceComma( -WrapAngle( vec.y ) );

        return isEntFile ? $"{x} {y} {z}" : $"< {x}, {y}, {z} >";
    }

    public static Vector3 ConvertApexAnglesToUnity( Vector3 vec )
    {
        return new Vector3( -WrapAngle( vec.x ), -WrapAngle( vec.y ), WrapAngle( vec.z ) );
    }

    /// <summary>
    /// Wraps Angles that are above 180
    /// </summary>
    /// <param name="angle">Angle to wrap</param>
    /// <returns></returns>
    public static float WrapAngle( float angle )
    {
        angle %= 360;

        if ( angle > 180 ) return angle - 360;
 
        return angle;
    }

    public static string BuildOrigin( GameObject go, VectorType vectorType )
    {
        return BuildOrigin( go.transform.position, false, false, vectorType );
    }

    /// <summary>
    /// Builds correct ingame origin from GameObject
    /// </summary>
    public static string BuildOrigin( GameObject go, bool isEntFile = false, bool returnWithOffset = false, VectorType vectorType = VectorType.Apex )
    {
        return BuildOrigin( go.transform.position, isEntFile, returnWithOffset, vectorType );
    }

    /// <summary>
    /// Builds correct ingame origin from Vector3
    /// </summary>
    public static string BuildOrigin( Vector3 vec, bool isEntFile = false, bool returnWithOffset = false, VectorType vectorType = VectorType.Apex )
    {
        Vector3 offset = UseStartingOffset() && returnWithOffset ? StartingOffset : Vector3.zero;

        bool isUnity = vectorType == VectorType.Unity;

        string x = ReplaceComma( isUnity ? vec.x : -vec.z + offset.x );
        string y = ReplaceComma( isUnity ? vec.y :  vec.x + offset.y );
        string z = ReplaceComma( isUnity ? vec.z :  vec.y + offset.z );

        return isEntFile ? $"{x} {y} {z}" : $"< {x}, {y}, {z} >";
    }

    public static Vector3 ConvertApexOriginToUnity( Vector3 vec )
    {
        return new Vector3( vec.y, vec.z, -vec.x );
    }

    public static Vector3 StringToVector3( string line, bool isEnt = false )
    {
        if ( isEnt )
        {
            var matches = Regex.Matches( line, @"\s*(-?\d+(\.\d+)?)\s+(-?\d+(\.\d+)?)\s+(-?\d+(\.\d+)?)\s*" );
            if ( matches.Count == 0 ) return Vector3.zero;

            var values = matches[0].Value.Split( ' ' );

            return new Vector3
            (
                float.TryParse( values[0], NumberStyles.Float, CultureInfo.InvariantCulture, out float x ) ? x : 0,
                float.TryParse( values[1], NumberStyles.Float, CultureInfo.InvariantCulture, out float y ) ? y : 0,
                float.TryParse( values[2], NumberStyles.Float, CultureInfo.InvariantCulture, out float z ) ? z : 0
            );
        }
        else
        {
            var matches = Regex.Matches( line, "<\\s*(-?\\d+(\\.\\d+)?),\\s*(-?\\d+(\\.\\d+)?),\\s*(-?\\d+(\\.\\d+)?)\\s*>" );
            if ( matches.Count == 0 ) return Vector3.zero;

            var values = matches[0].Value.Replace("<", "").Replace(">", "").Split(',');

            return new Vector3
            (
                float.TryParse( values[0], NumberStyles.Float, CultureInfo.InvariantCulture, out float x ) ? x : 0,
                float.TryParse( values[1], NumberStyles.Float, CultureInfo.InvariantCulture, out float y ) ? y : 0,
                float.TryParse( values[2], NumberStyles.Float, CultureInfo.InvariantCulture, out float z ) ? z : 0
            );
        }
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
        var code = new StringBuilder();

        var objectTasks = GetAllObjectType().Where( GetBoolFromGenerateObjects ).Select
        (
            async objectType => AppendCode( ref code, await BuildObjectsWithEnum( objectType, buildType, Selection ), 0 )
        );

        await Task.WhenAll( objectTasks );

        return code.ToString();
    }

    public static void ApplyComponentScriptData< T >( T source, T target ) where T : Component
    {
        typeof( T ).GetFields( BindingFlags.Public | BindingFlags.Instance ).ToList().ForEach
        (
            f => f.SetValue( target, f.GetValue( source ) )
        );
    }

    public static void ApplyTransformData( GameObject source, GameObject target )
    {
        if ( !IsValid( source ) || !IsValid( target ) ) return;

        target.transform.position = source.transform.position;
        target.transform.eulerAngles = source.transform.eulerAngles;
        target.transform.localScale = source.transform.localScale;
        target.transform.parent = FindParent( source );
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
        return _objectTypeData.TryGetValue( objectType, out ObjectTypeData objectTypeData ) && IsValid( objectTypeData ) ? objectTypeData.StringData[ ( int ) stringType ] : string.Empty;
    }

    public static Component GetComponentByEnum( GameObject obj, ObjectType objectType ) 
    {
        return _objectTypeData.TryGetValue( objectType, out var objectTypeData ) ? obj.GetComponent( objectTypeData.ComponentType ) : null;
    }


    public static Type GetImportExportClassByEnum( ObjectType objectType )
    {
        return _objectTypeData.TryGetValue( objectType, out var objectTypeData ) ? objectTypeData.ImportExportClass : null;
    }

    public static ObjectType? GetObjectTypeByObjName( string searchTerm )
    {
        return GetAllObjectType().FirstOrDefault( objectType => GetObjNameWithEnum( objectType ) == searchTerm );
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
        return GetAllObjectType().ToDictionary
        (
            objectType => GetObjRefWithEnum( objectType ), 
            objectType => GetObjTagNameWithEnum( objectType )
        );
    }

    private static Dictionary< ObjectType, bool > ObjectGenerateDictionaryInit()
    {
        return GetAllObjectType().ToDictionary( objectType => objectType, objectType => true );
    }

    public static ObjectType[] GetAllObjectType()
    {
        return ( ObjectType[] ) Enum.GetValues( typeof( ObjectType ) );
    }

    public static bool GetBoolFromGenerateObjects( ObjectType objectType )
    {
        return GenerateObjects[ objectType ];
    }

    public static bool GetBoolFromObjectsToHide( ObjectType objectType )
    {
        return ObjectsToHide[ objectType ];
    }

    public static void ForceSetBoolToGenerateObjects( ObjectType[] array, bool value )
    {
        foreach ( ObjectType objectType in array )
        {
            GenerateObjects[ objectType ] = value;
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

    public static bool IsObjectTypeExistInScene( ObjectType objectType )
    {
        string nameRef = Helper.GetObjRefWithEnum( objectType );

        return Helper.ObjectsInScene.Any( obj => obj.name.Contains( nameRef ) );
    }

    public static GameObject[] GetAllObjectTypeInScene( bool selection = false )
    {
        return Helper.GetAllObjectTypeWithEnum( Helper.GetAllObjectType(), selection );
    }

    public static GameObject[] GetAllObjectTypeWithEnum( ObjectType[] objectTypes, bool selectionOnly = false )
    {
        return objectTypes.SelectMany( objectType => GetAllObjectTypeWithEnum( objectType, selectionOnly ) ).ToArray();
    }

    public static GameObject[] GetAllObjectTypeWithEnum( ObjectType objectType, bool selectionOnly = false )
    {
        if ( selectionOnly )
        {
            var tag = Helper.GetObjTagNameWithEnum( objectType );

            GameObject[] selectedObject = Selection.gameObjects
            .SelectMany( obj => obj.GetComponentsInChildren< Transform >( true ) )
            .Concat( Selection.gameObjects.Where( obj => obj.transform.childCount > 0 )
            .SelectMany( obj => obj.GetComponentsInChildren< Transform >( true ) ) )
            .Distinct()
            .Where( child => child.gameObject.CompareTag( tag ) )
            .Select( child => child.gameObject )
            .Distinct().ToArray();

            return selectedObject;
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

    public static void ChangeSelection( GameObject obj )
    {
        Selection.objects = new GameObject[] { obj  };
    }

    /// <summary>
    /// Create a GameObject inside an other GameObject.
    /// </summary>
    /// <remarks>
    /// scene/path/path2 => func( "obj", "path/path2" ) => scene/path/path2/obj
    /// </remarks>
    public static GameObject CreateGameObject( string name, string modelPath, GameObject parent, PathType pathType = PathType.Path )
    {
        GameObject obj = CreateGameObject( name, modelPath, pathType );

        if ( IsValid( obj ) && IsValid( parent ) )
        {
            obj.transform.position = parent.transform.position;
            obj.transform.parent = parent.transform;
        }

        return obj;
    }

    public static GameObject CreateGameObject( string name = "", string modelPath = "", PathType pathType = PathType.Path )
    {        
        modelPath = string.IsNullOrEmpty( modelPath ) ? UnityInfo.relativePathEmptyPrefab : modelPath;

        UnityEngine.Object loadedPrefabResource = pathType switch
        {
            PathType.Path => AssetDatabase.LoadAssetAtPath( $"{modelPath}", typeof( UnityEngine.Object ) ) as GameObject,
            PathType.Name => UnityInfo.FindPrefabFromName( modelPath ),
            _ => null
        };

        if ( !IsValid( loadedPrefabResource ) ) return null;

        var obj = PrefabUtility.InstantiatePrefab( loadedPrefabResource ) as GameObject;

        if ( !string.IsNullOrEmpty( name ) ) obj.name = name;

        return obj;
    }

    /// <summary>
    /// Create a GameObject as folder and move the wanted GameObject.
    /// </summary>
    /// <remarks>
    /// scene/obj => func( "newFolder", obj ) => scene/newFolder/obj
    /// </remarks>
    public static GameObject MoveGameObject( string name, GameObject obj, bool ignoreFind = false )
    {
        GameObject subFolder = null;

        if ( !ignoreFind )
            subFolder = GameObject.Find( CreatePathString( obj ) );
        
        if ( !IsValid( subFolder ) )
            subFolder = CreateGameObject( name, "", GetParent( obj ) );

        if ( IsValid( subFolder ) )
        {
            obj.transform.parent = subFolder.transform;
        }
        return subFolder;
    }

    /// <summary>
    /// Create a GameObject as folder and move the wanted GameObject in an other path.
    /// </summary>
    /// <remarks>
    /// scene/obj => func( obj, "aFolder/anOtherFolder" ) => scene/aFolder/anOtherFolder/obj
    /// </remarks>
    public static GameObject MoveGameObject( GameObject obj, string path )
    {
        return Helper.CreatePath( path, obj );
    }

    public static GameObject CreatePath( string pathString, GameObject obj = null )
    {
        if ( string.IsNullOrEmpty( pathString ) ) return null;

        GameObject folder = GameObject.Find( pathString ); string path = "";

        if ( IsValid( folder ) )
        {
            if ( IsValid( obj ) )
            {
                obj.transform.SetParent( folder.transform );
            }
            return folder;
        }

        string[] pathStrings = pathString.Split( '/' );

        foreach ( string paths in pathStrings )
        {
            if ( string.IsNullOrEmpty( paths ) )
            {
                path = $"{paths}";
            }
            else path = $"{path}/{paths}";

            GameObject newFolder = GameObject.Find( path );

            if ( !IsValid( newFolder ) ) newFolder = new GameObject( paths );

            if ( IsValid( folder ) ) newFolder.transform.SetParent( folder.transform );

            folder = newFolder;
        }

        if ( IsValid( folder ) && IsValid( obj ) ) obj.transform.SetParent( folder.transform );

        return folder;
    }

    /// <summary>
    /// Get path string for a GameObject
    /// </summary>
    public static string CreatePathString( GameObject obj, bool includeSelf = false )
    {
        return FindPathString( obj, includeSelf );
    }

    public static void SetOriginAndAngles( GameObject obj, Vector3 origin, bool local = false )
    {
        SetOriginAndAngles( obj, origin, Vector3.zero, local );
    }

    public static void SetOriginAndAngles( GameObject obj, Vector3 origin, Vector3 angles, bool local = false )
    {
        if ( IsValid( obj ) )
        {
            if ( local )
            {
                obj.transform.localPosition = origin;
                obj.transform.localEulerAngles = angles;
            }
            else
            {
                obj.transform.position = origin;
                obj.transform.eulerAngles = angles;
            }
        }
    }

    public static GameObject GetParent( GameObject obj )
    {
        return obj.transform.parent.gameObject;
    }

    public static GameObject[] FindGameObjectInSphere( Vector3 position, float radius = 0.0001f )
    {
        return Physics.OverlapSphere( position, radius ).Select( collider => collider.gameObject ).ToArray();
    }

    public static void GetObjectsInScene()
    {
        ObjectsInScene = UnityInfo.GetAllGameObjectInScene( SelectionEnable() );
    }

    public static void IncrementEntityCount( int value = 1 )
    {
        CodeViews.CodeViewsWindow.EntityCount += value;
    }

    public static void RemoveEntityCount( int value = 1 )
    {
        CodeViews.CodeViewsWindow.EntityCount -= value;
    }

    public static void IncrementSendedEntityCount( int value = 1 )
    {
        CodeViews.CodeViewsWindow.SendedEntityCount += value;
    }

    public static void RemoveSendedEntityCount( int value = 1 )
    {
        CodeViews.CodeViewsWindow.SendedEntityCount -= value;
    }

    public static bool IsObjectFromTag( GameObject obj, ObjectType objectType )
    {
        return obj.CompareTag( GetObjTagNameWithEnum( objectType ) );
    }

    public static bool IsObjectFromTag( GameObject obj, ObjectType[] objectTypes )
    {
        return objectTypes.Any( objectType => obj.CompareTag( GetObjTagNameWithEnum( objectType ) ) );
    }

    public static ObjectType GetTypeFromObject( GameObject obj )
    {
        return GetAllObjectType().FirstOrDefault( t => obj.CompareTag( GetObjTagNameWithEnum( t ) ) );
    }

    public static bool IsValid< T >( T obj ) where T : class
    {
        return obj != null;
    }

    public static bool IsValid< T >( T [] array ) where T : class
    {
        return array != null && array.All( item => item != null );
    }

    public static bool IsEmpty< T >( T [] obj ) where T : class
    {
        return obj.Length == 0;
    }

    public static bool IsEmpty< T >( List< T > obj ) where T : class
    {
        return obj.Count == 0;
    }

    public static bool LOD0_Exist( string name )
    {
        return File.Exists( $"{UnityInfo.currentDirectoryPath}/{UnityInfo.relativePathModel}/{name}_LOD0.fbx" );
    }

    public static bool Material_Exist( string name, bool extention = false )
    {
        string ext = extention ? ".dds" : "";
        return File.Exists($"{UnityInfo.currentDirectoryPath}/{UnityInfo.relativePathMaterials}/{name}{extention}");
    }

    public static bool MoveFile( string origin, string target, bool addDir = true )
    {
        string originPath = addDir ? $"{UnityInfo.currentDirectoryPath}/" : "";
        string targetPath = addDir ? $"{UnityInfo.currentDirectoryPath}/" : "";

        originPath += $"{origin}".Replace( "\\", "/" );
        targetPath += $"{target}".Replace( "\\", "/" );

        try
        {
            if ( File.Exists( originPath ) )
            {
                if ( File.Exists( targetPath ) )
                {
                    File.Delete( targetPath );
                }

                File.Move( originPath, targetPath );

                if ( File.Exists( $"{originPath}.meta" ) )
                {
                    if ( File.Exists( $"{targetPath}.meta" ) )
                    {
                        File.Delete( $"{targetPath}.meta" );
                    }

                    File.Move( $"{originPath}.meta", $"{targetPath}.meta" );
                }

                return true;
            }
        }
        catch ( Exception msg )
        {
            Ping( msg );
        }

        return false;
    }

    public static bool DeleteFile( string filePath, bool addDir = true )
    {
        filePath = addDir ? $"{UnityInfo.currentDirectoryPath}/{filePath}" : filePath;

        filePath = filePath.Replace( "\\", "/" );

        try
        {
            if ( File.Exists( filePath ) )
            {
                File.Delete( filePath );

                if ( File.Exists( $"{filePath}.meta" ) )
                {
                    File.Delete(  $"{filePath}.meta" );
                }

                return true;
            }
        }
        catch ( Exception msg )
        {
            Ping( msg );
        }

        return false;
    }

    public static void CreateDirectory( string path, bool addDir = true )
    {
        path = addDir ? $"{UnityInfo.currentDirectoryPath}/{path}" : path;

        if ( !Directory.Exists( path ) )
        {
            Directory.CreateDirectory( path );
        }
    }

    public static void DeleteDirectory( string path, bool addDir = true, bool self = true )
    {
        try
        {
            path = addDir ? $"{UnityInfo.currentDirectoryPath}/{path}" : path;

            if ( Directory.Exists( path ) )
            {
                if ( self )
                {
                    Directory.Delete( path, true );
                }
                else
                {
                    DirectoryInfo dirInfo = new DirectoryInfo( path );
                    
                    foreach (FileInfo file in dirInfo.GetFiles())
                    {
                        file.Delete(); 
                    }

                    foreach (DirectoryInfo dir in dirInfo.GetDirectories())
                    {
                        dir.Delete( true ); 
                    }
                }
            }
        }
        catch ( Exception msg )
        {
            Ping( msg );
        }
    }

    public static void RemoveNull< T >( ref T [] array ) where T : class
    {
        array = array.Where( x => x != null ).ToArray();
    }

    public static void ArrayResize< T >( ref T [] array, int value ) where T : class
    {
        if ( array.Length + value >= 0 )
        {
            Array.Resize( ref array, array.Length + value );
        }
        else
        {
            Array.Resize( ref array, array.Length - array.Length );
        }
    }

    public static void ArrayAppend< T >( ref T [] array, T obj ) where T : class
    {
        int currentLength = array.Length;
        ArrayResize( ref array, currentLength + 1 );
        array[ currentLength ] = obj;
    }

    public static void Ping( params object[] args )
    {
        UnityInfo.Printt( args.Length > 0 ? string.Join( " ", args.Select( arg => arg.ToString() ) ) : "Ping!" );
    }

    public static Transform FindParent( Transform go )
    {
        return FindParent( go.gameObject );
    }

    public static Transform FindParent( GameObject go )
    {
        return go.transform.parent;
    }

    public static bool CoinFlip()
    {
        return UnityEngine.Random.Range( 0, 2 ) != 0;
    }

    public static void OverideWindowSize( float x, float y, float size )
    {
        GUILayout.Button( $"{x} x {y}", GUILayout.Width( size ) );
    }

    public static string ReplaceComma( float value, bool forceComma = false )
    {
        string str = value.ToString( "F4" ).TrimEnd( '0' ).Replace( ',', '.' ).TrimEnd( '.' );

        return str.Contains( '.' ) || !forceComma ? str : $"{str}.0";
    }

    public static string BoolToLower( bool value )
    {
        return value.ToString().ToLower();
    }

    public static string RemoveSpacesAfterChars( string str )
    {
        return Regex.Replace( str, @"\s*([\[\](),<>])\s*", "$1" ); // replace spaces around [, (, <, >, ), ] or ,
    }

    public static string DeleteNewLine( string str )
    {
        return str.Replace( "\r", "" ).Replace( "\n", "" );
    }

    public static string GetSceneName()
    {
        var scene = SceneManager.GetActiveScene().name;
        return ( string.IsNullOrEmpty( scene ) ? "Unnamed" : scene.Replace( " ", "_" ) );
    }

    public static string ReplaceBadCharacters( string name )
    {
        return BadChars.Aggregate( name, ( current, badChar ) => current.Replace( badChar.Key, badChar.Value ) );
    }

    public static string ReMapCredit( bool noSpace = false )
    {
        StringBuilder credit = new StringBuilder();

        string space = noSpace ? "" : "    ";

        AppendCode( ref credit, $"{space}// Generated with Unity ReMap Editor {UnityInfo.ReMapVersion}" );
        AppendCode( ref credit, $"{space}// Made with love by AyeZee#6969 & Julefox#0050 :)", 2 );

        return credit.ToString();
    }

    public static string GetScopeName( [ System.Runtime.CompilerServices.CallerMemberName ] string memberName = "" )
    {
        return memberName;
    }

    public static Stopwatch CreateStopwatch()
    {
        Stopwatch stopwatch = new ();
        stopwatch.Start();
        return stopwatch;
    }

    public static void StopStopwatch( ref Stopwatch stopwatch )
    {
        stopwatch.Stop(); Helper.Ping( "Time Elapsed:", stopwatch.ElapsedMilliseconds, "ms" );
    }

    public static async Task Wait( double value = 0.00001 )
    {
        await Task.Delay( TimeSpan.FromSeconds( value ) );
    } 
}
