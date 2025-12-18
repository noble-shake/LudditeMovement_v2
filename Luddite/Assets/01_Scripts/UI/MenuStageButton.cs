using TMPro;
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
    [SerializeField] private TMP_Text ClearCheckName;
    [SerializeField] private TMP_Text ClearCheckState;

    public int StageIDValue { get { return StageID; } set { StageID = value; } }

    // public MapData MapDataValue { get { return _mapData; } set { _mapData = value; } }
    public void SetClearState()
    {
        ClearCheckName.text = ResourceManager.Instance.GetMapData(StageID).StageName;

        if (LibraryManager.Instance.stageAnalyses[StageID].isLocked == false)
        {
            ClearImage.gameObject.SetActive(true);
            ClearCheckState.color = Color.white;
            if (LibraryManager.Instance.stageAnalyses[StageID].isCleared)
            {
                ClearCheckState.text = "클리어 했습니다.";
            }
            else
            {
                ClearCheckState.text = "도전 할 수 있음";
            }

        }
        else
        {
            ClearCheckState.color = Color.red;
            ClearCheckState.text = "* 아직 열리지 않았습니다.";
        }
    }


    public bool MenuClicked()
    {
        StageAnalysis data = LibraryManager.Instance.stageAnalyses[StageID];
        if (data.isLocked == true) return false;

        ButtonBackground.color = Color.orange;
        ButtonIcon.color = Color.white;
        return true;
    }

    public void MenuClearAction()
    {
        StageAnalysis data = LibraryManager.Instance.stageAnalyses[StageID];
        if (data.isLocked == true)
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
        ClearCheckCanvas.gameObject.SetActive(false);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        ClearCheckCanvas.gameObject.SetActive(true);
    }
}