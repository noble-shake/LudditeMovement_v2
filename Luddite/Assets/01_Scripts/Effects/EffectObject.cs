using UnityEngine;
using System.Collections;

public class EffectObject : MonoBehaviour
{
    [SerializeField] ParticleSystem _particle;
    IEnumerator Effect()
    {
        _particle = GetComponent<ParticleSystem>();
        _particle.Play();
        yield return new WaitUntil(() => _particle.isPlaying == false);

        ResourceManager.Instance.EffectRetrieve(this.gameObject);
    }

    private void OnEnable()
    {
        StartCoroutine(Effect());
    }
}
