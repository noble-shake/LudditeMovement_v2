using System.Collections;
using UnityEngine;
using UnityEngine.UI;


/*
 * Spawn Env
 * No Orb Interaction
 */

public class SpawnEnv : Environments
{
    [SerializeField] public int SpawnEntryID;
    [SerializeField] private bool isAllocated;
    [SerializeField] PlayerClassType AllocatedPlayer;
    [SerializeField] ParticleSystem PlayerAppearParticle;
    [SerializeField] public CanvasGroup AllocatedCanvas;
    [SerializeField] Image Portrait;

    public override void OrbInteracted()
    {
        return;
    }

    public void SetPlayerClass(PlayerClassType _class)
    {
        AllocatedPlayer = _class;
        if (_class == PlayerClassType.None)
        {
            Portrait.sprite = null;
        }
        else
        {
            Portrait.sprite = ResourceManager.Instance.GetPlayerInfo(_class).FacePortrait[0];
        }

    }

    public void AllocatedOnOff(bool isOn)
    {
        AllocatedCanvas.gameObject.SetActive(isOn);
    }

    public void SpawnCharacter()
    { 
        GameObject player = ResourceManager.Instance.GetPlayerResource(AllocatedPlayer);
        player.transform.position = transform.position;
        player.GetComponent<Player>().statusManager.StatusInit();
        player.GetComponent<Player>().GetSkillInstance();

    }
}