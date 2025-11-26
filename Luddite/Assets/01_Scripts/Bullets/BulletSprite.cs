using UnityEngine;

public class BulletSprite : MonoBehaviour
{
    private bool isGraze;
    private EnemyBullet enemyBullet;
    private float interval;

    private void Start()
    {
        enemyBullet = GetComponentInParent<EnemyBullet>();
        interval = 1f;
        isGraze = false;
    }

    private void Update()
    {
        interval -= Time.deltaTime;
        if (interval < 0f)
        {
            interval = 1f;
            isGraze = false;
        } 

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Orb"))
        {
            Debug.Log("HIT!!");
            enemyBullet.OnHit(other.gameObject);
        }
    }


    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("OrbIndirect") && isGraze == false)
        {
            Debug.Log("GRAZE!!");
            isGraze = true;
            // Drop Soul
            // Graze up
            // Score Up
        }
    }


}