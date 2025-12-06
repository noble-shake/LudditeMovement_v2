using TMPro;
using Unity.UI.Shaders.Sample;
using UnityEngine;
using UnityEngine.UI;

public class PlayerSlotUI : MonoBehaviour
{
    [SerializeField] Image Portrait;
    [SerializeField] private Meter SkillClockWiseReady;
    [SerializeField] private Image SkillClockBackground;
    [SerializeField] private Meter SkillCClockWiseReady;
    [SerializeField] private Image SkillCClockBackground;
    [SerializeField] private Meter HP;
    [SerializeField] private CanvasGroup ChatBox;
    [SerializeField] private TMP_Text ChatText;

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

    public float HPValue { get { return HP.Value; } set {
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

}