using System;
using UnityEngine;

public class SlimeSpreadBullet : IEnemyBullet
{
    float curFlow = 10;
    float Speed = UnityEngine.Random.Range(1.5f, 8.5f);
    private Transform ShootTrs;
    private Transform TargetTrs;

    public SlimeSpreadBullet() { }

    public void SetBullet(Transform _ShootTrs, Transform _TargetTrs)
    {
        ShootTrs = _ShootTrs;
        TargetTrs = _TargetTrs;
    }   

    public void Update()
    {
        ShootTrs.transform.position += ShootTrs.forward * Time.deltaTime * Speed;
        Speed -= Time.deltaTime /2f;
        if (Speed < 0.3f) Speed = 0.3f;

        curFlow -= Time.deltaTime;
        if (curFlow < 0f)
        {
            ResourceManager.Instance.ResourceRetrieve(ShootTrs.gameObject);
        }
    }
}