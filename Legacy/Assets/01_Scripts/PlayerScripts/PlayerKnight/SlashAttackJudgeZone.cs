using UnityEngine;

public class SlashAttackJudgeZone : MonoBehaviour
{
    // Sphere
    float curFlow = 0f;
    [SerializeField] private PlayerKnight knight;
    [SerializeField] private Collider coll;

    private void Start()
    {
        knight = GetComponentInParent<PlayerKnight>();
        coll = GetComponent<Collider>();
    }

    private void Update()
    {
        curFlow -= Time.deltaTime;
        if (curFlow < 0f)
        {
            curFlow = 0f;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (knight.ChargeCheck) return;

        if (other.CompareTag("Enemy") && knight.ActivatedCheck)
        {
            // coll.enabled = false;
            if (curFlow != 0f) return;
            curFlow = 3f;

            if (other.transform.position.x < transform.position.x)
            {
                knight.PlayerFlip(true);
            }
            else if (other.transform.position.x > transform.position.x)
            {
                knight.PlayerFlip(false);
            }

            float angle = Quaternion.FromToRotation(Vector3.left, other.transform.position - transform.position).eulerAngles.y;
            knight.SetSlashDirection(Quaternion.Euler(new Vector3(90f, angle, 0f)));
            knight.PlayNormalAttack();


        }
    }
}