using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Swap an existing model to an other
/// </summary>
public class ModelSwap : EditorWindow
{
    private GameObject[] prefabs = new GameObject[1];
    private GameObject[] activeSelection;
    private GameObject[] newSelection;
    private bool randomlyChanges = true;

    Vector2 scroll;

    /// <summary>
    /// ModelSwap.Init()
    /// </summary>
    [MenuItem("ReMap/Tools/Model Swap Tool", false, 100)]
    public static void Init()
    {
        ModelSwap window = ( ModelSwap )EditorWindow.GetWindow( typeof( ModelSwap ), false, "Model Swap Tool" );
        window.minSize = new Vector2(400, 295);
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

        GUILayout.BeginVertical("box");
            scroll = EditorGUILayout.BeginScrollView(scroll);

            for (int i = 0; i < prefabs.Length; i++)
            {
                int idx = i + 1;
                EditorGUILayout.BeginHorizontal("box");
                    EditorGUILayout.LabelField( $"Prefab {idx.ToString("00")}: ", labelStyle, GUILayout.Width( 60 ));
                    prefabs[i] = EditorGUILayout.ObjectField( prefabs[i], typeof( GameObject ), true ) as GameObject;
                    if ( GUILayout.Button( $"Clear Prefab {idx.ToString("00")}", GUILayout.Width( 120 ) ) ) prefabs[i] = null;
                EditorGUILayout.EndHorizontal();
            }

            GUILayout.EndScrollView();
            EditorGUILayout.BeginHorizontal();
                if ( GUILayout.Button( $"Remove 1 Row" ) ) Array.Resize( ref prefabs, prefabs.Length - 1 );
                if ( GUILayout.Button( $"Add 1 Row" ) ) Array.Resize( ref prefabs, prefabs.Length + 1 );
            EditorGUILayout.EndHorizontal();
        GUILayout.EndVertical();

        GUILayout.BeginVertical("box");
            EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Change randomly the selection: ", labelStyle, GUILayout.Width(370));
                randomlyChanges = EditorGUILayout.Toggle(randomlyChanges);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space( 4 );

            if ( GUILayout.Button( "Change Selection" ) ) ChangeSelection();
        GUILayout.EndVertical();
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

        if ( ArrayIsNull( prefabs ) ) return;

        while ( chosenObject == null )
        {
            chosenObject = prefabs[ UnityEngine.Random.Range( 0, prefabs.Length ) ];
            if ( chosenObject.name == selection.name )
            {
                chosenObject = null;

                if ( prefabs.Length == 1 ) return;
            }
        }

        Vector3 position = selection.transform.position;
        Vector3 rotation = selection.transform.eulerAngles;
        Vector3 localScale = selection.transform.localScale;

        GameObject obj = Helper.CreateGameObject( "", chosenObject.name, PathType.Name );
        if ( !Helper.IsValid( obj ) ) return;
        obj.transform.position = position;
        obj.transform.eulerAngles = rotation;
        obj.transform.localScale = localScale;
        obj.transform.parent = FindParent( selection );
        obj.name = chosenObject.name;

        PropScript compToAdd = obj.GetComponent<PropScript>();
        Helper.ApplyComponentScriptData<PropScript>( compToAdd, script );

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

    bool ArrayIsNull( GameObject[] arrays )
    {
        foreach ( GameObject array in arrays )
        {
            if ( array != null ) return false;
        }
        return true;
    }

    bool CoinFlip()
    {
        return UnityEngine.Random.Range( 0, 2 ) == 0 ? false : true;
    }
}
