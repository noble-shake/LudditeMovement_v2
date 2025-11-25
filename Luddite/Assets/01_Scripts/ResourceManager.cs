using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    [Header("Player")]
    [SerializeField] private PlayerOrb P_Orb_Prefab; 
    [SerializeField] private PlayerKnight P_Knight_Prefab; 

    [Header("Enemy")]
    [SerializeField] private EnemySlime M_Slime_Prefab;
}