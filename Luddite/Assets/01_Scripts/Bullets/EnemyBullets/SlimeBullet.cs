using UnityEngine;

public class SlimeBullet : IEnemyBullet
{
    float curFlow = 1.5f;
    float Speed = 2.5f;
    float Remained = 0.15f;
    private Transform ShootTrs;
    private Transform TargetTrs;

    public SlimeBullet() { }

    public void SetBullet(Transform _ShootTrs, Transform _TargetTrs)
    {
        ShootTrs = _ShootTrs;
        TargetTrs = _TargetTrs;
    }

    public void Update()
    {
        ShootTrs.transform.position += ShootTrs.forward * Time.deltaTime * Speed;
        Speed -= Time.deltaTime * 2f;
        curFlow -= Time.deltaTime;

        if (curFlow < 0f)
        {
            curFlow = 0f;
            Speed = 0f;
            Remained -= Time.deltaTime;
            ShootTrs.localScale -= -Time.deltaTime * 0.15f * Vector3.one;
        }

        if (Remained < 0f)
        {
            ShootTrs.localScale = Vector3.one;
            ResourceManager.Instance.ResourceRetrieve(ShootTrs.gameObject);
        }
    }
}