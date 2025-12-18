using TMPro;
using Unity.UI.Shaders.Sample;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class TopBorderUI : MonoBehaviour
{
    [SerializeField] private TMP_Text CurrentScore;
    [SerializeField] private TMP_Text CurrentPlayTime;
    [SerializeField] private TMP_Text HPText;
    [SerializeField] private TMP_Text SPText;
    [SerializeField] private Meter HP;
    [SerializeField] private Meter HPBackground;
    [SerializeField] private float MaxHP;
    [SerializeField] private Meter SP;
    [SerializeField] private Meter SPBackground;
    [SerializeField] private float MaxSP;

    public void GetScore(int value)
    { 
        CurrentScore.text = value.ToString("D12");
    }

    public void GetPlayTime(float value)
    {
        // GUILayout.Label($"GameTime : {Mathf.FloorToInt(TimeScheduling / 60f).ToString("D2")}:{Mathf.FloorToInt(TimeScheduling % 60f).ToString("D2")}", targetStyle);
        CurrentPlayTime.text = $"{Mathf.FloorToInt(value / 60f).ToString("D2")}:{Mathf.FloorToInt(value % 60f).ToString("D2")}";
    }

    public void GetHPValue(float value)
    {
        HP.Value = value / MaxHP;
        HPText.text = $"{value.ToString("F2")}/{(int)MaxHP}";
        Invoke("HPChanged", 0.3f);
    }

    public void GetMaxHPValue(float value)
    {
        MaxHP = Mathf.FloorToInt(value);
    }

    public void GetMaxSPValue(float value)
    { 
        MaxSP = Mathf.FloorToInt(value);

    }

    public void GetSPValue(float value)
    {
        SP.Value = Mathf.FloorToInt(value) / MaxSP;
        SPText.text = $"{(int)value}/{(int)MaxSP}";
        Invoke("SoulChanged", 0.3f);
    }

    public float MaxHPValue { get { return MaxHP; } set { GetMaxHPValue(value); } }
    public float MaxSPValue { get { return MaxSP; } set { GetMaxSPValue(value); } }

    #region Gauge Effect

    bool isSoulGaugeChanged;
    bool isHPGaugeChanged;
    private void Update()
    {
        SoulGaugeCheck();
    }

    private void SoulGaugeCheck()
    {
        if (isHPGaugeChanged)
        {
            HPBackground.Value = Mathf.Lerp(HPBackground.Value, HP.Value, Time.deltaTime * 10f);
            if (HP.Value >= HPBackground.Value - 0.01f)
            {
                isHPGaugeChanged = false;
                HPBackground.Value = HP.Value;
            }
        }

        if (isSoulGaugeChanged)
        {
            SPBackground.Value = Mathf.Lerp(SPBackground.Value, SP.Value, Time.deltaTime * 10f);
            if (SP.Value >= SPBackground.Value - 0.01f)
            {
                isSoulGaugeChanged = false;
                SPBackground.Value = SP.Value;
            }
        }
    }

    private void SoulChanged()
    {
        isSoulGaugeChanged = true;
    }

    private void HPChanged()
    {
        isHPGaugeChanged = true;
    }
    #endregion

}