using System.Collections;
using UnityEngine;
using static UnityEngine.ParticleSystem;


/*
 * Spawn Env
 * No Orb Interaction
 */

public class SpawnEnv : Environments
{
    [SerializeField] private bool isAllocated;
    [SerializeField] PlayerClassType AllocatedPlayer;
    [SerializeField] ParticleSystem PlayerAppearParticle;
    



    protected IEnumerator Start()
    {
        yield return null;
        SpawnCharacter();
    }

    public override void OrbInteracted()
    {
        return;
    }

    public void SetPlayerClass(PlayerClassType _class)
    {
        AllocatedPlayer = _class;
    }

    public void SpawnCharacter()
    { 

        GameObject player = ResourceManager.Instance.GetPlayerResource(AllocatedPlayer);
        player.transform.position = transform.position;

    }
}