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
    private GameObject[] prefabs = new GameObject[1];
    private GameObject[] activeSelection;
    private GameObject[] newSelection;
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

        for (int i = 0; i < prefabs.Length; i++)
        {
            int idx = i + 1;
            EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField( $"Prefab {idx.ToString("00")}: ", labelStyle, GUILayout.Width( 60 ));
                prefabs[i] = EditorGUILayout.ObjectField( prefabs[i], typeof( GameObject ), true ) as GameObject;
                if ( GUILayout.Button( $"Clear Prefab {idx.ToString("00")}", GUILayout.Width( 120 ) ) ) prefabs[i] = null;
            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.Space( 2 );

        EditorGUILayout.BeginHorizontal();

            if ( GUILayout.Button( $"Remove 1 Row" ) ) Array.Resize( ref prefabs, prefabs.Length - 1 );
            if ( GUILayout.Button( $"Add 1 Row" ) ) Array.Resize( ref prefabs, prefabs.Length + 1 );

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space( 4 );

        EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Change randomly the selection: ", labelStyle, GUILayout.Width( 180 ));
            randomlyChanges = EditorGUILayout.Toggle(randomlyChanges);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space( 4 );

        EditorGUILayout.BeginHorizontal();
            if ( GUILayout.Button( "Change Selection" ) ) ChangeSelection();
        EditorGUILayout.EndHorizontal();
    }

    void ChangeSelection()
    {
        activeSelection = Selection.gameObjects;
        newSelection = activeSelection;

        foreach ( GameObject selection in activeSelection )
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

        RemoveNullGameObject( newSelection );
        Selection.objects = newSelection;
    }

    void SwapModel( GameObject selection )
    {
        PropScript script = selection.GetComponent<PropScript>();
        if ( script == null || !selection.name.Contains( "mdl#" ) ) return;

        GameObject chosenObject = null;

        if ( ArrayIsNull() ) return;

        while ( chosenObject == null ) chosenObject = prefabs[ UnityEngine.Random.Range( 0, prefabs.Length ) ];

        UnityEngine.Object loadedPrefabResource = AssetDatabase.LoadAssetAtPath( AssetDatabase.GUIDToAssetPath( AssetDatabase.FindAssets( chosenObject.name )[0] ), typeof( UnityEngine.Object ) ) as GameObject;
        if ( loadedPrefabResource == null ) return;

        Vector3 position = selection.transform.position;
        Vector3 rotation = selection.transform.eulerAngles;
        Vector3 localScale = selection.transform.localScale;

        GameObject obj = PrefabUtility.InstantiatePrefab( loadedPrefabResource as GameObject ) as GameObject;
        obj.transform.position = position;
        obj.transform.eulerAngles = rotation;
        obj.transform.localScale = localScale;
        obj.transform.parent = FindParent( selection );
        obj.name = chosenObject.name;

        PropScript compToAdd = obj.AddComponent<PropScript>();
        compToAdd.allowMantle = script.allowMantle;
        compToAdd.realmID = script.realmID;
        compToAdd.playerClip = script.playerClip;
        compToAdd.playerNoClimb = script.playerNoClimb;
        compToAdd.playerNoCollision = script.playerNoCollision;

        GameObject.DestroyImmediate( selection );

        int currentLength = newSelection.Length;
        Array.Resize(ref newSelection, currentLength + 1);
        newSelection[currentLength] = obj;
    }

    void RemoveNullGameObject( GameObject[] array )
    {
        int nullCount = 0;
        for (int i = 0; i < array.Length; i++)
        {
            if (array[i] == null)
            {
                nullCount++;
            }
            else if (nullCount > 0)
            {
                Array.Copy(array, i, array, i - nullCount, 1);
            }
        }
        Array.Resize(ref array, array.Length - nullCount);
    }

    Transform FindParent( GameObject go )
    {
        if ( go.transform.parent != null ) return go.transform.parent;
        
        return null;
    }

    bool ArrayIsNull()
    {
        foreach ( GameObject prefab in prefabs )
        {
            if ( prefab != null ) return false;
        }
        return true;
    }

    bool CoinFlip()
    {
        return UnityEngine.Random.Range( 0, 2 ) == 0 ? false : true;
    }
}
