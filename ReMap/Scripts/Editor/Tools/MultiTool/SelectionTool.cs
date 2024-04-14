using UnityEngine;
using static WindowUtility.WindowUtility;

namespace MultiTool
{
    internal class SelectionTool
    {
        private static bool selectionOnly;

        private static readonly ObjectType[] ziplibeArray = { ObjectType.ZipLine, ObjectType.LinkedZipline, ObjectType.VerticalZipLine, ObjectType.NonVerticalZipLine };
        private static readonly ObjectType[] doorArray = { ObjectType.SingleDoor, ObjectType.DoubleDoor, ObjectType.VerticalDoor, ObjectType.HorzDoor };

        internal static void OnGUI()
        {
            int idx = 0;

            GUILayout.BeginVertical( "box" );
            GUILayout.BeginHorizontal();
            CreateToggle( ref selectionOnly, "Selection Only", "", 90, 20 );
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();

            foreach ( var objectType in Helper.GetAllObjectType() )
            {
                CreateButton( $"Select all {Helper.GetObjNameWithEnum( objectType )}", "", () => Helper.ChangeSelection( Helper.GetAllObjectTypeWithEnum( objectType, selectionOnly ) ), 194, 20 );

                idx++;

                Verify( idx );

                if ( objectType == ObjectType.NonVerticalZipLine )
                {
                    CreateButton( "Select all Ziplines Type", "", () => Helper.ChangeSelection( Helper.GetAllObjectTypeWithEnum( ziplibeArray, selectionOnly ) ), 194, 20 );
                    idx++;
                }

                if ( objectType == ObjectType.VerticalDoor )
                {
                    CreateButton( "Select all Doors Type", "", () => Helper.ChangeSelection( Helper.GetAllObjectTypeWithEnum( doorArray, selectionOnly ) ), 194, 20 );
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