using UnityEngine;

public class TileSand: MapTile
{

    private void Start()
    {
        tileType = TileType.Sand;
        tileBuildType = TileBuildType.Sand;
    }
}