using UnityEngine;

public enum PropType
{ 
    Obstacle,
    ItemDrop,
    CanPassable,
    None,
}

public abstract class Props : MonoBehaviour
{
    public Vector2Int GridPos;
    [SerializeField] public PropType propType;

    public virtual void OrbInteracted()
    {

    }
}