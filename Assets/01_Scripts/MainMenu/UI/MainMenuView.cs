using UnityEngine.UI;

using R3;

using RottenNoble.Core;
using RottenNoble.Core.UI;
using RottenNoble.MainMenu.Character;
using RottenNoble.MainMenu.Collection;
using RottenNoble.MainMenu.Settings;
using RottenNoble.MainMenu.StageSelect;

namespace RottenNoble.MainMenu.UI
{
    /// <summary>
    /// MainMenu 씬 루트 View — 하단 탭 버튼 및 각 탭 패널 참조를 보유합니다.
    /// 애니메이션 없음 — ViewBase 기본 Show/Hide 동작 사용
    /// </summary>
    public class MainMenuView : ViewBase
    {
        [UnityEngine.Header("[ Tab Buttons ]")]
        [UnityEngine.SerializeField] Button stageSelectTabButton;
        [UnityEngine.SerializeField] Button characterTabButton;
        [UnityEngine.SerializeField] Button collectionTabButton;
        [UnityEngine.SerializeField] Button settingsTabButton;

        [UnityEngine.Header("[ Tab Panels ]")]
        [UnityEngine.SerializeField] StageSelectView stageSelectPanel;
        [UnityEngine.SerializeField] CharacterView   characterPanel;
        [UnityEngine.SerializeField] CollectionView  collectionPanel;
        [UnityEngine.SerializeField] SettingsView    settingsPanel;

        public StageSelectView StageSelectPanel => stageSelectPanel;
        public CharacterView   CharacterPanel   => characterPanel;
        public CollectionView  CollectionPanel  => collectionPanel;
        public SettingsView    SettingsPanel    => settingsPanel;

        public Observable<Unit> OnStageSelectTabClicked()
            => stageSelectTabButton.OnClickAsObservable()
                .ThrottleFirst(AppConstants.ButtonThrottle).Share();

        public Observable<Unit> OnCharacterTabClicked()
            => characterTabButton.OnClickAsObservable()
                .ThrottleFirst(AppConstants.ButtonThrottle).Share();

        public Observable<Unit> OnCollectionTabClicked()
            => collectionTabButton.OnClickAsObservable()
                .ThrottleFirst(AppConstants.ButtonThrottle).Share();

        public Observable<Unit> OnSettingsTabClicked()
            => settingsTabButton.OnClickAsObservable()
                .ThrottleFirst(AppConstants.ButtonThrottle).Share();
    }
}
