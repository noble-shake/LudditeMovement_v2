using System;
using UnityEngine;


/*
 * Json
 */




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


}
