using System.Collections.Generic;
using UnityEngine;

public class MapEditorScriptableObject : ScriptableObject
{
    public bool isDrawCoordinate;
    public bool isDrawGrid;
    public GameObject GeneratePlane;

    public MapTile RoadTilePrefab;
    public MapTile DarkTilePrefab;
    public MapTile WaterTilePrefab;
    public MapTile SandTilePrefab;

    public MapTile BlockTopTilePrefab;
    public MapTile BlockDownTilePrefab;
    public MapTile BlockLeftTilePrefab;
    public MapTile BlockRightTilePrefab;
    public MapTile BlockTopLeftTilePrefab;
    public MapTile BlockTopRightTilePrefab;
    public MapTile BlockDownLeftTilePrefab;
    public MapTile BlockDownRightTilePrefab;
    public MapTile BlockTopWaterTilePrefab;
    public MapTile BlockDownWaterTilePrefab;
    public MapTile BlockLeftWaterTilePrefab;
    public MapTile BlockRightWaterTilePrefab;
    public MapTile BlockTopLeftWaterTilePrefab;
    public MapTile BlockTopRightWaterTilePrefab;
    public MapTile BlockDownLeftWaterTilePrefab;
    public MapTile BlockDownRightWaterTilePrefab;

    public List<GameObject> RoadEnvironment;

}