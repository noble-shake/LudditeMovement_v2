using Unity.UI.Shaders.Sample;
using UnityEditor;
using UnityEngine;

public class EnemyStatusManager : MonoBehaviour
{
    [SerializeField] private Enemy owner;
    [SerializeField] private Meter HPBar;
    private bool isHPChanged;

    [SerializeField] private Meter APBar;
    private bool isAPChanged;

    [SerializeField] private Meter BPBar;
    private bool isBPChanged;

    [Header("Status")]
    [SerializeField] private float HP;
    [SerializeField] private float MaxHP;
    [SerializeField] private float AP;
    [SerializeField] private float MaxAP;
    [SerializeField] private float BP;
    [SerializeField] private float MaxBP;



    #region Status Proerties
    [SerializeField] public float MaxHPValue { get {return MaxHP;} set {MaxHP =value;} }
    [SerializeField] public float HPValue 
    { 
        get {return HP;} 
        set 
        {
            HP =value;

            if (HP <= 0)
            {
                HP = 0;
                // Death
                owner.isDeadCheck = true;
            }

            if (HP > MaxHP)
            {
                HP = MaxHP;
            }

            HPBar.Value = HP / MaxHP;
        }
    }
    [SerializeField] public float MaxAPValue { get {return MaxAP;} set {MaxAP =value;} }
    [SerializeField] public float APValue 
    { 
        get {return AP;} 
        set 
        {
            AP =value;

            if (AP <= 0)
            {
                AP = 0;
            }

            if (AP > MaxAP)
            {
                if (owner.isStunCheck) return;
                owner.isAttackCheck = true;
                AP = MaxAP;
                owner.CurrentState = Enemy.EnemyBehaviour.ATTACK;
            }

            APBar.Value = AP / MaxAP;
        }
    }
    [SerializeField] public float MaxBPValue { get {return MaxBP;} set {MaxBP =value;} }
    [SerializeField] public float BPValue 
    { 
        get {return BP;} 
        set 
        {
            BP =value;

            if (BP <= 0)
            {
                BP = 0;
            }

            if (BP > MaxBP)
            {
                AP = 0;
                BP = MaxBP;
                // Stunned.
                owner.isStunCheck = true;
                owner.CurrentState = Enemy.EnemyBehaviour.STUNNED;
            }

            BPBar.Value = BP / MaxBP;
        }
    }
    #endregion

    private void Start()
    {
        owner = GetComponent<Enemy>();
    }

    private void Update()
    {

        APValue += Time.deltaTime;

        if (BPValue < MaxBPValue)
        {
            BPValue -= Time.deltaTime;
        }



    }

    public void OnHit(float value)
    {
        HPValue -= value;
        APValue += 1;
        if (value > 0f)
        {
            BPValue  += Mathf.Abs(value);
        }

    }
}