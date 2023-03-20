using System.Net;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class JsonData
{
    public List<PropClassData> Props;
    public List<ZipLineClassData> Ziplines;
}

/// <summary>
/// Use this to create derivative classes
/// </summary>
[Serializable]
public class GlobalClassData
{
    public TransformData TransformData;
    public string PathString;
    public List<PathClass> Path;
    
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

[Serializable]
public class PropClassData : GlobalClassData
{
    public string name;
    public bool allowMantle;
    public float fadeDistance;
    public int realmID;
    public List<PropScriptParameters> parameters;
    public List<string> customParameters;
}

[Serializable]
public class ZipLineClassData : GlobalClassData
{
    public bool showZipline;
    public float showZiplineDistance;
    public Vector3 zipline_start;
    public Vector3 zipline_end;
}

