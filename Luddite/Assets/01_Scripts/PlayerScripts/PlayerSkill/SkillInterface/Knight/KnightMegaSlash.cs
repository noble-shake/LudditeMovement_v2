/*
 * Indicator
 */

using DTT.AreaOfEffectRegions;
using UnityEngine;
using System.Collections.Generic;

public class KnightMegaSlash : IPlaySkill
{
    Player player;
    bool ischarging;
    float CurFlow;
    ArcRegion arcRegion;
    SphereCollider arcCollider;

    public KnightMegaSlash() 
    {
        CurFlow = 0f;
    }

    public void Init(Player _player)
    {
        player = _player;
        arcRegion = player.GetArcIndicator();
        arcCollider = arcRegion.GetComponent<SphereCollider>();
    }

    public void SkillActivated()
    {
        arcCollider.radius = 0.5f;
        arcRegion.gameObject.SetActive(true);
        ischarging = true;
    }

    public void SkillExecute()
    {
        List<Enemy> targets = arcRegion.GetComponent<ArcRangeCheck>().GetRangedTarget();
        arcRegion.gameObject.SetActive(false);
        foreach (Enemy e in targets)
        {
            e.OnHit(player.statusManager.AttackValue * 3f);
        }

    }
    public void SkillUpdate(float deltaTime)
    {
        if (ischarging == false) return;

        Vector3 direction = PlayerManager.Instance.GetPlayerTrs().position - player.transform.position;

        float angleRad = Mathf.Atan2(direction.z, direction.x);

        float angleDeg = angleRad * Mathf.Rad2Deg;
        if (angleDeg < 0f) angleDeg += 360f;
        if (angleDeg > 360f) angleDeg = 0f;
        arcRegion.Angle = 360f - (angleDeg - 90f);

        CurFlow += deltaTime;
        if (CurFlow > 3f) CurFlow = 3f;

        if (CurFlow / 3f < 0.5f)
        {
            arcRegion.FillProgress = 0.5f;
            arcCollider.radius = 0.5f;
        }
        else
        {
            arcRegion.FillProgress = CurFlow / 3f;
            arcCollider.radius = 2.5f * (CurFlow / 3f);
        }

        if (InputManager.Instance.AttackInput)
        {
            InputManager.Instance.AttackInput = false;
            SkillReady();

        }

    }

    public void SkillReady()
    {
        float DamageRatio = CurFlow / 3f;
        player.GetAnim().Play("SkillExecute");
        ischarging = false;
        CurFlow = 3f;
    }

    public void SkillDone()
    {
        player.ChargeCheck = false;
    }
}