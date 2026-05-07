using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName ="MapDataset", menuName ="Lucide-Boundary/MapData", order = -1)]
public class MapData : ScriptableObject
{

    [SerializeField] public string StageName;
    [SerializeField] public int BatchMembers;
    [SerializeField, TextArea] public string StageStory;
    [SerializeField, TextArea] public string StageHint;
    [SerializeField] public TileBuildType[] Tiles;
    [SerializeField] public int SpawnCnt;
    [SerializeField] public EnvironmentType[] Environment;
    [SerializeField] public PropType[] Buildings;
    [SerializeField] public StageData[] EnemyProgression;
}