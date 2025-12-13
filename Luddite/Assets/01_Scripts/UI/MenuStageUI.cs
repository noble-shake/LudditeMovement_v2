using System;
using System.Collections.Generic;
using UnityEngine;


/*
 * 1. 각 버튼들에게 MapData를 맵핑 해준다.
 * 2. 버튼을 눌러서 BattleReady에 들어선 순간, 타일 로드를 실행한다.
 * 3. 버튼을 누르면 맵 정보를, 맵 정보 상태에서 진입을 누르면 Battle Ready로
 * 4. 만약, 타일 로딩에 지연이 발생한다면 약간의 로딩을 넣어준다.
 */


public class MenuStageUI : MonoBehaviour
{
    [Header("Character Select Buttons")]
    [SerializeField] private List<MenuStageButton> MenuSelectButton;
    [SerializeField] private MenuStageButton ClickedButton;
    [SerializeField] private CanvasGroup CheckCanvas;
    [SerializeField] private MenuStagecheckButton CheckButton;
    public Action ButtonClearAction = new Action(() => { });


    private void Start()
    {
        for (int i = 0; i < MenuSelectButton.Count; i++)
        {
            int index = i;
            MenuSelectButton[index].StageButton.onClick.AddListener(() => OnClickedStageButton(index));
            ButtonClearAction += MenuSelectButton[index].MenuClearAction;
        }
    }

    // Button 최초 클릭 시,
    private void OnClickedStageButton(int order)
    {
        ButtonClearAction.Invoke();
        ClickedButton = MenuSelectButton[order];
        ClickedButton.MenuClicked();
        CheckCanvas.gameObject.SetActive(true);
        
    }


}