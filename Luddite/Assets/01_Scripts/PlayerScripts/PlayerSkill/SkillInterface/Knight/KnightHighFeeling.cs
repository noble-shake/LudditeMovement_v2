/*
 * Indicator
 */

using TMPro;
using UnityEngine;

public class KnightHighFeeling: IPlaySkill
{
    Player player;
    bool ischarging;
    float CurFlow;

    int RequireCycle;
    CanvasGroup CycleUI;
    TMP_Text CycleText;

    public KnightHighFeeling()
    {
    }

    public void Init(Player _player)
    {
        player = _player;
        RequireCycle = 3;
        CycleUI = player.GetCycleUI();
        CycleText = player.GetCycleText();
    }

    public void SkillActivated()
    {
        ischarging = true;
        RequireCycle--;
        CycleUI.gameObject.SetActive(true);

        if (RequireCycle >= 0) SkillExecute();
    }

    public void SkillExecute()
    {
        ischarging = false;
        CycleUI.gameObject.SetActive(false);
        RequireCycle = 3;
    }

    public void SkillUpdate(float deltaTime)
    {
        if (ischarging == false) return;
    }

    public void SkillReady()
    {

    }
}