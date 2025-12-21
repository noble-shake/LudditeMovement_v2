using UnityEngine;
using UnityEngine.UI;

public class PlayerSkillActionCanvas : MonoBehaviour
{
    [SerializeField] private Image Background;
    [SerializeField] private Image Main;
    [SerializeField] public Animator animator;
    [SerializeField] private Image SkillLine;
    [SerializeField] private Material SkillLineMat;

    private void OnEnable()
    {
        SkillLineMat = SkillLine.material;
        SkillLineMat.SetFloat("_Appear", 3f);
    }

    public void SetSprite(Sprite BG, Sprite MAIN)
    { 
        Background.sprite = BG;
        Main.sprite = MAIN;
    }

    public void SetLineMat(float value)
    {
        if (value < -3f) value = 3f;
        SkillLineMat.SetFloat("_Appear", value);
    }
}