using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEditor.Rendering;
using UnityEngine;


// ∞¯≈Î¡° : ¿Ãµø πÊΩƒ, »∞º∫»≠, »πµÊ, Ω∫≈»

public enum PlayerType
{ 
    Knight,
    Healer,
    Wizard,
    Ranger,
    Thief,
}

public abstract class Player : MonoBehaviour
{
    [SerializeField] protected PlayerStatusManager statusManager;
    [SerializeField] protected Animator animator;
    [SerializeField] protected bool isOrbTriggered = false;

    [Header("Materials")]
    [SerializeField] protected const float OriginMaterialProperyValue = 2.0f;
    [SerializeField] protected SpriteRenderer MainSprite;
    [SerializeField] protected SpriteRenderer OutlineSprite;
    [SerializeField] protected SpriteRenderer OutlineInnerSprite;
    [SerializeField] protected PlayerType playerType;


    [Header("OrbInteraction")]
    [SerializeField] ParticleSystem GetHPEffect;
    [SerializeField] EffectObject ActivatedeEffect;
    [SerializeField] bool isDetected;
    [SerializeField] float OrbDetectRemained = 0.5f;
    [SerializeField] float OrbDetectCurFlow;
    float CurInteractDelay;

    [SerializeField] bool isActivated = false;
    [SerializeField] bool isAttack = false;
    [SerializeField] bool isMove = false;

    [Header("Indicators")]
    [SerializeField] PlayerIndicator lineIndicator;
    [SerializeField] SkillReadyUI test;

    public bool ActivatedCheck
    { get { return isActivated; } 
        set {
            isActivated = value;
            if (isActivated == true)
            {

                MatLitResponseOff();
                PlayBattleIdle();
                if (ActivatedeEffect != null)
                {
                    GameObject effect = ResourceManager.Instance.GetResource(ActivatedeEffect.gameObject);
                    effect.transform.position = transform.position;
                }

            }
            else
            {
                MatLitResponseOff();
                PlayBattleIdleOff();
            }
        } }
    public bool AttackCheck { get { return isAttack; } set { isAttack = value; } }
    public bool MoveCheck { get { return isMove; } set { isMove = value; } }

    protected virtual void Start()
    {
        statusManager = GetComponent<PlayerStatusManager>();
        animator = GetComponent<Animator>();
        // lineIndicator = GetComponentInChildren<PlayerIndicator>();
    }

    #region MatLitResponse Control (isOrbTriggered / MainSprite / OutlineSprite)

    // Orb Triggered
    public void MatLitResponseOff()
    {
        isOrbTriggered = true;
        MainSprite.material.SetFloat("_LightResponse", 0f);
        OutlineSprite.material.SetColor("_BaseColor", new Color(60f / 256f, 1f, 0f));
        OutlineInnerSprite.gameObject.SetActive(false);
    }

    // Orb Exit
    public void MatLitResponseOn()
    {
        isOrbTriggered = false;
        MainSprite.material.SetFloat("_LightResponse", OriginMaterialProperyValue);
        OutlineSprite.material.SetColor("_BaseColor", new Color(27f / 256f, 115f / 256f, 0f));
        OutlineInnerSprite.gameObject.SetActive(true);
    }
    #endregion

    protected virtual void Update()
    {
        CurInteractDelay -= Time.deltaTime;
        if (CurInteractDelay < 0) CurInteractDelay = 0f;

        if (isDetected)
        {
            Debug.Log($"Remained = {OrbDetectRemained}");
            OrbDetectRemained -= Time.deltaTime;
        }

        if (OrbDetectRemained < 0f)
        {
            OrbDetectRemained = 0.5f;
            isDetected = false;
        }

        MoveUpdate();
    }

