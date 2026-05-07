using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/*
 * MainBackground / MenuBackground
 * TopBorder
 */

[Serializable]
public enum MenuEnum
{ 
    Menu,
    Characters,
    Stage,
    Library,
    Option,
}


public class MainMenuUI : MonoBehaviour
{
    public static MainMenuUI Instance;

    [Header("Background / Difficulty")]
    [SerializeField] private Button DifficultyChangeButton;
    [SerializeField] private TMP_Text DifficultyText;
    [SerializeField] private Image MainBackground;
    [SerializeField] private Image MenuBackground;
    [SerializeField] private Material MenuEffectMat; // Properties : Progression
    private IEnumerator MenuEffector;

    [Space(16)]
    [Header("Reference Library")]
    [SerializeField] private TMP_Text TextPanelPlayTimeText;
    [SerializeField] private TMP_Text TextPanelDifficultyText;
    [SerializeField] private TMP_Text TextPanelProgressText;
    [SerializeField] private TMP_Text TextPanelCollectorText;

    [Space(16)]
    [Header("Menu Canvas Groups")]
    [SerializeField] private TMP_Text TopBorderMenuText;
    [SerializeField] private CanvasGroup CurrentCanvas;
    [SerializeField] private CanvasGroup MenuCanvas;
    [SerializeField] private CanvasGroup MenuPanels;
    [SerializeField] private CanvasGroup CharactersCanvas;
    [SerializeField] private CanvasGroup StageCanvas;
    [SerializeField] private CanvasGroup LibraryCanvas;
    [SerializeField] private CanvasGroup OptionCanvas;

    [Space(16)]
    [Header("Menu Buttons")]
    [SerializeField] private Button MenuButton;
    [SerializeField] private Button CharactersButton;
    [SerializeField] private Button MCharactersButton;
    [SerializeField] private Button StageButton;
    [SerializeField] private Button MStageButton;
    [SerializeField] private Button LibraryButton;
    [SerializeField] private Button MLibraryButton;
    [SerializeField] private Button OptionButton;
    [SerializeField] private Button MOptionButton;

    private void Awake()
    {
        if (Instance == null) { Instance = this; } else { Destroy(gameObject); }
    }

    private IEnumerator Start()
    {
        yield return null;
        CurrentCanvas = MenuCanvas;
        DifficultyChangeButton.onClick.AddListener(DifficultyChangeButtonClicked);
        MenuEffectMat = MenuBackground.material;
        MenuEffectMat.SetFloat("_Progression", 0f);
        MenuBackground.material = MenuEffectMat;
        // GUILayout.Label($"GameTime : {Mathf.FloorToInt(TimeScheduling / 60f).ToString("D2")}:{Mathf.FloorToInt(TimeScheduling % 60f).ToString("D2")}", targetStyle);
        MenuBind();
    }

    private void MenuBind()
    {
        for (int i = 0; i < System.Enum.GetValues(typeof(MenuEnum)).Length; i++)
        {
            int index = i;
            switch ((MenuEnum)index)
            {
                default:
                case MenuEnum.Menu:
                    MenuButton.onClick.AddListener(() => OnMenuButtonClicked((MenuEnum)index));
                    break;
                case MenuEnum.Characters:
                    CharactersButton.onClick.AddListener(() => OnMenuButtonClicked((MenuEnum)index));
                    MCharactersButton.onClick.AddListener(() => OnMenuButtonClicked((MenuEnum)index));
                    break;
                case MenuEnum.Stage:
                    StageButton.onClick.AddListener(() => OnMenuButtonClicked((MenuEnum)index));
                    MStageButton.onClick.AddListener(() => OnMenuButtonClicked((MenuEnum)index));
                    break;
                case MenuEnum.Library:
                    LibraryButton.onClick.AddListener(() => OnMenuButtonClicked((MenuEnum)index));
                    MLibraryButton.onClick.AddListener(() => OnMenuButtonClicked((MenuEnum)index));
                    break;
                case MenuEnum.Option:
                    OptionButton.onClick.AddListener(() => OnMenuButtonClicked((MenuEnum)index));
                    MOptionButton.onClick.AddListener(() => OnMenuButtonClicked((MenuEnum)index));
                    break;
            }
        }
    }

