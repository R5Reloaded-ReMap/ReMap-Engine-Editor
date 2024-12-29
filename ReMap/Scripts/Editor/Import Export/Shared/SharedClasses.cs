using System;
using System.Collections.Generic;
using UnityEngine;

namespace ImportExport
{
    [Serializable]
    public class JsonData
    {
        public List< AnimatedCameraClassData > AnimatedCameras;
        public List< BubbleShieldClassData > BubbleShields;
        public List< ButtonClassData > Buttons;
        public List< CameraPathClassData > CameraPaths;
        public List< CheckPointClassData > CheckPoints;
        public List< DoubleDoorClassData > DoubleDoors;
        public List< FuncWindowHintClassData > FuncWindowHints;
        public List< HorzDoorClassData > HorizontalDoors;
        public List< JumppadClassData > JumpPads;
        public List< JumpTowerClassData > JumpTowers;
        public List< LinkedZipLinesClassData > LinkedZiplines;
        public List< LootBinClassData > LootBins;
        public List< NewLocPairClassData > NewLocPairs;
        public List< NonVerticalZipLineClassData > NonVerticalZipLines;
        public List< UOPlayerSpawnClassData > PlayerSpawns;
        public List< PropClassData > Props;
        public List< RespawnableHealClassData > RespawnableHeals;
        public List< SingleDoorClassData > SingleDoors;
        public List< SoundClassData > Sounds;
        public List< SpawnPointClassData > SpawnPoints;
        public List< SpeedBoostClassData > SpeedBoosts;
        public List< TextInfoPanelClassData > TextInfoPanels;
        public List< TriggerClassData > Triggers;
        public string Version;
        public List< VerticalDoorClassData > VerticalDoors;
        public List< VerticalZipLineClassData > VerticalZipLines;
        public List< WeaponRackClassData > WeaponRacks;
        public List< ZipLineClassData > Ziplines;
    }

    /// <summary>
    ///     Use this to create derivative classes
    /// </summary>
    [Serializable]
    public class GlobalClassData
    {
        public List< PathClass > Path;
        public string PathString;
        public TransformData TransformData;
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
    public class PropClassData : GlobalClassData
    {
        public bool AllowMantle;
        public bool ClientSide;

        public float FadeDistance;

        // If any changes are made here, do the same for JumppadClassData ( except string Name && bool ClientSide )
        public string Name;
        public string Options;
        public int RealmID;
    }

    [Serializable]
    public class ZipLineClassData : GlobalClassData
    {
        public bool ShowZipline;
        public float ShowZiplineDistance;
        public Vector3 Zipline_end;
        public Vector3 Zipline_start;
    }

    [Serializable]
    public class LinkedZipLinesClassData : GlobalClassData
    {
        public bool EnableSmoothing;
        public List< Vector3 > Nodes;
        public int SmoothAmount;
        public bool SmoothType;
    }

    [Serializable]
    public class VerticalZipLineClassData : GlobalClassData
    {
        public float AnglesOffset;
        public float ArmOffset;
        public float AutoDetachEnd;
        public float AutoDetachStart;
        public bool DetachEndOnSpawn;
        public bool DetachEndOnUse;
        public bool DropToBottom;
        public bool EnableAutoOffsetDistance;
        public float FadeDistance;
        public float HeightOffset;
        public bool IsMoving;
        public float LengthScale;
        public string Name;
        public int PanelMaxUse;
        public List< VCPanelsClassData > Panels;
        public float PanelTimerMax;
        public float PanelTimerMin;
        public bool PreserveVelocity;
        public bool PushOffInDirectionX;
        public bool RestPoint;
        public float Scale;
        public bool ShowArmOffset;
        public bool ShowAutoDetachDistance;
        public bool ShowZipline;
        public float ShowZiplineDistance;
        public float SpeedScale;
        public float Width;
    }

