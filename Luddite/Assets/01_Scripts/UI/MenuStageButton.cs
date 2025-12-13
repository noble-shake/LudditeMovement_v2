using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MenuStageButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private int StageID;
    // [SerializeField] private MapData _mapData;
    [SerializeField] public Button StageButton;
    [SerializeField] private Image ButtonBackground;
    [SerializeField] private Image ButtonIcon;
    [SerializeField] private Image ClearImage;
    [SerializeField] private CanvasGroup ClearCheckCanvas;

    public int StageIDValue { get { return StageID; } set { StageID = value; } }

    // public MapData MapDataValue { get { return _mapData; } set { _mapData = value; } }

    public void MenuClicked()
    {
        StageAnalysis data = LibraryManager.Instance.stageAnalyses[StageID];
        if (data.isLocked == false) return;

        ButtonBackground.color = Color.orange;
        ButtonIcon.color = Color.white;

    }

    public void MenuClearAction()
    {
        StageAnalysis data = LibraryManager.Instance.stageAnalyses[StageID];
        if (data.isLocked == false)
        {
            ButtonBackground.color = Color.black;
            ButtonIcon.color = Color.white;
            return;
        }
        else
        {
            if (data.isCleared)
            {
                ButtonBackground.color = Color.aliceBlue;
                ButtonIcon.color = Color.white;
                return;
            }
            else
            {
                ButtonBackground.color = Color.white;
                ButtonIcon.color = Color.white;
                return;
            }
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {

    }

    public void OnPointerEnter(PointerEventData eventData)
    {

    }
}