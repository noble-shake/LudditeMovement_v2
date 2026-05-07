/*
 * Indicator
 */

using DTT.AreaOfEffectRegions;
using UnityEngine;
using System.Collections.Generic;
using static UnityEngine.GraphicsBuffer;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class KnightMegaSlash : IPlaySkill
{
    Player player;
    bool ischarging;
    float CurFlow;
    ArcRegion arcRegion;
    SphereCollider arcCollider;
    SwitchableEffectObject aura;

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
        CurFlow = 0f;
        arcRegion.FillProgress = 0.5f;
        arcCollider.radius = 0.5f;
        arcRegion.gameObject.SetActive(true);
        ischarging = true;
        GameObject Slash = ResourceManager.Instance.GetEffectResource("KnightChargeEffect");
        aura = Slash.gameObject.GetComponent<SwitchableEffectObject>();
        Slash.transform.position = player.transform.position;
    }

    public void SkillExecute()
    {
        List<Collider> targets = arcRegion.GetComponent<ArcRangeCheck>().GetRangedTarget();
        arcRegion.gameObject.SetActive(false);
        GameObject Slash = ResourceManager.Instance.GetEffectResource("KnightChargeSlashEffect");
        Slash.transform.position = player.transform.position;
        Slash.transform.rotation = Quaternion.Euler(0f, arcRegion.Angle, 0f);

        foreach (Collider e in targets)
        {
            Enemy enemy = e.GetComponent<Enemy>();
            enemy.OnHit(player.statusManager.AttackValue * 3f);
            GameObject Impact = ResourceManager.Instance.GetEffectResource("KnightSlashHitEffect");
            Impact.transform.position = e.ClosestPoint(player.transform.position);
            enemy.GetComponent<Rigidbody>().AddForce((enemy.transform.position - player.transform.position).normalized * 2f, ForceMode.Impulse);
        }

    }
    public void SkillUpdate(float deltaTime)
    {
        if (ischarging == false) return;

        Vector3 direction = PlayerManager.Instance.GetPlayerTrs().position - player.transform.position;
        if (PlayerManager.Instance.GetPlayerTrs().position.x > player.transform.position.x)
        {
            player.PlayerFlip(false);
        }
        else if (PlayerManager.Instance.GetPlayerTrs().position.x < player.transform.position.x)
        {
            player.PlayerFlip(true);
        }


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
        GameManager.Instance.PlayerSkillAction(ResourceManager.Instance.GetPlayerInfo(player.playerType).FullBodyPortrait, ResourceManager.Instance.GetPlayerInfo(player.playerType).FullBodySilouhettePortrait);
        GameObject CircleEffect = ResourceManager.Instance.GetEffectResource("CircleExpandEffect");
        CircleEffect.transform.position = player.transform.position;
        float DamageRatio = CurFlow / 3f;
        player.GetAnim().Play("SkillExecute");
        ischarging = false;
        CurFlow = 3f;
    }

    public void SkillDone()
    {
        player.ChargeCheck = false;
        aura.ResourceRetrieve();
    }
}