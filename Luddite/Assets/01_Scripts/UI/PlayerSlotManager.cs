using System;
using UnityEngine;

public class PlayerSlotManager : MonoBehaviour
{
    public static PlayerSlotManager Instance;

    [SerializeField] private PlayerSlotUI[] playerSlot;
    [SerializeField] private Player[] playerObject;

    private void Awake()
    {
        if (Instance == null) { Instance = this; } else { Destroy(gameObject); }
    }

}