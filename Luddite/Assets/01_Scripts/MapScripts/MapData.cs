using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="MapDataset", menuName ="Lucide-Boundary/MapData", order = -1)]
public class MapData : ScriptableObject
{
    public string StageName;
    public List<KeyValuePair<(int, int), MapTile>> Tiles; 
    public List<KeyValuePair<(int, int), GameObject>> Environment;
    public List<KeyValuePair<(int, int), Props>> Buildings;

    public void SetData((int, int) _indexer, TileCompression _compressed)
    {
        Tiles.Add(new KeyValuePair<(int, int), MapTile>(_indexer, _compressed.tile));
        Environment.Add(new KeyValuePair<(int, int), GameObject>(_indexer, _compressed.environment));
        Buildings.Add(new KeyValuePair<(int, int), Props>(_indexer, _compressed.building));
    }
}