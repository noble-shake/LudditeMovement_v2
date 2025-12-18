using System;
using UnityEngine;


//None,
//HP,
//HPRegen,
//Attack,
//AttackSpeed,
//AttackSoulDrop,
//AttackHPRegen,
//Speed,
//Critical,
//ReduceDamage,
//ReduceRequireAP,
//ReduceCycle,
//BuffDuration,
//ReduceCharging,
//ReduceCooldown,
//DamageSouldrop,

public class PlayerStatusManager : MonoBehaviour
{
    [SerializeField] private Player player;
    [SerializeField] private float HP;
    [SerializeField] private float MaxHP;
    [SerializeField] private float Attack;
    [SerializeField] private float Speed;
    [SerializeField] private float Critical;
    [SerializeField] private float RequireAP;
    [SerializeField] private int CurRequireCycle;
    [SerializeField] private int RequireCycle;
    [SerializeField] private float ClockwiseSkillPoint;
    [SerializeField] private float ClockwiseSkillRequire;
    [SerializeField] private float CounterClockwiseSkillPoint;
    [SerializeField] private float CounterClockwiseSkillRequire;

    private float PassiveHP;
    private float PassiveHPRegen;
    private float PassiveAttack;
    private float PassiveAttackSpeed;
    private float PassiveAttackSoulDrop;
    private float PassiveAttackHPRegen;
    private float PassiveSpeed;
    private float PassiveCritical;
    private float PassiveReduceDamage;
    private float PassiveReduceRequireAP;
    private float PassiveReduceCooldown;
    private float PassiveReduceCycle;
    private float PassiveBuffDuration;
    private float PassiveReduceCharging;
    private float PassiveDamageSoulDrop;

    public event Action<float> HPChanged;
    public event Action<float> MaxHPChanged;
    public event Action<int> RequireCycleChanged;
    public event Action<float> ClockSkillChanged;
    public event Action<float> ClockSkillReqChanged;
    public event Action<float> CClockSkillChanged;
    public event Action<float> CClockSkillReqChanged;

    #region Properties
    public float HPValue 
    { get { return HP; }  
        set 
        { 
            HP = value;
            HPChanged?.Invoke(value);

        }
    }
    public float MaxHPValue 
    {get{ return MaxHP; } set { MaxHP = value; MaxHPChanged?.Invoke(value); }}

    public float AttackValue
    { get { return Attack; } set { Attack = value; } }

    public float SpeedValue
    { get { return Speed; } set { Speed= value; } }

    public float CriticalValue
    { get { return Critical; } set { Critical = value; } }

    public float RequireAPValue
    { get { return RequireAP; } set { RequireAP = value; } }

    public int RequireCycleValue
    { get { return RequireCycle; } 
        set { 
            RequireCycle = value;
            RequireCycleChanged?.Invoke(value);
        } }

    public int CurRequireCycleValue
    {
        get { return CurRequireCycle; }
        set
        {
            CurRequireCycle = value;
        }
    }

    public float ClockwiseSkillPointValue 
    { get{ return ClockwiseSkillPoint; } 
        set 
        { 
            ClockwiseSkillPoint = value;
            ClockSkillChanged?.Invoke(value);
        }}
    public float ClockwiseSkillRequireValue 
    { get{ return ClockwiseSkillRequire; } set { ClockwiseSkillRequire = value; ClockSkillReqChanged?.Invoke(value); } }
    public float CounterClockwiseSkillPointValue 
    { get{ return CounterClockwiseSkillPoint; } 
        set 
        { 
            CounterClockwiseSkillPoint = value;
            CClockSkillChanged?.Invoke(value);
        }
    }
    public float CounterClockwiseSkillRequireValue 
    { get{ return CounterClockwiseSkillRequire; } set { CounterClockwiseSkillRequire = value; CClockSkillReqChanged?.Invoke(value); } }
    #endregion


    private void Start()
    {
        player = GetComponent<Player>();
        HP = 0;
        ClockwiseSkillPoint = 0;
        CounterClockwiseSkillPoint = 0;

    }

    public void StatusInit()
    {
        SkillAdjust();

        

        MaxHP = player.PlayerInfo.GetHP();
        MaxHP += MaxHP * (PassiveHP / 100f);

        Attack = player.PlayerInfo.GetAttack();
        Attack += Attack * (PassiveAttack / 100f);

        Speed = player.PlayerInfo.GetSpeed();
        Speed += Speed * (PassiveAttack / 100f);

        Critical = player.PlayerInfo.GetCritical();
        Critical += PassiveCritical;

        RequireAP = player.PlayerInfo.GetRequireAP();
        RequireAP -= RequireAP * (PassiveReduceRequireAP / 100f);

        RequireCycle = player.PlayerInfo.GetRequireCycle();
        RequireCycle -= (int)PassiveReduceCycle;
        CurRequireCycle = RequireCycle;
    }

    public void SkillAdjust()
    {
        TreeNode normalNode = LibraryManager.Instance.playerAnalyses[player.playerType].NormalSkillTree.StartNode;
        TreeSkillAdjust(normalNode);

        TreeNode Active1Node = LibraryManager.Instance.playerAnalyses[player.playerType].Active1SkillTree.StartNode;
        TreeSkillAdjust(Active1Node);

        TreeNode Active2Node = LibraryManager.Instance.playerAnalyses[player.playerType].Active2SkillTree.StartNode;
        TreeSkillAdjust(Active2Node);
    }

    public TreeNode TreeSkillAdjust(TreeNode node)
    {
        foreach (TreeNode child in node.Children)
        {
            if (child.isEarned)
            {
                if (child.SkillObject.skillType == PlayerSkillType.Passive)
                {
                    PassiveAllocate(child.SkillObject.passiveType);
                }
                return TreeSkillAdjust(child);
            }
        }

        return null;
    }

    private void PassiveAllocate(PassiveSkill _passive)
    {
        switch (_passive)
        {
            default:
            case PassiveSkill.None:
                break;
            case PassiveSkill.HP:
                PassiveHP += 2;
                break;
            case PassiveSkill.Attack:
                PassiveAttack += 2;
                break;
            case PassiveSkill.AttackSpeed:
                PassiveAttackSpeed += 2;
                break;
            case PassiveSkill.BuffDuration:
                PassiveBuffDuration += 2;
                break;
            case PassiveSkill.ReduceCharging:
                PassiveReduceCharging += 2;
                break;
            case PassiveSkill.ReduceCooldown:
                PassiveReduceCooldown += 2;
                break;
            case PassiveSkill.Speed:
                PassiveSpeed += 2;
                break;
            case PassiveSkill.HPRegen:
                PassiveHPRegen += 1;
                break;
            case PassiveSkill.AttackSoulDrop:
                PassiveAttackSoulDrop += 1;
                break;
            case PassiveSkill.ReduceCycle:
                PassiveReduceCycle += 1;
                break;
            case PassiveSkill.ReduceDamage:
                PassiveReduceDamage += 1;
                break;
            case PassiveSkill.DamageSouldrop:
                PassiveDamageSoulDrop += 1;
                break;
            case PassiveSkill.AttackHPRegen:
                PassiveAttackHPRegen += 1;
                break;
            case PassiveSkill.ReduceRequireAP:
                PassiveReduceRequireAP += 5;
                break;

        }
    }
}