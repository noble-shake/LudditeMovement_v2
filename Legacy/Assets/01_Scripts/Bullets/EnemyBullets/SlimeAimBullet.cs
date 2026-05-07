using System;
using UnityEngine;

public class SlimeAimBullet : IEnemyBullet
{
    float curFlow = 10;
    float Speed = 3.5f;
    private Transform ShootTrs;
    private Transform TargetTrs;

    public SlimeAimBullet() { }

    public void SetBullet(Transform _ShootTrs, Transform _TargetTrs)
    {
        ShootTrs = _ShootTrs;
        TargetTrs = _TargetTrs;
    }   

    public void Update()
    {
        ShootTrs.transform.position += ShootTrs.forward * Time.deltaTime * Speed;
        Speed += Time.deltaTime * 2f;
        curFlow -= Time.deltaTime;
        if (curFlow < 0f)
        {
            ResourceManager.Instance.ResourceRetrieve(ShootTrs.gameObject);
        }
    }
}