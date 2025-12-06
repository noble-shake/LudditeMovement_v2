using UnityEngine;
using System.Collections;

public class EffectObject : MonoBehaviour
{
    [SerializeField] ParticleSystem _particle;
    IEnumerator Start()
    {
        _particle = GetComponent<ParticleSystem>();
        _particle.Play();
        yield return new WaitUntil(() => _particle.isPlaying == false);

        ResourceManager.Instance.ResourceRetrieve(this.gameObject);
    }
}
