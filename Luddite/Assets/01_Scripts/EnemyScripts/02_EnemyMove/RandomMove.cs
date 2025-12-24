using System.Collections;
using UnityEngine;

public class RandomMove : IEnemyMove
{
    float curFlow = 5f;
    public bool isMoveDone = true;
    Transform Owner;

    public RandomMove() { }

    public void Move()
    {
        Vector3 currentPos = Owner.position;
        Vector3 nextDirection;
        while (true)
        {
            int direction = Random.Range(0, 4);

            switch (direction)
            {
                default:
                case 0:
                    nextDirection = Vector3.forward;

                    break;
                case 1:
                    nextDirection = Vector3.back;
                    break;
                case 2:
                    nextDirection = Vector3.right;
                    break;
                case 3:
                    nextDirection = Vector3.left;
                    break;
            }

            if (currentPos.x + nextDirection.x > -8.5f && currentPos.x + nextDirection.x < 8.5f && currentPos.z + nextDirection.z > -4.5f && currentPos.z + nextDirection.z < 4.5f)
            {
                break;
            }
        }

        Owner.GetComponent<Rigidbody>().AddForce(nextDirection * 2f, ForceMode.Impulse);
    }

    public void MoveUpdate()
    {
        curFlow -= Time.deltaTime;
        if (curFlow < 0f)
        {
            curFlow = 5f;
            if (Owner.GetComponent<Enemy>().CurrentState == EnemyBehaviour.CHARGE) return;
            if (Owner.GetComponent<Enemy>().CurrentState == EnemyBehaviour.STUNNED) return;
            isMoveDone = true;
            Owner.GetComponent<Enemy>().CurrentState = EnemyBehaviour.MOVE;
        }

    }

    public bool MoveDone()
    {
        return isMoveDone;
    }

    public void SetInit(Transform _trs)
    {
        Owner = _trs;
    }

    public void SetInit(Transform _trs, Transform _target)
    {
        throw new System.NotImplementedException();
    }

    public void SetInit()
    {
        throw new System.NotImplementedException();
    }
}
