using UnityEngine;

public class TileRoad : MapTile
{

    private void Start()
    {
        tileType = TileType.Road;
        tileBuildType = TileBuildType.Road;
    }
}