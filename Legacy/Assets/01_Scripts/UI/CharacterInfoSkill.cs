using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class CharacterInfoSkill: CharacterInfoIcon
{
    [SerializeField] private CanvasGroup SkillCanvas;
    [SerializeField] private SkillTreeMap KnightCanvas;
    [SerializeField] private SkillTreeMap ArcherCanvas;
    [SerializeField] private SkillTreeMap SpellCasterCanvas;
    [SerializeField] private SkillTreeMap BufferCanvas;
    [SerializeField] private SkillTreeMap SoulMasterCanvas;
    [SerializeField] private SkillTreeMap ThiefCanvas;
    private SkillTreeMap CurrentSkillTreeMap;

    public override void OnIconClicked()
    {
        base.OnIconClicked();
        SkillCanvas.gameObject.SetActive(true);
        if (CurrentSkillTreeMap != null)
        {
            CurrentSkillTreeMap.gameObject.SetActive(false);
        }
        switch (CurrentClassType)
        {
            default:
            case PlayerClassType.Knight:
                CurrentSkillTreeMap = KnightCanvas;
                break;
            case PlayerClassType.Archer:
                CurrentSkillTreeMap = ArcherCanvas;
                break;
            case PlayerClassType.SpellMaster:
                CurrentSkillTreeMap = SpellCasterCanvas;
                break;
            case PlayerClassType.Buffer:
                CurrentSkillTreeMap = BufferCanvas;
                break;
            case PlayerClassType.SoulMaster:
                CurrentSkillTreeMap = SoulMasterCanvas;
                break;
            case PlayerClassType.Thief:
                CurrentSkillTreeMap = ThiefCanvas;
                break;
        }

        CurrentSkillTreeMap.gameObject.SetActive(true);

        PlayerAnalysis data = LibraryManager.Instance.playerAnalyses[CurrentClassType];
        PlayerScriptableObject playerData = ResourceManager.Instance.GetPlayerInfo(CurrentClassType);

    }

    public override void DisSelect()
    {
        base.DisSelect();
        SkillCanvas.gameObject.SetActive(false);
        // InfoCanvas.gameObject.SetActive(false);
    }
}