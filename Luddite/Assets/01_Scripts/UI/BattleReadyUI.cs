using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using Unity.VisualScripting;

public class BattleReadyUI : MonoBehaviour
{
    public static BattleReadyUI Instance;

    [SerializeField] public PlayerClassType CurrentPlayer;
    [SerializeField] private Image StandingPortrait;
    [SerializeField] private CanvasGroup StandingLabel;
    [SerializeField] private TMP_Text StandingClass;
    [SerializeField] private TMP_Text StandingType;
    [SerializeField] private TMP_Text StageName;
    [SerializeField] private TMP_Text StageStory;
    [SerializeField] private TMP_Text StageHint;

    [Header("Preview")]
    [SerializeField] private CanvasGroup PreviewTargetPanel;
    [SerializeField] private Button PreviewButton;
    [SerializeField] private MeshRenderer PreviewSideTop;
    [SerializeField] private MeshRenderer PreviewSideBottom;

    [Header("Start Preview")]
    [SerializeField] private Button StartButton;

    [SerializeField] List<CharacterSelectButton> CharacterSelectButton;
    [SerializeField] List<SelectSlot> EntrySlots;
    [SerializeField] List<Button> SelectSlotButton;
    [SerializeField] List<PlayerSlotUI> playerSlots;

    [SerializeField] int MemberBatch;

    IEnumerator StandingEffector;

    private void Awake()
    {
        if (Instance == null) { Instance = this; } else { Destroy(gameObject); }
    }

    private void Start()
    {
        StandingLabel.alpha = 0f;
        for (int i = 0; i < CharacterSelectButton.Count; i++)
        {
            int temp = i;
            CharacterSelectButton[temp].CharacterButton.onClick.AddListener(() => OnCharacterSelectButtonClicked(temp));
        }

        for (int i = 0; i < EntrySlots.Count; i++)
        {
            int temp = i;
            SelectSlotButton[temp].onClick.AddListener(() => OnClickedEntrySlot(temp));
        }

        PreviewSideTop.gameObject.SetActive(false);
        PreviewSideBottom.gameObject.SetActive(false);

        PreviewButton.onClick.AddListener(BattlePreviewButton);

        StartButton.onClick.AddListener(BattleEngageButton);
    }

    public Sprite SetPortrait { get { return StandingPortrait.sprite; } 
        set 
        {
            StandingLabel.alpha = 1f;
            StandingPortrait.sprite = value; 
        } 
    }



    public void SetPlayerCharacter(PlayerScriptableObject _player)
    {
        CurrentPlayer = _player.classType;
        SetPortrait = _player.FullBodyPortrait;
        StandingClass.text = _player.classType.ToString();
        StandingType.text = _player.PlayerBattleType;
        if(StandingEffector != null) StopCoroutine(StandingEffector);
        StandingEffector = StandingEffect();

        StartCoroutine(StandingEffector);
    }

    public void SetStage(MapData _stage)
    {
        StageStory.text = _stage.StageStory;
        StageName.text = _stage.StageName;
        StageHint.text = _stage.StageHint;
        MemberBatch = _stage.BatchMembers;
        for (int i = 0; i < MemberBatch; i++)
        {
            EntrySlots[i].playerClass = PlayerClassType.None;
            EntrySlots[i].Portrait.sprite = null;
            EntrySlots[i].BlockedCanvas.gameObject.SetActive(false);
            playerSlots[i].gameObject.SetActive(true);
        }

        GameManager.Instance.CurrentMap = _stage;
    }

    IEnumerator StandingEffect()
    {
        float curFlow = 0f;
        while (curFlow < 1f)
        { 
            curFlow += Time.deltaTime * 2f;
            if (curFlow > 1f) curFlow = 1f;
            StandingPortrait.rectTransform.anchoredPosition = Vector3.Lerp(new Vector2(320f, StandingPortrait.rectTransform.anchoredPosition.y), new Vector2(0f, StandingPortrait.rectTransform.anchoredPosition.y), curFlow);
            StandingPortrait.GetComponent<CanvasGroup>().alpha = Mathf.Lerp(0, 1, curFlow);
            yield return null;
        }

    }

    public void OnCharacterSelectButtonClicked(int _index)
    {

        PlayerAnalysis saved = LibraryManager.Instance.dataContainer.playerAnalyses[CharacterSelectButton[_index].classType];


        if (saved.isLocked == false)
        {
            PlayerScriptableObject data = ResourceManager.Instance.GetPlayerInfo(CharacterSelectButton[_index].classType);
            SetPlayerCharacter(data);
        }

    }

