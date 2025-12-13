using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterInfoDescription : CharacterInfoIcon
{
    [SerializeField] private CanvasGroup InfoCanvas;
    [SerializeField] TMP_Text Description;
    [SerializeField] TMP_Text Memory1;
    [SerializeField] TMP_Text Memory2;
    [SerializeField] TMP_Text Memory3;

    public override void OnIconClicked()
    {
        base.OnIconClicked();
        InfoCanvas.gameObject.SetActive(true);
        PlayerAnalysis data = LibraryManager.Instance.playerAnalyses[CurrentClassType];
        PlayerScriptableObject playerData = ResourceManager.Instance.GetPlayerInfo(CurrentClassType);
        Description.text = playerData.Description;
        if (data.Memory1)
        {
            Memory1.text = playerData.Memory1;
        }
        else
        {
            Memory1.text = "Unlocked At LV.05";
        }

        if (data.Memory2)
        {
            Memory2.text = playerData.Memory2;
        }
        else
        {
            Memory2.text = "Unlocked At LV.10";
        }
        if (data.Memory3)
        {
            Memory3.text = playerData.Memory3;
        }
        else
        {
            Memory3.text = "Unlocked At LV.15";
        }
    }

    public override void DisSelect()
    {
        base.DisSelect();
        InfoCanvas.gameObject.SetActive(false);
    }
}