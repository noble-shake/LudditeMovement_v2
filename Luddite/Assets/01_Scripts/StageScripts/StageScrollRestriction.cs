using UnityEngine;
using UnityEngine.UI;

public class StageScrollRestriction : MonoBehaviour
{
    [SerializeField] RectTransform ContentTrs;

    private void Update()
    {
        if (ContentTrs.anchoredPosition.x >= 0f)
        {
            ContentTrs.anchoredPosition = new Vector2(0f, ContentTrs.anchoredPosition.y);
        }

    }

}