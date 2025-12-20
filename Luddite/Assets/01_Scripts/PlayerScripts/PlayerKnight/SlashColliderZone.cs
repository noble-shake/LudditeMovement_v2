using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System;
using System.Runtime.InteropServices.WindowsRuntime;

// Knight Only.
public class SlashColliderZone : MonoBehaviour
{
    // 0f = West
    // 90f = North
    // 180f = East
    // 240f = South

    [SerializeField] private PlayerKnight knight;
    [SerializeField] private Collider[] colliders;
    [SerializeField] List<Collider> collisions = new List<Collider>();
    [SerializeField] GameObject ImpactEffect;
    private IEnumerator SlashEffect;
    private WaitForSeconds SlashDuration = new WaitForSeconds(0.125f);

    private void OnEnable()
    {
        if (GetComponentInParent<PlayerKnight>())
        {
            knight = GetComponentInParent<PlayerKnight>();
            knight.SlashAction += SlashOn;
        }

    }

    private void Start()
    {
        knight = GetComponentInParent<PlayerKnight>();
        colliders = GetComponents<Collider>();

    }

    private void OnDisable()
    {
        if (GetComponentInParent<PlayerKnight>())
        {
            knight = GetComponentInParent<PlayerKnight>();
            knight.SlashAction -= SlashOn;
        }
    }

    public void SlashOn()
    {
        SlashEffect = ColliderOnOff();
        StartCoroutine(SlashEffect);
    }

    IEnumerator ColliderOnOff()
    {
        foreach (Collider coll in colliders)
        {
            coll.enabled = true;
        }
        yield return SlashDuration;
        TriggerOn();
        foreach (Collider coll in colliders)
        {
            coll.enabled = false;
        }
    }

    public void TriggerOn()
    {
        if (colliders.Length > 0)
        {
            HitSpeedChange();
        }

        // Transit To Player
        foreach (Collider target in collisions)
        {
            Vector3 hitPoint = target.ClosestPoint(transform.position);
            GameObject Impact = ResourceManager.Instance.GetResource(ImpactEffect);
            Impact.transform.position = hitPoint;
            target.GetComponent<Rigidbody>().AddForce((target.transform.position - knight.transform.position).normalized * 0.5f, ForceMode.Impulse);
            target.GetComponent<Enemy>().OnHit(knight.statusManager.AttackValue);
        }
        collisions.Clear();
    }

    async Awaitable HitSpeedChange()
    {
        knight.GetAnim().speed = 0f;
        float CurFlow = 0f;
        while (CurFlow < 1f)
        { 
            CurFlow += Time.unscaledDeltaTime * 2f;
            if (CurFlow > 1f) CurFlow = 1f;
            knight.GetAnim().speed = Mathf.Lerp(0f, 1f, CurFlow);
            await Awaitable.NextFrameAsync();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            if (collisions.Contains(other)) return;

            collisions.Add(other);
        }
    }
}