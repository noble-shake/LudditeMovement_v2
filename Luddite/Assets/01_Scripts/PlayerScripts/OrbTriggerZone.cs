using System.Runtime.CompilerServices;
using UnityEngine;

public class OrbTriggerZone : MonoBehaviour
{
    Player player;

    private void Start()
    {
        player = GetComponentInParent<Player>();
    }

    private void OnTriggerEnter(Collider other)
    {
        
    }

    private void OnTriggerStay(Collider other)
    {
        if (player == null)
        {
            Debug.LogWarning("OrbTriggerZone : Player Not Assigned");
            return;
        }

        if(other.CompareTag("Orb"))
        {
            player.MatLitResponseOff();

        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (player == null)
        {
            Debug.LogWarning("OrbTriggerZone : Player Not Assigned");
            return;
        }

        if (other.CompareTag("Orb"))
        {
            player.MatLitResponseOn();

        }
    }
}