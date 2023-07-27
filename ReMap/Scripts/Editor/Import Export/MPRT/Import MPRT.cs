
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

public class ImportMPRTModels : EditorWindow
{
    private string mprtPath = "";
    private string mprtName = "";
    private string filter = "";
    private int radius = 0;
    private UnityEngine.Object source;

    public Dictionary<string, GameObject> objDictionary = new Dictionary<string, GameObject>();

    [ MenuItem( "ReMap/Import/MPRT", false, 51 ) ]
    private static void OpenImportMPRTWindow()
    {
        ImportMPRTModels window = (ImportMPRTModels)GetWindow(typeof(ImportMPRTModels), false, "Import MPRT");
        window.Show();
    }

    void OnGUI()
    {
        GUILayout.BeginHorizontal("box");
        GUILayout.Label("MPRT File:");
        GUILayout.TextField(mprtName, GUILayout.Width(400));
        if (GUILayout.Button("Select File"))
            SelectMPRTLocation();
        GUILayout.EndHorizontal();

        GUILayout.BeginVertical("box");
        GUILayout.Label("Prefab For Location: (Optional)");
        source = EditorGUILayout.ObjectField(source, typeof(UnityEngine.Object), true);
        GUILayout.EndVertical();

        GUILayout.BeginVertical("box");
        GUILayout.Label("Radius: (Optional)");
        radius = EditorGUILayout.IntField("", radius);
        GUILayout.EndHorizontal();

        GUILayout.BeginVertical("box");
        GUILayout.Label("Filter: (Optional)");
        GUILayout.Label("Seperate each filter with a comma.");
        filter = GUILayout.TextField(filter);
        GUILayout.EndHorizontal();

        GUILayout.BeginVertical("box");
        if (GUILayout.Button("Import MPRT"))
            ImportMPRTCode();
        GUILayout.EndHorizontal();
    }

    public void SelectMPRTLocation()
    {
        mprtPath = EditorUtility.OpenFilePanel( "MPRT Import", "", "mprt" );

        string[] splitArray = mprtPath.Split(char.Parse("/"));
        mprtName = splitArray[splitArray.Length - 1];
    }

    public async void ImportMPRTCode()
    {
        await ImportMPRTCodeAsync();
    }

    public async Task ImportMPRTCodeAsync()
    {
        objDictionary = new Dictionary<string, GameObject>();

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

            //Create Parent If It Doesnt Exist
            GameObject parent = GameObject.Find("Map Assets");
            if (parent == null)
            {
                GameObject objToSpawn = new GameObject("Map Assets");
                objToSpawn.name = "Map Assets";
            }

            reader.Read(buffer, 0, 4);
            int header2 = BitConverter.ToInt32(buffer, 0);

            reader.Read(buffer, 0, 4);
            int numObjects = BitConverter.ToInt32(buffer, 0);
                
            string[] filterList = filter.Replace(" ", "").Split(',');
            var DataList = new List<MPRTData>();

            for (int i = 0; i < numObjects; i++)
            {
                //Read Name
                string name = ReadNullTerminatedString(reader);
                
                //Read Position, Rotation, Scale
                float[] posRotScale = new float[7];
                for (int j = 0; j < posRotScale.Length; j++)
                {
                    reader.Read(buffer, 0, 4);
                    posRotScale[j] = BitConverter.ToSingle(buffer, 0);
                }

                //Create Data
                MPRTData data = new MPRTData();
                data.name = name;
                data.pos = new Vector3(posRotScale[1], posRotScale[2], -posRotScale[0]);
                data.rot = new Vector3(-posRotScale[4], -posRotScale[5], posRotScale[3]);
                data.scale = new Vector3(posRotScale[6], posRotScale[6], posRotScale[6]);

                //Check Filters And Radius
                bool filtercheck = FilterCheck(name, filterList);
                bool radiuscheck = RadiusCheck(data);

                if(filtercheck || radiuscheck)
                    continue;

                //Add To List
                DataList.Add(data);

                //Update Progress Bar
                EditorUtility.DisplayProgressBar( $"Parsing MPRT File: {i}/{numObjects}", $"", ( i + 1 ) / ( float )numObjects );

                await Helper.Wait();
            }

            //Sort List
            UnityInfo.SortListByKey( DataList, o => o.name );
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            //Create Props
            for (int i = 0; i < DataList.Count; i++)
            {
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

    private void CreateMPRTProp(MPRTData data)
    {
        //Check If Object Already Exists
        GameObject obj;
        if(objDictionary.ContainsKey(data.name))
            obj = Instantiate(objDictionary[data.name]);
        else
            obj = Helper.CreateGameObject( "", data.name, PathType.Name );

        //Check If Object Is Valid
        if ( !Helper.IsValid( obj ) ) return;

        //Set Position, Rotation, Scale
        obj.transform.position = data.pos;
        obj.transform.eulerAngles = data.rot;
        obj.name = data.name;
        obj.gameObject.transform.localScale = data.scale;

        //Set Parent
        GameObject parent = GameObject.Find("Map Assets");
        if (parent != null)
            obj.gameObject.transform.parent = parent.transform;

        //Add To Dictionary
        if(!objDictionary.ContainsKey(data.name))
            objDictionary.Add(data.name, obj);
    }

    private string ReadNullTerminatedString(BinaryReader reader)
    {
        List<byte> byteList = new List<byte>();
        byte b;
        while ((b = reader.ReadByte()) != 0)
        {
            byteList.Add(b);
        }
        return Encoding.UTF8.GetString(byteList.ToArray());
    }

    private bool RadiusCheck(MPRTData data)
    {
        //Check If Radius Is Set
        if (radius == 0)
            return false;
        
        //Check If Source Is Set
        if(source == null)
            return false;

        //Check If Object Is Within Radius
        GameObject obj = source as GameObject;
        float distance = Vector3.Distance(obj.transform.position, data.pos);
        if (distance > radius)
            return true;

        return false;
    }

    private bool FilterCheck(string name, string[] filterList)
    {
        //Check If Filter Is Set
        if (string.IsNullOrEmpty(filter))
            return false;

        //Check If Object Contains Filter
        foreach (var x in filterList)
            if (name.Contains(x))
                return true;

        return false;
    }

    void OnFocus() 
    {
	    SceneView.duringSceneGui -= this.OnSceneGUI;
	    SceneView.duringSceneGui += this.OnSceneGUI;
    }

    void OnDestroy()
    {
        SceneView.duringSceneGui -= this.OnSceneGUI;
    }

    void OnSceneGUI(SceneView sceneView)
    {
        //Draw Radius If Set
        if(radius != 0 && source != null)
        {
            GameObject obj = source as GameObject;
            
            Handles.color = Color.red;
            Handles.DrawWireDisc(obj.transform.position, Vector3.up, radius, 3.0f);
            GUI.color = Color.red;
            Handles.Label(obj.transform.position, "Radius: " + radius.ToString());
        }
    }
}

public class MPRTData
{
    public string name;
    public Vector3 pos;
    public Vector3 rot;
    public Vector3 scale;
}