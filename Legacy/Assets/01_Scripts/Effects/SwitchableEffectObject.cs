using UnityEngine;
using System.Collections;

public class SwitchableEffectObject : MonoBehaviour
{
    [SerializeField] ParticleSystem _particle;
    private void OnEnable()
    {
        _particle = GetComponent<ParticleSystem>();
        _particle.Play();
    }

    public void ResourceRetrieve()
    {
        ResourceManager.Instance.EffectRetrieve(this.gameObject);
    }
}
