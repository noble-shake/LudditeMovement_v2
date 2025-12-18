using UnityEngine;
using System.Collections.Generic;
using static UnityEngine.GraphicsBuffer;
using static UnityEditor.Experimental.GraphView.GraphView;
using System;

public class ResourceManager : MonoBehaviour
{
    public static ResourceManager Instance;

    [Header("Player")]
    [SerializeField] private List<PlayerScriptableObject> PlayerObjects;
    [SerializeField] private List<PlayerSkillScriptableObject> PlayerSkills;
//[SerializeField] private PlayerOrb P_Orb_Prefab; 
//[SerializeField] private PlayerKnight P_Knight_Prefab; 

[Header("Enemy")]
    [SerializeField] private List<EnemyScriptableObject> EnemyObjects;
    [SerializeField] private List<EnemyScriptableObject> BossObjects;
    //[SerializeField] private EnemySlime M_Slime_Prefab;
    public List<EnemyScriptableObject> GetEnemyObjects() { return EnemyObjects; }
    public List<EnemyScriptableObject> GetBossObjects() { return BossObjects; }
    public List<PlayerScriptableObject> GetPlayerObjects() { return PlayerObjects; }

    [Header("Bullet")]
    [SerializeField] private List<GameObject> BulletObjects;

    [Header("Tile")]
    [SerializeField] private List<MapData> MapDataset;
    [SerializeField] private List<GameObject> TileObjects; // 9, 16 => 144.
    [SerializeField] private List<GameObject> EnvObjects; // 9, 16 => 144.
    [SerializeField] private List<GameObject> PropsObjects; // 9, 16 => 144.

    [Header("Item")]
    [SerializeField] private List<Sprite> SoulSprite;
    [SerializeField] private SoulItem SoulItemPrefab;
    private List<SoulItem> SoulPool;

    [Header("Effect")]
    [SerializeField] private List<GameObject> Effects;
    [SerializeField] public Texture2D BattleReadySide;
    [SerializeField] public Texture2D BattleEngageSide;
    [SerializeField] public Texture2D BattlePreviewSide;


    private Dictionary<PlayerSkillScriptableObject, IPlaySkill> PlayerSkillInstances = new Dictionary<PlayerSkillScriptableObject, IPlaySkill>();
    private Dictionary<PlayerClassType, GameObject> PlayerPool = new Dictionary<PlayerClassType, GameObject>();
    private Dictionary<EnemyName, List<GameObject>> EnemyPool = new Dictionary<EnemyName, List<GameObject>>();
    private Dictionary<GameObject, List<GameObject>> ObjectPool = new Dictionary<GameObject, List<GameObject>>(); // Hash값을 이용한다.

    /*
     * TilePool
     * 1. MapTile Objects
     * 2. Environment Objects
     * 3. Props Objects
     */
    private Dictionary<TileBuildType, Dictionary<(int, int), GameObject>> TilePool = new Dictionary<TileBuildType, Dictionary<(int, int), GameObject>>(); // Pooled Tile 관리
    private Dictionary<EnvironmentType, Dictionary<(int, int), GameObject>> EnvPool = new Dictionary<EnvironmentType, Dictionary<(int, int), GameObject>>();
    private Dictionary<PropType, Dictionary<(int, int), GameObject>> PropPool = new Dictionary<PropType, Dictionary<(int, int), GameObject>>();
    
    private void Awake()
    {
        if (Instance == null) { Instance = this; } else { Destroy(gameObject); }
    }

    private void Start()
    {
        CreateTileInstance();
        CreateEnvsInstance();
        CreatePropInstance();
        CreatePlayerInstance(PlayerObjects);
        CreateEnemyInstance(EnemyObjects);
        CreateInstance(BulletObjects, 256);
        CreateInstance(Effects, 128);
        CreatedSoulInstance();
        CreatePlayerSkillInstance();

        LibraryManager.Instance.SaveLoad();
    }

    private void CreateInstance(List<GameObject> _objects, int num_of_instance = 16)
    {
        int currentCnt = 0;
        int totalCnt = _objects.Count * num_of_instance;

        foreach (GameObject target in _objects)
        {
            if (ObjectPool.ContainsKey(target) == false) ObjectPool[target] = new List<GameObject>();
            for (int i = 0; i < num_of_instance; i++)
            {
                GameObject newObj = Instantiate(target, this.transform);
                newObj.SetActive(false);

                ObjectPool[target].Add(newObj);
                currentCnt++;
            }
        }
    }

    private void CreateEnemyInstance(List<EnemyScriptableObject> _objects, int num_of_instance = 16)
    {
        int currentCnt = 0;
        int totalCnt = _objects.Count * num_of_instance;

        foreach (EnemyScriptableObject target in _objects)
        {
            
            if (EnemyPool.ContainsKey(target.enemyName) == false) EnemyPool[target.enemyName] = new List<GameObject>();
            for (int i = 0; i < num_of_instance; i++)
            {
                GameObject newObj = Instantiate(target.EnemyPrefab, this.transform);
                newObj.SetActive(false);

                EnemyPool[target.enemyName].Add(newObj);
                currentCnt++;
            }
        }
    }

