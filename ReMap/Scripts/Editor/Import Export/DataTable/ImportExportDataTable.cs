using System;
using System.IO;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

public class ImportExportDataTable
{
    public static async void ImportDataTable()
    {
        string path = EditorUtility.OpenFilePanel( "Datatable Import", "", "csv" );
        if ( path.Length == 0 )
            return;

        ReMapConsole.Log( "[Datatable Import] Reading file: " + path, ReMapConsole.LogType.Warning );
        string[] splitArray = File.ReadAllText( path ).Split( char.Parse( "\n" ) );

        var collectionList = Helper.BuildCollectionList( splitArray );
        foreach ( string col in collectionList )
        {
            var objToSpawn = new GameObject( col );
            objToSpawn.name = col;
        }

        //Import datatable
        int i = 0;
        foreach ( string item in splitArray )
        {
            string[] itemsplit = item.Replace( "\"", "" ).Split( char.Parse( "," ) );

            if ( itemsplit.Length < 12 )
                continue;

            ReMapConsole.Log( "[Datatable Import] Importing: " + itemsplit[11], ReMapConsole.LogType.Info );
            EditorUtility.DisplayProgressBar( "Importing DataTable", "Importing: " + itemsplit[11], ( i + 1 ) / ( float )splitArray.Length );

            //Build DataTable
            var dt = Helper.BuildDataTable( item );

            ReMapConsole.Log( "Origin: " + dt.Origin, ReMapConsole.LogType.Info );
            ReMapConsole.Log( "Angles: " + dt.Angles, ReMapConsole.LogType.Info );
            ReMapConsole.Log( "Scale: " + dt.Scale, ReMapConsole.LogType.Info );
            ReMapConsole.Log( "Type: " + dt.Type, ReMapConsole.LogType.Info );
            ReMapConsole.Log( "Collection: " + dt.Collection, ReMapConsole.LogType.Info );

            //Find Model GUID in Assets
            string[] results = AssetDatabase.FindAssets( dt.Model );
            if ( results.Length == 0 )
                continue;

            //Get model path from guid and load it
            Object loadedPrefabResource = AssetDatabase.LoadAssetAtPath( AssetDatabase.GUIDToAssetPath( results[0] ), typeof(Object) ) as GameObject;
            if ( loadedPrefabResource == null )
                continue;

            //Create new model in scene
            Helper.CreateDataTableItem( dt, loadedPrefabResource );

            await Task.Delay( TimeSpan.FromSeconds( 0.001 ) );
            i++;
        }

        ReMapConsole.Log( "[Datatable Import] Finished", ReMapConsole.LogType.Success );
        EditorUtility.ClearProgressBar();
    }
}