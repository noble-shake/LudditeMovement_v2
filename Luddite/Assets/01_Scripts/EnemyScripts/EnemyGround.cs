using UnityEngine;

public class EnemyGround : Enemy
{


    protected virtual void Start()
    {
        rigid = GetComponent<Rigidbody>();
        enemyType = EnemyType.Ground;
    }

    protected virtual void Update() 
    {
        base.Update();
        // rigid.linearVelocity = Vector3.Lerp(rigid.linearVelocity, Vector3.zero, Time.deltaTime * 3f);
    }
}