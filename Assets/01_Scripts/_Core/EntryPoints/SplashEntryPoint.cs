using System.Threading;

using Cysharp.Threading.Tasks;
using VContainer;
using VContainer.Unity;

using RottenNoble.Core;
using RottenNoble.Core.UI;
using RottenNoble.Splash.UI;

/// <summary>
/// Splash 씬 EntryPoint — SplashView 로드 → ViewModel/Model 주입 → 표시
/// </summary>
public class SplashEntryPoint : EntryPointBase, IAsyncStartable
{
    readonly UIConfigSO uiConfig;

    [Inject]
    public SplashEntryPoint(UIConfigSO uiConfig)
    {
        this.uiConfig = uiConfig;
    }

    public async UniTask StartAsync(CancellationToken cancellation)
    {
        var viewModel = await uiNavigator.LoadAsync<SplashView, SplashViewModel, SplashModel>(
            path:       uiConfig.uiSplashView,
            model:      new SplashModel(),
            canvasType: CanvasType.Hud,
            onComplete: async () =>
            {
                await dataManager.ScenePath.LoadPatchAsync();
            });

        await uiNavigator.ShowAsync<SplashView>();
        AddUICache<SplashView>();

        await UniTask.WaitUntil(
            () => viewModel.View.VisibleState == VisibleState.Disappeared,
            cancellationToken: cancellation);
    }
}
