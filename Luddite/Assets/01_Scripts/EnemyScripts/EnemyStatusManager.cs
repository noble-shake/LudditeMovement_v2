using Unity.UI.Shaders.Sample;
using UnityEditor;
using UnityEngine;

public class EnemyStatusManager : MonoBehaviour
{
    [SerializeField] private Enemy owner;
    [SerializeField] private Meter HPBar;
    [SerializeField] private Meter HPBackground;
    private bool isHPChanged;

    [SerializeField] private Meter APBar;
    [SerializeField] private Meter APBackground;
    private bool isAPChanged;

    [SerializeField] private Meter BPBar;
    [SerializeField] private Meter BPBackground;
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
    [SerializeField] public float MaxAPValue { get {return MaxAP;} set {MaxAP =value;} }
    [SerializeField] public float MaxBPValue { get {return MaxBP;} set {MaxBP =value;} }

    public void SetHP(float value)
    {
        HP += value;

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

        Invoke("HPChanged", 0.3f);
    }

    public void SetAP(float value)
    {
        AP += value;

        if (AP <= 0)
        {
            AP = 0;
        }

        if (AP > MaxAP)
        {
            owner.isAttackCheck = true;
            AP = MaxAP;
        }

        Invoke("APChanged", 0.3f);
    }

    public void SetBP(float value)
    {
        BP += value;

        if (BP <= 0)
        {
            BP = 0;
        }

        if (BP > MaxBP)
        {
            owner.isAttackCheck = false;
            owner.isIdleCheck = false;
            owner.isMoveCheck = false;
            owner.isStunCheck = true;
            AP = 0;
            BP = MaxBP;
            // Stunned.
        }

        Invoke("BPChanged", 0.3f);
    }
    #endregion

    #region Gauge Effect
    private void HPGaugeCheck()
    {
        if (HPBar != null) HPBar.Value = Mathf.Lerp(HPBar.Value, HP / MaxHP, Time.unscaledDeltaTime * 5f);

        if (isHPChanged)
        {
            if (HPBackground != null) HPBackground.Value = Mathf.Lerp(HPBackground.Value, HPBar.Value, Time.unscaledDeltaTime * 10f);
            if (HPBar.Value >= HPBackground.Value - 0.01f)
            {
                isHPChanged = false;
                HPBackground.Value = HPBar.Value;
                if (owner.isDeadCheck == true)
                {
                    // TODO:  Actual Death Judge
                    
                }
            }
        }
    }

    private void APGaugeCheck()
    {
        if (APBar != null) APBar.Value = Mathf.Lerp(APBar.Value, AP / MaxAP, Time.unscaledDeltaTime * 5f);

        if (isAPChanged)
        {
            if (APBackground != null) APBackground.Value = Mathf.Lerp(APBackground.Value, APBar.Value, Time.unscaledDeltaTime * 10f);
            if (APBar.Value >= APBackground.Value - 0.01f)
            {
                isAPChanged = false;
                APBackground.Value = APBar.Value;
            }
        }
    }

    private void BPGaugeCheck()
    {
        if (BPBar != null) BPBar.Value = Mathf.Lerp(BPBar.Value, BP / MaxBP, Time.unscaledDeltaTime * 5f);

        if (isBPChanged)
        {
            if (BPBackground != null) BPBackground.Value = Mathf.Lerp(BPBackground.Value, BPBar.Value, Time.unscaledDeltaTime * 10f);
            if (BPBar.Value >= BPBackground.Value - 0.01f)
            {
                isBPChanged = false;
                BPBackground.Value = BPBar.Value;
            }
        }
    }

    private void HPChanged()
    {
        isHPChanged = true;
    }

    private void APChanged()
    {
        isAPChanged = true;
    }

    private void BPChanged()
    {
        isBPChanged = true;
    }


    #endregion

    private void Start()
    {
        owner = GetComponent<Enemy>();
    }

    private void Update()
    {
        SetAP(Time.deltaTime);

        if (BP < MaxBP)
        {
            SetBP(-Time.deltaTime * 2f);
        }



    }

    public void OnHit(float value)
    {
        SetHP(value);
        SetAP(1);
        if (value < 0f)
        {
            SetBP(Mathf.Abs(value));
        }

    }
}