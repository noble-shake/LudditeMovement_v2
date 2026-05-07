using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// 9, 16
public class MapManager : MonoBehaviour
{
    public Vector3 OriginPoint;
    public static MapManager Instance;
    public GridPathfinding gridPathfinding;
    public MapData MapDataset;
    public Transform TargetPoint;
    public Player TestPlayer;

    private void Awake()
    {
        if (Instance == null) { Instance = this; } else { Destroy(gameObject); }
    }

    private void Start()
    {
        OriginPoint = transform.position;

        gridPathfinding = new GridPathfinding(16, 9, 1, new Vector3(-7.5f, 0f, -4f));
    }

    public void SetMapData(MapData data)
    {
        MapDataset = data;
        gridPathfinding.SetWalkable(MapDataset.Tiles, MapDataset.Buildings);
    }

    public void MapLoad()
    {
        /*
         *  1. Tile Batch
         *  2. Environment Batch
         *  3. Props Batch
        */

        PropType[] Props = MapDataset.Buildings;
        EnvironmentType[] Envs = MapDataset.Environment;
        TileBuildType[] Tiles = MapDataset.Tiles;

        int totalTile = 144;
        // Tile
        for(int index=0; index < totalTile; index++)
        {
            if (Tiles[index] == TileBuildType.None)
            {
                Tiles[index] = TileBuildType.Dark;
            }

            (int, int) indexer = ResourceManager.Instance.FindTileIndex(index);
            ResourceManager.Instance.TileActive(indexer, Tiles[index]);

            if (Envs[index] != EnvironmentType.None)
            {
                ResourceManager.Instance.TileActive(indexer, Envs[index]);
            }

            if (Props[index] != PropType.None)
            {
                ResourceManager.Instance.TileActive(indexer, Props[index]);
            }
        }
    }


    #region LEGACY
    public void Test()
    {
        List<PathNode> paths = gridPathfinding.GetPath(TestPlayer.transform.position, TargetPoint.position);
        Debug.Log($"CNT: {paths.Count}");
        foreach (PathNode p in paths)
        {
            Debug.Log($"{p.GetMapPos()}");
        }
    }

    #endregion


}
