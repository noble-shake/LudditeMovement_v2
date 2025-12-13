using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;

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
    [SerializeField] private Button PreviewButton;
    [SerializeField] private Button StartButton;

    IEnumerator StandingEffector;

    private void Start()
    {
        StandingLabel.alpha = 0f;
        for (int i = 0; i < CharacterSelectButton.Count; i++)
        {
            int index = i;
            CharacterSelectButton[index].CharacterButton.onClick.AddListener(() => OnCharacterSelectButtonClicked(index));
        }
    }

    public Sprite SetPortrait { get { return StandingPortrait.sprite; } 
        set 
        {
            StandingLabel.alpha = 1f;
            StandingPortrait.sprite = value; 
        } 
    }

    [SerializeField] List<CharacterSelectButton> CharacterSelectButton;
    [SerializeField] List<Button> SelectSlotButton;
    [SerializeField] List<PlayerSlotUI> playerSlots;

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

}
