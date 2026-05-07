using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MenuStageMapInfoCanvas : MonoBehaviour
{
    [SerializeField] public CanvasGroup UnknownCanvas;
    [SerializeField] TMP_Text StageName;
    [SerializeField] TMP_Text MemberRestrict;
    [SerializeField] TMP_Text EasyScore;
    [SerializeField] TMP_Text NormalScore;
    [SerializeField] TMP_Text HardScore;
    [SerializeField] TMP_Text NightmareScore;
    [SerializeField] TMP_Text CurrentPlayTime;

    public void SetUnclearCover(bool isOn)
    {
        UnknownCanvas.gameObject.SetActive(isOn);
    }

    public void Mapping(int order)
    {
        bool isLocked = LibraryManager.Instance.stageAnalyses[order].isLocked;
        if (isLocked == true) 
        {
            SetUnclearCover(true);
            return;
        } 

        SetUnclearCover(false);
        StageName.text = ResourceManager.Instance.GetMapData(order).StageName;
        MemberRestrict.text = ResourceManager.Instance.GetMapData(order).BatchMembers.ToString();
        EasyScore.text = LibraryManager.Instance.stageAnalyses[order].EasyScore.ToString("D12");
        NormalScore.text = LibraryManager.Instance.stageAnalyses[order].NormalScore.ToString("D12");
        HardScore.text = LibraryManager.Instance.stageAnalyses[order].HardScore.ToString("D12");
        NightmareScore.text = LibraryManager.Instance.stageAnalyses[order].NightmareScore.ToString("D12");

        float TimeScheduling = LibraryManager.Instance.stageAnalyses[order].PlayTime;
        CurrentPlayTime.text = $"{Mathf.FloorToInt(TimeScheduling / 600f).ToString("D2")}:{Mathf.FloorToInt(TimeScheduling / 60f).ToString("D2")}:{Mathf.FloorToInt(TimeScheduling % 60f).ToString("D2")}";
    }
}