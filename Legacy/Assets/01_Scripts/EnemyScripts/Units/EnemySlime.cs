using System.Collections;
using UnityEngine;

// 슬라임만 할 수 있는 거?
// 슬라임용 공격, 슬라임용 움직임

public class EnemySlime : EnemyGround
{
    [SerializeField] float interval;
    [SerializeField] float curFlow;
    [SerializeField] EnemyBullet BulletPrefab;

    [SerializeField] SlimeBullet BulletPattern;

    //Skill Scriptable Object

    //public void Attack()
    //{
    //    /*
    //     * 슬라임 공격 : 6, 9, 12
    //     */
    //    int diff = (int)GameManager.Instance.difficulty;
    //    //StartCoroutine(SlimeShot(diff));
    //    StartCoroutine(AttackPattern.Attack());
    //    isAttackCheck = false;
    //    status.APValue-= status.MaxAPValue;
    //}

    //public void Charging()
    //{
    //    throw new System.NotImplementedException();
    //}

    //public void Move()
    //{
    //    StartCoroutine(MovePattern.Move());
    //}

    //protected override void Start()
    //{
    //    base.Start();
    //    curFlow = interval;
    //    AttackPattern = GameManager.Instance.attackScript.GetInstance();
    //    //AttackPattern = GameManager.Instance.enemyAttacks[0];
    //    AttackPattern.SetInit(transform, BulletPrefab);

    //    MovePattern = GameManager.Instance.moveScript.GetInstance();
    //    MovePattern.SetInit(transform);
    //}

    //protected override void Update()
    //{
    //    base.Update();

    //    if (isIdleCheck) return;
    //    if (isStunCheck) return;
    //    if (MovePattern.MoveDone() && isAttackCheck) Move();

    //    if (isAttackCheck == false) return;
    //    Attack();


    //}


}