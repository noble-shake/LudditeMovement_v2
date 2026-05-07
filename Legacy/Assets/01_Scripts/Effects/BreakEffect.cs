using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BreakEffect : MonoBehaviour
{
    CanvasGroup canvas;
    IEnumerator CanvasEffect;


    private void Awake()
    {
        canvas = GetComponent<CanvasGroup>();
    }

    private void OnEnable()
    {
        if (CanvasEffect != null) StopCoroutine(CanvasEffect);
        CanvasEffect = StartEffect();
        StartCoroutine(CanvasEffect);
    }

    private IEnumerator StartEffect()
    {


        float elapsed = 0.0f;

        yield return null;
        Vector3 originPos = transform.position;

        while (elapsed < 0.8f)
        {
            // 랜덤한 위치값 생성
            float x = Random.Range(-1f, 1f) * 0.2f;
            float z = Random.Range(-1f, 1f) * 0.2f;

            transform.position = new Vector3(originPos.x + x, 0f, originPos.z+ z);
            elapsed += Time.deltaTime;
            yield return null;
        }

        yield return new WaitForSeconds(1.5f);

        ResourceManager.Instance.EffectRetrieve(this.gameObject);
    }
}