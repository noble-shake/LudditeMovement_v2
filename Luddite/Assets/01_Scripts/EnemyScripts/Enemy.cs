using System.Collections.Generic;
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

    [SerializeField] public EnemyBehaviour CurrentState;

    [SerializeField] protected List<IEnemyAttack> AttackPattern;
    [SerializeField] protected IEnemyAttack CurrentAttackPattern;
    [SerializeField] protected List<IEnemyMove> MovePattern;
    [SerializeField] protected IEnemyMove CurrentMovePattern;

    [SerializeField] protected Rigidbody rigid;
    protected AppearEffect appearEffect;
    protected bool isIdle = true;
    protected bool isBerserk = false;
    protected bool isDead = false;
    protected bool isAttack = false;
    protected bool isMove;
    protected bool isCharging;
    protected bool isStunned;

    float stunnedTime;

    public bool isIdleCheck { get { return isIdle; } set { isIdle = value;} }
    public bool isBerserkCheck { get { return isBerserk; } set { isBerserk = value;} }
    public bool isDeadCheck { get { return isDead; } set { isDead = value;} }
    public bool isAttackCheck { get { return isAttack; } set { isAttack = value;} }
    public bool isMoveCheck { get { return isMove; } set { isMove = value;} }
    public bool isChargingCheck { get { return isCharging; } set { isCharging = value;} }
    public bool isStunCheck { get { return isStunned; } set { isStunned = value;} }

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

    public void OnHit(float value)
    {
        HitEffect();
        status.OnHit(value);
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
        if (isStunned)
        { 
            // Analysis On
        }


    }

    #region FSM

    public enum EnemyBehaviour
    { 
        IDLE,
        MOVE,
        ATTACK,
        CHARGE,
        STUNNED,
    }

    // AP는 계속 지속적으로 찬다.
    // Charge 기술일 경우, AP는 움직이지 않는다.
    // Stunned일 경우에는 Charge도 막는다.

    public void FSM_Update()
    {
        if (GameManager.Instance.currentCondition != GameCondition.Game) return;

        switch (CurrentState)
        {
            default:
            case EnemyBehaviour.IDLE:
                break;
            case EnemyBehaviour.ATTACK:  // Attack to Charge or Idle.
                CurrentAttackPattern?.Attack();
                break;
            case EnemyBehaviour.MOVE:
                CurrentMovePattern?.Move();
                break;
            case EnemyBehaviour.CHARGE:
                CurrentAttackPattern?.Charge();
                break;
            case EnemyBehaviour.STUNNED:
                if (isStunned)
                {
                    stunnedTime -= Time.deltaTime;
                    if (stunnedTime < 0f)
                    {
                        isStunned = false;
                        stunnedTime = 5f;
                        CurrentState = EnemyBehaviour.IDLE;
                    }
                }
                break;

        }
    }


    #endregion
}