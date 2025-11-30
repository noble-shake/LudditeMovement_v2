using System.Collections;
using UnityEngine;

public class RandomMove : IEnemyMove
{
    public bool isMoveDone = true;
    Transform Owner;

    public RandomMove() { }

    public IEnumerator Move()
    {
        isMoveDone = false;
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

        while (Vector3.Distance(Owner.position, currentPos + nextDirection) > 0.01f)
        {
            Owner.transform.position = Vector3.MoveTowards(Owner.position, currentPos + nextDirection, Time.deltaTime * 2f);
            yield return null;
        }
        Owner.position = currentPos + nextDirection;
        yield return new WaitForSeconds(2f);
        isMoveDone = true;


    }

    public void MoveUpdate()
    {

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