    public void UpdatePlaytime()
    {
        // 모델은 라이브러리 매니저이니, 받아 읽는 것만 해주자.
        float TimeScheduling = LibraryManager.Instance.TotalPlayTime;
        TextPanelPlayTimeText.text = $"{Mathf.FloorToInt(TimeScheduling / 600f).ToString("D2")}:{Mathf.FloorToInt(TimeScheduling / 60f).ToString("D2")}:{Mathf.FloorToInt(TimeScheduling % 60f).ToString("D2")}";
    }

    public void UpdateProgress()
    { 
    
    }

    public void UpdateCollector()
    { 
        
    }

    private void DifficultyChangeButtonClicked()
    {
        Difficulty _target = GameManager.Instance.GetDifficulty();
        int NextIndex = (int)_target + 1;
        if (NextIndex >= System.Enum.GetValues(typeof(Difficulty)).Length) NextIndex = 0;
        _target = (Difficulty)(NextIndex);
        Color DiffColor = PersonalColors.GetDifficultyColors(_target);

        MainBackground.color = DiffColor;
        MenuBackground.color = DiffColor;
        DifficultyText.color = DiffColor;
        DifficultyText.text = _target.ToString();
        TextPanelDifficultyText.text = _target.ToString();

        GameManager.Instance.SetDifficulty(_target);
    }

    private void OnMenuButtonClicked(MenuEnum _menu)
    {
        if (MenuEffector != null) StopCoroutine(MenuEffector);
        CurrentCanvas.interactable = false;
        CurrentCanvas.blocksRaycasts = false;
        if (CurrentCanvas == MenuCanvas)
        {
            MenuPanels.alpha = 0f;
        }
        else
        { 
            CurrentCanvas.gameObject.SetActive(false);
        }

        MenuEffector = MenuTransition();
        TopBorderMenuText.text = _menu.ToString();
        switch (_menu)
        {
            default:
            case MenuEnum.Menu:
                CurrentCanvas = MenuCanvas;
                MenuEffector = MainMenuTransition();
                break;
            case MenuEnum.Characters:
                CurrentCanvas = CharactersCanvas;
                break;
            case MenuEnum.Stage:
                CurrentCanvas = StageCanvas;
                break;
            case MenuEnum.Library:
                CurrentCanvas = LibraryCanvas;
                break;
            case MenuEnum.Option:
                CurrentCanvas = OptionCanvas;
                break;
        }

        CurrentCanvas.interactable = true;
        CurrentCanvas.blocksRaycasts = true;

        if (CurrentCanvas != MenuCanvas)
        {
            CurrentCanvas.gameObject.SetActive(true);
        }

        StartCoroutine(MenuEffector);
    }

    // After SetActive.
    private IEnumerator MenuTransition()
    {
        float CurFlow = 0f;
        MenuEffectMat.SetFloat("_Progression", CurFlow);
        MenuBackground.material = MenuEffectMat;

        while (CurFlow < 1f)
        {
            CurFlow += Time.deltaTime;
            if (CurFlow >= 1f) CurFlow = 1f;
            MenuEffectMat.SetFloat("_Progression", CurFlow);
            MenuBackground.material = MenuEffectMat;
            yield return null;
        }
    }

    private IEnumerator MainMenuTransition()
    {
        float CurFlow = 1f;
        MenuPanels.alpha = 0f;
        MenuEffectMat.SetFloat("_Progression", CurFlow);
        MenuBackground.material = MenuEffectMat;

        while (CurFlow > 0f)
        {
            CurFlow -= Time.deltaTime;
            if (CurFlow <= 0f) CurFlow = 0f;
            MenuPanels.alpha = 1 - CurFlow;
            MenuEffectMat.SetFloat("_Progression", CurFlow);
            MenuBackground.material = MenuEffectMat;
            yield return null;
        }
    }


}