using UnityEngine;
using System.Collections.Generic;
using static UnityEngine.GraphicsBuffer;

public class ResourceManager : MonoBehaviour
{
    public static ResourceManager Instance;

    [Header("Player")]
    [SerializeField] private List<GameObject> PlayerObjects;
    //[SerializeField] private PlayerOrb P_Orb_Prefab; 
    //[SerializeField] private PlayerKnight P_Knight_Prefab; 

    [Header("Enemy")]
    [SerializeField] private List<GameObject> EnemyObjects;
    [SerializeField] private List<GameObject> BossObjects;
    //[SerializeField] private EnemySlime M_Slime_Prefab;

    [Header("Bullet")]
    [SerializeField] private List<GameObject> BulletObjects;

    [Header("Tile")]
    [SerializeField] private List<MapData> MapDataset;
    [SerializeField] private List<GameObject> TileObjects; // 9, 16 => 144.

    private Dictionary<GameObject, List<GameObject>> ObjectPool = new Dictionary<GameObject, List<GameObject>>(); // Hash값을 이용한다.
    private Dictionary<(int, int), List<GameObject>> TilePool = new Dictionary<(int, int), List<GameObject>>(); // Pooled Tile 관리

    private void Awake()
    {
        if (Instance == null) { Instance = this; } else { Destroy(gameObject); }
    }

    private void Start()
    {
        CreateTileInstance();
        CreateInstance(EnemyObjects);
        CreateInstance(BulletObjects, 256);
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