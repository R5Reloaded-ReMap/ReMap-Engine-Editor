using System;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;
using System.Reflection;
using System.Text;
using UnityEngine;
using ReMap.Global;
using ReMap.Build;
using ReMap.ImportExport;
using UnityEditor;
using Unity.VisualScripting;

public enum eObjectType // GLOBAL ENUM
{
    Prop
}

namespace ReMap.Build
{
    public enum eBuildType
    {
        Script,
        EntFile,
        Precache,
        LiveMap
    }
}

namespace ReMap.Global
{
    static class ReMapGlobalRegistry
    {
        class ReMapObjectInfo
        {
            public Type MonoType;
            public string Tag;
        }

        static readonly Dictionary<eObjectType, ReMapObjectInfo> Types = new ();

        public static Type GetMonoType(eObjectType type)
        {
            return Types.TryGetValue(type, out var info) ? info.MonoType : null;
        }

        public static void RegisterMonoType(eObjectType type, Type monoType)
        {
            if (!Types.ContainsKey(type))
            {
                Types[type] = new ReMapObjectInfo()
                {
                    MonoType = monoType,
                    Tag = type.ToString()
                };
            }
        }

        public static void UnregisterMonoType(eObjectType type)
        {
            if (Types.ContainsKey(type))
                Types.Remove(type);
        }

        public static List<string> GetTags()
        {
            List<string> tags = new ();
            
            foreach (var kvp in Types)
                tags.Add(kvp.Value.Tag);

            return tags;
        }
    }
    
    public interface IReMapObject // GLOBAL INTERFACE
    {
        eObjectType Type { get; }
        abstract string TagName { get; }
        abstract string ObjectName { get; }
        abstract string ObjectsName { get; }
        abstract string ObjectNameDesc { get; }
        bool IsUnityOnly { get; }

        // Fill GlobalClassData (or derived) with data from the MonoBehaviour
        void FillData(GlobalClassData data);

        // Apply data from GlobalClassData (or derived) to the MonoBehaviour
        void ApplyData(GlobalClassData data);

        // Generate code for building the object
        void Build(StringBuilder sb, eBuildType buildType);
    }

    public abstract class ReMapObjectBase : MonoBehaviour, IReMapObject // GLOBAL ABSTRACT CLASS
    {
        public abstract eObjectType Type { get; }
        public abstract string TagName { get; }
        public abstract string ObjectName { get; }
        public abstract string ObjectsName { get; }
        public abstract string ObjectNameDesc { get; }
        public abstract void FillData(GlobalClassData data);
        public abstract void ApplyData(GlobalClassData data);
        public abstract void Build(StringBuilder sb, eBuildType buildType);
        public virtual bool IsUnityOnly { get; } = false;
    }
}

namespace ReMap.ImportExport
{
    
    /// <summary>
    ///     Use this to create derivative classes
    /// </summary>
    [Serializable]
    public class GlobalClassData
    {
        eObjectType ObjectType;
        public List< PathClass > Path;
        public string PathString;
        public TransformData TransformData;
    }
    
    public enum ReMapVarType
    {
        Int,
        Bool,
        Float,
        String,
        Vector3
    }

    [Serializable]
    public class ComponentVar
    {
        public string Name;
        public ReMapVarType Type;
        public string Value;
    }

    [Serializable]
    public class DynamicData : GlobalClassData
    {
        public List<ComponentVar> Vars = new();
    }

    [AttributeUsage(AttributeTargets.Field)]
    public class ReMapVarAttribute : Attribute
    {
        public string Name { get; }
        public ReMapVarType Type { get; }

        public ReMapVarAttribute(string name, ReMapVarType type)
        {
            Name = name;
            Type = type;
        }
    }

