using System.Collections;
using UnityEngine;

public interface IEnemyMove
{
    public void SetInit(Transform _trs);
    public void SetInit(Transform _trs, Transform _target);
    public void SetInit();

    public void MoveUpdate();
    public bool MoveDone();

    public IEnumerator Move();
}