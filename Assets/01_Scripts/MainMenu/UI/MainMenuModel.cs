using R3;

using RottenNoble.Core.UI;

namespace RottenNoble.MainMenu.UI
{
    /// <summary>
    /// MainMenu 씬 데이터 모델 — 현재 활성 탭 상태를 보관합니다.
    /// </summary>
    public class MainMenuModel : ModelBase
    {
        public ReactiveProperty<MainMenuTab> CurrentTab { get; } = new(MainMenuTab.StageSelect);

        public void SwitchTab(MainMenuTab tab)
        {
            if (CurrentTab.Value == tab) return;
            CurrentTab.Value = tab;
            NotifyChanged(nameof(CurrentTab));
        }
    }

    public enum MainMenuTab
    {
        StageSelect,
        Character,
        Collection,
        Settings,
    }
}
