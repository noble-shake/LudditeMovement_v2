using UnityEngine;

public enum PropType
{ 
    Obstacle,
    ItemDrop,
    CanPassable,
}

public abstract class Props : MonoBehaviour
{
    public virtual void OrbInteracted()
    {

    }
}