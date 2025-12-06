using UnityEngine;
using System.Collections.Generic;
using static UnityEngine.GraphicsBuffer;

public class ResourceManager : MonoBehaviour
{
    public static ResourceManager Instance;

    [Header("Player")]
    [SerializeField] private List<PlayerScriptableObject> PlayerObjects;
    //[SerializeField] private PlayerOrb P_Orb_Prefab; 
    //[SerializeField] private PlayerKnight P_Knight_Prefab; 

    [Header("Enemy")]
    [SerializeField] private List<EnemyScriptableObject> EnemyObjects;
    [SerializeField] private List<EnemyScriptableObject> BossObjects;
    //[SerializeField] private EnemySlime M_Slime_Prefab;
    public List<EnemyScriptableObject> GetEnemyObjects() { return EnemyObjects; }
    public List<EnemyScriptableObject> GetBossObjects() { return BossObjects; }

    [Header("Bullet")]
    [SerializeField] private List<GameObject> BulletObjects;

    [Header("Tile")]
    [SerializeField] private List<MapData> MapDataset;
    [SerializeField] private List<GameObject> TileObjects; // 9, 16 => 144.

    [Header("Item")]
    [SerializeField] private List<Sprite> SoulSprite;
    [SerializeField] private SoulItem SoulItemPrefab;
    private List<SoulItem> SoulPool;

    [Header("Effect")]
    [SerializeField] private List<GameObject> Effects;

    private Dictionary<PlayerClassType, GameObject> PlayerPool = new Dictionary<PlayerClassType, GameObject>();
    private Dictionary<GameObject, List<GameObject>> ObjectPool = new Dictionary<GameObject, List<GameObject>>(); // Hash값을 이용한다.
    private Dictionary<(int, int), List<GameObject>> TilePool = new Dictionary<(int, int), List<GameObject>>(); // Pooled Tile 관리

    private void Awake()
    {
        if (Instance == null) { Instance = this; } else { Destroy(gameObject); }
    }

    private void Start()
    {
        CreateTileInstance();
        CreatePlayerInstance(PlayerObjects);
        CreateEnemyInstance(EnemyObjects);
        CreateInstance(BulletObjects, 256);
        CreateInstance(Effects, 128);
        CreatedSoulInstance();
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
            GameObject targetObject = target.EnemyPrefab;
            if (ObjectPool.ContainsKey(targetObject) == false) ObjectPool[targetObject] = new List<GameObject>();
            for (int i = 0; i < num_of_instance; i++)
            {
                GameObject newObj = Instantiate(targetObject, this.transform);
                newObj.SetActive(false);

                ObjectPool[targetObject].Add(newObj);
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
            PlayerPool[target.classType] = newObj;
            currentCnt++;
            
        }
    }

    public GameObject GetPlayerResource(PlayerClassType _class)
    {
        if (PlayerPool[_class].activeSelf) return null;
        PlayerPool[_class].SetActive(true);
        return PlayerPool[_class];
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
        int rand = Random.Range(0, SoulSprite.Count);

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
        int col = 9;
        int row = 16;

        // -8.5f ~ 8f
        // -4.5f ~ 4.5f

        foreach (GameObject tile in TileObjects)
        {
            if (ObjectPool.ContainsKey(tile) == false) ObjectPool[tile] = new List<GameObject>();
            for (int i = 0; i < row; i++)
            {
                for (int j = 0; j < col; j++)
                {
                    GameObject newObj = Instantiate(tile, this.transform);
                    newObj.name = $"({j},{i}){tile.name}";
                    newObj.transform.position = new Vector3(-7.5f + 1.0f * i, 0f, -4f + 1.0f * j);
                    newObj.SetActive(false);
                    ObjectPool[tile].Add(newObj);
                }
            }
        }
    }

    public void TileActive((int, int) _indexer, GameObject _object)
    {
        int tcol = _indexer.Item1;
        int trow = _indexer.Item2;
        int targetCnt = (trow) * 9 + tcol;
        ObjectPool[_object][targetCnt].SetActive(true);

        if(TilePool.ContainsKey(_indexer) == false) TilePool[_indexer] = new List<GameObject>();
        TilePool[_indexer].Add(_object);
    }

    #endregion
}