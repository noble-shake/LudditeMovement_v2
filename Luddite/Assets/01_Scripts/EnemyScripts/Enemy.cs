using UnityEngine;

public enum EnemyType
{ 
    Ground,
    Air,
    Other,
}

public abstract class Enemy : MonoBehaviour
{
    [SerializeField] protected EnemyStatusManager status;

    [SerializeField] protected GameObject MainSprite; 
    [SerializeField] protected GameObject OutlineSprite; 

    protected EnemyType enemyType;
    protected AppearEffect appearEffect;
    protected bool isIdle = true;
    protected bool isBerserk = false;
    public bool isIdleCheck { get { return isIdle; } set { isIdle = value;} }
    public bool isBerserkCheck { get { return isBerserk; } set { isBerserk = value;} }

    public void AppearOn()
    { 
        MainSprite.SetActive(true);
        OutlineSprite.SetActive(true);
    }

    public EnemyType GetEnemyType()
    {
        return enemyType;
    }

    public void OrbInteract(bool isClockwise)
    { 
        
    }
}