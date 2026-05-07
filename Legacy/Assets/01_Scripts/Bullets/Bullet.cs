using UnityEngine;

public enum BulletIdentifierType
{ 
    Enemy,
    Player,
}

public abstract class Bullet : MonoBehaviour
{

    public virtual void OnHit(GameObject _Triggered, bool isOrb=false)
    {

    }
}