    public static class DynamicDataHelper
    {
        public static void FillFromObject(object target, List<ComponentVar> vars)
        {
            vars.Clear();

            foreach (var field in target.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
            {
                var attr = field.GetCustomAttribute<ReMapVarAttribute>();
                if (attr == null)
                    continue;

                object value = field.GetValue(target);
                vars.Add(new ComponentVar
                {
                    Name = attr.Name,
                    Type = attr.Type,
                    Value = Serialize(value, attr.Type)
                });
            }
        }

        public static void ApplyToObject(object target, List<ComponentVar> vars)
        {
            var byName = vars.ToDictionary(v => v.Name, v => v);

            foreach (var field in target.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
            {
                var attr = field.GetCustomAttribute<ReMapVarAttribute>();
                if (attr == null || !byName.TryGetValue(attr.Name, out var cv))
                    continue;

                field.SetValue(target, Deserialize(cv.Value, attr.Type));
            }
        }

        static string Serialize(object value, ReMapVarType type)
        {
            switch (type)
            {
                case ReMapVarType.Bool:
                    return value is bool b ? b.ToString() : bool.FalseString;
                case ReMapVarType.Int:
                    return value is int i ? i.ToString() : "0";
                case ReMapVarType.Float:
                    return value is float f ? f.ToString(CultureInfo.InvariantCulture) : "0";
                case ReMapVarType.String:
                    return value as string ?? string.Empty;
                case ReMapVarType.Vector3:
                    return value is Vector3 v ? $"{v.x.ToString(CultureInfo.InvariantCulture)},{v.y.ToString(CultureInfo.InvariantCulture)},{v.z.ToString(CultureInfo.InvariantCulture)}" : "0,0,0";
                default:
                    return string.Empty;
            }
        }

        static object Deserialize(string value, ReMapVarType type)
        {
            switch (type)
            {
                case ReMapVarType.Bool:
                    return bool.TryParse(value, out var b) && b;
                case ReMapVarType.Int:
                    return int.TryParse(value, out var i) ? i : 0;
                case ReMapVarType.Float:
                    return float.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out var f) ? f : 0f;
                case ReMapVarType.String:
                    return value;
                case ReMapVarType.Vector3:
                    return ParseVector3(value);
                default:
                    return null;
            }
        }

        static Vector3 ParseVector3(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return Vector3.zero;

            var parts = value.Split(',');
            if (parts.Length != 3)
                return Vector3.zero;

            float x = float.TryParse(parts[0], NumberStyles.Float, CultureInfo.InvariantCulture, out var xf) ? xf : 0f;
            float y = float.TryParse(parts[1], NumberStyles.Float, CultureInfo.InvariantCulture, out var yf) ? yf : 0f;
            float z = float.TryParse(parts[2], NumberStyles.Float, CultureInfo.InvariantCulture, out var zf) ? zf : 0f;

            return new Vector3(x, y, z);
        }
    }

    /// <summary>
    ///     TransformData is used to determine the position / angles of a prefab
    /// </summary>
    [Serializable]
    public class TransformData
    {
        public Vector3 eulerAngles;
        public Vector3 localScale;
        public Vector3 position;
    }

    /// <summary>
    ///     Save the part of a path for the creation of a prefab
    /// </summary>
    [Serializable]
    public class PathClass
    {
        public string FolderName;
        public TransformData TransformData;
    }

    [Serializable]
    public class JsonData
    {
        public List< List< GlobalClassData > > Lists = new ();
    }

    public static class DataHelper
    {
        public static T CreateData<T>(eObjectType type) where T : GlobalClassData
        {
            Type classType = ReMapGlobalRegistry.GetMonoType(type);
            if (classType == null)
                return null;

            return Activator.CreateInstance(classType) as T;
        }

        public static void AddToJson( JsonData json, eObjectType type, GlobalClassData data )
        {
            Type classType = ReMapGlobalRegistry.GetMonoType( type );
            if ( classType == null || !classType.IsInstanceOfType( data ) )
                return;

            var propertyName = type.ToString() + "s";
            var propertyInfo = typeof( JsonData ).GetProperty( propertyName );
            
            if ( propertyInfo != null )
            {
                var list = propertyInfo.GetValue(json);
                if ( list == null )
                {
                    var listType = typeof( List<> ).MakeGenericType( data.GetType() );
                    list = Activator.CreateInstance( listType );
                    propertyInfo.SetValue( json, list );
                }
            
                var addMethod = list.GetType().GetMethod( "Add" );
                addMethod?.Invoke( list, new object[] { data } );
            }
        }
    }
}
