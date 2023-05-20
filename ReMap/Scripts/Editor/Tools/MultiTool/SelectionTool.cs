
using UnityEngine;
using UnityEditor;

using static WindowUtility.WindowUtility;

namespace MultiTool
{
    internal class SelectionTool
    {
        private static bool selectionOnly = false;
        
        private static readonly ObjectType[] ziplibeArray = new ObjectType[] { ObjectType.ZipLine, ObjectType.LinkedZipline, ObjectType.VerticalZipLine, ObjectType.NonVerticalZipLine };
        private static readonly ObjectType[] doorArray = new ObjectType[] { ObjectType.SingleDoor, ObjectType.DoubleDoor, ObjectType.VerticalDoor, ObjectType.HorzDoor };
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
                        CreateButton( $"Select all {Helper.GetObjNameWithEnum( objectType )}", "", () => Helper.ChangeSelection( Helper.GetAllObjectTypeWithEnum( objectType, selectionOnly ) ), 194, 20 );

                        idx++;

                        Verify( idx );

                        if ( objectType == ObjectType.NonVerticalZipLine )
                        {
                            GameObject[][] ziplibeArray = new GameObject[][]
                            {
                                Helper.GetAllObjectTypeWithEnum( ObjectType.ZipLine, selectionOnly ),
                                Helper.GetAllObjectTypeWithEnum( ObjectType.LinkedZipline, selectionOnly ),
                                Helper.GetAllObjectTypeWithEnum( ObjectType.VerticalZipLine, selectionOnly ),
                                Helper.GetAllObjectTypeWithEnum( ObjectType.NonVerticalZipLine, selectionOnly )
                            };

                            CreateButton( $"Select all Ziplines Type", "", () => Helper.ChangeSelection( ziplibeArray ), 194, 20 );
                            idx++;
                        }

                        if ( objectType == ObjectType.VerticalDoor )
                        {
                            GameObject[][] doorArray = new GameObject[][]
                            {
                                Helper.GetAllObjectTypeWithEnum( ObjectType.SingleDoor, selectionOnly ),
                                Helper.GetAllObjectTypeWithEnum( ObjectType.DoubleDoor, selectionOnly ),
                                Helper.GetAllObjectTypeWithEnum( ObjectType.VerticalDoor, selectionOnly ),
                                Helper.GetAllObjectTypeWithEnum( ObjectType.HorzDoor, selectionOnly )
                            };

                            CreateButton( $"Select all Doors Type", "", () => Helper.ChangeSelection( doorArray ), 194, 20 );
                            idx++;
                        }

                        Verify( idx );
                    }

                GUILayout.EndHorizontal();
            GUILayout.EndVertical();
        }

        internal static void Verify( int value )
        {
            if ( value % 3 == 0 )
            {
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
            }
        }
    }
}
