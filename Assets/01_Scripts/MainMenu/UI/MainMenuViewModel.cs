using Cysharp.Threading.Tasks;
using R3;

using RottenNoble.Core.UI;
using RottenNoble.MainMenu.Character;
using RottenNoble.MainMenu.Collection;
using RottenNoble.MainMenu.Settings;
using RottenNoble.MainMenu.StageSelect;

namespace RottenNoble.MainMenu.UI
{
    /// <summary>
    /// MainMenu 씬 루트 ViewModel — 탭 전환 및 서브탭 ViewModel 초기화
    /// </summary>
    public class MainMenuViewModel : ViewModelBase<MainMenuView, MainMenuModel>
    {
        public override async UniTask Initialize(MainMenuView view, MainMenuModel model)
        {
            await base.Initialize(view, model);

            // ── 서브탭 ViewModel 초기화 ───────────────────
            await view.StageSelectPanel.InjectPresenter<StageSelectViewModel>()
                .Initialize(view.StageSelectPanel, new StageSelectModel());

            await view.CharacterPanel.InjectPresenter<CharacterViewModel>()
                .Initialize(view.CharacterPanel, new CharacterModel());

            await view.CollectionPanel.InjectPresenter<CollectionViewModel>()
                .Initialize(view.CollectionPanel, new CollectionModel());

            await view.SettingsPanel.InjectPresenter<SettingsViewModel>()
                .Initialize(view.SettingsPanel, new SettingsModel());

            // ── 탭 버튼 구독 ──────────────────────────────
            view.OnStageSelectTabClicked()
                .Subscribe(_ => model.SwitchTab(MainMenuTab.StageSelect))
                .AddTo(ref disposableBag);

            view.OnCharacterTabClicked()
                .Subscribe(_ => model.SwitchTab(MainMenuTab.Character))
                .AddTo(ref disposableBag);

            view.OnCollectionTabClicked()
                .Subscribe(_ => model.SwitchTab(MainMenuTab.Collection))
                .AddTo(ref disposableBag);

            view.OnSettingsTabClicked()
                .Subscribe(_ => model.SwitchTab(MainMenuTab.Settings))
                .AddTo(ref disposableBag);

            // ── 탭 전환 → 패널 표시/숨김 ─────────────────
            model.CurrentTab
                .Subscribe(OnTabChanged)
                .AddTo(ref disposableBag);

            view.ShowImmediate();
        }

        void OnTabChanged(MainMenuTab tab)
        {
            View.StageSelectPanel.gameObject.SetActive(tab == MainMenuTab.StageSelect);
            View.CharacterPanel  .gameObject.SetActive(tab == MainMenuTab.Character);
            View.CollectionPanel .gameObject.SetActive(tab == MainMenuTab.Collection);
            View.SettingsPanel   .gameObject.SetActive(tab == MainMenuTab.Settings);
        }
    }
}
