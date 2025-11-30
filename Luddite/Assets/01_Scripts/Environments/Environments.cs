using System;
using UnityEngine;

/*
 * Positive : Restore Souls
 * Negative : Accelerate Lose Souls 
 * Negative : Random Range Attack
 */

[Serializable]
public enum EnvironmentType
{ 
    Positive,
    Negative1, Negative2, Negative3
}


public abstract class Environments : MonoBehaviour
{
    protected bool isActiavted;

    public virtual void OrbInteracted()
    { 
        
    }
}