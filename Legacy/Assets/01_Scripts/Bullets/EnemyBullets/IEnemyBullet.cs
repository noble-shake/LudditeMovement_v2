using UnityEngine;

public interface IEnemyBullet
{
    public abstract void SetBullet(Transform _ShootTrs, Transform _TargetTrs);
    public abstract void Update();
}