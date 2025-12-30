using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BatMove : IEnemyMove
{
    float curFlow = 5f;
    public bool isMoveDone = true;
    Transform Owner;

    float moveDist;
    Vector3 currentPos;
    Vector3 TrueDestinationPos;

    public BatMove() { }


    // Set Destination
    public void Move()
    {
        currentPos = Owner.position;
        Vector3 destinationPos = PlayerManager.Instance.GetPlayerTrs().position;

        (int, int) indexer = GameManager.Instance.EdgeIndexer(destinationPos); // col , row

        int centerCol = indexer.Item1;
        int centerRow = indexer.Item2;

        int minCol = Mathf.Clamp(centerCol - 4, 0, 16);
        int maxCol = Mathf.Clamp(centerCol + 4, 0, 16);
        int minRow = Mathf.Clamp(centerRow - 3, 0, 9);
        int maxRow = Mathf.Clamp(centerRow + 3, 0, 9);

        Vector3 randomPos = destinationPos + (Random.insideUnitSphere * Random.Range(5f, 8f));
        randomPos.x = Mathf.Clamp(randomPos.x, -7.5f, 7.5f);
        randomPos.z = Mathf.Clamp(randomPos.z, -3.5f, 3.5f);
        randomPos.y = 0f;

        TrueDestinationPos = randomPos;
        moveDist = 0f;
    }

    public void MoveUpdate()
    {
        if (moveDist != 1f)
        {
            moveDist += Time.deltaTime;
            if (moveDist > 1f) moveDist = 1f;
            Owner.transform.position = Vector3.Lerp(currentPos, TrueDestinationPos, moveDist);
        }


        if (moveDist == 1f)
        {
            curFlow -= Time.deltaTime;
            if (curFlow < 0f)
            {
                curFlow = 10f;
                if (Owner.GetComponent<Enemy>().CurrentState == EnemyBehaviour.CHARGE) return;
                if (Owner.GetComponent<Enemy>().CurrentState == EnemyBehaviour.STUNNED) return;
                isMoveDone = true;
                Owner.GetComponent<Enemy>().CurrentState = EnemyBehaviour.MOVE;
            }
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
