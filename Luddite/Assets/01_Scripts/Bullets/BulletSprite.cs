using UnityEngine;

public class BulletSprite : MonoBehaviour
{
    private bool isGraze = false;
    private bool isHitPlayer = false;
    private bool isOrb = false;
    private EnemyBullet enemyBullet;
    private float GrazeDelay;
    private float HitDelay;
    private float HitOrbDelay;

    private void Start()
    {
        enemyBullet = GetComponentInParent<EnemyBullet>();
        GrazeDelay = 1f;
        HitDelay = 1f;
        HitOrbDelay = 0.25f;
    }

    private void Update()
    {
        GrazeDelay -= Time.deltaTime;
        HitDelay -= Time.deltaTime;
        HitOrbDelay -= Time.deltaTime;

        if (GrazeDelay < 0f)
        {
            GrazeDelay = 1f;
            isGraze = false;
        }

        if (HitDelay < 0f)
        {
            HitDelay = 1f;
            isHitPlayer = false;
        }

        if (HitOrbDelay < 0f)
        {
            HitOrbDelay = 0.25f;
            isOrb = false;
        }

    }

    //private void OnTriggerEnter(Collider other)
    //{

    //}


    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("OrbIndirect") && isGraze == false)
        {
            //Debug.Log("GRAZE!!");
            isGraze = true;
            SoulItem soul = ResourceManager.Instance.GetSoulItem();
            soul.SoulValue = Random.Range(1f, 2f);
            soul.transform.position = this.transform.position;
            // Drop Soul
            // Graze up
            // Score Up
        }
        
        if (other.CompareTag("Orb") && isOrb == false)
        {
            isOrb= true;
            enemyBullet.OnHit(other.gameObject, true);
        }
        
        if (other.CompareTag("Player") && isHitPlayer == false)
        {
            isHitPlayer = true;
            enemyBullet.OnHit(other.gameObject, false);
            other.GetComponent<Rigidbody>().AddForce((enemyBullet.transform.position - other.transform.position).normalized *0.25f, ForceMode.Impulse);
            other.GetComponent<Player>().OnHit();
        }
    }


}