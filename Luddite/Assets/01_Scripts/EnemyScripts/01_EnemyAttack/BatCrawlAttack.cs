using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class BatCrawlAttack : IEnemyAttack
{
    private List<EnemyBullet> BulletPrefab;
    private List<BulletScriptableObject> BulletPattern;
    private Transform Owner;
    private bool isCharge;
    private int requireCycle = 3;
    private int currentCycle;
    private float ChargeTime = 10f;

    public BatCrawlAttack() { }

    public void GetNeeds(List<EnemyBullet> _bullets, List<BulletScriptableObject> _bulletSetting)
    {
        BulletPrefab = _bullets;
        BulletPattern = _bulletSetting;
    }

    public void SetInit(Transform _Owner)
    {
        Owner = _Owner;
    }

    public IEnumerator Attack()
    {
        int _diff = (int)GameManager.Instance.difficulty; // 12, 16, 20, 24
        EnemyBullet bullet1 = ResourceManager.Instance.GetResource(BulletPrefab[0].gameObject).GetComponent<EnemyBullet>();
        bullet1.pattern = BulletPattern[0].GetInstance();
        bullet1.transform.position = Owner.transform.position;
        bullet1.transform.SetParent(null);
        bullet1.pattern.SetBullet(bullet1.transform, null);
        bullet1.transform.rotation = Quaternion.Euler(Vector3.zero);

        EnemyBullet bullet2 = ResourceManager.Instance.GetResource(BulletPrefab[0].gameObject).GetComponent<EnemyBullet>();
        bullet2.pattern = BulletPattern[0].GetInstance();
        bullet2.transform.SetParent(null);
        bullet2.transform.position = Owner.transform.position;
        bullet2.pattern.SetBullet(bullet2.transform, bullet2.transform);

        bullet2.transform.rotation = Quaternion.Euler(Vector3.zero);


        yield return null;
    }

    public void Charge()
    {
        currentCycle = requireCycle++;
        isCharge = true;
        Enemy enemy = Owner.GetComponent<Enemy>();
        GameObject circle = ResourceManager.Instance.GetEffectResource("CircleExpandEffect");
        circle.transform.position = Owner.position;
        enemy.ChargeCycle.text = currentCycle.ToString();
        enemy.ChargeProgress.fillAmount = 0f;
        enemy.ChargeUI.alpha = 1f;
        enemy.ChargeUI.gameObject.SetActive(true);
        ChargeTime = 10f;
    }

    public void Interupt()
    {
        if (isCharge == false) return;

        currentCycle--;
        Enemy enemy = Owner.GetComponent<Enemy>();
        enemy.ChargeCycle.text = currentCycle.ToString();
        if (currentCycle <= 0)
        {
            enemy.SetStunned(10f);
            isCharge = false;
            currentCycle = requireCycle;
            enemy.AttackIndex++;
            if (enemy.AttackIndex >= enemy.AttackPattern.Count) enemy.AttackIndex = 0;
        }

    }

    public void Update()
    {
        if(isCharge == false) return;

        ChargeTime -= Time.deltaTime;
        if (ChargeTime < 0f)
        {
            ChargeTime = 0f;
        }
        Enemy enemy = Owner.GetComponent<Enemy>();
        enemy.ChargeProgress.fillAmount = 1 - (ChargeTime/10f);

        if (ChargeTime == 0f)
        {
            isCharge = false;
            enemy.CurrentState = EnemyBehaviour.ATTACK;
            enemy.ChargeCycle.text = requireCycle.ToString();
            enemy.ChargeProgress.fillAmount = 0f;
            enemy.ChargeUI.alpha = 0f;
            enemy.ChargeUI.gameObject.SetActive(false);
        }

    }


}
