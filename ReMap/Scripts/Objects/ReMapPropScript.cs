using UnityEngine;

using System.Text;

using ReMap.Global;
using ReMap.Build;
using ReMap.ImportExport;

[AddComponentMenu("ReMap/Prop Settings", 0)]
public class ReMapPropScript : ReMapObjectBase
{
    [ReMapVar("AllowMantle", ReMapVarType.Bool)] public bool AllowMantle = true;
    [ReMapVar("FadeDistance", ReMapVarType.Float)] public float FadeDistance = 50000;
    [ReMapVar("RealmID", ReMapVarType.Int)] public int RealmID = -1;
    [ReMapVar("ClientSide", ReMapVarType.Bool)] public bool ClientSide = false;
    [ReMapVar("Options", ReMapVarType.String), TextArea] public string Options = "";

    public override eObjectType Type => eObjectType.Prop;
    public override string TagName => "Prop";
    public override string ObjectName => "Prop";
    public override string ObjectsName => "Props";
    public override string ObjectNameDesc => "";
    public override void FillData( GlobalClassData data )
    {
        switch (data)
        {
            case DynamicData dyn:
                DynamicDataHelper.FillFromObject(this, dyn.Vars);
                break;
        }
    }

    public override void ApplyData(GlobalClassData data)
    {
        switch (data)
        {
            case DynamicData dyn:
                DynamicDataHelper.ApplyToObject(this, dyn.Vars);
                break;
        }
    }

    public override void Build(StringBuilder sb, eBuildType buildType)
    {
        switch (buildType)
        {
            case eBuildType.Script:
                sb.AppendLine("prop_dynamic");
                break;
        }
    }
}
