using System.Collections;
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

public enum EnemyBehaviour
{
    IDLE,
    MOVE,
    ATTACK,
    CHARGE,
    STUNNED,
    WAIT,
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

    [SerializeField] protected int AttackIndex;
    [SerializeField] protected List<IEnemyAttack> AttackPattern;
    [SerializeField] protected IEnemyAttack CurrentAttackPattern;
    [SerializeField] protected int MoveIndex;
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

    public void SetStatus(float HP, float AP, float BP)
    {
        status.MaxAPValue = AP;
        status.MaxHPValue = HP;
        status.HPValue = status.MaxHPValue;
        status.MaxBPValue = BP;
    }

    public void AppearOn()
    {
        statusUI.alpha = 1f;
        anim.SetBool("Activated", true);
        MainSprite.SetActive(true);
        OutlineSprite.SetActive(true);
        CurrentMovePattern = MovePattern[0];
        CurrentMovePattern.SetInit(this.transform);
        CurrentState = EnemyBehaviour.MOVE;
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

    protected virtual void Update()
    {
        FSM_Update();
        CurrentMovePattern?.MoveUpdate();
        CurrentAttackPattern?.Update();

    }

    #region FSM



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
            case EnemyBehaviour.WAIT:
                break;
            case EnemyBehaviour.ATTACK:  // Attack to Charge or Idle.
                if (isAttack == false) return;
                Attack();
                isAttack = false;
                break;
            case EnemyBehaviour.MOVE:
                Move();
                CurrentState = EnemyBehaviour.IDLE;
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

    public void Move()
    {
        CurrentMovePattern = MovePattern[MoveIndex++];
        CurrentMovePattern.SetInit(this.transform);
        if (MoveIndex >= MovePattern.Count) MoveIndex = 0;
        CurrentMovePattern?.Move();
    }

    public void Attack()
    {
        status.APValue = 0f;
        CurrentAttackPattern = AttackPattern[AttackIndex++];
        CurrentAttackPattern.SetInit(this.transform);
        if (AttackIndex >= AttackPattern.Count) AttackIndex = 0;
        if (Wrapper != null) StopCoroutine(Wrapper);
        Wrapper = AttackWrapper(CurrentAttackPattern?.Attack());
        StartCoroutine(Wrapper);
        CurrentState = EnemyBehaviour.WAIT;
    }

    //TODO : Optimization Need.
    IEnumerator Wrapper;
    public IEnumerator AttackWrapper(IEnumerator Attack)
    {
        GameObject circle = ResourceManager.Instance.GetEffectResource("CircleExpandEffect");
        circle .transform.position = transform.position;
        Material tempMat = MainSprite.GetComponent<SpriteRenderer>().material;
        tempMat.SetFloat("_WhiteValue", 0f);
        MainSprite.GetComponent<SpriteRenderer>().material = tempMat;
        yield return new WaitForSeconds(0.2f);
        tempMat.SetFloat("_WhiteValue", 1f);
        MainSprite.GetComponent<SpriteRenderer>().material = tempMat;
        yield return new WaitForSeconds(0.2f);
        tempMat.SetFloat("_WhiteValue", 0f);
        MainSprite.GetComponent<SpriteRenderer>().material = tempMat;
        yield return new WaitForSeconds(0.2f);
        tempMat.SetFloat("_WhiteValue", 1f);
        MainSprite.GetComponent<SpriteRenderer>().material = tempMat;

        yield return StartCoroutine(Attack);
        CurrentState = EnemyBehaviour.IDLE;
    }

    public void SetMovePattern(List<IEnemyMove> patterns)
    { 
        MovePattern = patterns;
    }

    public void SetAttackPattern(List<IEnemyAttack> patterns)
    {
        AttackPattern = patterns;
    }

}