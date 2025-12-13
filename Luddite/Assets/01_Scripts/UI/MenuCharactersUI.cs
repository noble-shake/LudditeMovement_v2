using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


/*
 * Slot check.
 * Standing Process
 * ----------- Info group
 * Info
 * Level
 * Skill
 * ----------
 */

public class MenuCharactersUI : MonoBehaviour
{
    [Header("Character Select Buttons")]
    [SerializeField] private List<CharacterSelectButton> CharacterSelectButtons;
    [SerializeField] private CharacterInfoIcon InfoIcon;
    [SerializeField] private CharacterInfoLevel LevelIcon;
    [SerializeField] private CharacterInfoSkill SkillIcon;
    [SerializeField] private Image StandingPortriat;
    [SerializeField] private Image StandingOutline; // Outline Component (pc) + Image Componnet (cc)

    private IEnumerator Start()
    {
        yield return null;
        for (int i = 0; i < CharacterSelectButtons.Count; i++)
        {
            int index = i;
            CharacterSelectButtons[index].CharacterButton.onClick.AddListener(() => OnCharacterSelectClicked(CharacterSelectButtons[index].classType));
            InfoIcon.OnIconClicked();
        }

        OnCharacterSelectClicked(PlayerClassType.Knight);
    }

    private void OnCharacterSelectClicked(PlayerClassType _class)
    {
        InfoIcon.CurrentClassType = _class;
        Sprite Target = ResourceManager.Instance.GetPlayerInfo(_class).FullBodyPortrait;
        Sprite WTarget = ResourceManager.Instance.GetPlayerInfo(_class).FullBodySilouhettePortrait;
        StandingPortriat.sprite = Target;
        StandingOutline.sprite = WTarget;
        StandingOutline.color = PersonalColors.GetColors(_class);
        StandingOutline.GetComponent<Outline>().effectColor = PersonalColors.GetCompColors(_class);
    }
}