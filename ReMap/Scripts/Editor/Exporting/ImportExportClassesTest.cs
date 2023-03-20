using System.Net;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class JsonData
{
    public List<PropScriptClassData> Props;
}

[Serializable]
public class PropScriptClassData
{
    public string Name;
    public TransformData Transform;
    public PropScriptData PropScript;
    public string PathString;
    public List<PathClass> Path;
    
}

[Serializable]
public class TransformData
{
    public Vector3 position;
    public Vector3 eulerAngles;
    public Vector3 localScale;
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
