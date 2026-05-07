using System.Collections;
using UnityEngine;

public class EnemyTests : MonoBehaviour
{
    [SerializeField] float CoolTime = 2.0f;
    [SerializeField] TestBullet testBullet;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        CoolTime -= Time.deltaTime; 
        if (CoolTime < 0f)
        {
            CoolTime = 2.0f;

            Transform player = PlayerManager.Instance.GetPlayerTrs();
            if (player == null) return;

            StartCoroutine(Shot());

        }
    }

    IEnumerator Shot()
    {
        for (int i = 0; i < 3; i++)
        {
            Transform player = PlayerManager.Instance.GetPlayerTrs();
            if (player == null) continue;

            TestBullet bullet = Instantiate<TestBullet>(testBullet);
            bullet.transform.position = transform.position;
            bullet.transform.rotation = Quaternion.LookRotation(player.position - bullet.transform.position);
            yield return new WaitForSeconds(0.2f);
        }
    }
}
