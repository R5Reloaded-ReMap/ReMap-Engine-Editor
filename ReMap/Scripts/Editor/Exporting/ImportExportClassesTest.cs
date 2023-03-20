using System.Net;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class JsonData
{
    public List< PropClassData > Props;
    public List< ZipLineClassData > Ziplines;
    public List< LinkedZipLinesClassData > LinkedZiplines;
    public List< VerticalZipLineClassData > VerticalZipLines;
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

[Serializable]
public class LinkedZipLinesClassData : GlobalClassData
{
    public bool enableSmoothing;
    public int smoothAmount;
    public bool smoothType;
    public List<Vector3> nodes;
}

[Serializable]
public class VerticalZipLineClassData : GlobalClassData
{
    public bool ShowZipline;
    public float ShowZiplineDistance;
    public bool ShowAutoDetachDistance;
    public bool EnableAutoOffsetDistance;
    public string ZiplineType;
    public float ArmOffset;
    public float HeightOffset;
    public float AnglesOffset;
    public float FadeDistance;
    public float Scale;
    public float Width;
    public float SpeedScale;
    public float LengthScale;
    public bool PreserveVelocity;
    public bool DropToBottom;
    public float AutoDetachStart;
    public float AutoDetachEnd;
    public bool RestPoint;
    public bool PushOffInDirectionX;
    public bool IsMoving;
    public bool DetachEndOnSpawn;
    public bool DetachEndOnUse;
    public List<VCPanelsClassData> Panels;
    public float PanelTimerMin;
    public float PanelTimerMax;
    public int PanelMaxUse;
}

[Serializable]
public class VCPanelsClassData : GlobalClassData
{
    public string Model;
}
