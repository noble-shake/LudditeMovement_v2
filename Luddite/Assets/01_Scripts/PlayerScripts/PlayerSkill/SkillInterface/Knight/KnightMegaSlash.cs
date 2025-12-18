/*
 * Indicator
 */

using DTT.AreaOfEffectRegions;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.GraphicsBuffer;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class KnightMegaSlash : IPlaySkill
{
    Player player;
    bool ischarging;
    float CurFlow;
    ArcRegion arcRegion;
    
    public KnightMegaSlash() 
    {
        CurFlow = 0f;
    }

    public void Init(Player _player)
    {
        player = _player;
        arcRegion = player.GetArcIndicator();
    }

    public void SkillActivated()
    {
        arcRegion.gameObject.SetActive(true);
        ischarging = true;
    }

    public void SkillExecute()
    {
        float DamageRatio = CurFlow / 3f;
        arcRegion.gameObject.SetActive(false);
        ischarging = false;
        CurFlow = 3f;
    }
    public void SkillUpdate(float deltaTime)
    {
        if (ischarging == false) return;

        Vector3 direction = PlayerManager.Instance.GetPlayerTrs().position - player.transform.position;

        float angleRad = Mathf.Atan2(direction.z, direction.x);

        // 라디안을 도로 변환
        float angleDeg = angleRad * Mathf.Rad2Deg;
        if (angleDeg < 0f) angleDeg += 360f;
        if (angleDeg > 360f) angleDeg = 0f;
        arcRegion.Angle = 360f - (angleDeg - 90f);

        Debug.Log($"{angleDeg}, {arcRegion.Angle}");

        CurFlow += deltaTime;
        if (CurFlow > 3f) CurFlow = 3f;

        if (CurFlow / 3f < 0.5f)
        {
            arcRegion.FillProgress = 0.5f;
        }
        else
        {
            arcRegion.FillProgress = CurFlow / 3f;
        }

        if (InputManager.Instance.AttackInput)
        {
            InputManager.Instance.AttackInput = false;
            SkillExecute();

        }

    }

    public void SkillReady()
    {

    }
}