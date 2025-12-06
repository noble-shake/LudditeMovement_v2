using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
using UnityEngine.UIElements;
using static UnityEngine.Rendering.DebugUI.MessageBox;
using static UnityEngine.Rendering.DebugUI.Table;

[Serializable]
public class StageData
{
    public float SpawnTime;
    public EnemyName MonsterType;
    public int MonsterLevel;
    public int SpawnColumn;
    public int SpawnRow;

}

[SerializeField]
public struct TileCompressed
{
    public MapTile tile;
    public Props building; // TODO: Update TBD
    public Environments environment;

    public void SetTile(MapTile _tile)
    {
        tile = _tile;
    }
}

[SerializeField]
public class TileCompression
{
    public MapTile tile;
    public Props building; // TODO: Update TBD
    public Environments environment;

    public TileCompression() { }
}


public class MapEditor : EditorWindow
{
    [MenuItem("LucidBoundary/MapEditor")]
    public static void ShowExample()
    {
        MapEditor wnd = GetWindow<MapEditor>();
        wnd.titleContent = new GUIContent("MapEditor");
    }

    private MapEditorScriptableObject MapData;
    private MapData SaveData;
    private const string AssetPath = "Assets/MapDataset/MapEditorStroe.asset";
    private const string SavePath = "Assets/MapDataset/";
    private const string SaveTail= ".asset";
    MapData LoadData;
    private string StageName = "Default";
    private string StageStory = "Story Summary";
    private string StageHint = "Hint";
    private int StageBatchMember = 4;

    // ScrollBar
    [SerializeField] private Vector2 entirePos = Vector2.zero;

    // isGenerateMode 는 실제 Scene에 영향력을 주므로, False를 디폴트로 한다.
    private bool isGenerateMode = false;
    private bool isRemoveMode = false;

    private TileBuildType CurrentTile;
    private TileTarget CurrentBuildTarget = TileTarget.Tile;
    private EnemyName CurrentEnemy;
    Enemy tempEnemy;

    private EnvironmentType CurrentEnvType;
    private Environments TempEnvPrefab;

    //private MapManager mapInstance;
    private Text TitleLabel;
    public GameObject GeneratePlane;

    // TileMap
    [SerializeField] private MapTile CurrentTilePrefab;
    public Dictionary<(int, int), TileCompression> tileRecords;
    private MapTile TempTilePrefab;

    // Stage Edit
    public bool isStageEditMode = false;
    ScriptableObject scriptableObj;
    public SerializedObject SerialObject;
    public SerializedProperty serialProp;

    public SerializedObject SerialLoadObject;
    public SerializedProperty serialEnvProp;
    public SerializedProperty serialBuildingProp;
    public SerializedProperty serialTileProp;

    // Time Slider
    private float TimeScheduling;

