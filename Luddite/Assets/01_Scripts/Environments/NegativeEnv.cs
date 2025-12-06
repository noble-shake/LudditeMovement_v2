using NUnit;
using UnityEngine;

public class NegativeEnv : Environments
{
    [SerializeField] GameObject ParticleObject;
    [SerializeField] ParticleSystem SoulParticle;
    [SerializeField] ParticleSystem NegativeParticle;

    private int EnvHP;
    private float SoulTimer = 20f;
    private float CurRegentTime;
    private float CurInteractDelay;

    private float NegativeTimer = 5f;
    private float CurNegativeTime;

    private void Start()
    {
        EnvHP = Random.Range(2, 6);

        isActiavted = true;
    }

    private void Update()
    {
        CurInteractDelay -= Time.deltaTime;
        if (CurInteractDelay < 0f) CurInteractDelay = 0;

        if (isActiavted) NegativeEffect();

        if (isActiavted == false)
        {
            CurRegentTime += Time.deltaTime;
            if (SoulTimer <= CurRegentTime)
            {
                ParticleObject.SetActive(true);
                CurRegentTime = 0f;
                isActiavted = true;
            }
        }
    }

    public void NegativeEffect()
    {
        CurNegativeTime -= Time.deltaTime;
        if (CurNegativeTime <= 0f)
        {
            PlayerManager.Instance.SoulValue -= Mathf.Clamp(PlayerManager.Instance.SoulValue * 0.1f, 10f, 100f);
            CurNegativeTime = NegativeTimer;
            NegativeParticle.Play();
        }


    }

    public override void OrbInteracted()
    {
        // ++Souls Up
        // Score Up
        if (isActiavted == false) return;
        if (CurInteractDelay > 0f) return;

        if (SoulParticle != null) SoulParticle.Play();
        CurInteractDelay = 0.5f;

        EnvHP--;
        if (EnvHP <= 0)
        {
            EnvHP = Random.Range(2, 6);
            ParticleObject.SetActive(false);
            isActiavted = false;
        }

    }
}