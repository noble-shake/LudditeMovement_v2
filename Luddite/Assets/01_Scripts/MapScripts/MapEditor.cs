using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

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
    //private bool isDrawCoordinate;
    //private bool isDrawGrid;

    // isGenerateMode 는 실제 Scene에 영향력을 주므로, False를 디폴트로 한다.
    private bool isGenerateMode = false;
    private bool isRemoveMode = false;

    //[SerializeField] private GameObject GeneratePlane;

    private TileBuildType CurrentTile;
    private TileTarget CurrentBuildTarget = TileTarget.Tile;
    //private MapManager mapInstance;
    private Text TitleLabel;
    public GameObject GeneratePlane;

    [SerializeField] private MapTile CurrentTilePrefab;
    [SerializeField] private MapTile RoadTilePrefab;
    [SerializeField] private MapTile BlockTilePrefab;
    [SerializeField] private MapTile DarkTilePrefab;
    [SerializeField] private MapTile WaterTilePrefab;
    [SerializeField] private MapTile SandTilePrefab;

    public Dictionary<(int, int), TileCompression> tileRecords;
    private MapTile TempTilePrefab;

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

    }

    private void OnDisable()
    {
        RecordClear();
        RenderPipelineManager.endContextRendering -= EndFrameRendering;
        SceneView.duringSceneGui -= OnSceneGUI;

    }

    private void OnGUI()
    {


        ////isGenerateMode = EditorGUILayout.Toggle(isGenerateMode, )
        GUILayout.BeginVertical();
        GUIStyle header = new GUIStyle();
        header.normal.textColor = Color.white;
        header.fontSize = 16;
        GUILayout.Label("맵 에디터", header);
        //MapData.MapInstance = ((MapManager)EditorGUILayout.ObjectField("MapManagerInstance", MapData.MapInstance, typeof(MapManager), true));
        MapData.isDrawCoordinate = EditorGUILayout.Toggle("Draw Coordinate", MapData.isDrawCoordinate);
        MapData.isDrawGrid = EditorGUILayout.Toggle("Draw Grid", MapData.isDrawGrid);
        GUILayout.EndVertical();


        GUIStyle tileStyle = new GUIStyle();
        tileStyle.normal.textColor = Color.white;
        tileStyle.fontSize = 12;

        GUILayout.BeginHorizontal();
        GUILayout.Label("RoadTilePrefab", tileStyle);
        MapData.RoadTilePrefab = ((MapTile)EditorGUILayout.ObjectField(MapData.RoadTilePrefab, typeof(MapTile), true));
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("DarkTilePrefab", tileStyle);
        MapData.DarkTilePrefab = ((MapTile)EditorGUILayout.ObjectField(MapData.DarkTilePrefab, typeof(MapTile), true));
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        GUILayout.Label("WaterTilePrefab", tileStyle);
        MapData.WaterTilePrefab = ((MapTile)EditorGUILayout.ObjectField(MapData.WaterTilePrefab, typeof(MapTile), true));
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        GUILayout.Label("SandTilePrefab", tileStyle);
        MapData.SandTilePrefab = ((MapTile)EditorGUILayout.ObjectField(MapData.SandTilePrefab, typeof(MapTile), true));
        GUILayout.EndHorizontal();

        GUILayout.Label("BlockTilePrefab", tileStyle);
        MapData.BlockTopTilePrefab = ((MapTile)EditorGUILayout.ObjectField(MapData.BlockTopTilePrefab, typeof(MapTile), true));
        MapData.BlockDownTilePrefab = ((MapTile)EditorGUILayout.ObjectField(MapData.BlockDownTilePrefab, typeof(MapTile), true));
        MapData.BlockLeftTilePrefab = ((MapTile)EditorGUILayout.ObjectField(MapData.BlockTopTilePrefab, typeof(MapTile), true));
        MapData.BlockRightTilePrefab = ((MapTile)EditorGUILayout.ObjectField(MapData.BlockDownTilePrefab, typeof(MapTile), true));
        MapData.BlockTopLeftTilePrefab = ((MapTile)EditorGUILayout.ObjectField(MapData.BlockTopTilePrefab, typeof(MapTile), true));
        MapData.BlockDownRightTilePrefab = ((MapTile)EditorGUILayout.ObjectField(MapData.BlockDownWaterTilePrefab, typeof(MapTile), true));
        MapData.BlockTopWaterTilePrefab = ((MapTile)EditorGUILayout.ObjectField(MapData.BlockTopWaterTilePrefab, typeof(MapTile), true));
        MapData.BlockDownWaterTilePrefab = ((MapTile)EditorGUILayout.ObjectField(MapData.BlockDownWaterTilePrefab, typeof(MapTile), true));
        MapData.BlockLeftWaterTilePrefab = ((MapTile)EditorGUILayout.ObjectField(MapData.BlockTopWaterTilePrefab, typeof(MapTile), true));
        MapData.BlockRightWaterTilePrefab = ((MapTile)EditorGUILayout.ObjectField(MapData.BlockDownWaterTilePrefab, typeof(MapTile), true));
        MapData.BlockTopLeftWaterTilePrefab = ((MapTile)EditorGUILayout.ObjectField(MapData.BlockTopWaterTilePrefab, typeof(MapTile), true));
        MapData.BlockDownRightWaterTilePrefab = ((MapTile)EditorGUILayout.ObjectField(MapData.BlockDownWaterTilePrefab, typeof(MapTile), true));


        //EditorGUI.BeginChangeCheck();
        //if (RoadTilePrefab != null)
        //{
        //    RoadTilePreview = Editor.CreateEditor(RoadTilePrefab.gameObject);
        //    RoadTilePreview.OnPreviewGUI(GUILayoutUtility.GetRect(256, 256), );
        //}

        //EditorGUI.EndChangeCheck();
        GUILayout.BeginHorizontal();
        EditorGUI.BeginChangeCheck();
        GUILayout.Label("Selected Tile", tileStyle);
        CurrentTile = ((TileBuildType)EditorGUILayout.EnumFlagsField(CurrentTile));

        GUILayout.EndHorizontal();
        GUILayout.Label($"Current Tile {CurrentTile.ToString()}", tileStyle);
        EditorGUI.EndChangeCheck();

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
                CurrentTilePrefab = MapData.RoadTilePrefab;
                break;
            case TileBuildType.BlockDownRight:
                CurrentTilePrefab = MapData.RoadTilePrefab;
                break;
            case TileBuildType.BlockTopWater:
                CurrentTilePrefab = MapData.RoadTilePrefab;
                break;
            case TileBuildType.BlockDownWater:
                CurrentTilePrefab = MapData.RoadTilePrefab;
                break;
            case TileBuildType.BlockLeftWater:
                CurrentTilePrefab = MapData.RoadTilePrefab;
                break;
            case TileBuildType.BlockRightWater:
                CurrentTilePrefab = MapData.RoadTilePrefab;
                break;
            case TileBuildType.BlockTopLeftWater:
                CurrentTilePrefab = MapData.RoadTilePrefab;
                break;
            case TileBuildType.BlockTopRightWater:
                CurrentTilePrefab = MapData.RoadTilePrefab;
                break;
            case TileBuildType.BlockDownLeftWater:
                CurrentTilePrefab = MapData.RoadTilePrefab;
                break;
            case TileBuildType.BlockDownRightWater:
                CurrentTilePrefab = MapData.RoadTilePrefab;
                break;
        }

        //GUILayout.BeginHorizontal();
        //// GeneratePlane = ((GameObject)EditorGUILayout.ObjectField(MapData.GeneratePlane, typeof(GameObject), false));
        //GUILayout.EndHorizontal();
        isGenerateMode = EditorGUILayout.Toggle("Add Tile Mode", isGenerateMode);
        GUILayout.Label("Selected Tile Target (Tile, Building, Environment)", tileStyle);
        CurrentBuildTarget = ((TileTarget)EditorGUILayout.EnumFlagsField(CurrentBuildTarget));
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
        isRemoveMode = EditorGUILayout.Toggle("Remove Tile Mode", isRemoveMode);
        EditorGUI.EndChangeCheck();

    }

    void OnSceneGUI(SceneView sceneView)
    {
        EditorGUI.BeginChangeCheck();
        if (isGenerateMode)
        {
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