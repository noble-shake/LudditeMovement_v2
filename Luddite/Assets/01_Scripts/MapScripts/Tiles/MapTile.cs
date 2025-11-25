using UnityEngine;


public enum TileTarget
{ 
    Tile,
    Building,
    Environment,
}

public enum directionalTile
{ 
    Left,
    Top,
    Right,
    Down
}

public enum TileType
{
    Road,
    Water,
    Sand,
    Dark,
    Block,
}

public enum TileBuildType
{
    Road,
    Water,
    Sand,
    Dark,
    BlockTop,
    BlockDown,
    BlockLeft,
    BlockRight,
    BlockTopLeft,
    BlockTopRight,
    BlockDownLeft,
    BlockDownRight,
    BlockTopWater,
    BlockDownWater,
    BlockLeftWater,
    BlockRightWater,
    BlockTopLeftWater,
    BlockTopRightWater,
    BlockDownLeftWater,
    BlockDownRightWater,
}

public struct directionalGridReleation
{
    public MapTile LeftTile;
    public MapTile TopTile;
    public MapTile RightTile;
    public MapTile DownTile;
}

public abstract class MapTile : MonoBehaviour
{
    public bool isBlocked;
    public bool isBuildingExist;
    protected TileType tileType;
    protected TileBuildType tileBuildType;
    protected directionalGridReleation closerGrid;

    public TileType GetTileType()
    {
        return tileType;
    }

    public TileBuildType GetTileBuildType()
    {
        return tileBuildType;
    }

    public void SetRelationalTile(ref MapTile _tile, directionalTile _direction)
    {
        switch (_direction)
        {
            default:
            case directionalTile.Left:
                closerGrid.LeftTile = _tile;
                break;
            case directionalTile.Top:
                closerGrid.TopTile = _tile;
                break;
            case directionalTile.Right:
                closerGrid.RightTile = _tile;
                break;
            case directionalTile.Down:
                closerGrid.DownTile = _tile;
                break;
        }
    }

    
}