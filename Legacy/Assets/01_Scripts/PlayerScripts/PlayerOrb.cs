using UnityEngine;

public class PlayerOrb : MonoBehaviour
{
    [SerializeField] private ParticleSystem EngageParticle;
    [SerializeField] private ParticleSystem LostParticle;

    private void OnEnable()
    {
        // EngageParticle.Play();
    }

    public void LostSoul()
    { 
        // LostParticle.Play();
    }

    // Circle Collider로부터 값을 받아온다.
    private void OnTriggerEnter(Collider other)
    {
        
    }

    private void OnTriggerStay(Collider other)
    {
        
    }

    private void OnTriggerExit(Collider other)
    {
        
    }
}