    public void OnClickedEntrySlot(int order)
    {
        Debug.Log(order);
        if (order > MemberBatch - 1) return;
        if (CurrentPlayer == PlayerClassType.None) return;
        if (CurrentPlayer == EntrySlots[order].playerClass) return;

        for (int j = 0; j < EntrySlots.Count; j++)
        {
            if (j == order) continue;
            SelectSlot e = EntrySlots[j];
            
            if (e.playerClass == CurrentPlayer && e.playerClass != PlayerClassType.None)
            {


                Player player = ResourceManager.Instance.GetPlayerResource(e.playerClass, false).GetComponent<Player>();
                playerSlots[j].Unmapping();
                player.statusManager.HPChanged -= playerSlots[j].GetHPValue;
                player.statusManager.MaxHPChanged -= playerSlots[j].GetMaxHPValue;
                player.statusManager.ClockSkillChanged -= playerSlots[j].GetClockValue;
                player.statusManager.ClockSkillReqChanged -= playerSlots[j].GetClockRequire;
                player.statusManager.CClockSkillChanged -= playerSlots[j].GetCClockValue;
                player.statusManager.CClockSkillReqChanged -= playerSlots[j].GetCClockRequire;

                e.playerClass = PlayerClassType.None;
                e.Portrait.sprite = null;
                e.EmptyCanvas.gameObject.SetActive(true);

                ResourceManager.Instance.SpawnPoints[j].SetPlayerClass(PlayerClassType.None);
                break;
            }
        }

        EntrySlots[order].playerClass = CurrentPlayer;
        EntrySlots[order].Portrait.sprite = ResourceManager.Instance.GetPlayerInfo(CurrentPlayer).FacePortrait[0];
        EntrySlots[order].EmptyCanvas.gameObject.SetActive(false);

        // presenter role
        Player curPlayer = ResourceManager.Instance.GetPlayerResource(CurrentPlayer, false).GetComponent<Player>();
        curPlayer.statusManager.HPChanged += playerSlots[order].GetHPValue;
        curPlayer.statusManager.MaxHPChanged += playerSlots[order].GetMaxHPValue;
        curPlayer.statusManager.ClockSkillChanged += playerSlots[order].GetClockValue;
        curPlayer.statusManager.ClockSkillReqChanged += playerSlots[order].GetClockRequire;
        curPlayer.statusManager.CClockSkillChanged += playerSlots[order].GetCClockValue;
        curPlayer.statusManager.CClockSkillReqChanged += playerSlots[order].GetCClockRequire;
        playerSlots[order].Mapping();
        playerSlots[order].SetMaxHP(curPlayer.statusManager.MaxHPValue);
        playerSlots[order].SetSkillClockWiseRequire(curPlayer.statusManager.ClockwiseSkillRequireValue);
        playerSlots[order].SetSkillClockWiseRequire(curPlayer.statusManager.CounterClockwiseSkillRequireValue);
        ResourceManager.Instance.SpawnPoints[order].SetPlayerClass(CurrentPlayer);

        bool isEmpty = false;
        for (int m = 0; m < MemberBatch; m++)
        {
            if (EntrySlots[m].playerClass == PlayerClassType.None)
            {
                StartButton.interactable = false;
                isEmpty = true;
                break;
            }
        }

        if(isEmpty == false)
        {
            StartButton.interactable = true;
        }
    }

    bool PreviewPlaying;

    public void BattlePreviewButton()
    {
        PreviewPlaying = !PreviewPlaying;

        if (PreviewPlaying == true)
        {
            PreviewTargetPanel.alpha = 0f;
            PreviewTargetPanel.blocksRaycasts = false;
            PreviewTargetPanel.interactable = false;

            Material previewTextureTopMat = PreviewSideTop.material;
            Material previewTextureBottomMat = PreviewSideBottom.material;
            previewTextureTopMat.SetTexture("_MainTex", ResourceManager.Instance.BattlePreviewSide);
            previewTextureBottomMat.SetTexture("_MainTex", ResourceManager.Instance.BattlePreviewSide);
            PreviewSideTop.material = previewTextureTopMat;
            PreviewSideBottom.material = previewTextureBottomMat;
            PreviewSideTop.gameObject.SetActive(true);
            PreviewSideBottom.gameObject.SetActive(true);
            foreach (SpawnEnv env in ResourceManager.Instance.SpawnPoints)
            {
                env.AllocatedOnOff(true);
            }

        }
        else
        {
            PreviewTargetPanel.alpha = 1f;
            PreviewTargetPanel.blocksRaycasts = true;
            PreviewTargetPanel.interactable = true;
            PreviewSideTop.gameObject.SetActive(false);
            PreviewSideBottom.gameObject.SetActive(false);

            foreach (SpawnEnv env in ResourceManager.Instance.SpawnPoints)
            {
                env.AllocatedOnOff(false);
            }
        }

    }


    public void BattleEngageButton()
    {
        BattleEngageUI.Instance.BattleEngageStart();
    }

}
