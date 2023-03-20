using System.Net;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class JsonData
{
    public List<PropClassData> Props;
}

[Serializable]
public class GlobalClassData
{
    public TransformData TransformData;
    public string PathString;
    public List<PathClass> Path;
    
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
