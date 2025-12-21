using System.Collections;
using UnityEngine;

public interface IEnemyAttack
{
    public void SetInit(Transform _trs, EnemyBullet _bulletPrefab);
    public void SetInit(Transform _trs, Transform _target, EnemyBullet _bulletPrefab);
    public void SetInit(EnemyBullet _bulletPrefab);

    public void Shot();

    public IEnumerator Attack();
    public void Charge();

    public void Update();



}