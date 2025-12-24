using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEnemyAttack
{
    public void GetNeeds(List<EnemyBullet> _bullets, List<BulletScriptableObject> _enemyAttack);
    public void SetInit(Transform _trs);

    public void Shot();

    public IEnumerator Attack();
    public void Charge();

    public void Update();



}