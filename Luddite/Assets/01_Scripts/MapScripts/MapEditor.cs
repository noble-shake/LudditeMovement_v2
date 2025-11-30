using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
using UnityEngine.UIElements;

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
    public GameObject environment;

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
    public GameObject environment;

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
    private const string SavePath = "Assets/MapDataset/MapData.asset";

    // ScrollBar
    [SerializeField] private Vector2 entirePos = Vector2.zero;

    // isGenerateMode 는 실제 Scene에 영향력을 주므로, False를 디폴트로 한다.
    private bool isGenerateMode = false;
    private bool isRemoveMode = false;

    private TileBuildType CurrentTile;
    private TileTarget CurrentBuildTarget = TileTarget.Tile;
    private EnemyName CurrentEnemy;

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

    // Time Slider
    private float TimeScheduling;

    private void OnEnable()
    {
        tileRecords = new Dictionary<(int, int), TileCompression>();
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

        GUILayout.Label("Tile Prefab Collection", SubHeader);

        #region Prefab Collections

        GUILayout.Label("Normal Tile", SubHeader);
        GUILayout.BeginHorizontal();
        GUILayout.BeginVertical();
        GUILayout.Label("Road", tileStyle);
        MapData.RoadTilePrefab = ((MapTile)EditorGUILayout.ObjectField(MapData.RoadTilePrefab, typeof(MapTile), true, GUILayout.Width(48f)));
        GUILayout.EndVertical();
        GUILayout.BeginVertical();
        GUILayout.Label("Dark", tileStyle);
        MapData.DarkTilePrefab = ((MapTile)EditorGUILayout.ObjectField(MapData.DarkTilePrefab, typeof(MapTile), true, GUILayout.Width(48f)));
        GUILayout.EndVertical();
        GUILayout.BeginVertical();
        GUILayout.Label("Water", tileStyle);
        MapData.WaterTilePrefab = ((MapTile)EditorGUILayout.ObjectField(MapData.WaterTilePrefab, typeof(MapTile), true, GUILayout.Width(48f)));
        GUILayout.EndVertical();
        GUILayout.BeginVertical();
        GUILayout.Label("Sand", tileStyle);
        MapData.SandTilePrefab = ((MapTile)EditorGUILayout.ObjectField(MapData.SandTilePrefab, typeof(MapTile), true, GUILayout.Width(48f)));
        GUILayout.EndVertical();
        GUILayout.EndHorizontal();
        GUILayout.Space(10);

        GUILayout.Label("Block Normal", SubHeader);
        GUILayout.BeginHorizontal();
        GUILayout.BeginVertical();
        GUILayout.Label("Top", tileStyle);
        MapData.BlockTopTilePrefab = ((MapTile)EditorGUILayout.ObjectField(MapData.BlockTopTilePrefab, typeof(MapTile), true, GUILayout.Width(48f)));
        GUILayout.EndVertical();
        GUILayout.BeginVertical();
        GUILayout.Label("Down", tileStyle);
        MapData.BlockDownTilePrefab = ((MapTile)EditorGUILayout.ObjectField(MapData.BlockDownTilePrefab, typeof(MapTile), true, GUILayout.Width(48f)));
        GUILayout.EndVertical();
        GUILayout.BeginVertical();
        GUILayout.Label("Left", tileStyle);
        MapData.BlockLeftTilePrefab = ((MapTile)EditorGUILayout.ObjectField(MapData.BlockLeftTilePrefab, typeof(MapTile), true, GUILayout.Width(48f)));
        GUILayout.EndVertical();
        GUILayout.BeginVertical();
        GUILayout.Label("Right", tileStyle);
        MapData.BlockRightTilePrefab = ((MapTile)EditorGUILayout.ObjectField(MapData.BlockRightTilePrefab, typeof(MapTile), true, GUILayout.Width(48f)));
        GUILayout.EndVertical();
        GUILayout.EndHorizontal();
        GUILayout.Space(10);

        GUILayout.Label("Block Normal 2side", SubHeader);
        GUILayout.BeginHorizontal();
        GUILayout.BeginVertical();
        GUILayout.Label("Top+Left", tileStyle);
        MapData.BlockTopLeftTilePrefab = ((MapTile)EditorGUILayout.ObjectField(MapData.BlockTopLeftTilePrefab, typeof(MapTile), true, GUILayout.Width(48f)));
        GUILayout.EndVertical();
        GUILayout.BeginVertical();
        GUILayout.Label("Top+Right", tileStyle);
        MapData.BlockTopRightTilePrefab= ((MapTile)EditorGUILayout.ObjectField(MapData.BlockTopRightTilePrefab, typeof(MapTile), true, GUILayout.Width(48f)));
        GUILayout.EndVertical();
        GUILayout.BeginVertical();
        GUILayout.Label("Down+Left", tileStyle);
        MapData.BlockDownLeftTilePrefab = ((MapTile)EditorGUILayout.ObjectField(MapData.BlockDownLeftTilePrefab, typeof(MapTile), true, GUILayout.Width(48f)));
        GUILayout.EndVertical();
        GUILayout.BeginVertical();
        GUILayout.Label("Down+Right", tileStyle);
        MapData.BlockDownRightTilePrefab = ((MapTile)EditorGUILayout.ObjectField(MapData.BlockDownRightTilePrefab, typeof(MapTile), true, GUILayout.Width(48f)));
        GUILayout.EndVertical();
        GUILayout.EndHorizontal();
        GUILayout.Space(10);

        GUILayout.Label("Block Water", SubHeader);
        GUILayout.BeginHorizontal();
        GUILayout.BeginVertical();
        GUILayout.Label("Top", tileStyle);
        MapData.BlockTopWaterTilePrefab = ((MapTile)EditorGUILayout.ObjectField(MapData.BlockTopWaterTilePrefab, typeof(MapTile), true, GUILayout.Width(48f)));
        GUILayout.EndVertical();
        GUILayout.BeginVertical();
        GUILayout.Label("Down", tileStyle);
        MapData.BlockDownWaterTilePrefab = ((MapTile)EditorGUILayout.ObjectField(MapData.BlockDownWaterTilePrefab, typeof(MapTile), true, GUILayout.Width(48f)));
        GUILayout.EndVertical();
        GUILayout.BeginVertical();
        GUILayout.Label("Left", tileStyle);
        MapData.BlockLeftWaterTilePrefab = ((MapTile)EditorGUILayout.ObjectField(MapData.BlockLeftWaterTilePrefab, typeof(MapTile), true, GUILayout.Width(48f)));
        GUILayout.EndVertical();
        GUILayout.BeginVertical();
        GUILayout.Label("Right", tileStyle);
        MapData.BlockRightWaterTilePrefab = ((MapTile)EditorGUILayout.ObjectField(MapData.BlockRightWaterTilePrefab, typeof(MapTile), true, GUILayout.Width(48f)));
        GUILayout.EndVertical();
        GUILayout.EndHorizontal();
        GUILayout.Space(10);

        GUILayout.Label("Block Water 2side", SubHeader);
        GUILayout.BeginHorizontal();
        GUILayout.BeginVertical();
        GUILayout.Label("Top+Left", tileStyle);
        MapData.BlockTopLeftWaterTilePrefab = ((MapTile)EditorGUILayout.ObjectField(MapData.BlockTopLeftWaterTilePrefab, typeof(MapTile), true, GUILayout.Width(48f)));
        GUILayout.EndVertical();
        GUILayout.BeginVertical();
        GUILayout.Label("Top+Right", tileStyle);
        MapData.BlockTopRightWaterTilePrefab = ((MapTile)EditorGUILayout.ObjectField(MapData.BlockTopRightWaterTilePrefab, typeof(MapTile), true, GUILayout.Width(48f)));
        GUILayout.EndVertical();
        GUILayout.BeginVertical();
        GUILayout.Label("Down+Left", tileStyle);
        MapData.BlockDownLeftWaterTilePrefab = ((MapTile)EditorGUILayout.ObjectField(MapData.BlockDownLeftWaterTilePrefab, typeof(MapTile), true, GUILayout.Width(48f)));
        GUILayout.EndVertical();
        GUILayout.BeginVertical();
        GUILayout.Label("Down+Right", tileStyle);
        MapData.BlockDownRightWaterTilePrefab = ((MapTile)EditorGUILayout.ObjectField(MapData.BlockDownRightWaterTilePrefab, typeof(MapTile), true, GUILayout.Width(48f)));
        GUILayout.EndVertical();

        GUILayout.EndHorizontal();
        #endregion
        
        GUILayout.Space(16);
        //EditorGUI.EndChangeCheck();


        EditorGUI.EndChangeCheck();

        #region Prefab Switching

        EditorGUI.BeginChangeCheck();
        switch (CurrentTile)
        {
            default:
            case TileBuildType.Road:
                CurrentTilePrefab = MapData.RoadTilePrefab;
                break;
            case TileBuildType.Dark:
                CurrentTilePrefab = MapData.DarkTilePrefab;
                break;
            case TileBuildType.Sand:
                CurrentTilePrefab = MapData.SandTilePrefab;
                break;
            case TileBuildType.Water:
                CurrentTilePrefab = MapData.WaterTilePrefab;
                break;
            case TileBuildType.BlockTop:
                CurrentTilePrefab = MapData.BlockTopTilePrefab;
                break;
            case TileBuildType.BlockDown:
                CurrentTilePrefab = MapData.BlockDownTilePrefab;
                break;
            case TileBuildType.BlockLeft:
                CurrentTilePrefab = MapData.BlockLeftTilePrefab;
                break;
            case TileBuildType.BlockRight:
                CurrentTilePrefab = MapData.BlockRightTilePrefab;
                break;
            case TileBuildType.BlockTopLeft:
                CurrentTilePrefab = MapData.BlockTopLeftTilePrefab;
                break;
            case TileBuildType.BlockTopRight:
                CurrentTilePrefab = MapData.BlockTopRightTilePrefab;
                break;
            case TileBuildType.BlockDownLeft:
                CurrentTilePrefab = MapData.BlockDownLeftTilePrefab;
                break;
            case TileBuildType.BlockDownRight:
                CurrentTilePrefab = MapData.BlockDownRightTilePrefab;
                break;
            case TileBuildType.BlockTopWater:
                CurrentTilePrefab = MapData.BlockTopWaterTilePrefab;
                break;
            case TileBuildType.BlockDownWater:
                CurrentTilePrefab = MapData.BlockDownWaterTilePrefab;
                break;
            case TileBuildType.BlockLeftWater:
                CurrentTilePrefab = MapData.BlockLeftWaterTilePrefab;
                break;
            case TileBuildType.BlockRightWater:
                CurrentTilePrefab = MapData.BlockRightWaterTilePrefab;
                break;
            case TileBuildType.BlockTopLeftWater:
                CurrentTilePrefab = MapData.BlockTopLeftWaterTilePrefab;
                break;
            case TileBuildType.BlockTopRightWater:
                CurrentTilePrefab = MapData.BlockTopRightWaterTilePrefab;
                break;
            case TileBuildType.BlockDownLeftWater:
                CurrentTilePrefab = MapData.BlockDownLeftWaterTilePrefab;
                break;
            case TileBuildType.BlockDownRightWater:
                CurrentTilePrefab = MapData.BlockDownRightWaterTilePrefab;
                break;
        }

        #endregion

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
        CurrentTile = ((TileBuildType)EditorGUILayout.EnumFlagsField(CurrentTile));

        GUILayout.EndHorizontal();
        GUILayout.Label($"Current Tile : [ {CurrentTile.ToString()} ]", tileStyle);

        GUILayout.Space(10);
        GUILayout.Label($"Select BuildType", targetStyle);
        CurrentBuildTarget = ((TileTarget)EditorGUILayout.EnumFlagsField(CurrentBuildTarget));
        GUILayout.Label("Selected Tile Target (Tile, Building, Environment)", tileStyle);

        GUILayout.Space(10);
        GUILayout.Label($"Select EnemyType", targetStyle);
        CurrentEnemy = ((EnemyName)EditorGUILayout.EnumFlagsField(CurrentEnemy));
        GUILayout.Label("Selecte Enemy Type", tileStyle);

        EditorGUI.BeginChangeCheck();
        GUILayout.Space(16);
        GUILayout.Label($"Time Schedule", SubHeader);
        GUILayout.Label($"GameTime : {Mathf.FloorToInt(TimeScheduling/60f).ToString("D2")}:{Mathf.FloorToInt(TimeScheduling% 60f).ToString("D2")}", targetStyle);
        TimeScheduling = EditorGUILayout.Slider(TimeScheduling, 0f, 60f * 10, GUILayout.Height(20f));
        EditorGUI.EndChangeCheck();
        GUILayout.Space(10);

        #region Model Conditions
        if (isGenerateMode)
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
        if (isStageEditMode)
        {
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
        EditorGUILayout.EndScrollView();

        #endregion



        // Generate Map
        EditorGUI.BeginChangeCheck();
        bool ButtonPressed = false;
        if (GUILayout.Button("TileMap Generate", EditorStyles.miniButton))
        {
            ButtonPressed = true;
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

                    switch (CurrentBuildTarget)
                    {
                        case TileTarget.Environment:
                        case TileTarget.Tile:
                            Vector3 EdgePosition = EdgeRunner(hit.point);
                            (int, int) EdgeIndex= EdgeIndexer(hit.point);
                            TempTilePrefab = Instantiate(CurrentTilePrefab);
                            TempTilePrefab.transform.position = EdgePosition;
                            if (tileRecords.ContainsKey(EdgeIndex))
                            {
                                if (tileRecords[EdgeIndex].tile != null)
                                {
                                    DestroyImmediate(tileRecords[EdgeIndex].tile);
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

                    MapData.EnemyProgression[size - 1].SpawnRow = EdgeIndex.Item2;
                    MapData.EnemyProgression[size - 1].SpawnColumn = EdgeIndex.Item1;
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

        return ((int)indexerX, (int)indexerY);
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

}