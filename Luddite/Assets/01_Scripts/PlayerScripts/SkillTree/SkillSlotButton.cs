using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SkillSlotButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (AllocatedNode == null) return;
        if (isEmpty) return;
        if (SkillCanavs == null) return;
        SkillCanavs.alpha = 1f;
        SkillCanavsIcon.color = Color.white;
        SkillCanavsIcon.sprite = AllocatedNode.SkillObject.Icon;
        SkillCanavsText.text = AllocatedNode.SkillObject.Description;
        SkillCanavs.GetComponent<RectTransform>().anchoredPosition = this.GetComponent<RectTransform>().anchoredPosition;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (AllocatedNode == null) return;
        if (isEmpty) return;
        if (SkillCanavs == null) return;
        SkillCanavs.alpha = 0f;
        SkillCanavsIcon.color = Color.black;

    }

    private Button SlotButton;
    public SlotType slotType;
    public SkillTreeMap treeMap;

    [SerializeField] private Image SkillIcon;
    [SerializeField] private Image SkillBackground;
    [SerializeField] private TreeNode AllocatedNode;
    [SerializeField] private CanvasGroup EmptyCover;
    public CanvasGroup SkillCanavs;
    public Image SkillCanavsIcon;
    public TMP_Text SkillCanavsText;

    public TreeNode NodeValue { get { return AllocatedNode; } set { AllocatedNode = value; } }
    [SerializeField] public bool isEmpty;
    [SerializeField] public bool isEnd;
    [SerializeField] public bool isStart;

    private void Start()
    {
        SlotButton = GetComponentInChildren<Button>();
        SetEmpty(isEmpty);
    }

    public void SetEmpty(bool isOn)
    { 
        EmptyCover.gameObject.SetActive(isOn);
        EmptyCover.interactable = isOn;
        EmptyCover.blocksRaycasts = isOn;
        EmptyCover.alpha = isOn ? 1f : 0f;
    }

    public void SetEndNode(bool isOn)
    {
        isEnd = isOn;
    }

    public void SetStartNode(bool isOn)
    {
        isStart = isOn;
    }

    public void ButtonEarned()
    {
        if(SlotButton == null) SlotButton = GetComponentInChildren<Button>();
        SlotButton.interactable = false;
        SkillIcon.color = Color.white;
        SkillBackground.color = Color.orange;
    }

    public void ButtonNotActivatedYet()
    {
        SkillIcon.color = Color.white;
        SkillBackground.color = Color.black;
    }

    public void ButtonActivated(int Tier)
    {
        if (Tier != AllocatedNode.Tier - 1) return;
        PlayerClassType _class = GetComponentInParent<CharacterInfoIcon>().CurrentClassType;
        if (PlayerSkillManager.Instance.SkillEarnableCheck(slotType, AllocatedNode, _class) == false) return;

        SlotButton.interactable = true;
        SkillIcon.color = Color.white;
    }

    public void ButtonDeActivated()
    {
        SlotButton.interactable = false;
        SkillIcon.color = Color.gray;
        SkillBackground.color = Color.black;
    }

    public void ButtonClicked()
    {
        if (AllocatedNode == null) return;
        if (isEmpty) return;
        if (AllocatedNode.isEarned) return;  // Already Earned.

        PlayerClassType _class = GetComponentInParent<CharacterInfoIcon>().CurrentClassType;
        if (PlayerSkillManager.Instance.SkillEarnableCheck(slotType, AllocatedNode, _class) == false) return;  // 부모 스킬을 얻었는지, Synergy 체크까지 완료

        // Skill Point 남아 있는지 최종 체크
        

        // SkillPoint Use
        AllocatedNode.isEarned = true;
        SkillIcon.color = Color.white;
        SkillBackground.color = Color.orange;
        treeMap.ActivateInvoke(AllocatedNode.Tier);
    }

    public void SetSkillButton()
    {
        SkillIcon.sprite = AllocatedNode.SkillObject.Icon;


    }
}
