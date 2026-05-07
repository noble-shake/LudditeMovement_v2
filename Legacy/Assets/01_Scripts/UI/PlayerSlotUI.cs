using System;
using TMPro;
using Unity.UI.Shaders.Sample;
using UnityEngine;
using UnityEngine.UI;

public class PlayerSlotUI : MonoBehaviour
{
    [SerializeField] Image Portrait;
    [SerializeField] private Meter SkillClockWiseReady;
    [SerializeField] private float SkillClockWiseRequire;
    [SerializeField] private Image SkillClockBackground;
    [SerializeField] private Meter SkillCClockWiseReady;
    [SerializeField] private float SkillCClockWiseRequire;
    [SerializeField] private Image SkillCClockBackground;
    [SerializeField] private Meter HP;
    [SerializeField] private float MaxHP;
    [SerializeField] private CanvasGroup ChatBox;
    [SerializeField] private TMP_Text ChatText;
    [SerializeField] private CanvasGroup EmptyCover;

    public void SetMaxHP(float value)
    {
        MaxHP = value;
    }

    public void SetSkillClockWiseRequire(float value)
    {
        SkillClockWiseRequire = value;
    }

    public void SetSkillCClockWiseRequire(float value)
    {
        SkillCClockWiseRequire = value;
    }

    public float SkillClockValue 
    { get { return SkillClockWiseReady.Value; } 
        set { 
            SkillClockWiseReady.Value = value;
            if (SkillClockWiseReady.Value >= 1f)
            {
                SkillClockBackground.color = new Color(1f, 222f/256f, 0f);
            }
            else
            {
                SkillClockBackground.color = Color.black;
            }
        } 
    }
    public float SkillCClockValue 
    { get { return SkillCClockWiseReady.Value; } 
        set { 
            SkillCClockWiseReady.Value = value;
            if (SkillCClockWiseReady.Value >= 1f)
            {
                SkillCClockBackground.color = new Color(1f, 222f / 256f, 0f);
            }
            else
            {
                SkillCClockBackground.color = Color.black;
            }
        } 
    }

    public float HPValue {
        get { return HP.Value; } 
        set {
            if (value > 1f)
            {
                value = 1f;
            }
            if (value < 0f)
            {
                value = 0f;
            }

            HP.Value = value; } 
    }

    public void GetHPValue(float value)
    {
        HPValue = value / MaxHP;
    }

    public void GetMaxHPValue(float value)
    {
        HPValue = value / MaxHP;
    }

    public void GetClockValue(float value)
    {
        SkillClockValue = value / SkillClockWiseRequire;
    }

    public void GetClockRequire(float value)
    {
        SkillClockWiseRequire = value;
    }

    public void GetCClockValue(float value)
    {
        SkillCClockValue = value / SkillCClockWiseRequire;
    }

    public void GetCClockRequire(float value)
    {
        SkillCClockWiseRequire = value;
    }

    public void SetActivatedCharacter(bool isOn)
    {
        if (isOn)
        {
            Portrait.color = Color.white;
        }
        else
        {
            Portrait.color = new Color(92f/256f, 92f / 256f, 92f / 256f);
        }
    }

    public void SetPortriat(Sprite _sprite)
    {
        Portrait.sprite = _sprite;
    }

    public void PortriatClear()
    { 
        Portrait.sprite = null;
    }

    public void Mapping()
    {
        EmptyCover.alpha = 0f;
    }

    public void Unmapping()
    {
        EmptyCover.alpha = 1f;
    }

}