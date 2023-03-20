using System.Net;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class JsonData
{
    public List<PropScriptClassData> Props;
    public List<DrawZiplineClassData> ZipLines;
}

[Serializable]
public class PropScriptClassData
{
    public string Name;
    public TransformData TransformData;
    public PropScriptData ComponentData;
    public string PathString;
    public List<PathClass> Path;
    
}

[Serializable]
public class PropScriptData
{
    public bool allowMantle;
    public float fadeDistance;
    public int realmID;
    public List<PropScriptParameters> parameters;
    public List<string> customParameters;
}

[Serializable]
public class DrawZiplineClassData
{
    public DrawZiplineData DrawZipline;
    public string PathString;
    public List<PathClass> Path;
    
}

[Serializable]
public class DrawZiplineData
{
    public Vector3 start_position;
    public Vector3 end_position;
    public bool ShowZipline;
    public float ShowZiplineDistance;
}


/// <summary>
/// TransformData is used to determine the position / angles of a prefab
/// </summary>
[Serializable]
public class TransformData
{
    public Vector3 position;
    public Vector3 eulerAngles;
    public Vector3 localScale;
}
