using System;
using System.Collections.Generic;
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
        gridPathfinding.SetWalkable(MapDataset.Tiles, MapDataset.Buildings);

        // InvokeRepeating("Test", 1f, 2f);
    }

    public void Test()
    {
        List<PathNode> paths = gridPathfinding.GetPath(TestPlayer.transform.position, TargetPoint.position);
        Debug.Log($"CNT: {paths.Count}");
        foreach (PathNode p in paths)
        {
            Debug.Log($"{p.GetMapPos()}");
        }
    }




}
