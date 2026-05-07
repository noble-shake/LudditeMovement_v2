using System;
using UnityEngine;

public class BatCrawlBullet : IEnemyBullet
{
    float curFlow = 1f;
    float Speed = 0.5f;
    int cycle = 1;
    private Transform ShootTrs;
    private Transform TargetTrs;
    Vector3 StartPos;
    public bool isReverse;
    float cycleFlow;

    public BatCrawlBullet() { }

    public void SetBullet(Transform _ShootTrs, Transform _TargetTrs)
    {
        ShootTrs = _ShootTrs;
        TargetTrs = _TargetTrs;
        StartPos = ShootTrs.transform.position;
        if (TargetTrs == null)
        {
            isReverse = false;
        }
        else
        {
            isReverse = true;
        }
    }



    public void Update()
    {
        // Z +1 ~ Z - 1
        int _diff = (int)GameManager.Instance.difficulty;
        cycleFlow += Time.deltaTime * Speed * (_diff + 1);
        if (cycleFlow > 1f)
        {
            cycleFlow = 0f;
            if (cycle == 3)
            {
                ResourceManager.Instance.ResourceRetrieve(ShootTrs.gameObject);
            }
            cycle++;
        }

        ShootTrs.position = StartPos + new Vector3((isReverse ? -1 : 1) * (MathF.Cos(Mathf.Deg2Rad * Mathf.Lerp(0f, 360f, cycleFlow)) * cycle), 0f, Mathf.Sin(Mathf.Deg2Rad * Mathf.Lerp(0f, 360f, cycleFlow)));

    }
}