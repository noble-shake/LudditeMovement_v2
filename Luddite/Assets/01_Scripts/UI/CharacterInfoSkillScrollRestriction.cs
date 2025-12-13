using UnityEngine;
using UnityEngine.UI;

public class CharacterInfoSkillScrollRestriction : MonoBehaviour
{
    [SerializeField] private float Restrict = -1024;
    [SerializeField] private RectTransform ScrollImage;

    private void Update()
    {
        if (ScrollImage.anchoredPosition.x > 0f)
        {
            ScrollImage.anchoredPosition = new Vector2(0f, ScrollImage.anchoredPosition.y);
        }

        if (ScrollImage.anchoredPosition.x < Restrict)
        {
            ScrollImage.anchoredPosition = new Vector2(Restrict, ScrollImage.anchoredPosition.y);
        }
    }
}