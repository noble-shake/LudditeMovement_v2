using System.Collections;
using UnityEngine;
using static UnityEngine.ParticleSystem;

public class PositiveEnv : Environments
{

    [SerializeField] GameObject ParticleObject;
    [SerializeField] ParticleSystem SoulParticle;

    int EnvHP = 5;
    int CurHP;
    float SoulTimer = 150f; // 60 * 2 + 30
    float CurRegentTime;
    float CurInteractDelay;

    private void Start()
    {
        isActiavted = true;
        CurHP= EnvHP;
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
        CurInteractDelay = 1f;

        CurHP--;
        StartCoroutine(SoulDrop(Random.Range(8, 13)));  

        if (CurHP <= 0)
        {
            CurHP = EnvHP;
            ParticleObject.SetActive(false);
            isActiavted = false;
        }

    }

    IEnumerator SoulDrop(int _numb)
    {
        for (int i = 0; i < _numb; i++)
        {
            SoulItem soul = ResourceManager.Instance.GetSoulItem();
            soul.SoulValue = Random.Range(8, 21);
            soul.transform.position = this.transform.position;
            yield return null;
        }
    }
}