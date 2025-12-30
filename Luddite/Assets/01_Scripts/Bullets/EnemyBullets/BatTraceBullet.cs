using System;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class BatTraceBullet : IEnemyBullet
{
    float curFlow = 10;
    float Speed;
    private Transform ShootTrs;
    private Transform TargetTrs;

    public BatTraceBullet() { }

    public void SetBullet(Transform _ShootTrs, Transform _TargetTrs)
    {
        ShootTrs = _ShootTrs;
        TargetTrs = _TargetTrs;
    }   

    public void Update()
    {
        Vector3 StartPos = ShootTrs.position;
        Vector3 TargetPos = TargetTrs.position;

        Vector2 targetDir = TargetPos - StartPos;
        Vector3 crossVec = Vector3.Cross(ShootTrs.transform.forward, targetDir);
        float inner = Vector3.Dot(Vector3.forward, crossVec);
        float addAngle = inner > 0 ? 10f * Time.fixedDeltaTime : -10f* Time.fixedDeltaTime;
        float saveAngle = addAngle + ShootTrs.rotation.eulerAngles.y;
        ShootTrs.rotation = Quaternion.Euler(0, saveAngle, 0);

        float moveDirAngle = ShootTrs.rotation.eulerAngles.y * Mathf.Deg2Rad;
        ShootTrs.position = new Vector3(Mathf.Cos(moveDirAngle), 0f, Mathf.Sin(moveDirAngle));

    }
}