using UnityEngine;
using UnityEngine.UI;

public class StageScrollRestriction : MonoBehaviour
{
    [SerializeField] RectTransform ContentTrs;
    [SerializeField] RectTransform CompareTrs;

    private void Update()
    {
        if (ContentTrs.anchoredPosition.x >= 0f)
        {
            ContentTrs.anchoredPosition = new Vector2(0f, ContentTrs.anchoredPosition.y);
        }

        if (ContentTrs.anchoredPosition.x <= -(CompareTrs.rect.width - 1920f))
        {
            ContentTrs.anchoredPosition = new Vector2(-(CompareTrs.rect.width - 1920f), ContentTrs.anchoredPosition.y);
        }

    }

}