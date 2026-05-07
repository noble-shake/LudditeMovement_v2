using System.Collections;
using UnityEngine;

public class CircleExpandEffect : MonoBehaviour
{
    private void OnEnable()
    {
        StartCoroutine(Effect());
    }

    private IEnumerator Effect()
    { 
        transform.localScale = Vector3.one;
        float CurFlow = 0f;

        while (CurFlow < 1f)
        {
            CurFlow += Time.deltaTime * 5f;
            if (CurFlow > 1f) CurFlow = 1f;
            transform.localScale = Vector3.Lerp(Vector3.one, Vector3.one * 8f, CurFlow);
            yield return null;
        }

        yield return new WaitForSeconds(1.25f);

        ResourceManager.Instance.EffectRetrieve(this.gameObject);

    }

    private void OnDisable()
    {
        transform.localScale = Vector3.one;
    }
}