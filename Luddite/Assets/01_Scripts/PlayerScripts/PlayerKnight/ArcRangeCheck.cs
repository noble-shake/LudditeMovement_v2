using DTT.AreaOfEffectRegions;
using System.Collections.Generic;
using UnityEngine;

public class ArcRangeCheck : MonoBehaviour
{
    ArcRegion arcRegion;
    public List<Collider> enemyList;

    void Start()
    {
        arcRegion = GetComponent<ArcRegion>();
        enemyList = new List<Collider>();
    }

    public List<Collider> GetRangedTarget()
    {
        float left = arcRegion.LeftAngle;
        float right = arcRegion.RightAngle;

        for (int index = enemyList.Count - 1; index >= 0; index--)
        {
            Vector3 direction = enemyList[index].transform.position - transform.position;
            float angleRad = Mathf.Atan2(direction.z, direction.x);
            float angleDeg = angleRad * Mathf.Rad2Deg;
            if (angleDeg < 0f)
            {
                angleDeg += 360f;
            }

            float tempLeft = (360f - (left - 90f));
            float tempRight= (360f - (right - 90f));
            Debug.Log($"{angleDeg}, {tempLeft}, {tempRight}");
            if (tempRight < 0f)
            {
                if (angleDeg < tempLeft && angleDeg > 0f)
                {
                    Debug.Log("Inner");
                }
                else
                {
                    Debug.Log("Outer");
                    enemyList.RemoveAt(index);
                }
            }
            else
            {
                if (angleDeg < tempLeft && angleDeg > tempRight)
                {
                    Debug.Log("Inner");
                }
                else
                {
                    Debug.Log("Outer");
                    enemyList.RemoveAt(index);
                }
            }


        }

        List<Collider> targets = new List<Collider>(enemyList);
        enemyList.Clear();
        return targets;
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            if (enemyList.Contains(other) == false)
            {
                enemyList.Add(other);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            if (enemyList.Contains(other) == true)
            {
                enemyList.Remove(other);
            }
        }
    }
}
