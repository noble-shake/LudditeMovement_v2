using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class SlimeSpreadAttack : IEnemyAttack
{
    private List<EnemyBullet> BulletPrefab;
    private List<BulletScriptableObject> BulletPattern;
    private Transform Owner;

    public SlimeSpreadAttack() { }

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

        int _diff = (int)GameManager.Instance.difficulty; // 3 5 7 9
        Vector3 direction = PlayerManager.Instance.GetPlayerTrs().position - Owner.transform.position;
        direction = new Vector3(direction.x, 0f, direction.z);
        float angle = Vector3.SignedAngle(Vector3.forward, direction, Vector3.up);
        float tempAngle = angle;

        int halfIndex = (3 + 2 * _diff) / 2; // 1, 2, 3, 4
        angle -= halfIndex * 10f;

        for (int j = 0; j < 2; j++)
        {
            tempAngle = angle;
            for (int i = -halfIndex; i < (3 + 2 * _diff) - halfIndex; i++)
            {
                EnemyBullet bullet = ResourceManager.Instance.GetResource(BulletPrefab[0].gameObject).GetComponent<EnemyBullet>();
                bullet.pattern = BulletPattern[0].GetInstance();
                //bullet.pattern = BulletPattern;
                bullet.pattern.SetBullet(bullet.transform, null);
                bullet.transform.SetParent(null);
                bullet.transform.position = Owner.transform.position;
                bullet.SetAngle(new Vector3(0f, tempAngle, 0f));
                tempAngle += 10f;

            }
            yield return new WaitForSeconds(0.3f);
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
