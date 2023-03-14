using UnityEngine;
using UnityEditor;
using System;
using System.Linq;
using System.Collections.Generic;

/// <summary>
/// Swap an existing model to an other
/// </summary>
public class ModelSwap : EditorWindow
{
    private GameObject prefab;
    private bool randomlyChanges = true;

    /// <summary>
    /// ModelSwap.Init()
    /// </summary>
    [MenuItem("ReMap/Tools/Model Swap Tool", false, 100)]
    public static void Init()
    {
        ModelSwap window = ( ModelSwap )EditorWindow.GetWindow( typeof( ModelSwap ), false, "Model Swap Tool" );
        //window.minSize = new Vector2(606, 252);
        //window.maxSize = new Vector2(606, 252);
        window.Show();
    }

    /// <summary>
    /// ModelSwap.OnGUI() : When the window is displayed
    /// </summary>
    void OnGUI()
    {
        GUIStyle labelStyle = new GUIStyle();
        labelStyle.normal.textColor = Color.white;

        EditorGUILayout.Space( 2 );

        EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Prefabs: ", labelStyle, GUILayout.Width( 50 ));
            prefab = EditorGUILayout.ObjectField( prefab, typeof( GameObject ), true ) as GameObject;
            if ( GUILayout.Button( "Clear Prefabs", GUILayout.Width( 100 ) ) ) prefab = null;
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space( 4 );

        EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Change randomly the selection: ", labelStyle, GUILayout.Width( 180 ));
            randomlyChanges = EditorGUILayout.Toggle(randomlyChanges);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space( 4 );

        EditorGUILayout.BeginHorizontal();
            if ( GUILayout.Button( "Change Selection" ) )
            {
                // Enregistre l'état actuel de l'objet pour permettre l'annulation
                Undo.RecordObject(this, "Change Selection");

                // Appeler la méthode pour effectuer la modification
                ChangeSelection();

                // Si l'utilisateur appuie sur Ctrl + Z, l'action sera annulée
                Undo.PerformUndo();
            }
        EditorGUILayout.EndHorizontal();
    }

    void ChangeSelection()
    {
        GameObject[] selections = Selection.gameObjects;

        foreach ( GameObject selection in selections )
        {
            if ( randomlyChanges && CoinFlip() )
            {
                SwapModel( selection );
            }
            else if ( !randomlyChanges )
            {
                SwapModel( selection );
            }
        }
    }

    void SwapModel( GameObject selection )
    {
        PropScript script = selection.GetComponent<PropScript>();
        if ( script == null || !selection.name.Contains( "mdl#" ) ) return;

        UnityEngine.Object loadedPrefabResource = AssetDatabase.LoadAssetAtPath( AssetDatabase.GUIDToAssetPath( AssetDatabase.FindAssets( prefab.name )[0] ), typeof( UnityEngine.Object ) ) as GameObject;
        if ( loadedPrefabResource == null ) return;

        Vector3 position = selection.transform.position;
        Vector3 rotation = selection.transform.eulerAngles;
        Vector3 localScale = selection.transform.localScale;

        GameObject obj = PrefabUtility.InstantiatePrefab(loadedPrefabResource as GameObject) as GameObject;
        obj.transform.position = position;
        obj.transform.eulerAngles = rotation;
        obj.transform.localScale = localScale;
        obj.transform.parent = FindParent( selection );
        obj.name = prefab.name;

        PropScript compToAdd = obj.AddComponent<PropScript>();
        compToAdd.allowMantle = script.allowMantle;
        compToAdd.realmID = script.realmID;
        compToAdd.playerClip = script.playerClip;
        compToAdd.playerNoClimb = script.playerNoClimb;
        compToAdd.playerNoCollision = script.playerNoCollision;

        GameObject.DestroyImmediate( selection );
    }

    Transform FindParent( GameObject go )
    {
        if (go.transform.parent != null) return go.transform.parent;
        
        return null;
    }

    bool CoinFlip()
    {
        return UnityEngine.Random.Range(0, 2) == 0 ? false : true;
    }
}
