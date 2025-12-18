using UnityEngine;
using UnityEngine.UI;

public class TextEffect : MonoBehaviour
{
    [SerializeField] CanvasGroup Canvas;
    private float CurFlow;

    private void Start()
    {
        Canvas = GetComponent<CanvasGroup>();
    }

    private void Update()
    {
        Effect();
    }

    private void Effect()
    {
        if (Canvas == null) return;
        CurFlow += Time.deltaTime;
        if (CurFlow > 1f) CurFlow = 0f;
        Canvas.alpha = CurFlow;
    }
}