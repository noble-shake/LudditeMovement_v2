using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterInfoLevel: CharacterInfoIcon
{
    [SerializeField] private CanvasGroup LevelCanvas;


    public override void OnIconClicked()
    {
        base.OnIconClicked();
        LevelCanvas.gameObject.SetActive(true);
    }

    public override void DisSelect()
    {
        base.DisSelect();
        LevelCanvas.gameObject.SetActive(false);
    }
}