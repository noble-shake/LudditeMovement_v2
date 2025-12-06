using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;

public class BattleReadyUI : MonoBehaviour
{
    public static BattleReadyUI Instance;

    [SerializeField] private Image StandingPortrait;
    [SerializeField] private CanvasGroup StandingLabel;
    [SerializeField] private TMP_Text StandingClass;
    [SerializeField] private TMP_Text StandingType;
    [SerializeField] private TMP_Text StageName;
    [SerializeField] private TMP_Text StageStory;
    [SerializeField] private TMP_Text StageHint;
    [SerializeField] private Button PreviewButton;
    [SerializeField] private Button StartButton;

    private void Start()
    {
        StandingLabel.alpha = 0f;
    }

    public Sprite SetPortrait { get { return StandingPortrait.sprite; } 
        set 
        {
            StandingLabel.alpha = 1f;
            StandingPortrait.sprite = value; 
        } 
    }

    [SerializeField] List<Button> CharacterSelectButton;
    [SerializeField] List<Button> SelectSlotButton;
    [SerializeField] List<PlayerSlotUI> playerSlots;

    public void SetPlayerCharacter(PlayerScriptableObject _player)
    {
        SetPortrait = _player.FullBodyPortrait;
        StandingClass.text = _player.classType.ToString();
        StandingType.text = _player.PlayerBattleType;
        StartCoroutine(StandingEffect());
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
            StandingPortrait.rectTransform.position = Vector3.Lerp(new Vector3(0f, -270f, 0f), new Vector3(-350f, -270f, 0f), curFlow);
            StandingPortrait.GetComponent<CanvasGroup>().alpha = Mathf.Lerp(0, 1, curFlow);
        }
        yield return null;
    }

    public void OnCharacterSelectButtonClicked()
    { 
        
    }

}
