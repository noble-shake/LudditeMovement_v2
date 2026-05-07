using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class BatTraceShot : IEnemyAttack
{
    private List<EnemyBullet> BulletPrefab;
    private List<BulletScriptableObject> BulletPattern;
    private Transform Owner;

    public BatTraceShot() { }

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
        // 120 angle => -60 ~ 60 

        Vector3 direction = PlayerManager.Instance.GetPlayerTrs().position - Owner.transform.position;
        direction = new Vector3(direction.x, 0f, direction.z);
        float angle = Vector3.SignedAngle(Vector3.forward, direction, Vector3.up);
        float tempAngle = angle;

        EnemyBullet bullet = ResourceManager.Instance.GetResource(BulletPrefab[0].gameObject).GetComponent<EnemyBullet>();
        bullet.pattern = BulletPattern[0].GetInstance();
        //bullet.pattern = BulletPattern;
        bullet.pattern.SetBullet(bullet.transform, PlayerManager.Instance.GetPlayerTrs());
        bullet.transform.SetParent(null);
        bullet.transform.position = Owner.transform.position;
        bullet.SetAngle(new Vector3(0f, tempAngle, 0f));

        yield return null;
    }

    public void Charge()
    {
    }

    public void Update()
    {
    }

    public void Interupt()
    {
        throw new System.NotImplementedException();
    }
}
