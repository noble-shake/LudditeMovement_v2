using UnityEngine;

public class EnemyGround : Enemy
{
    protected bool isAttack;
    protected bool isMove;
    protected bool isCharging;

    protected virtual void Start()
    {
        
        enemyType = EnemyType.Ground;
    }
}