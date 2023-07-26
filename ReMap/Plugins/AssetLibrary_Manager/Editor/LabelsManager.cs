using UnityEngine;
using UnityEditor;
using System.Linq;

namespace AssetLibraryManager
{
    public class LabelsManager : EditorWindow
    {
        private string textField = "Seperate labels with a space...";

        public static void ShowWindow()
        {
            GetWindow<LabelsManager>("Labels Manager");
        }

        void OnGUI()
        {
            EditorGUILayout.Space(10);

            GUIStyle someGuiStyle = new GUIStyle(EditorStyles.largeLabel);
            someGuiStyle.alignment = TextAnchor.MiddleCenter;
            someGuiStyle.fontSize = 16;

            EditorGUILayout.LabelField("Enter asset labels", someGuiStyle, GUILayout.Height(25));

            EditorGUILayout.Space(10);



            textField = EditorGUILayout.TextField("", textField);

            EditorGUILayout.Space(10);

            EditorGUILayout.BeginHorizontal();

            //Set Labels-------------------------------------------------------------------------------
            if (GUILayout.Button("Set Labels"))
            {
                string[] newLabels = textField.Split(' ');

                foreach (var obj in Selection.gameObjects)
                {
                    //Get existing labels ----------------------------------------------
                    string getPath = AssetDatabase.GetAssetPath(obj);

                    GUID guid = AssetDatabase.GUIDFromAssetPath(getPath);

                    string[] existingLabels = AssetDatabase.GetLabels(guid);
                    //------------------------------------------------------------------

                    string[] combinedLabels = newLabels.Concat(existingLabels).ToArray();

                    AssetDatabase.SetLabels(obj, combinedLabels);
                }

                Debug.Log($"Labels added.");
            }

            //Clear Labels-----------------------------------------------------------------------------
            if (GUILayout.Button("Clear Labels"))
            {
                foreach (var obj in Selection.gameObjects)
                {
                    AssetDatabase.ClearLabels(obj);
                }

                Debug.Log($"Labels cleared");
            }

            EditorGUILayout.EndHorizontal();


            GUILayout.FlexibleSpace();

            ////Clear All Project Labels--------------------------------------------------------------------
            if (GUILayout.Button("Clear all project Labels"))
            {
                if (EditorUtility.DisplayDialog("Delete Confirmation", "This will delete all asset labels on every prefab in your project.\n\nAre you sure?", "Ok", "Cancel"))
                {
                    string[] getGUIDs = AssetDatabase.FindAssets("t:prefab");

                    for (int i = 0; i < getGUIDs.Length; i++)
                    {
                        string getPath = AssetDatabase.GUIDToAssetPath(getGUIDs[i]);

                        Object getName = AssetDatabase.LoadAssetAtPath<Object>(getPath);

                        AssetDatabase.ClearLabels(getName);
                    }

                    Debug.Log($"Project labels cleared.");
                }
            }

            EditorGUILayout.Space(10);
        }
    }
}
