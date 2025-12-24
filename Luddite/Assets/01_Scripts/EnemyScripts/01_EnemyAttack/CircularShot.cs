using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class CircularShot : IEnemyAttack
{
    private List<EnemyBullet> BulletPrefab;
    private List<BulletScriptableObject> BulletPattern;
    private Transform Owner;

    public CircularShot() { }

    public void GetNeeds(List<EnemyBullet> _bullets, List<BulletScriptableObject> _bulletSetting)
    {
        BulletPrefab = _bullets;
        BulletPattern = _bulletSetting;
    }

    public void SetInit(Transform _Owner)
    {
        Owner = _Owner;
    }

    public void Shot()
    {
        
    }

    public IEnumerator Attack()
    {
        int _diff = (int)GameManager.Instance.difficulty;
        float angle = Quaternion.FromToRotation(Vector3.forward, Vector3.back).eulerAngles.z;
        for (int i = 0; i < 6 + 3 * _diff; i++)
        {
            EnemyBullet bullet = ResourceManager.Instance.GetResource(BulletPrefab[0].gameObject).GetComponent<EnemyBullet>();
            bullet.pattern = BulletPattern[0].GetInstance();
            //bullet.pattern = BulletPattern;
            bullet.pattern.SetBullet(bullet.transform, null);
            bullet.transform.SetParent(null);
            bullet.transform.position = Owner.transform.position;
            bullet.SetAngle(new Vector3(0f, angle, 0f));
            angle += (360f / (6 + 3 * _diff));

        }

        yield return null;
    }

    public void Charge()
    {
    }

    public void Update()
    {
    }


}
