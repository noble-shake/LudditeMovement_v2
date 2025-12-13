using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MenuStageMapInfoCanvas : MonoBehaviour
{
    [SerializeField] public RectTransform rectTransform;
    [SerializeField] public CanvasGroup UnknownCanvas;
    [SerializeField] TMP_Text StageName;
    [SerializeField] TMP_Text MemberRestrict;
    [SerializeField] TMP_Text CurrentScore;
    [SerializeField] TMP_Text CurrentPlayTime;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }
}