using System;
using UnityEngine;

/*
 * Player Spawn
 * Positive : Restore Souls
 * Negative : Accelerate Lose Souls 
 * Negative : Random Range Attack
 */

[Serializable]
public enum EnvironmentType
{ 
    PositiveTree, PositiveStone, SpawnEnv,
    NegativeEye, 
    None,
}

public abstract class Environments : MonoBehaviour
{
    public Vector2Int GridPos;
    [SerializeField] public EnvironmentType envType;
    protected bool isActiavted;

    public virtual void OrbInteracted()
    { 
        
    }
}