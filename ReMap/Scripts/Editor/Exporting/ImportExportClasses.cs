using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SaveJson
{
    public List<PropsClass> Props;
    public List<JumpPadsClass> JumpPads;
    public List<ButtonsClass> Buttons;
    public List<BubbleShieldsClass> BubbleShields;
    public List<WeaponRacksClass> WeaponRacks;
    public List<LootBinsClass> LootBins;
    public List<ZipLinesClass> ZipLines;
    public List<LinkedZipLinesClass> LinkedZipLines;
    public List<VerticalZipLinesClass> VerticalZipLines;
    public List<NonVerticalZipLinesClass> NonVerticalZipLines;
    public List<DoorsClass> Doors;
    public List<TriggersClass> Triggers;
    public List<SoundClass> Sounds;
}

[Serializable]
public class PropScriptClass
{
    public bool AllowMantle;
    public float FadeDistance;
    public int RealmID;
}

[Serializable]
public class PropsClass
{
    public string Name;
    public Vector3 Position;
    public Vector3 Rotation;
    public Vector3 Scale;
    public PropScriptClass script;
    public string Collection;
}

[Serializable]
public class JumpPadsClass
{
    public Vector3 Position;
    public Vector3 Rotation;
    public Vector3 Scale;
    public PropScriptClass script;
    public string Collection;
}

[Serializable]
public class ButtonsClass
{
    public Vector3 Position;
    public Vector3 Rotation;
    public string UseText;
    public string OnUseCallback;
    public string Collection;
}

[Serializable]
public class BubbleShieldsClass
{
    public Vector3 Position;
    public Vector3 Rotation;
    public Vector3 Scale;
    public string Color;
    public string Model;
    public string Collection;
}

[Serializable]
public class WeaponRacksClass
{
    public Vector3 Position;
    public Vector3 Rotation;
    public string Weapon;
    public float RespawnTime;
    public string Collection;
}

[Serializable]
public class LootBinsClass
{
    public Vector3 Position;
    public Vector3 Rotation;
    public int Skin;
    public string Collection;
}

[Serializable]
public class ZipLinesClass
{
    public Vector3 Start;
    public Vector3 End;
    public string Collection;
}

[Serializable]
public class LinkedZipLinesClass
{
    public bool IsSmoothed;
    public bool SmoothType;
    public int SmoothAmount;
    public List<Vector3> Nodes;
    public string Collection;
}

[Serializable]
public class VerticalZipLinesClass
{
    public string ZiplineType;
    public Vector3 ZiplinePosition;
    public Vector3 ZiplineAngles;
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
    public VCPanelsClass[] Panels;
    public float PanelTimerMin;
    public float PanelTimerMax;
    public int PanelMaxUse;
    public string Collection;
}

[Serializable]
public class NonVerticalZipLinesClass
{
    public string ZiplineType;
    public Vector3 ZiplineStartPosition;
    public Vector3 ZiplineStartAngles;
    public Vector3 ZiplineEndPosition;
    public Vector3 ZiplineEndAngles;
    public float ArmStartOffset;
    public float ArmEndOffset;
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
    public VCPanelsClass[] Panels;
    public float PanelTimerMin;
    public float PanelTimerMax;
    public int PanelMaxUse;
    public string Collection;
}

[Serializable]
public class VCPanelsClass
{
    public string Model;
    public Vector3 Position;
    public Vector3 Angles;
    public string Collection;
}

[Serializable]
public class DoorsClass
{
    public Vector3 Position;
    public Vector3 Rotation;
    public string Type;
    public bool Gold;
    public string Collection;
}

[Serializable]
public class TriggersClass
{
    public Vector3 Position;
    public Vector3 Rotation;
    public string EnterCallback;
    public string ExitCallback;
    public float Radius;
    public float Height;
    public bool Debug;
    public string Collection;
}

[Serializable]
public class SoundClass
{
    public Vector3 Position;
    public float Radius;
    public bool IsWaveAmbient;
    public bool Enable;
    public string SoundName;
    public List<Vector3> PolylineSegments;
    public string Collection;
}