using DTT.AreaOfEffectRegions;
using System.Collections.Generic;
using UnityEngine;

public class ArcRangeCheck : MonoBehaviour
{
    ArcRegion arcRegion;
    public List<Enemy> enemyList;

    void Start()
    {
        arcRegion = GetComponent<ArcRegion>();
        enemyList = new List<Enemy>();
    }

    public List<Enemy> GetRangedTarget()
    {
        float left = arcRegion.LeftAngle;
        float right = arcRegion.RightAngle;

        for (int index = enemyList.Count - 1; index >= 0; index--)
        {
            Vector3 direction = enemyList[index].transform.position - transform.position;
            float angleRad = Mathf.Atan2(direction.z, direction.x);
            float angleDeg = angleRad * Mathf.Rad2Deg;

            float tempLeft = (360f - (left - 90f));
            float tempRight= (360f - (right - 90f));
            Debug.Log($"{angleDeg}, {tempLeft}, {tempRight}");
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

        List<Enemy> targets = enemyList;
        enemyList.Clear();
        return targets;
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            if (enemyList.Contains(other.GetComponent<Enemy>()) == false)
            {
                enemyList.Add(other.GetComponent<Enemy>());
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            if (enemyList.Contains(other.GetComponent<Enemy>()) == true)
            {
                enemyList.Remove(other.GetComponent<Enemy>());
            }
        }
    }
}
