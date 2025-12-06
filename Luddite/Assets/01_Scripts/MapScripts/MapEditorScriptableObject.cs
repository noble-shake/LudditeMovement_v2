using System.Collections.Generic;
using UnityEngine;

public class MapEditorScriptableObject : ScriptableObject
{
    public bool isDrawCoordinate;
    public bool isDrawGrid;
    public GameObject GeneratePlane;

    public List<MapTile> TilePrefabs;
    public List<Enemy> EnemyPrefabs;

    public Environments[] EnvironmentPrefabs;

    public Props[] PropPrefabs;
    public StageData[] EnemyProgression;

}