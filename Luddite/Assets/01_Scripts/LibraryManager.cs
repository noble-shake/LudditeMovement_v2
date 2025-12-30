using System;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;
using System.Threading;


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
    public List<(int, int)> NormalSkillMapper;
    public List<(int, int)> Active1SkillMapper;
    public List<(int, int)> Active2SkillMapper;
    public (int, int) CurrentActive1;
    public (int, int) CurrentActive2;
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

public class SkillMap
{
    public SkillTree NormalSkillTree;
    public SkillTree Active1SkillTree;
    public SkillTree Active2SkillTree;
    public TreeNode CurrentActive1;
    public TreeNode CurrentActive2;
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
    public Dictionary<PlayerClassType, SkillMap> playerSkillTrees;
    public Dictionary<int, StageAnalysis> stageAnalyses;

    private void Awake()
    {
        if (Instance == null) { Instance = this; } else { Destroy(gameObject); }
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

        PlayerSkillInitialize();
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
                enemyType = e.enemyName,
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
                isLocked = true,
                NormalSkillMapper = new List<(int, int)>() { (0, 0)},
                Active1SkillMapper = new List<(int, int)>() { (0, 0)},
                Active2SkillMapper = new List<(int, int)>() { (0, 0)},
            };
            if (e.classType == PlayerClassType.Knight) info.isLocked = false;
            playerAnalyses[e.classType] = info;


        }

        dataContainer.playerAnalyses = playerAnalyses;

    }

    private void PlayerSkillInitialize()
    {
        playerSkillTrees = new Dictionary<PlayerClassType, SkillMap>();
        List<PlayerScriptableObject> Dataset = ResourceManager.Instance.GetPlayerObjects();
        foreach (PlayerScriptableObject e in Dataset)
        {
            playerSkillTrees[e.classType] = new SkillMap();
            playerSkillTrees[e.classType].NormalSkillTree = new SkillTree("0_0");
            playerSkillTrees[e.classType].Active1SkillTree = new SkillTree("0_0");
            playerSkillTrees[e.classType].Active2SkillTree = new SkillTree("0_0");

            PlayerSkillManager.Instance.BuildSkillTree(e.classType, SlotType.Normal);
            PlayerSkillManager.Instance.BuildSkillTree(e.classType, SlotType.Skill1);
            PlayerSkillManager.Instance.BuildSkillTree(e.classType, SlotType.Skill2);

            foreach ((int, int) mapper in playerAnalyses[e.classType].NormalSkillMapper)
            {
                TreeNode node = playerSkillTrees[e.classType].NormalSkillTree.FindNode(mapper.Item1, mapper.Item2);  // tier, index
                node.isEarned = true;
            }

            foreach ((int, int) mapper in playerAnalyses[e.classType].Active1SkillMapper)
            {
                TreeNode node = playerSkillTrees[e.classType].Active1SkillTree.FindNode(mapper.Item1, mapper.Item2);  // tier, index
                node.isEarned = true;
            }

            foreach ((int, int) mapper in playerAnalyses[e.classType].Active2SkillMapper)
            {
                TreeNode node = playerSkillTrees[e.classType].Active2SkillTree.FindNode(mapper.Item1, mapper.Item2);  // tier, index
                node.isEarned = true;
            }
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

    #region Enemy Analysis

    [SerializeField] private AnalysisUI AnalysisPrefab;

    public void EnemyAnalysisEvent(EnemyName _enemy, string AttackPattern, string MovePattern)
    {
        EnemyAnalysis analysis = enemyAnalyses[_enemy];
        if (analysis.Comment1 && analysis.Comment2 && analysis.Comment3) return;
        EnemyLibraryAdd(_enemy);

        // save

        EnemyScriptableObject enemyInfo = ResourceManager.Instance.GetEnemyInfo(_enemy);
        AnalysisUI canvas = Instantiate<AnalysisUI>(AnalysisPrefab);
       
        canvas.NameValue = enemyInfo.Name;
        canvas.DescriptionValue = enemyInfo.EnemyDescription;
        canvas.PortraitValue = enemyInfo.Portrait;
        canvas.Comment1Value= enemyAnalyses[_enemy].Comment1 ? enemyInfo.Comment1 : "???????";
        canvas.Comment2Value= enemyAnalyses[_enemy].Comment2 ? enemyInfo.Comment1 : "???????";
        canvas.Comment3Value= enemyAnalyses[_enemy].Comment3 ? enemyInfo.Comment1 : "???????";
        canvas.AttackDescriptionValue = AttackPattern;
        canvas.MoveDescriptionValue = MovePattern;
        canvas.AudioValue = enemyInfo.Sound;

        byte[] bytes;
        string data = JsonConvert.SerializeObject(dataContainer);
        bytes = System.Text.Encoding.UTF8.GetBytes(data);
        string encoded = System.Convert.ToBase64String(bytes);
        File.WriteAllText(FileDirectory, encoded);

    }

    public void EnemyLibraryAdd(EnemyName _enemy)
    {
        EnemyAnalysis analysis = enemyAnalyses[_enemy];
        analysis.Meet = true;
        if (analysis.Comment2)
        {
            analysis.Comment3 = true;
        }

        if (analysis.Comment1)
        { 
            analysis.Comment2 = true;
        }

        analysis.Comment1 = true;
    }

    #endregion
}