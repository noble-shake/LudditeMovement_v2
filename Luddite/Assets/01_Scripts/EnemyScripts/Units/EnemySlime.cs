using System.Collections;
using UnityEngine;

// 슬라임만 할 수 있는 거?
// 슬라임용 공격, 슬라임용 움직임

public class EnemySlime : EnemyGround, IEnemyBehavior
{
    [SerializeField] float interval;
    [SerializeField] float curFlow;
    [SerializeField] EnemyBullet BulletPrefab;
    [SerializeField] SlimeBullet BulletPattern;

    public void Attack()
    {
        /*
         * 슬라임 공격 : 6, 9, 12
         */
        int diff = (int)GameManager.Instance.difficulty;
        StartCoroutine(SlimeShot(diff));
    }

    IEnumerator SlimeShot(int _diff)
    {
        float angle = Quaternion.FromToRotation(Vector3.forward, Vector3.back).eulerAngles.z;
        for (int i = 0; i < 6 + 3 * _diff; i++)
        {
            EnemyBullet bullet = ResourceManager.Instance.GetResource(BulletPrefab.gameObject).GetComponent<EnemyBullet>();
            bullet.pattern = new SlimeBullet();
            //bullet.pattern = BulletPattern;
            bullet.pattern.SetBullet(bullet.transform, PlayerManager.Instance.GetPlayerTrs());
            bullet.transform.SetParent(null);
            bullet.transform.position = transform.position;
            bullet.transform.rotation = Quaternion.Euler(new Vector3(0f, angle, 0f));
            angle += (360f / (6 + 3 * _diff));

        }

        yield return null;
    }

    public void Charging()
    {
        throw new System.NotImplementedException();
    }

    public void Move()
    {
        throw new System.NotImplementedException();
    }

    protected override void Start()
    {
        base.Start();
        curFlow = interval;
    }

    private void Update()
    {
        curFlow -= Time.deltaTime;
        if (curFlow < 0f)
        {
            curFlow = interval;
            Attack();
        }
    }


}