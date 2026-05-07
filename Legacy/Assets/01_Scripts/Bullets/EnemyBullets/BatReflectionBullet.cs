using System;
using UnityEngine;

public class BatReflectionBullet : IEnemyBullet
{
    float curFlow = 10;
    float Speed;
    private Transform ShootTrs;
    private Transform TargetTrs;

    public BatReflectionBullet() { }

    public void SetBullet(Transform _ShootTrs, Transform _TargetTrs)
    {
        ShootTrs = _ShootTrs;
        TargetTrs = _TargetTrs;
    }   

    public void Update()
    {
        Vector3 StartPos = ShootTrs.transform.position;
        Vector3 DestinationPos = ShootTrs.transform.position + ShootTrs.forward * Time.deltaTime * Speed;
        Vector3 normVector;
        Vector3 relflector;
        Speed = 2.5f + (int)GameManager.Instance.difficulty; // 3 5 7 9
        if (DestinationPos.x < -8.5f)
        {
            normVector = Vector3.right;
            relflector = Vector3.Reflect(DestinationPos - StartPos, normVector);
            ShootTrs.transform.rotation = Quaternion.LookRotation(relflector);
            ShootTrs.transform.position += ShootTrs.forward * Time.deltaTime * Speed;
            Speed += 0.5f;
            curFlow -= Time.deltaTime;
            if (curFlow < 0f)
            {
                ResourceManager.Instance.ResourceRetrieve(ShootTrs.gameObject);
            }
        }
        else if (DestinationPos.x > 8.5f)
        {
            normVector = Vector3.left;
            relflector = Vector3.Reflect(DestinationPos - StartPos, normVector);
            ShootTrs.transform.rotation = Quaternion.LookRotation(relflector);
            ShootTrs.transform.position += ShootTrs.forward * Time.deltaTime * Speed;
            Speed += 0.5f;
            curFlow -= Time.deltaTime;
            if (curFlow < 0f)
            {
                ResourceManager.Instance.ResourceRetrieve(ShootTrs.gameObject);
            }
        }
        else if (DestinationPos.z < -4.5f)
        {
            normVector = Vector3.forward;
            relflector = Vector3.Reflect(DestinationPos - StartPos, normVector);
            ShootTrs.transform.rotation = Quaternion.LookRotation(relflector);
            ShootTrs.transform.position += ShootTrs.forward * Time.deltaTime * Speed;
            Speed += 0.5f;
            curFlow -= Time.deltaTime;
            if (curFlow < 0f)
            {
                ResourceManager.Instance.ResourceRetrieve(ShootTrs.gameObject);
            }
        }
        else if (DestinationPos.z > 4.5f)
        {
            normVector = Vector3.back;
            relflector = Vector3.Reflect(DestinationPos - StartPos, normVector);
            ShootTrs.transform.rotation = Quaternion.LookRotation(relflector);
            ShootTrs.transform.position += ShootTrs.forward * Time.deltaTime * Speed;
            Speed += 0.5f;
            curFlow -= Time.deltaTime;
            if (curFlow < 0f)
            {
                ResourceManager.Instance.ResourceRetrieve(ShootTrs.gameObject);
            }
        }
        else
        {
            ShootTrs.transform.position += ShootTrs.forward * Time.deltaTime * Speed;
            
            curFlow -= Time.deltaTime;
            if (curFlow < 0f)
            {
                ResourceManager.Instance.ResourceRetrieve(ShootTrs.gameObject);
            }
            return;
        }




    }
}