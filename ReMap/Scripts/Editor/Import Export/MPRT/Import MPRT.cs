
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

public class ImportMPRTModels
{
    public static string mprtPath = "";
    public static Vector3 coordinates;



    [ MenuItem( "ReMap/Import/MPRT", false, 51 ) ]
    private static async void ImportMPRTCode()
    {
        var mprtPath = EditorUtility.OpenFilePanel( "MPRT Import", "", "mprt" );

        byte[] buffer = new byte[4];

            using (BinaryReader reader = new BinaryReader(File.Open(mprtPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)))
            {
                // Read and validate header
                reader.Read(buffer, 0, 4);
                int header1 = BitConverter.ToInt32(buffer, 0);
                if (header1 != 0x7472706D) // check if matches with the expected value
                {
                    Helper.Ping("Invalid file.");
                    return;
                }

                GameObject objToSpawn = new GameObject("Map Assets");
                objToSpawn.name = "Map Assets";

                reader.Read(buffer, 0, 4);
                int header2 = BitConverter.ToInt32(buffer, 0);

                reader.Read(buffer, 0, 4);
                int numObjects = BitConverter.ToInt32(buffer, 0);
                
                // Object lists
                var scaleList = new List<Vector3>();
                var posList = new List<Vector3>();
                var rotList = new List<Vector3>();
                var nameList = new List<string>();

                var DataList = new List<MPRTData>();

                for (int i = 0; i < numObjects; i++)
                {
                    string name = ReadNullTerminatedString(reader);
                    
                    float[] posRotScale = new float[7];
                    for (int j = 0; j < posRotScale.Length; j++)
                    {
                        reader.Read(buffer, 0, 4);
                        posRotScale[j] = BitConverter.ToSingle(buffer, 0);
                    }

                    MPRTData data = new MPRTData();
                    data.name = name;
                    data.pos = new Vector3(posRotScale[1], posRotScale[2], -posRotScale[0]);
                    data.rot = new Vector3(-posRotScale[4], -posRotScale[5], posRotScale[3]);
                    data.scale = new Vector3(posRotScale[6], posRotScale[6], posRotScale[6]);

                    //Debug.Log($"X: {posRotScale[3]}, Y: {posRotScale[4]}, Z:{posRotScale[5]}");

                    DataList.Add(data);

                    EditorUtility.DisplayProgressBar( $"Parsing MPRT File: {i}/{numObjects}", $"", ( i + 1 ) / ( float )numObjects );

                    await Helper.Wait();
                }
                //EditorUtility.ClearProgressBar();
                //return;

                // Proceed with the implementation of loading the models based on the parsed data...

                UnityInfo.SortListByKey( DataList, o => o.name );

                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();

                for (int i = 0; i < DataList.Count; i++)
                {
                    if ( i == 200 ) break;
                    CreateMPRTProp(DataList[i]);

                    double progress = (i + 1) / (double)DataList.Count;
                    double elapsedTime = stopwatch.Elapsed.TotalSeconds;
                    double estimatedTotalTime = elapsedTime / progress;
                    double estimatedRemainingTime = estimatedTotalTime - elapsedTime;

                    int remainingMinutes = (int)(estimatedRemainingTime / 60);
                    int remainingSeconds = (int)(estimatedRemainingTime % 60);

                    EditorUtility.DisplayProgressBar($"Placing Models: {i}/{DataList.Count}", $"Time remaining: {remainingMinutes:D2}:{remainingSeconds:D2} min", (float)progress);

                    await Helper.Wait();
                }

                stopwatch.Stop();

                EditorUtility.ClearProgressBar();
            }
    }

    private static void CreateMPRTProp(MPRTData data)
    {
        GameObject obj = Helper.CreateGameObject( "", data.name, PathType.Name );

        if ( !Helper.IsValid( obj ) ) return;

        obj.transform.position = data.pos;
        obj.transform.eulerAngles = data.rot;
        obj.name = data.name;
        obj.gameObject.transform.localScale = data.scale;

        GameObject parent = GameObject.Find("Map Assets");
        if (parent != null)
            obj.gameObject.transform.parent = parent.transform;
    }

    private static string ReadNullTerminatedString(BinaryReader reader)
    {
        List<byte> byteList = new List<byte>();
        byte b;
        while ((b = reader.ReadByte()) != 0)
        {
            byteList.Add(b);
        }
        return Encoding.UTF8.GetString(byteList.ToArray());
    }
}

public class MPRTData
{
    public string name;
    public Vector3 pos;
    public Vector3 rot;
    public Vector3 scale;
}