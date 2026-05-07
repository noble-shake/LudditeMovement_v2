using UnityEngine;

public class TestBullet : MonoBehaviour
{
    [SerializeField] float Speed = 8.0f;
    [SerializeField] float curFlow = 4.0f;


    // Update is called once per frame
    void Update()
    {
        curFlow -= Time.deltaTime;
        if (curFlow < 0f) 
        {
            Destroy(gameObject);
        }

        transform.position += transform.forward * Time.deltaTime * Speed;
    }
}
