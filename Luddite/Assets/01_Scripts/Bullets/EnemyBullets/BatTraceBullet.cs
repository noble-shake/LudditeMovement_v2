using System;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class BatTraceBullet : IEnemyBullet
{
    float curFlow = 5;
    float Speed = 3f;
    private Transform ShootTrs;
    private Transform TargetTrs;

    public BatTraceBullet() { }

    public void SetBullet(Transform _ShootTrs, Transform _TargetTrs)
    {
        ShootTrs = _ShootTrs;
        TargetTrs = _TargetTrs;
    }   

    public void Update()
    {
        Vector3 StartPos = ShootTrs.position;
        Vector3 TargetPos = TargetTrs.position;

        float d = MathF.Sqrt((TargetPos.x - StartPos.x) * (TargetPos.x - StartPos.x) + (TargetPos.z - StartPos.z) * (TargetPos.z - StartPos.z));
        float x = (TargetPos.x - StartPos.x) / d * Speed;
        float z = (TargetPos.z - StartPos.z) / d * Speed;

        if (d == 0)
        {
            x = Speed;
            z = Speed;
        }

        ShootTrs.position += new Vector3(x * Time.deltaTime, 0f, z * Time.deltaTime);

        curFlow -= Time.deltaTime;
        if (curFlow < 0f)
        {
            curFlow = 10f;
            ResourceManager.Instance.ResourceRetrieve(ShootTrs.gameObject);
        }

    }
}