using System;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;
using System.Threading.Tasks;

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
public class DataContainer
{

    List<EnemyAnalysis> enemyAnalyses;
}


// AnalysUI's Model, PlayerPrefs
public class LibraryManager : MonoBehaviour
{
    public static LibraryManager Instance;
    public DataContainer dataContainer;
    TextAsset SaveData;

    [SerializeField] private string FileDirectory;
    public Dictionary<GameObject, EnemyAnalysis> enemyAnalyses;

    private void Awake()
    {
        if (Instance == null) { Instance = this; } else { Destroy(gameObject); }
    }

    // Initialize
    // Load
    // Save

    private void Start()
    {
        dataContainer = new DataContainer();
        SaveLoad();
    }

    public void SaveLoad()
    {
        FileDirectory = Path.Combine(Application.persistentDataPath, "TempAssettScripting.data");
        byte[] bytes;
        if (!File.Exists(FileDirectory))
        {
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

    }


    private void EnemyLibraryInitialize()
    {
        enemyAnalyses = new Dictionary<GameObject, EnemyAnalysis>();
        List<EnemyScriptableObject> Dataset = ResourceManager.Instance.GetEnemyObjects();
        foreach (EnemyScriptableObject e in Dataset)
        {
            EnemyAnalysis info = new EnemyAnalysis() { Name = e.Name, Description = false, Meet = false, Comment1 = false, Comment2 = false, Comment3 = false };
            enemyAnalyses[e.EnemyPrefab] = info;
        }

        //JsonConvert 
    }

}