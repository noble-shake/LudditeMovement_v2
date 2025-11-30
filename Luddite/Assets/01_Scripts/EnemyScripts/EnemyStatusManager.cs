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

    #region Status Proerties
    [SerializeField] public int HP { get; private set; }
    [SerializeField] public int MaxHP { get; private set; }
    [SerializeField] public int AP { get; private set; }
    [SerializeField] public int MaxAP { get; private set; }
    [SerializeField] public int BP { get; private set; }
    [SerializeField] public int MaxBP { get; private set; }

    public void SetHP(int value)
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

    public void SetAP(int value)
    {
        AP += value;

        if (AP <= 0)
        {
            AP = 0;
        }

        if (AP > MaxAP)
        {
            AP = MaxAP;
        }

        Invoke("APChanged", 0.3f);
    }

    public void SetBP(int value)
    {
        BP += value;

        if (BP <= 0)
        {
            BP = 0;
        }

        if (BP > MaxBP)
        {
            BP = MaxBP;
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
}