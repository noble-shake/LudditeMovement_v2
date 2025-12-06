using System.Threading.Tasks;
using UnityEngine;

public enum EnemyName
{ 
    None,
    Slime,
}


public enum EnemyType
{ 
    Ground,
    Air,
    Other,
}

public abstract class Enemy : MonoBehaviour
{
    [SerializeField] protected EnemyStatusManager status;
    [SerializeField] protected Animator anim;
    [SerializeField] private CanvasGroup statusUI;

    [SerializeField] protected GameObject MainSprite; 
    [SerializeField] protected GameObject OutlineSprite;

    [SerializeField] protected EnemyType enemyType;
    [SerializeField] protected EnemyName enemyName;

    [SerializeField] protected Rigidbody rigid;
    protected AppearEffect appearEffect;
    protected bool isIdle = true;
    protected bool isBerserk = false;
    protected bool isDead = false;
    protected bool isAttack = false;
    protected bool isMove;
    protected bool isCharging;

    public bool isIdleCheck { get { return isIdle; } set { isIdle = value;} }
    public bool isBerserkCheck { get { return isBerserk; } set { isBerserk = value;} }
    public bool isDeadCheck { get { return isDead; } set { isDead = value;} }
    public bool isAttackCheck { get { return isAttack; } set { isAttack = value;} }

    private Awaitable HitAwait;

    public void AppearOn()
    {
        statusUI.alpha = 1f;
        anim.SetBool("Activated", true);
        MainSprite.SetActive(true);
        OutlineSprite.SetActive(true);
    }

    public void AppearOff()
    {
        statusUI.alpha = 0f;
        anim.SetBool("Activated", false);
        anim.Play("Idle");
        //anim.enabled = false;
        MainSprite.SetActive(false);
        OutlineSprite.SetActive(false);
    }

    public void OnHit()
    {
        HitEffect();
    }

    async Awaitable HitEffect()
    {
        //await new Task().;
        MainSprite.GetComponent<SpriteRenderer>().color = Color.red;
        await Awaitable.WaitForSecondsAsync(1f);
        MainSprite.GetComponent<SpriteRenderer>().color = Color.white;
    }

    public EnemyType GetEnemyType()
    {
        return enemyType;
    }

    public EnemyName GetEnemyName()
    {
        return enemyName;
    }

    public Animator GetAnimator() { return anim; }

    public virtual void OrbInteracted()
    { 
        
    }
}