using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class SlimeSpreadAttack : IEnemyAttack
{
    private List<EnemyBullet> BulletPrefab;
    private List<BulletScriptableObject> BulletPattern;
    private Transform Owner;
    private bool isCharge;
    private int requireCycle = 3;
    private int currentCycle;
    private float ChargeTime = 10f;

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

    public IEnumerator Attack()
    {
        int _diff = (int)GameManager.Instance.difficulty; // 12, 16, 20, 24

        for (int i = 0; i < 12 + 4 * _diff; i++)
        {
            EnemyBullet bullet = ResourceManager.Instance.GetResource(BulletPrefab[0].gameObject).GetComponent<EnemyBullet>();
            bullet.pattern = BulletPattern[0].GetInstance();
            //bullet.pattern = BulletPattern;
            bullet.pattern.SetBullet(bullet.transform, null);
            bullet.transform.SetParent(null);
            bullet.transform.position = Owner.transform.position;
            bullet.SetAngle(new Vector3(0f, Random.Range(0f, 360f), 0f));
            yield return new WaitForEndOfFrame();
        }


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
