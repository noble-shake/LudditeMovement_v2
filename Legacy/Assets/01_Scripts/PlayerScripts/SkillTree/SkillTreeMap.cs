using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillTreeMap : MonoBehaviour
{
    [SerializeField] public SkillSlotRow[] skillSlotRows;
    [SerializeField] private CanvasGroup SkillCanvas;
    [SerializeField] private Image SkillIcon;
    [SerializeField] private TMP_Text SkillText;

    public Action<int> SkillEarnAction = new Action<int>((value) => { });

    private void Start()
    {
        int cnt = 0;
        foreach (SkillSlotRow skill in skillSlotRows)
        {
            skill.TierValue = cnt++;
            skill.TierText = $"LV.{skill.TierValue + 1}";

            foreach (SkillSlotButton btn in skill.NormalSlots)
            {
                if (btn.isEmpty) continue;
                btn.SkillCanavs = SkillCanvas;
                btn.SkillCanavsIcon = SkillIcon;
                btn.SkillCanavsText = SkillText;
                SkillEarnAction += btn.ButtonActivated;
            }

            foreach (SkillSlotButton btn in skill.Active1Slots)
            {
                if (btn.isEmpty) continue;
                btn.SkillCanavs = SkillCanvas;
                btn.SkillCanavsIcon = SkillIcon;
                btn.SkillCanavsText = SkillText;
                SkillEarnAction += btn.ButtonActivated;
            }

            foreach (SkillSlotButton btn in skill.Active2Slots)
            {
                if (btn.isEmpty) continue;
                btn.SkillCanavs = SkillCanvas;
                btn.SkillCanavsIcon = SkillIcon;
                btn.SkillCanavsText = SkillText;
                SkillEarnAction += btn.ButtonActivated;
            }
        }
    }

    public void ActivateInvoke(int Tier)
    {
        SkillEarnAction.Invoke(Tier);
    }
}