    private void OnEnable()
    {
        tileRecords = new Dictionary<(int, int), TileCompression>();
        //RenderPipelineManager.endContextRendering += EndFrameRendering;
        RenderPipelineManager.endContextRendering += EndFrameRendering;
        SceneView.duringSceneGui += OnSceneGUI;

        MapData = AssetDatabase.LoadAssetAtPath<MapEditorScriptableObject>(AssetPath);
        if (MapData == null)
        {
            MapData = ScriptableObject.CreateInstance<MapEditorScriptableObject>();
            AssetDatabase.CreateAsset(MapData, AssetPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        scriptableObj = this;
        SerialObject = new SerializedObject(MapData);
        serialProp = SerialObject.FindProperty("EnemyProgression");




    }

    private void OnDisable()
    {
        RecordClear();
        RenderPipelineManager.endContextRendering -= EndFrameRendering;
        SceneView.duringSceneGui -= OnSceneGUI;

    }

    private void CreateGUI()
    {

    }

    private void OnGUI()
    {
        entirePos = EditorGUILayout.BeginScrollView(entirePos);

        #region Font Style
        GUIStyle header = new GUIStyle();
        header.normal.textColor = Color.white;
        header.fontSize = 24;

        GUIStyle SubHeader = new GUIStyle();
        SubHeader.normal.textColor = Color.aliceBlue;
        SubHeader.fontSize = 16;

        GUIStyle tileStyle = new GUIStyle();
        tileStyle.normal.textColor = Color.white;
        tileStyle.fontSize = 12;

        GUIStyle targetStyle = new GUIStyle();
        targetStyle.normal.textColor = Color.cyan;
        targetStyle.fontSize = 14;

        #endregion

        ////isGenerateMode = EditorGUILayout.Toggle(isGenerateMode, )
        GUILayout.BeginVertical();

        GUILayout.Label("맵 에디터", header);
        GUILayout.EndVertical();

        GUILayout.Space(16);
        
        GUILayout.Space(16);
        //EditorGUI.EndChangeCheck();


        EditorGUI.EndChangeCheck();

        EditorGUI.BeginChangeCheck();
        GUILayout.Label("Map Data Load", SubHeader);

        LoadData = (MapData)EditorGUILayout.ObjectField(LoadData, typeof(MapData), false);
        if (GUILayout.Button("Load", EditorStyles.miniButton))
        {
            CreateInstanceFromLoadButton();

        }
        EditorGUI.EndChangeCheck();

        GUILayout.Space(10);
        GUILayout.Label("Drawing Mode", SubHeader);
        GUILayout.Space(10);

        EditorGUI.BeginChangeCheck();
        MapData.isDrawCoordinate = EditorGUILayout.Toggle("Draw Coordinate", MapData.isDrawCoordinate);
        MapData.isDrawGrid = EditorGUILayout.Toggle("Draw Grid", MapData.isDrawGrid);

        GUILayout.Label("Edit Mode", SubHeader);
        GUILayout.Space(10);
        isGenerateMode = EditorGUILayout.Toggle("Add Tile Mode", isGenerateMode);
        isRemoveMode = EditorGUILayout.Toggle("Remove Tile Mode", isRemoveMode);
        isStageEditMode = EditorGUILayout.Toggle("Stage Edit Mode", isStageEditMode);
        EditorGUI.EndChangeCheck();

        GUILayout.Space(16);
        GUILayout.Label("Setup Targets", SubHeader);
        GUILayout.Space(10);

        GUILayout.Label($"Select Tile", targetStyle);
        GUILayout.BeginHorizontal();
        EditorGUI.BeginChangeCheck();
        GUILayout.Label("Selected Tile", tileStyle);
        CurrentTile = ((TileBuildType)EditorGUILayout.EnumPopup(CurrentTile));

        GUILayout.EndHorizontal();
        GUILayout.Label($"Current Tile : [ {CurrentTile.ToString()} ]", tileStyle);
        foreach (MapTile tile in MapData.TilePrefabs)
        {
            if (tile.GetTileBuildType() == CurrentTile)
            {
                CurrentTilePrefab = tile;
                break;
            }
        }
        Rect textureTileRect = GUILayoutUtility.GetLastRect();
        textureTileRect = new Rect(textureTileRect.x, textureTileRect.y + 20f, 40f, 40f);
        EditorGUI.DrawPreviewTexture(textureTileRect, CurrentTilePrefab.GetComponentInChildren<SpriteRenderer>().sprite.texture);

        GUILayout.Space(50);
        //GUILayout.Box(CurrentTile);

        GUILayout.Label($"Current Environment Type", targetStyle);
        CurrentEnvType = ((EnvironmentType)EditorGUILayout.EnumPopup(CurrentEnvType));
        for (int i = 0; i < MapData.EnvironmentPrefabs.Length; i++)
        {
            if (MapData.EnvironmentPrefabs[i].envType == CurrentEnvType)
            {
                TempEnvPrefab = MapData.EnvironmentPrefabs[i];
                break;
            }
        }
        
        Rect textureEnvRect= GUILayoutUtility.GetLastRect();
        textureEnvRect = new Rect(textureEnvRect.x, textureEnvRect.y + 20f, 40f, 40f);
        EditorGUI.DrawPreviewTexture(textureEnvRect, TempEnvPrefab.GetComponentInChildren<SpriteRenderer>().sprite.texture);

        GUILayout.Space(50);
        GUILayout.Label($"Select BuildType", targetStyle);
        CurrentBuildTarget = ((TileTarget)EditorGUILayout.EnumPopup(CurrentBuildTarget));
        GUILayout.Label("Selected Tile Target (Tile, Building, Environment)", tileStyle);

        GUILayout.Space(10);
        GUILayout.Label($"Select EnemyType", targetStyle);
        CurrentEnemy = ((EnemyName)EditorGUILayout.EnumPopup(CurrentEnemy));


        Rect textureEnemyRect = GUILayoutUtility.GetLastRect();
        textureEnemyRect = new Rect(textureEnemyRect.x, textureEnemyRect.y + 20f, 40f, 40f);
        
        foreach (Enemy e in MapData.EnemyPrefabs)
        {
            if (e.GetEnemyName() == CurrentEnemy)
            {
                tempEnemy = e;
                
                break;
            }
        }
        if(tempEnemy != null) EditorGUI.DrawPreviewTexture(textureEnemyRect, tempEnemy.GetComponentInChildren<SpriteRenderer>().sprite.texture);
        GUILayout.Space(50);
        EditorGUI.EndChangeCheck();

        EditorGUI.BeginChangeCheck();
        GUILayout.Space(16);
        GUILayout.Label($"Time Schedule", SubHeader);
        GUILayout.Label($"GameTime : {Mathf.FloorToInt(TimeScheduling/60f).ToString("D2")}:{Mathf.FloorToInt(TimeScheduling% 60f).ToString("D2")}", targetStyle);
        TimeScheduling = EditorGUILayout.Slider(TimeScheduling, 0f, 60f * 10, GUILayout.Height(20f));

        GUILayout.Label($"Stage Batching", SubHeader);
        StageBatchMember = EditorGUILayout.IntField(StageBatchMember, GUILayout.Height(30f));
        GUILayout.Label($"Stage Story", SubHeader);
        StageStory= GUILayout.TextArea(StageStory, GUILayout.Height(40f));
        GUILayout.Label($"Stage Hint", SubHeader);
        StageHint= GUILayout.TextArea(StageHint, GUILayout.Height(40f));
        EditorGUI.EndChangeCheck();
        GUILayout.Space(10);

        #region Model Conditions
        EditorGUI.BeginChangeCheck();
        if (isGenerateMode || isStageEditMode)
        {
            isRemoveMode = false;

            if (MapData.GeneratePlane != null && GeneratePlane == null)
            {
                GeneratePlane = Instantiate(MapData.GeneratePlane);
                GeneratePlane.transform.SetAsFirstSibling();
                GeneratePlane.gameObject.SetActive(true);
            }
        }
        else
        {
            if (GeneratePlane != null)
            {
                DestroyImmediate(GeneratePlane.gameObject);
            }
        }

        EditorGUI.EndChangeCheck();

        EditorGUI.BeginChangeCheck();
        EditorGUILayout.PropertyField(serialProp, true);
        SerialObject.ApplyModifiedProperties();
        EditorGUI.EndChangeCheck();
        EditorGUILayout.EndScrollView();

        #endregion

        EditorGUI.EndChangeCheck();

        EditorGUI.BeginChangeCheck();
        GUILayout.Label($"Stage Name", SubHeader);
        StageName = GUILayout.TextField(StageName, GUILayout.Height(30f));

        GUILayout.Label($"Current Save Path : {SavePath + StageName + SaveTail}", targetStyle);
        EditorGUI.EndChangeCheck();

        // Generate Map
        EditorGUI.BeginChangeCheck();
        if (GUILayout.Button("TileMap Generate", GUILayout.Height(40f)))
        {
            MapData StageDataset = ScriptableObject.CreateInstance<MapData>();
            SerialLoadObject = new SerializedObject(StageDataset);
            serialEnvProp = SerialLoadObject.FindProperty("Buildings");
            serialBuildingProp = SerialLoadObject.FindProperty("Environment");
            serialTileProp = SerialLoadObject.FindProperty("Tiles");
            StageDataset.StageName = StageName;
            StageDataset.StageStory = StageStory;
            StageDataset.StageHint= StageHint;
            StageDataset.BatchMembers = StageBatchMember;
            StageDataset.SpawnCnt = 0;
            StageDataset.Environment = new EnvironmentType[9 * 16];
            StageDataset.Tiles = new TileBuildType[9 * 16];
            StageDataset.Buildings= new PropType[9 * 16];

            for (int col = 0; col < 16; col++)
            {
                for (int row = 0; row < 9; row++)
                {
                    if (tileRecords.ContainsKey((row, col)) == false)
                    {
                        StageDataset.Environment[col + 16*row] = EnvironmentType.None;
                        StageDataset.Tiles[col + 16 * row] = TileBuildType.None;
                        StageDataset.Buildings[col + 16 * row] = PropType.None;
                    }
                    else
                    {
                        TileCompression comp = tileRecords[(row, col)];
                        if (comp.tile != null)
                        {
                            StageDataset.Tiles[col + 16 * row] = comp.tile.GetTileBuildType();
                        }
                        else
                        {
                            StageDataset.Tiles[col + 16 * row] = TileBuildType.None;
                        }

                        if (comp.building != null)
                        {
                            StageDataset.Buildings[col + 16 * row] = comp.building.propType;
                        }
                        else
                        {
                            StageDataset.Buildings[col + 16 * row] = PropType.None;
                        }

                        if (comp.environment != null)
                        {
                            StageDataset.Environment[col + 16 * row] = comp.environment.envType;
                            if (comp.environment.envType == EnvironmentType.SpawnEnv)
                            {
                                StageDataset.SpawnCnt++;
                            }
                        }
                        else
                        {
                            StageDataset.Environment[col + 16 * row] = EnvironmentType.None;
                        }
                    }
                }
            }

            StageDataset.EnemyProgression = MapData.EnemyProgression;
            SerialLoadObject.ApplyModifiedProperties();
            SerialLoadObject.Update();
            AssetDatabase.CreateAsset(StageDataset, SavePath + StageName + SaveTail);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

        }
        EditorGUI.EndChangeCheck();
    }

    void OnSceneGUI(SceneView sceneView)
    {
        EditorGUI.BeginChangeCheck();
        if (isGenerateMode)
        {
            int id = GUIUtility.GetControlID(FocusType.Passive);
            HandleUtility.AddDefaultControl(id);

            Event mouseEvent = Event.current;
            if (mouseEvent.type == EventType.MouseDown && mouseEvent.button == 0) // Left mouse button click
            {
                Ray ray = HandleUtility.GUIPointToWorldRay(mouseEvent.mousePosition);
                //HandleUtility.GUIPointToScreenPixelCoordinate
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask.GetMask("Editor")))
                {
                    
                    // Instantiate a new GameObject at the hit point
                    //GameObject newObject = GameObject.CreatePrimitive(PrimitiveType.Cube); // Example: create a cube
                    //newObject.transform.position = hit.point;
                    Debug.Log($"Plane Hit : {hit.point}");
                    Debug.Log($"Hit Index : {EdgeIndexer(hit.point).Item1}, {EdgeIndexer(hit.point).Item2}");
                    Debug.Log($"Target Position: {EdgeRunner(hit.point)}");

                    Vector3 EdgePosition = EdgeRunner(hit.point);
                    (int, int) EdgeIndex = EdgeIndexer(hit.point);

                    switch (CurrentBuildTarget)
                    {
                        case TileTarget.Environment:
                            for (int i = 0; i < MapData.EnvironmentPrefabs.Length; i++)
                            {
                                if (MapData.EnvironmentPrefabs[i].envType == CurrentEnvType)
                                {
                                    TempEnvPrefab = Instantiate(MapData.EnvironmentPrefabs[i]);
                                    TempEnvPrefab.transform.position = EdgePosition;
                                    TempEnvPrefab.GridPos = new Vector2Int(EdgeIndex.Item1, EdgeIndex.Item2);
                                    if (tileRecords.ContainsKey(EdgeIndex))
                                    {
                                        if (tileRecords[EdgeIndex].environment != null)
                                        {
                                            DestroyImmediate(tileRecords[EdgeIndex].environment.gameObject);
                                            tileRecords[EdgeIndex].environment = TempEnvPrefab;
                                        }
                                        else
                                        {
                                            tileRecords[EdgeIndex].environment = TempEnvPrefab;
                                        }
                                    }
                                    else
                                    {
                                        tileRecords[EdgeIndexer(hit.point)] = new TileCompression();
                                        tileRecords[EdgeIndexer(hit.point)].environment = TempEnvPrefab;
                                        Debug.Log(tileRecords[EdgeIndexer(hit.point)]);
                                    }

                                }
                            }

                            break;
                        case TileTarget.Tile:

                            TempTilePrefab = Instantiate(CurrentTilePrefab);
                            TempTilePrefab.transform.position = EdgePosition;
                            TempTilePrefab.GridPos = new Vector2Int(EdgeIndex.Item1, EdgeIndex.Item2);
                            if (tileRecords.ContainsKey(EdgeIndex))
                            {
                                if (tileRecords[EdgeIndex].tile != null)
                                {
                                    DestroyImmediate(tileRecords[EdgeIndex].tile.gameObject);
                                    tileRecords[EdgeIndex].tile = TempTilePrefab;
                                }
                                else
                                {
                                    tileRecords[EdgeIndex].tile = TempTilePrefab;
                                }

                            }
                            else
                            {
                                tileRecords[EdgeIndexer(hit.point)] = new TileCompression();
                                tileRecords[EdgeIndexer(hit.point)].tile = TempTilePrefab;
                                Debug.Log(tileRecords[EdgeIndexer(hit.point)]);
                            }
                            break;
                        case TileTarget.Building:

                            break;
                    }
                    //Vector3 TargetEdge = EdgeRunner(hit.point);
                    //MapTile tileInstance = Instantiate(CurrentTilePrefab);

                    //newObject.name = "InstantiatedObject";
                    //Undo.RegisterCreatedObjectUndo(newObject, "Create Object"); // For undo functionality
                }
                else
                {
                    Vector3 worldPosition = ray.GetPoint(10); // 10 units in front of the camera
                    Debug.Log($"Plane does not Hit : {worldPosition}");
                }

            }
        }
        EditorGUI.EndChangeCheck();

        EditorGUI.BeginChangeCheck();
        if (isStageEditMode)
        {
            int id = GUIUtility.GetControlID(FocusType.Passive);
            HandleUtility.AddDefaultControl(id);

            Event mouseEvent = Event.current;
            if (mouseEvent.type == EventType.MouseDown && mouseEvent.button == 0) // Left mouse button click
            {
                Ray ray = HandleUtility.GUIPointToWorldRay(mouseEvent.mousePosition);
                //HandleUtility.GUIPointToScreenPixelCoordinate
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask.GetMask("Editor")))
                {
                    SerialObject.Update();

                    int size = MapData.EnemyProgression.Length;
                    Array.Resize(ref MapData.EnemyProgression, ++size);
                    MapData.EnemyProgression[size-1] = new StageData();

                    Vector3 EdgePosition = EdgeRunner(hit.point);
                    (int, int) EdgeIndex = EdgeIndexer(hit.point);
                    MapData.EnemyProgression[size - 1].MonsterType = CurrentEnemy;
                    MapData.EnemyProgression[size - 1].SpawnRow = EdgeIndex.Item1;
                    MapData.EnemyProgression[size - 1].SpawnColumn = EdgeIndex.Item2;
                    MapData.EnemyProgression[size - 1].SpawnTime = TimeScheduling;

                    SerialObject.Update();
                }
                else
                {
                    Vector3 worldPosition = ray.GetPoint(10); // 10 units in front of the camera
                    Debug.Log($"Plane does not Hit : {worldPosition}");
                }
            }


        }
        EditorGUI.EndChangeCheck();
    }

    
    public void EndFrameRendering(ScriptableRenderContext context, List<Camera> cameras)
    {
        DrawBounds();
    }

    void DrawBounds()
    {
        Material material = AssetDatabase.GetBuiltinExtraResource<Material>("Sprites-Default.mat");
        material.SetPass(0);

        EditorGUI.BeginChangeCheck();

        if (MapData.isDrawGrid)
        {
            for (int i = 0; i < 10; i++)
            {
                GL.PushMatrix();
                GL.Begin(GL.LINES);
                GL.Color(Color.white);
                GL.Vertex3(-8f, 0f, -4.5f + i);
                GL.Vertex3(8f, 0f, -4.5f + i);
                GL.End();
                GL.PopMatrix();
            }

            for (int i = 0; i < 17; i++)
            {
                GL.PushMatrix();
                GL.Begin(GL.LINES);
                GL.Color(Color.white);
                GL.Vertex3(-8f + i, 0f, -4.5f);
                GL.Vertex3(-8f + i, 0f, 4.5f);
                GL.End();
                GL.PopMatrix();
                //Gizmos.DrawLine(new Vector3(-9f + i, 0, -4f), new Vector3(-9f + i, 0, 4f));
            }
        }

        
        if (MapData.isDrawCoordinate)
        {
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 16; j++)
                {
                    GUIStyle style = new GUIStyle();
                    style.normal.textColor = Color.white;
                    style.fontSize = 12;
                    Handles.Label(new Vector3(-7.5f + j, 0f, -4f + i), $"({j}, {i})", style);
                }
            }
        }
        EditorGUI.EndChangeCheck();
    }

    private Vector3 EdgeRunner(Vector3 HitPoint)
    {
        float x = HitPoint.x;
        x = Mathf.FloorToInt(x) + 0.5f;

        float z = HitPoint.z;
        if (z - Mathf.FloorToInt(z) < 0.5f)
        {
            z = Mathf.FloorToInt(z);
        }
        else
        {
            z = Mathf.CeilToInt(z);
        }

        return new Vector3(x, 0f, z);
    }

    private (int, int) EdgeIndexer(Vector3 HitPoint)
    {

        int x = Mathf.FloorToInt(HitPoint.x);

        // -9f, 9f : 16
        float indexerX = remap(x, -9f, 8f, 0f, 16f);

        float z = HitPoint.z;
        if (z - Mathf.FloorToInt(z) < 0.5f)
        {
            z = Mathf.FloorToInt(z);
        }
        else
        {
            z = Mathf.CeilToInt(z);
        }
        float indexerY = remap(z, -4f, 4f, 0f, 8f);

        return ((int)indexerY, (int)indexerX);
    }

    private float remap(float val, float in1, float in2, float out1, float out2)
    {
        return out1 + (val - in1) * (out2 - out1) / (in2 - in1);
    }

    private void RecordClear()
    {
        foreach(TileCompression record in tileRecords.Values)
        {
            if (record.tile != null)
            { 
                DestroyImmediate(record.tile.gameObject);
            }

            if (record.building != null)
            {
                DestroyImmediate(record.building.gameObject);
            }

            if (record.environment != null)
            {
                DestroyImmediate(record.environment.gameObject);
            }
        }
    }

    private void RecordRemove((int, int) _targetIndex)
    {
        TileCompression record = tileRecords[_targetIndex];

        if (record.tile != null)
        {
            DestroyImmediate(record.tile.gameObject);
        }

        if (record.building != null)
        {
            DestroyImmediate(record.building.gameObject);
        }

        if (record.environment != null)
        {
            DestroyImmediate(record.environment.gameObject);
        }

    }

    private void CreateInstanceFromLoadButton()
    {
        MapData.EnemyProgression = LoadData.EnemyProgression;
        // SerialLoadObject.Update();
        StageName = LoadData.StageName;
        StageStory = LoadData.StageStory;
        StageHint= LoadData.StageHint;
        StageBatchMember = LoadData.BatchMembers;

        Dictionary<(int, int), TileCompression> tempRecords = new Dictionary<(int, int), TileCompression>();

        for (int row = 0; row < 9; row++)
        {
            for (int col = 0; col < 16; col++)
            {
                if (tileRecords.ContainsKey((row, col)))
                {
                    if (tileRecords[(row, col)].building != null)
                    {
                        DestroyImmediate(tileRecords[(row, col)].building.gameObject);
                    }
                    if (tileRecords[(row, col)].tile != null)
                    {
                        DestroyImmediate(tileRecords[(row, col)].tile.gameObject);
                    }
                    if (tileRecords[(row, col)].environment != null)
                    {
                        DestroyImmediate(tileRecords[(row, col)].environment.gameObject);
                    }
                }

                Vector3 targetVec = new Vector3(-8.0f+ col, 0, -4.5f + row);
                Vector3 EdgePosition = EdgeRunner(targetVec);
                (int, int) EdgeIndex = EdgeIndexer(targetVec);
                if (LoadData.Environment[col + 16 * row] != EnvironmentType.None)
                {
                    foreach(Environments envPrefab in MapData.EnvironmentPrefabs)
                    {
                        if (envPrefab.envType == LoadData.Environment[col + 16 * row])
                        {
                            Environments envObject = Instantiate(envPrefab);
                            envObject.transform.position = EdgePosition;
                            envObject.GridPos = new Vector2Int(row, col);

                            if (tempRecords.ContainsKey((row, col)) == false)
                            {
                                tempRecords[(row, col)] = new TileCompression() { environment = envObject };
                            }
                            else
                            {
                                tempRecords[(row, col)].environment = envObject;
                            }

                        }


                    }
                }

                if (LoadData.Tiles[col + 16 * row] != TileBuildType.None)
                {
                    foreach (MapTile envPrefab in MapData.TilePrefabs)
                    {
                        if (envPrefab.GetTileBuildType() == LoadData.Tiles[col + 16 * row])
                        {
                            MapTile envObject = Instantiate(envPrefab);
                            envObject.transform.position = EdgePosition;
                            envObject.GridPos = new Vector2Int(row, col);

                            if (tempRecords.ContainsKey((row, col)) == false)
                            {
                                tempRecords[(row, col)] = new TileCompression() { tile = envObject };
                            }
                            else
                            {
                                tempRecords[(row, col)].tile = envObject;
                            }
                        }
                    }
                }

                if (    LoadData.Buildings[col + 16 * row] != PropType.None)
                {
                    foreach (Props envPrefab in MapData.PropPrefabs)
                    {
                        if (envPrefab.propType == LoadData.Buildings[col + 16 * row])
                        {
                            Props envObject = Instantiate(envPrefab);
                            envObject.transform.position = EdgePosition;
                            envObject.GridPos = new Vector2Int(row, col);

                            if (tempRecords.ContainsKey((row, col)) == false)
                            {
                                tempRecords[(row, col)] = new TileCompression() { building = envObject };
                            }
                            else
                            {
                                tempRecords[(row, col)].building = envObject;
                            }
                        }
                    }
                }

            }
        }

        tileRecords = tempRecords;


    }

}