    [Serializable]
    public class NonVerticalZipLineClassData : GlobalClassData
    {
        public float ArmOffsetEnd;
        public float ArmOffsetStart;
        public float AutoDetachEnd;
        public float AutoDetachStart;
        public bool DetachEndOnSpawn;
        public bool DetachEndOnUse;
        public bool DropToBottom;
        public float FadeDistance;
        public bool IsMoving;
        public float LengthScale;
        public string Name;
        public int PanelMaxUse;
        public List< VCPanelsClassData > Panels;
        public float PanelTimerMax;
        public float PanelTimerMin;
        public bool PreserveVelocity;
        public bool PushOffInDirectionX;
        public bool RestPoint;
        public float Scale;
        public bool ShowArmOffsetEnd;
        public bool ShowArmOffsetStart;
        public bool ShowAutoDetachDistance;
        public bool ShowZipline;
        public float ShowZiplineDistance;
        public float SpeedScale;
        public float Width;
        public TransformData ZiplineEnd;
        public TransformData ZiplineStart;
    }

    [Serializable]
    public class VCPanelsClassData : GlobalClassData
    {
        public string Model;
    }

    [Serializable]
    public class SingleDoorClassData : GlobalClassData
    {
        public bool AppearOpen;
        public bool GoldDoor;
    }

    [Serializable]
    public class DoubleDoorClassData : GlobalClassData
    {
        public bool AppearOpen;
        public bool GoldDoor;
    }

    [Serializable]
    public class HorzDoorClassData : GlobalClassData
    {
        public bool AppearOpen;
    }

    [Serializable]
    public class VerticalDoorClassData : GlobalClassData
    {
        public bool AppearOpen;
    }

    [Serializable]
    public class JumpTowerClassData : GlobalClassData
    {
        public float Height;
        public bool ShowZipline;
        public float ShowZiplineDistance;
    }

    [Serializable]
    public class ButtonClassData : GlobalClassData
    {
        public string OnUseCallback;
        public string UseText;
    }

    [Serializable]
    public class JumppadClassData : GlobalClassData
    {
        // Uses the same classes as PropClassData ( except string Name )
        public bool AllowMantle;
        public float FadeDistance;
        public string Options;
        public int RealmID;
    }

    [Serializable]
    public class LootBinClassData : GlobalClassData
    {
        public LootBinSkinType LootbinSkin;
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
        public float Height;
        public TransformData HelperData;
        public string LeaveCallback;
        public bool UseHelperForTP;
        public float Width;
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
        public string Description;
        public float Scale;
        public bool showPIN;
        public string Title;
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
        public bool Enable;
        public bool IsWaveAmbient;
        public Vector3[] PolylineSegment;
        public float Radius;
        public bool ShowPolylineSegments;
        public float ShowPolylineSegmentsDistance;
        public string SoundName;
    }

    [Serializable]
    public class CameraPathClassData : GlobalClassData
    {
        public bool EnableSpacing;
        public float Fov;
        public List< TransformData > PathNode;
        public bool ShowPath;
        public float ShowPathDistance;
        public float Spacing;
        public float SpeedTransition;
        public TransformData TargetRef;
        public bool TrackTarget;
    }

    [Serializable]
    public class NewLocPairClassData : GlobalClassData
    {
        // Stub script
    }

    [Serializable]
    public class UOPlayerSpawnClassData : GlobalClassData
    {
        // Stub script
    }

    [Serializable]
    public class RespawnableHealClassData : GlobalClassData
    {
        public int HealAmount;
        public float HealDuration;
        public string Name;
        public bool Progressive;
        public float RespawnTime;
    }

    [Serializable]
    public class SpeedBoostClassData : GlobalClassData
    {
        public Color32 Color;
        public float Duration;
        public float FadeTime;
        public float RespawnTime;
        public float Strengh;
    }

    [Serializable]
    public class AnimatedCameraClassData : GlobalClassData
    {
        public float AngleOffset;
        public float MaxLeft;
        public float MaxRight;
        public float RotationTime;
        public float TransitionTime;
    }

    [Serializable]
    public class CheckPointClassData : GlobalClassData
    {
        // Stub class
    }

    [Serializable]
    public class EmptyClassData : GlobalClassData
    {
        // Stub script
    }
}