
using System.Threading.Tasks;
using UnityEngine;
using UnityEditor;

using static CodeViewsWindow.LiveMap;
using static WindowUtility.WindowUtility;

namespace MultiTool
{
    internal class ModelPosition
    {
        private static bool processActive = false;
        internal static void OnGUI()
        {
            if ( !ApexProcessIsActive() )
            {
                GUILayout.BeginVertical( "box" );
                    CreateButton( "Find Apex Process", "", () => External_FindApexWindow() );
                    FlexibleSpace();

                        CreateTextInfoCentered( "Apex Process Not Found !" );

                    FlexibleSpace();
                GUILayout.EndVertical();
                return;
            }
            else
            {
                GUILayout.BeginVertical( "box" );
                    FlexibleSpace();
                        if ( processActive )
                        {
                            CreateTextInfoCentered( "Processing..." );
                        }
                        else
                        {
                            CreateButton( "Set Object Origin", "Set all selected objects to player 0 position", () => StartProcess() );
                            CreateButton( "Set Object Origin And Angles", "Set all selected objects to player 0 position and angles", () => StartProcess( true ) );
                        }
                    FlexibleSpace();
                GUILayout.EndVertical();
            }
        }

        private static async void StartProcess( bool angles = false )
        {
            processActive = true;
                await ApplyOffset( angles );
            processActive = false;
        }

        private static async Task ApplyOffset( bool angles )
        {
            GetApexPlayerInfo( true );

            await Task.Delay( 150 ); // 150 ms

            foreach ( GameObject go in Selection.gameObjects )
            {
                go.transform.position = Helper.ConvertApexOriginToUnity( PlayerInfoOrigin );
                if ( angles ) go.transform.eulerAngles = Helper.ConvertApexAnglesToUnity( PlayerInfoAngles );
            }
        }
    }
}
