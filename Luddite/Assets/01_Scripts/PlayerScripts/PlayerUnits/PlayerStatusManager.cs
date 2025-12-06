using UnityEngine;

public class PlayerStatusManager : MonoBehaviour
{
    [SerializeField] private Player player;
    [SerializeField] private float HP;
    [SerializeField] private float MaxHP;
    [SerializeField] private float Speed;
    [SerializeField] private float ClockwiseSkillPoint;
    [SerializeField] private float ClockwiseSkillRequire;
    [SerializeField] private float CounterClockwiseSkillPoint;
    [SerializeField] private float CounterClockwiseSkillRequire;

    #region Properties
    public float HPValue 
    { get { return HP; }  
        set 
        { 
            HP = value;
            if (HP == MaxHP && player.ActivatedCheck == false) player.ActivatedCheck = true;
        }
    }
    public float MaxHPValue 
    {get{ return MaxHP; } set { MaxHP = value; }}

    public float SpeedValue
    { get { return Speed; } set { Speed= value; } }
    public float ClockwiseSkillPointValue 
    { get{ return ClockwiseSkillPoint; } set { ClockwiseSkillPoint = value; }}
    public float ClockwiseSkillRequireValue 
    { get{ return ClockwiseSkillRequire; } set { ClockwiseSkillRequire = value; } }
    public float CounterClockwiseSkillPointValue 
    { get{ return CounterClockwiseSkillPoint; } set { CounterClockwiseSkillPoint = value; }}
    public float CounterClockwiseSkillRequireValue 
    { get{ return CounterClockwiseSkillRequire; } set { CounterClockwiseSkillRequire = value; } }
    #endregion


    private void Start()
    {
        player = GetComponent<Player>();
        HP = 0;
        ClockwiseSkillPoint = 0;
        CounterClockwiseSkillPoint = 0;

    }
}