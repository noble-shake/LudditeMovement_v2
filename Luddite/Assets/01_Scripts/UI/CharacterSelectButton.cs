using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CharacterSelectButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
{
    [SerializeField] public PlayerClassType classType;
    [SerializeField] public Button CharacterButton;
    [SerializeField] private CanvasGroup HoverInfo;
    [SerializeField] private CanvasGroup HoverSkill1;
    [SerializeField] private CanvasGroup HoverSkill2;
    [SerializeField] private CanvasGroup InfoCanvas;
    [SerializeField] private TMP_Text InfoText;
    [SerializeField] private CanvasGroup Skill1Canvas;
    [SerializeField] private Image Skill1Icon;
    [SerializeField] private TMP_Text Skill1Text;
    [SerializeField] private CanvasGroup Skill2Canvas;
    [SerializeField] private Image Skill2Icon;
    [SerializeField] private TMP_Text Skill2Text;
    [SerializeField] public CanvasGroup LockCanvas;
    [SerializeField] public bool isLocked;

    private void Start()
    {
        isLocked = true;
        if (classType == PlayerClassType.Knight)
        {
            LockCanvas.alpha = 0f;
            LockCanvas.interactable = false;
            LockCanvas.blocksRaycasts = false;
            isLocked = false;
        }

        // CharacterButton.targetGraphic.color = PersonalColors.GetColors(classType);
        // Library Check.
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        List<RaycastResult> raycastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, raycastResults);

        if (raycastResults.Count > 0)
        {
            foreach (var go in raycastResults)
            {
                if (HoverInfo != null)
                {
                    if (go.gameObject == HoverInfo.gameObject)
                    {
                        InfoCanvas.alpha = 1f;
                        InfoCanvas.blocksRaycasts = true;
                        InfoCanvas.interactable = true;
                        PlayerScriptableObject pInfo = ResourceManager.Instance.GetPlayerInfo(classType);
                        InfoText.text = pInfo.Description;
                        Debug.Log("HoverInfo");
                        return;
                    }
                }


                if (go.gameObject == HoverSkill1.gameObject)
                {
                    Skill1Canvas.alpha = 1f;
                    Skill1Canvas.blocksRaycasts = true;
                    Skill1Canvas.interactable = true;
                    TreeNode Active1Tree = LibraryManager.Instance.playerAnalyses[classType].CurrentActive1;
                    PlayerSkillScriptableObject skillObject = Active1Tree.SkillObject;

                    if (skillObject == null)
                    {
                        Skill1Icon.sprite = null;
                        Skill1Icon.color = Color.black;
                        Skill1Text.text = "Not Allocated";
                    }
                    else
                    {
                        Skill1Icon.sprite = skillObject.Icon;
                        Skill1Icon.color = Color.white;
                        Skill1Text.text = $"[{skillObject.Name}] \n {skillObject.Description}";
                    }
                    Debug.Log("Skill1 Info");
                    return;
                }
                else if (go.gameObject == HoverSkill2.gameObject)
                {
                    Skill2Canvas.alpha = 1f;
                    Skill2Canvas.blocksRaycasts = true;
                    Skill2Canvas.interactable = true;
                    TreeNode Active2Tree = LibraryManager.Instance.playerAnalyses[classType].CurrentActive2;
                    PlayerSkillScriptableObject skillObject = Active2Tree.SkillObject;

                    if (skillObject == null)
                    {
                        Skill2Icon.sprite = null;
                        Skill2Icon.color = Color.black;
                        Skill2Text.text = "Not Allocated";
                    }
                    else
                    {
                        Skill2Icon.sprite = skillObject.Icon;
                        Skill2Icon.color = Color.white;
                        Skill2Text.text = $"[{skillObject.Name}] \n {skillObject.Description}";
                    }
                    Debug.Log("Skill2 Info");
                    return;
                }
                else
                {
                    continue;
                }
            }
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (InfoCanvas != null)
        {
            InfoCanvas.alpha = 0f;
            InfoCanvas.blocksRaycasts = false;
            InfoCanvas.interactable = false;
        }


        Skill1Canvas.alpha = 0f;
        Skill1Canvas.blocksRaycasts = false;
        Skill1Canvas.interactable = false;

        Skill2Canvas.alpha = 0f;
        Skill2Canvas.blocksRaycasts = false;
        Skill2Canvas.interactable = false;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (InfoCanvas != null)
        {
            InfoCanvas.alpha = 0f;
            InfoCanvas.blocksRaycasts = false;
            InfoCanvas.interactable = false;
        }


        Skill1Canvas.alpha = 0f;
        Skill1Canvas.blocksRaycasts = false;
        Skill1Canvas.interactable = false;

        Skill2Canvas.alpha = 0f;
        Skill2Canvas.blocksRaycasts = false;
        Skill2Canvas.interactable = false;
    }
}