    private void CreatePlayerInstance(List<PlayerScriptableObject> _objects)
    {
        int currentCnt = 0;
        int totalCnt = _objects.Count;

        foreach (PlayerScriptableObject target in _objects)
        {
            GameObject targetObject = target.PlayerPrefab.gameObject;

            GameObject newObj = Instantiate(targetObject, this.transform);
            newObj.SetActive(false);
            Player player = newObj.GetComponent<Player>();
            player.PlayerInfo = target;
            // player.statusManager.StatusInit();
            PlayerPool[target.classType] = newObj;
            currentCnt++;
            
        }
    }

    public GameObject GetPlayerResource(PlayerClassType _class, bool isActive= true)
    {
        if (_class == PlayerClassType.None) return null;


        if (isActive)
        {
            if (PlayerPool[_class].activeSelf) return null;
            PlayerPool[_class].SetActive(true);
            PlayerPool[_class].transform.parent = null;
            PlayerPool[_class].transform.SetAsLastSibling();
            return PlayerPool[_class];
        }
        else
        {
            return PlayerPool[_class];
        }

    }

    public void PlayerRetrieve(PlayerClassType _class)
    {
        PlayerPool[_class].SetActive(false);
        PlayerPool[_class].transform.SetParent(this.transform);
    }

    public PlayerScriptableObject GetPlayerInfo(Player _player)
    {
        foreach (PlayerScriptableObject target in PlayerObjects)
        {
            if (_player == target.PlayerPrefab)
            {
                return target;
            }
        }

        return null;
    }

    public PlayerScriptableObject GetPlayerInfo(PlayerClassType _class)
    {
        foreach (PlayerScriptableObject target in PlayerObjects)
        {
            if (_class == target.classType)
            {
                return target;
            }
        }

        return null;
    }



    public GameObject GetResource(GameObject _object)
    {
        foreach (GameObject target in ObjectPool[_object])
        {
            if (target.activeSelf == false)
            {
                target.SetActive(true);
                return target;
            }
        }

        GameObject newObj = Instantiate(_object);
        ObjectPool[_object].Add(newObj);
        newObj.SetActive(true);
        return newObj;

    }

    public GameObject GerEnemyResource(EnemyName _object)
    {
        foreach (GameObject target in EnemyPool[_object])
        {
            if (target.activeSelf == false)
            {
                target.transform.SetAsLastSibling();
                target.SetActive(true);
                return target;
            }
        }

        GameObject newObj = Instantiate(EnemyPool[_object][0]);
        EnemyPool[_object].Add(newObj);
        newObj.SetActive(true);
        return newObj;
    }

    public void ResourceRetrieve(GameObject _object)
    { 
        _object.gameObject.SetActive(false);
        _object.transform.parent = this.transform;
    }

    private void CreatedSoulInstance()
    {
        if (SoulPool == null) SoulPool = new List<SoulItem>();
        for (int i = 0; i < 256; i++)
        {
            SoulItem newObj = Instantiate(SoulItemPrefab, this.transform);
            newObj.gameObject.SetActive(false);

            SoulPool.Add(newObj);
        }
    }

    public SoulItem GetSoulItem()
    {
        int rand = UnityEngine.Random.Range(0, SoulSprite.Count);

        foreach (SoulItem target in SoulPool)
        {
            if (target.gameObject.activeSelf == false)
            {
                target.transform.parent = null;
                target.SetSprite(SoulSprite[rand]);
                target.gameObject.SetActive(true);
                return target;
            }
        }

        SoulItem newObj = Instantiate(SoulItemPrefab, this.transform);
        newObj.transform.parent = null;
        newObj.SetSprite(SoulSprite[rand]);
        newObj.gameObject.SetActive(true);
        SoulPool.Add(newObj);
        return newObj;

    }

    public void SoulRetrieve(SoulItem _soul)
    {
        _soul.transform.parent = this.transform;
        _soul.gameObject.SetActive(false);
    }

    #region TileSet Pooling



    private void CreateTileInstance()
    {
        int col = 16;
        int row = 9;

        // -8.5f ~ 8f
        // -4.5f ~ 4.5f
        foreach (GameObject tile in TileObjects)
        {
            TileBuildType buildType = tile.GetComponent<MapTile>().GetTileBuildType();
            if (TilePool.ContainsKey(buildType) == false) TilePool[buildType] = new Dictionary<(int, int), GameObject>();
            for (int i = 0; i < row; i++)
            {
                for (int j = 0; j < col; j++)
                {
                    GameObject newObj = Instantiate(tile, this.transform);
                    newObj.name = $"({i},{j}){tile.name}";
                    Vector3 targetVec = new Vector3(-7.5f + j, 0, -4.0f + i);
                    newObj.transform.position = targetVec;
                    newObj.SetActive(false);
                    TilePool[buildType][(i, j)] = newObj;
                }
            }
        }
    }

