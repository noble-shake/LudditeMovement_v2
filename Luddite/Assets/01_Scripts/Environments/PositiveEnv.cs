using UnityEngine;
using static UnityEngine.ParticleSystem;

public class PositiveEnv : Environments
{
    [SerializeField] GameObject ParticleObject;
    [SerializeField] ParticleSystem SoulParticle;

    int SoulStorage = 300;
    int CurStorage;
    float SoulTimer = 150f; // 60 * 2 + 30
    float CurRegentTime;
    float CurInteractDelay;

    private void Start()
    {
        isActiavted = true;
        CurStorage = SoulStorage;
    }

    private void Update()
    {
        CurInteractDelay -= Time.deltaTime;
        if(CurInteractDelay < 0f) CurInteractDelay = 0;

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

    public override void OrbInteracted()
    {
        // ++Souls Up
        // Score Up
        if (isActiavted == false) return;
        if (CurInteractDelay > 0f) return;

        if(SoulParticle != null) SoulParticle.Play();
        CurInteractDelay = 0.5f;
        if (CurStorage >= 10)
        {
            CurStorage -= 10;
            PlayerManager.Instance.SoulValue += 10;
        }
        else
        {
            PlayerManager.Instance.SoulValue += CurStorage;
            CurStorage = 0;
        }

        if (CurStorage <= 0)
        {
            CurStorage = 300;
            ParticleObject.SetActive(false);
            isActiavted = false;
        }

    }
}