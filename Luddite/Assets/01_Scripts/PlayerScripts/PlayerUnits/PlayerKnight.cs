using System;
using System.Collections;
using UnityEngine;

public class PlayerKnight : Player
{
    [SerializeField] private SlashAttackJudgeZone SlashAttackZone; // Jduge SlashDirection;
    [SerializeField] private ParticleSystem SlashParticle;
    [SerializeField] public GameObject MegaSlashEffect;
    public Action SlashAction = new Action(() => { });


    protected override void Update()
    {
        base.Update();
    }

    public void SetSlashDirection(Quaternion quat)
    {
        SlashParticle.transform.rotation = quat;
    }

    #region Animation Event

    public override void AttackProcess()
    {
        SlashParticle.Play();
        SlashAction.Invoke();
    }

    #endregion
}