using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class DropItem : ItemScript
{
    private bool isPlayerTriggered;
    private IEnumerator ItemCollecter;
    private Rigidbody rigid;
    private Vector3 AlignVector;
    private Vector3 GravityVector = new Vector3(0f, 0f, Physics.gravity.y);
    private float stableValue = 0f;


    protected virtual void Start()
    {
        rigid = GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        isPlayerTriggered = false;
        GetComponent<Collider>().enabled = false;
        stableValue = 0f;
        if(rigid == null) rigid = GetComponent<Rigidbody>();
        rigid.linearVelocity = Vector3.zero;
        InitEffect();
        Invoke("EnableCollider", 0.5f);
    }

    private void EnableCollider()
    {
        isPlayerTriggered = false;
        GetComponent<Collider>().enabled = true;
    }

    public virtual void SetSprite(Sprite _sprite){}

    public void InitEffect()
    {
        AlignVector.x = Random.Range(-3f, 3f);
        AlignVector.z = Random.Range(2f, 5f);

        GetComponent<Rigidbody>().AddForce(AlignVector, ForceMode.Impulse);
    }

    public virtual void ItemEarned()
    {
        if(ItemCollecter != null) StopCoroutine(ItemCollecter);
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.CompareTag("OrbAbsorber") && isPlayerTriggered == false)
        {
            isPlayerTriggered = true;
            //if (CollectionTrigger == true) return;
            //CollectionTrigger = true;
            GetComponent<Collider>().enabled = false;
            ItemCollecter = ItemCollection();
            StartCoroutine(ItemCollecter);
        }
    }

    IEnumerator ItemCollection()
    {
        float e = 2.71828f;
        float speed = 0f;
        float ActualSpeed = 0f;
        while (true)
        {
            speed += Time.deltaTime;
            ActualSpeed = Mathf.Log(e, speed / 3f);
            transform.position = Vector3.MoveTowards(transform.position, PlayerManager.Instance.GetPlayerTrs().position, speed);

            yield return null;

            if (Vector3.Distance(transform.position, PlayerManager.Instance.GetPlayerTrs().position) < 0.1f)
            {
                break;
            }
        }

        ItemEarned();
    }


    private void FixedUpdate()
    {

        if (isPlayerTriggered) return;
        stableValue += Time.fixedDeltaTime / 10f;
        if(stableValue > 1f) stableValue = 1f;
        rigid.linearVelocity = Vector3.Lerp(rigid.linearVelocity, GravityVector / 10f, stableValue);
        //transform.position += GravityVector * ;
    }
}