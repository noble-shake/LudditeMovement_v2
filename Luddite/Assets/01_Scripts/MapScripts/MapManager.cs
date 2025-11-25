using UnityEngine;
using UnityEditor;

public class MapManager : MonoBehaviour
{
    public Vector3 OriginPoint;
    public static MapManager Instance;

    private void Awake()
    {
        if (Instance == null) { Instance = this; } else { Destroy(gameObject); }
    }

    private void Start()
    {
        OriginPoint = transform.position;
    }

    private void Update()
    {

    }
    // -8.5f, -3.5f ~ 8.5f, 3.5f

    //private void OnDrawGizmos()
    //{
    //    for (int i = 0; i < 9; i++)
    //    {
    //        Gizmos.DrawLine(transform.position - new Vector3(0.5f, 0, 0.5f - i), transform.position - new Vector3(0.5f, 0, 0.5f - i) + new Vector3(18f, 0f, 0f));
    //    }

    //    for (int i = 0; i < 19; i++)
    //    {
    //        Gizmos.DrawLine(transform.position - new Vector3(0.5f - i, 0, 0.5f), transform.position - new Vector3(0.5f - i, 0, 0.5f) + new Vector3(0f, 0f, 8f));
    //    }

    //}


}
