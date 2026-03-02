using UnityEditor;
using UnityEngine;
using ReMap.Global;

/// <summary>
/// Bootstrap d'éditeur qui enregistre tous les types ReMap au chargement.
/// </summary>
[InitializeOnLoad]
public static class ReMapBootstrap
{
    static ReMapBootstrap()
    {
        ReMapGlobalRegistry.RegisterMonoType(eObjectType.Prop, typeof(ReMapPropScript));

        TagHelper.CheckAndCreateTags();
    }
}
