
using UnityEngine;
using UnityEditor;

using static WindowUtility.WindowUtility;

namespace MultiTool
{
    internal class SelectionTool
    {
        private static bool selectionOnly = false;
        internal static void OnGUI()
        {
            int idx = 0;

            GUILayout.BeginVertical( "box" );
                GUILayout.BeginHorizontal();
                    CreateToggle( ref selectionOnly, "Selection Only", "", 90, 20 );
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();

                    foreach ( ObjectType objectType in Helper.GetAllObjectType() )
                    {
                        CreateButton( $"Select all {Helper.GetObjNameWithEnum( objectType )}", "", () => Helper.SelectAllObjectTypeInScene( objectType, selectionOnly ), 194, 20 );

                        idx++;

                        if ( objectType == ObjectType.NonVerticalZipLine )
                        {
                            //CreateButton( $"Select all Ziplines Type", "", () => Helper.SelectAllObjectTypeInScene( objectType, selectionOnly ), 194, 20 );
                        }

                        if ( idx % 3 == 0 )
                        {
                            GUILayout.EndHorizontal();
                            GUILayout.BeginHorizontal();
                        }
                    }

                GUILayout.EndHorizontal();
            GUILayout.EndVertical();
        }
    }
}
