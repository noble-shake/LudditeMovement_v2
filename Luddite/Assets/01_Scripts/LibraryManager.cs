using System;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;
using System.Data;
using Unity.Collections.LowLevel.Unsafe;


[Serializable]
public class EnemyAnalysis
{
    public EnemyName enemyType;
    public string Name;
    public bool Description;
    public bool Meet;
    public bool Comment1;
    public bool Comment2;
    public bool Comment3;
}

[Serializable]
public class PlayerAnalysis
{
    public PlayerClassType classType;
    public int Level;
    public int RemainedSkillPoint;
    public string Name;
    public bool Memory1;
    public bool Memory2;
    public bool Memory3;
    public bool isLocked;
    public SkillTree NormalSkillTree;
    public SkillTree Active1SkillTree;
    public SkillTree Active2SkillTree;
    public TreeNode CurrentActive1;
    public TreeNode CurrentActive2;
}

[Serializable]
public class StageAnalysis
{
    public int StageID;
    public float PlayTime;
    public int EasyScore;
    public int NormalScore;
    public int HardScore;
    public int NightmareScore;
    public bool isLocked;
    public bool isCleared;

    public int GetScore(Difficulty _diff)
    {
        switch (_diff)
        {
            default:
            case Difficulty.Normal:
                return NormalScore;
            case Difficulty.Easy:
                return EasyScore;
            case Difficulty.Hard:
                return HardScore;
            case Difficulty.Nightmare:
                return NightmareScore;
        }
    }
}

[Serializable]
public class DataContainer
{
    public float TotalPlaytime;
    public Dictionary<int, StageAnalysis> stageAnalyses;
    public Dictionary<PlayerClassType, PlayerAnalysis> playerAnalyses;
    public Dictionary<EnemyName, EnemyAnalysis> enemyAnalyses;
}


// AnalysUI's Model, PlayerPrefs
public class LibraryManager : MonoBehaviour
{
    public static LibraryManager Instance;
    public DataContainer dataContainer;
    TextAsset SaveData;

    [SerializeField] private string FileDirectory;

    [Header("Local")]
    public float TotalPlayTime;
    public Dictionary<EnemyName, EnemyAnalysis> enemyAnalyses;
    public Dictionary<PlayerClassType, PlayerAnalysis> playerAnalyses;
    public Dictionary<int, StageAnalysis> stageAnalyses;

    private void Awake()
    {
        if (Instance == null) { Instance = this; } else { Destroy(gameObject); }
    }

    // Initialize
    // Load
    // Save

    private void Start()
    {
        // After Pooling Done.

        // SaveLoad();

    }

    public void SaveLoad()
    {
        FileDirectory = Path.Combine(Application.persistentDataPath, "SaveData.data");
        byte[] bytes;
        if (!File.Exists(FileDirectory))
        {
            dataContainer = new DataContainer();
            EnemyLibraryInitialize();
            PlayerLibraryInitialize();
            StageLibraryInitialize();

            // Create. 
            string data = JsonConvert.SerializeObject(dataContainer);
            bytes = System.Text.Encoding.UTF8.GetBytes(data);
            string encoded = System.Convert.ToBase64String(bytes);
            File.WriteAllText(FileDirectory, encoded);

        }

        string jsonData = File.ReadAllText(FileDirectory);
        bytes = System.Convert.FromBase64String(jsonData);
        string decoded = System.Text.Encoding.UTF8.GetString(bytes);
        SaveData = new TextAsset(decoded);

        DataContainer LoadData = JsonConvert.DeserializeObject<DataContainer>(SaveData.text);
        dataContainer = LoadData;
        enemyAnalyses = dataContainer.enemyAnalyses;
        playerAnalyses = dataContainer.playerAnalyses;
        TotalPlayTime = dataContainer.TotalPlaytime;
        stageAnalyses = dataContainer.stageAnalyses;
        MainMenuUI.Instance.UpdatePlaytime();

        List<PlayerScriptableObject> Dataset = ResourceManager.Instance.GetPlayerObjects();
        foreach (PlayerScriptableObject e in Dataset)
        {
            PlayerSkillManager.Instance.SkillTreeMapping(e.classType);
        }

    }


    private void EnemyLibraryInitialize()
    {
        enemyAnalyses = new Dictionary<EnemyName, EnemyAnalysis>();
        List<EnemyScriptableObject> Dataset = ResourceManager.Instance.GetEnemyObjects();
        foreach (EnemyScriptableObject e in Dataset)
        {
            EnemyAnalysis info = new EnemyAnalysis()
            { 
                Name = e.Name,
                Description = false,
                Meet = false,
                Comment1 = false,
                Comment2 = false,
                Comment3 = false 
            };
            enemyAnalyses[e.enemyName] = info;
        }

        dataContainer.enemyAnalyses = enemyAnalyses;
    }

    private void PlayerLibraryInitialize()
    {
        playerAnalyses = new Dictionary<PlayerClassType, PlayerAnalysis>();
        List<PlayerScriptableObject> Dataset = ResourceManager.Instance.GetPlayerObjects();
        foreach (PlayerScriptableObject e in Dataset)
        {
            PlayerAnalysis info = new PlayerAnalysis() 
            { 
                Name = e.Name,
                Memory1 = false,
                Memory2 = false,
                Memory3 = false,
                isLocked = true
            };
            if (e.classType == PlayerClassType.Knight) info.isLocked = false;
            playerAnalyses[e.classType] = info;

            playerAnalyses[e.classType].NormalSkillTree = new SkillTree("0_0");
            playerAnalyses[e.classType].Active1SkillTree = new SkillTree("0_0");
            playerAnalyses[e.classType].Active2SkillTree = new SkillTree("0_0");
        }

        dataContainer.playerAnalyses = playerAnalyses;
        foreach (PlayerScriptableObject e in Dataset)
        {
            PlayerSkillManager.Instance.BuildSkillTree(e.classType, SlotType.Normal);
            PlayerSkillManager.Instance.BuildSkillTree(e.classType, SlotType.Skill1);
            PlayerSkillManager.Instance.BuildSkillTree(e.classType, SlotType.Skill2);

        }

    }

    private void StageLibraryInitialize()
    {
        stageAnalyses = new Dictionary<int, StageAnalysis>();
        List<MapData> Dataset = ResourceManager.Instance.GetMapDataset();

        int cnt = 0;
        foreach (MapData m in Dataset)
        {
            StageAnalysis stageAnalysis = new StageAnalysis()
            {
                StageID = cnt,
                PlayTime = 0,
                EasyScore = 0,
                NormalScore = 0,
                HardScore = 0,
                NightmareScore = 0,
                isLocked = true,
                isCleared = false
            };
            if (cnt == 0) stageAnalysis.isLocked = false;
            stageAnalyses[cnt++] = stageAnalysis;
        }

        dataContainer.stageAnalyses = stageAnalyses;
    }
}