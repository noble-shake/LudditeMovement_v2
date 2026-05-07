using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum EnemyName
{ 
    None,
    Slime,
    Bat,
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

public class Enemy : MonoBehaviour
{
    [SerializeField] protected EnemyStatusManager status;
    [SerializeField] protected Animator anim;
    [SerializeField] private CanvasGroup statusUI;

    [SerializeField] protected GameObject MainSprite; 
    [SerializeField] protected GameObject OutlineSprite;

    [SerializeField] protected EnemyType enemyType;
    [SerializeField] protected EnemyName enemyName;

    [SerializeField] public EnemyBehaviour CurrentState;

    [Header("OrbInteraction")]
    //[SerializeField] ParticleSystem GetHPEffect;
    //[SerializeField] EffectObject ActivatedeEffect;
    [SerializeField] bool isDetected;
    [SerializeField] float OrbDetectRemained = 0.5f;
    [SerializeField] float OrbDetectCurFlow;
    float CurInteractDelay;
    [SerializeField] ParticleSystem InteractionParticle;

    [Header("IEnemy Instances")]
    [SerializeField] public int AttackIndex;
    [SerializeField] public List<(EnemyAttackScriptable, IEnemyAttack)> AttackPattern;
    [SerializeField] public (EnemyAttackScriptable, IEnemyAttack) CurrentAttackPattern;
    [SerializeField] protected int MoveIndex;
    [SerializeField] protected List<(EnemyMoveScriptable,IEnemyMove)> MovePattern;
    [SerializeField] protected (EnemyMoveScriptable, IEnemyMove)CurrentMovePattern;

    [Header("Charge UI")]
    [SerializeField] public CanvasGroup ChargeUI;
    [SerializeField] public Image ChargeProgress;
    [SerializeField] public TMP_Text ChargeCycle;

    [SerializeField] protected Rigidbody rigid;
    protected AppearEffect appearEffect;
    [SerializeField] protected bool isIdle = true;
    [SerializeField] protected bool isBerserk = false;
    [SerializeField] protected bool isDead = false;
    [SerializeField] protected bool isAttack = false;
    [SerializeField] protected bool isMove;
    [SerializeField] protected bool isCharging;
    [SerializeField] protected bool isStunned;

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

    public void SetStunned(float stunned = 5f)
    {
        ChargeUI.gameObject.SetActive(false);
        stunnedTime = stunned;
        isCharging = false;
        isStunned = true;
        status.APValue = 0f;
        status.BPValue = status.MaxBPValue;
        CurrentState = EnemyBehaviour.STUNNED;
        GameObject effect = ResourceManager.Instance.GetEffectResource("BreakEffect");
        effect.transform.position = this.transform.position;
        MainSprite.GetComponent<SpriteRenderer>().color = Color.black;
    }

    public void AppearOn()
    {
        statusUI.alpha = 1f;
        anim.SetBool("Activated", true);
        MainSprite.SetActive(true);
        OutlineSprite.SetActive(true);
        CurrentMovePattern = MovePattern[0];
        CurrentMovePattern.Item2.SetInit(this.transform);
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
        if (CurInteractDelay > 0f) return;
        CurInteractDelay = 1f;

        isDetected = true;
        OrbDetectRemained = 2f;

        if (isStunned)
        {
            // Analysis On
            LibraryManager.Instance.EnemyAnalysisEvent(enemyName, CurrentAttackPattern.Item1.Description, CurrentMovePattern.Item1.Description);
            InteractionParticle?.Play();

        }

        if (isCharging)
        {
            InteractionParticle?.Play();
            CurrentAttackPattern.Item2?.Interupt();
        }


    }

    protected virtual void Update()
    {
        CurInteractDelay -= Time.deltaTime;
        if (CurInteractDelay < 0) CurInteractDelay = 0f;

        if (isDetected)
        {
            OrbDetectRemained -= Time.deltaTime;
        }

        if (OrbDetectRemained < 0f)
        {
            // 초기화 해버리니까, 다시 사이클 돌려야 하는게 힘들 수 있다.
            //CycleUI.gameObject.SetActive(false);
            //CycleText.text = statusManager.RequireCycleValue.ToString();
            //statusManager.CurRequireCycleValue = statusManager.RequireCycleValue;
            OrbDetectRemained = 0.5f;
            isDetected = false;
        }

        FSM_Update();

        if (isStunned) return;
        CurrentMovePattern.Item2?.MoveUpdate();
        CurrentAttackPattern.Item2?.Update();

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
                anim.Play("Move");
                break;
            case EnemyBehaviour.CHARGE:
                anim.Play("Charge");
                CurrentAttackPattern = AttackPattern[AttackIndex];
                CurrentAttackPattern.Item2.SetInit(this.transform);
                CurrentAttackPattern.Item2?.Charge();
                isCharging = true;
                CurrentState = EnemyBehaviour.WAIT;
                break;
            case EnemyBehaviour.STUNNED:
                if (isStunned)
                {
                    stunnedTime -= Time.deltaTime;
                    if (stunnedTime < 0f)
                    {
                        isStunned = false;
                        stunnedTime = 0f;
                        CurrentState = EnemyBehaviour.MOVE;
                        status.BPValue = 0f;
                        status.APValue = 0f;
                        MainSprite.GetComponent<SpriteRenderer>().color = Color.white;
                    }
                }
                break;

        }
    }


    #endregion

    public void Move()
    {
        if (isCharging) return;
        CurrentMovePattern = MovePattern[MoveIndex++];
        CurrentMovePattern.Item2.SetInit(this.transform);
        if (MoveIndex >= MovePattern.Count) MoveIndex = 0;
        CurrentMovePattern.Item2?.Move();
    }

    public void Attack()
    {
        isCharging = false;
        status.APValue = 0f;
        CurrentAttackPattern = AttackPattern[AttackIndex++];
        CurrentAttackPattern.Item2.SetInit(this.transform);
        if (AttackIndex >= AttackPattern.Count) AttackIndex = 0;
        if (Wrapper != null) StopCoroutine(Wrapper);
        Wrapper = AttackWrapper(CurrentAttackPattern.Item2?.Attack());
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
        anim.Play("Attack");
        yield return StartCoroutine(Attack);
        CurrentState = EnemyBehaviour.IDLE;
    }

    public void SetMovePattern(List<(EnemyMoveScriptable, IEnemyMove)> patterns)
    { 
        MovePattern = patterns;
    }

    public void SetAttackPattern(List<(EnemyAttackScriptable, IEnemyAttack)> patterns)
    {
        AttackPattern = patterns;
    }

}