    public void MovePoint(Vector3 Destination)
    {

        List<PathNode> paths = MapManager.Instance.gridPathfinding.GetPath(transform.position, Destination);
        foreach (PathNode p in paths)
        {
            Debug.Log($"{p.xPos} / {p.yPos}");
            Debug.Log($"{new Vector3(p.xPos, 0f, p.yPos) + OffsetDestination}");
        }
        if (paths.Count == 0) return;

        MoveQueue = new Queue<PathNode>();
        isMove = true;
        if (paths.Count == 1)
        {
            CurDestination = Destination;
            return;
        }

        Queue<PathNode> ResultQueue = new Queue<PathNode>();
        Queue<PathNode> TempQueue = new Queue<PathNode>(paths);


        PathNode CurNode = TempQueue.Dequeue();
        ResultQueue.Enqueue(CurNode);

        PathNode CompNode = TempQueue.Dequeue();
        
        if (TempQueue.Count == 0)
        {
            ResultQueue.Enqueue(CompNode);
            MoveQueue = ResultQueue;
            PathNode node = MoveQueue.Dequeue();
            CurDestination = new Vector3(node.xPos, 0f, node.yPos) + OffsetDestination;
            return;
        }

        Vector2Int CompVector = new Vector2Int(CompNode.xPos - CurNode.xPos, CompNode.yPos - CurNode.yPos);
        while (TempQueue.Count > 0)
        {
            CurNode = CompNode;
            CompNode = TempQueue.Dequeue();
            Vector2Int UpdatedVector = new Vector2Int(CompNode.xPos - CurNode.xPos, CompNode.yPos - CurNode.yPos);
            if (CompVector != UpdatedVector)
            {
                ResultQueue.Enqueue(CompNode);
                if (TempQueue.Count == 0) break;
            }

            CompVector = UpdatedVector;

            if (TempQueue.Count == 0)
            {
                ResultQueue.Enqueue(CompNode);
            }


        }

        MoveQueue = ResultQueue;
        PathNode fnode = MoveQueue.Dequeue();
        CurDestination = new Vector3(fnode.xPos, 0f, fnode.yPos) + OffsetDestination;

    }

    public void MoveAttack(Vector3 Destination, Enemy Target)
    {
        List<PathNode> paths = MapManager.Instance.gridPathfinding.GetPath(transform.position, Destination);
        paths.RemoveAt(0);
        MoveQueue = new Queue<PathNode>(paths);
        PathNode node = MoveQueue.Dequeue();
    }
    private Vector3 OffsetDestination = new Vector3(-7.5f, 0f, -4f);
    private Vector3 CurDestination;
    private Queue<PathNode> MoveQueue;
    public void MoveUpdate()
    {
        if (isMove == false) return;

        transform.position = Vector3.MoveTowards(transform.position, CurDestination, statusManager.SpeedValue * Time.deltaTime);
        if (transform.position.x < CurDestination.x)
        {
            PlayerFlip(false);
        }
        else if(transform.position.x > CurDestination.x)
        {
            PlayerFlip(true);
        }

        if (Vector3.Distance(transform.position, CurDestination) > 0.001f) return;

        if (MoveQueue.Count <= 0)
        {
            isMove = false;
            return;
        }

        transform.position = CurDestination;
        PathNode node = MoveQueue.Dequeue();
        CurDestination = new Vector3(node.xPos, 0f, node.yPos) + OffsetDestination;

    }

    int cnt = 0;
    public void OrbInteract(bool isClockwise)
    {
        if (CurInteractDelay > 0f) return;
        CurInteractDelay = 1f;

        isDetected = true;
        OrbDetectRemained = 0.5f;

        if (isActivated == false)
        {
            if (PlayerManager.Instance.SoulValue < 30f) return;
            PlayerManager.Instance.SoulValue -= 30f;
            GetHPEffect.Play();
            statusManager.HPValue += statusManager.MaxHPValue * 0.2f;

            return;
        }
        else
        {
            // isClockwise == true => Clockwise
            // isClockwise == false => CounterClockwise
            if (isClockwise)
            {

            }
            else
            { 
                
            }
        }
    }

    public void OrbPointInteract(bool isOn)
    { 
        lineIndicator.gameObject.SetActive(isOn);
        
    }

    public void PlayerFlip(bool isLeft)
    {
        if (isLeft)
        {
            MainSprite.flipX = false;
            OutlineSprite.flipX = false;
            OutlineInnerSprite.flipX = false;
        }
        else
        {
            MainSprite.flipX = true;
            OutlineSprite.flipX = true;
            OutlineInnerSprite.flipX = true;
        }
    }

    #region Animation Play

    public Animator GetAnim()
    { 
        return animator;
    }

    public void PlayIdle()
    {
        animator.SetBool("Activated", false);
    }
    public void PlayActivate()
    {
        animator.SetBool("Activated", true);
    }

    public void PlayBattleIdleOff()
    {
        animator.SetBool("BattleReady", false);
    }

    public void PlayBattleIdle()
    {
        animator.SetBool("BattleReady", true);
    }

    public void PlayNormalAttack()
    {
        animator.Play("NormalAttack");
    }

    public void PlaySkillReady()
    {
        animator.Play("SkillReady");
    }

    #endregion

    #region Animation Event

    public void OnHit()
    {
        HitEffect();
    }

    async Awaitable HitEffect()
    {
        //await new Task().;
        MainSprite.color = Color.red;
        await Awaitable.WaitForSecondsAsync(1f);
        MainSprite.color = Color.white;
    }


    public virtual void AttackProcess()
    { 
        
    }

    #endregion

}