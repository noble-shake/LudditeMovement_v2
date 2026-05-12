using System.Threading;

using Cysharp.Threading.Tasks;
using VContainer;
using VContainer.Unity;

using RottenNoble.Core;
using RottenNoble.Core.UI;
using RottenNoble.MainMenu.UI;

/// <summary>
/// MainMenu 씬 EntryPoint — MainMenuView 로드 → ViewModel/Model 주입 → 표시
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
        var viewModel = await uiNavigator.LoadAsync<MainMenuView, MainMenuViewModel, MainMenuModel>(
            path:       uiConfig.uiMainMenuView,
            model:      new MainMenuModel(),
            canvasType: CanvasType.Hud);

        await uiNavigator.ShowAsync<MainMenuView>();
        AddUICache<MainMenuView>();

        await UniTask.WaitWhile(
            () => viewModel.View != null && viewModel.View.VisibleState != VisibleState.Disappeared,
            cancellationToken: cancellation);
    }
}