    private void CreateEnvsInstance()
    {
        int col = 16;
        int row = 9;

        // -8.5f ~ 8f
        // -4.5f ~ 4.5f
        foreach (GameObject tile in EnvObjects)
        {
            EnvironmentType buildType = tile.GetComponent<Environments>().envType;
            
            if (EnvPool.ContainsKey(buildType) == false) EnvPool[buildType] = new Dictionary<(int, int), GameObject>();
            for (int i = 0; i < row; i++)
            {
                for (int j = 0; j < col; j++)
                {
                    GameObject newObj = Instantiate(tile, this.transform);
                    if (buildType == EnvironmentType.SpawnEnv)
                    {
                        newObj.GetComponent<SpawnEnv>().SpawnEntryID = -1;
                    }
                    newObj.name = $"({i},{j}){tile.name}";
                    Vector3 targetVec = new Vector3(-7.5f + j, 0, -4.0f + i);
                    newObj.transform.position = targetVec;
                    newObj.SetActive(false);
                    EnvPool[buildType][(i, j)] = newObj;
                }
            }
        }
    }

    private void CreatePropInstance()
    {
        int col = 16;
        int row = 9;

        // -8.5f ~ 8f
        // -4.5f ~ 4.5f
        foreach (GameObject tile in PropsObjects)
        {
            PropType buildType = tile.GetComponent<Props>().propType;
            if (PropPool.ContainsKey(buildType) == false) PropPool[buildType] = new Dictionary<(int, int), GameObject>();
            for (int i = 0; i < row; i++)
            {
                for (int j = 0; j < col; j++)
                {
                    GameObject newObj = Instantiate(tile, this.transform);
                    newObj.name = $"({i},{j}){tile.name}";
                    Vector3 targetVec = new Vector3(-7.5f + j, 0, -4.0f + i);
                    newObj.transform.position = targetVec;
                    newObj.SetActive(false);
                    PropPool[buildType][(i, j)] = newObj;
                }
            }
        }
    }

    List<GameObject> ActivatedTiles = new List<GameObject>();
    public List<SpawnEnv> SpawnPoints = new List<SpawnEnv>();
    public void TileClear()
    {
        foreach (SpawnEnv env in SpawnPoints)
        {
            env.SpawnEntryID = -1;
        }

        foreach (GameObject tile in ActivatedTiles)
        { 

            tile.gameObject.SetActive(false);
        }
        ActivatedTiles.Clear();
        SpawnPoints.Clear();
    }

    public (int, int) FindTileIndex(int cnt)
    {
        //int targetCnt = (trow) * 9 + tcol;
        int row = (int)(cnt / 16);
        int col = (int)(cnt % 16);
        return (row, col);
    }

    public void TileActive((int, int) _indexer, TileBuildType _type)
    {
        int tcol = _indexer.Item1;
        int trow = _indexer.Item2;
        // int targetCnt = (trow) * 9 + tcol;
        TilePool[_type][_indexer].SetActive(true);
        ActivatedTiles.Add(TilePool[_type][_indexer]);
    }


    public void TileActive((int, int) _indexer, EnvironmentType _type)
    {
        int tcol = _indexer.Item1;
        int trow = _indexer.Item2;
        EnvPool[_type][_indexer].SetActive(true);
        ActivatedTiles.Add(EnvPool[_type][_indexer]);
        if (_type == EnvironmentType.SpawnEnv)
        {
            SpawnEnv target = EnvPool[_type][_indexer].GetComponent<SpawnEnv>();
            target.SpawnEntryID = SpawnPoints.Count;
            SpawnPoints.Add(target);
        }
    }


    public void TileActive((int, int) _indexer, PropType _type)
    {
        int tcol = _indexer.Item1;
        int trow = _indexer.Item2;
        // int targetCnt = (trow) * 9 + tcol;
        PropPool[_type][_indexer].SetActive(true);
        ActivatedTiles.Add(PropPool[_type][_indexer]);
    }

    #endregion

    public List<MapData> GetMapDataset()
    {
        return MapDataset;
    }

    public MapData GetMapData(int StageID)
    {
        return MapDataset[StageID];
    }

    public void CreatePlayerSkillInstance()
    {
        foreach (PlayerSkillScriptableObject so in PlayerSkills)
        {
            IPlaySkill sInst = so.GetInstance();
            PlayerSkillInstances[so] = sInst;
        }
    }

    public IPlaySkill GetPlayerSkillInstance(PlayerSkillScriptableObject so)
    {
        return PlayerSkillInstances[so];
    }
}