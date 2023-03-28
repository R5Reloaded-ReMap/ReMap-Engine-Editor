
using System.Net;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace ImportExport.Shared
{
    [Serializable]
    public class JsonData
    {
        public List< PropClassData > Props;
        public List< ZipLineClassData > Ziplines;
        public List< LinkedZipLinesClassData > LinkedZiplines;
        public List< VerticalZipLineClassData > VerticalZipLines;
        public List< NonVerticalZipLineClassData > NonVerticalZipLines;
        public List< SingleDoorClassData > SingleDoors;
        public List< DoubleDoorClassData > DoubleDoors;
        public List< HorzDoorClassData > HorzDoors;
        public List< VerticalDoorClassData > VerticalDoors;
        public List< ButtonClassData > Buttons;
        public List< JumppadClassData > Jumppads;
        public List< LootBinClassData > LootBins;
        public List< WeaponRackClassData > WeaponRacks;
        public List< TriggerClassData > Triggers;
        public List< BubbleShieldClassData > BubbleShields;
        public List< SpawnPointClassData > SpawnPoints;
        public List< NewLocPairClassData > NewLocPairs;
        public List< TextInfoPanelClassData > TextInfoPanels;
        public List< FuncWindowHintClassData > FuncWindowHints;
        public List< SoundClassData > Sounds;
    }

    /// <summary>
    /// Use this to create derivative classes
    /// </summary>
    [Serializable]
    public class GlobalClassData
    {
        public TransformData TransformData;
        public string PathString;
        public List< PathClass > Path;

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

    /// <summary>
    /// Save the part of a path for the creation of a prefab
    /// </summary>
    [Serializable]
    public class PathClass
    {
        public string FolderName;
        public TransformData TransformData;
    }

    [Serializable]
    public class PropClassData : GlobalClassData
    {
        // If any changes are made here, do the same for JumppadClassData ( except string Name )
        public string Name;
        public bool AllowMantle;
        public float FadeDistance;
        public int RealmID;
        public PropScriptOptions Option;
    }

    [Serializable]
    public class ZipLineClassData : GlobalClassData
    {
        public bool ShowZipline;
        public float ShowZiplineDistance;
        public Vector3 Zipline_start;
        public Vector3 Zipline_end;
    }

    [Serializable]
    public class LinkedZipLinesClassData : GlobalClassData
    {
        public bool EnableSmoothing;
        public int SmoothAmount;
        public bool SmoothType;
        public List< Vector3 > Nodes;
    }

    [Serializable]
    public class VerticalZipLineClassData : GlobalClassData
    {
        public bool ShowZipline;
        public float ShowZiplineDistance;
        public bool ShowAutoDetachDistance;
        public bool EnableAutoOffsetDistance;
        public string Name;
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
        public List< VCPanelsClassData > Panels;
        public float PanelTimerMin;
        public float PanelTimerMax;
        public int PanelMaxUse;
        public bool ShowArmOffset;
    }

    [Serializable]
    public class NonVerticalZipLineClassData : GlobalClassData
    {
        public bool ShowZipline;
        public float ShowZiplineDistance;
        public bool ShowAutoDetachDistance;
        public string Name;
        public TransformData ZiplineStart;
        public TransformData ZiplineEnd;
        public float ArmOffsetStart;
        public float ArmOffsetEnd;
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
        public List< VCPanelsClassData > Panels;
        public float PanelTimerMin;
        public float PanelTimerMax;
        public int PanelMaxUse;
        public bool ShowArmOffsetStart;
        public bool ShowArmOffsetEnd;
    }

    [Serializable]
    public class VCPanelsClassData : GlobalClassData
    {
        public string Model;
    }

    [Serializable]
    public class SingleDoorClassData : GlobalClassData
    {
        public bool GoldDoor;
    }

    [Serializable]
    public class DoubleDoorClassData : GlobalClassData
    {
        public bool GoldDoor;
    }

    [Serializable]
    public class HorzDoorClassData : GlobalClassData
    {
        // Stub class
    }

    [Serializable]
    public class VerticalDoorClassData : GlobalClassData
    {
        // Stub class
    }

    [Serializable]
    public class ButtonClassData : GlobalClassData
    {
        public string UseText;
        public string OnUseCallback;
    }

    [Serializable]
    public class JumppadClassData : GlobalClassData
    {
        // Uses the same classes as PropClassData ( except string Name )
        public bool AllowMantle;
        public float FadeDistance;
        public int RealmID;
        public PropScriptOptions Option;
    }

    [Serializable]
    public class LootBinClassData : GlobalClassData
    {
        public int LootbinSkin;
    }

    [Serializable]
    public class WeaponRackClassData : GlobalClassData
    {
        public string Name;
        public float RespawnTime;
    }

    [Serializable]
    public class TriggerClassData : GlobalClassData
    {
        public bool Debug;
        public string EnterCallback;
        public string LeaveCallback;
    }

    [Serializable]
    public class BubbleShieldClassData : GlobalClassData
    {
        public string Name;
        public Color32 ShieldColor;
    }

    [Serializable]
    public class SpawnPointClassData : GlobalClassData
    {
        // Stub class
    }

    [Serializable]
    public class TextInfoPanelClassData : GlobalClassData
    {
        public string Title;
        public string Description;
        public bool showPIN;
        public float Scale;
    }

    [Serializable]
    public class FuncWindowHintClassData : GlobalClassData
    {
        public float HalfHeight;
        public float HalfWidth;
        public Vector3 Right;
    }

    [Serializable]
    public class SoundClassData : GlobalClassData
    {
        public bool ShowPolylineSegments;
        public float ShowPolylineSegmentsDistance;
        public float Radius;
        public bool IsWaveAmbient;
        public bool Enable;
        public string SoundName;
        public Vector3[] PolylineSegment;
    }

    [Serializable]
    public class NewLocPairClassData : GlobalClassData
    {
        // Stub script
    }
}
