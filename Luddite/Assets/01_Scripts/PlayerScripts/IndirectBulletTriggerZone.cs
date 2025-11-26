using UnityEngine;

public class IndirectBulletTriggerZone : MonoBehaviour
{
    /*
     * 플레이어가 탄막에 대하여 충돌 체크를 할 필요는 없다.
     * 개별 탄막 별로 이 트리거 존을 체크 하도록 해야한다.
     */

    private PlayerOrb orb;

    private void Start() => orb = GetComponentInParent<PlayerOrb>();

    //private void OnTriggerEnter(Collider other)
    //{
    //    if (orb == null)
    //    {
    //        Debug.LogWarning("IndirectBulletTriggerZone : orb Not Assigned");
    //        return;
    //    }

    //    if (other.CompareTag("EnemyBullet"))
    //    {

    //    }
    //}
}