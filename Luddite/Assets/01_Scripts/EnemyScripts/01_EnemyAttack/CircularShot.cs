using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class CircularShot : IEnemyAttack
{
    private EnemyBullet BulletPrefab;
    private Transform Owner;

    public CircularShot() { }

    public void SetInit(Transform _trs, EnemyBullet _bulletPrefab)
    {
        Owner = _trs;
        BulletPrefab = _bulletPrefab;
    }

    public void SetInit(Transform _trs, Transform _target, EnemyBullet _bulletPrefab) { }

    public void SetInit(EnemyBullet _bulletPrefab) { }
    
    public void Shot()
    {
        
    }

    public IEnumerator Attack()
    {
        int _diff = (int)GameManager.Instance.difficulty;
        float angle = Quaternion.FromToRotation(Vector3.forward, Vector3.back).eulerAngles.z;
        for (int i = 0; i < 6 + 3 * _diff; i++)
        {
            EnemyBullet bullet = ResourceManager.Instance.GetResource(BulletPrefab.gameObject).GetComponent<EnemyBullet>();
            bullet.pattern = new SlimeBullet();
            //bullet.pattern = BulletPattern;
            bullet.pattern.SetBullet(bullet.transform, PlayerManager.Instance.GetPlayerTrs());
            bullet.transform.SetParent(null);
            bullet.transform.position = Owner.transform.position;
            bullet.SetAngle(new Vector3(0f, angle, 0f));
            angle += (360f / (6 + 3 * _diff));

        }

        yield return null;
    }
}
