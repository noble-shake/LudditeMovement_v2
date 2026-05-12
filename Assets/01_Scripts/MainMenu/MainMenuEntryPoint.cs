using System.Threading;

using Cysharp.Threading.Tasks;
using VContainer;
using VContainer.Unity;

using RottenNoble.Core;
using RottenNoble.Core.UI;
using RottenNoble.MainMenu.UI;

/// <summary>
/// MainMenu 씬 EntryPoint — MainMenuView 로드.
/// 탭 전환 및 스테이지 진입은 각 서브탭 ViewModel이 담당합니다.
/// </summary>
public class MainMenuEntryPoint : EntryPointBase, IAsyncStartable
{
    readonly UIConfigSO uiConfig;

    [Inject]
    public MainMenuEntryPoint(UIConfigSO uiConfig)
    {
        this.uiConfig = uiConfig;
    }

    public async UniTask StartAsync(CancellationToken cancellation)
    {
        await uiNavigator.LoadAsync<MainMenuView, MainMenuViewModel, MainMenuModel>(
            path:       uiConfig.uiMainMenuView,
            model:      new MainMenuModel(),
            canvasType: CanvasType.Hud);
    }
}
