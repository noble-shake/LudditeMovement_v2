using System.Threading;

using Cysharp.Threading.Tasks;
using VContainer;
using VContainer.Unity;

using RottenNoble.Core;
using RottenNoble.Core.UI;
using RottenNoble.Intro.UI;

/// <summary>
/// Intro 씬 EntryPoint — IntroView 로드 → ViewModel/Model 주입 → 표시
/// </summary>
public class IntroEntryPoint : EntryPointBase, IAsyncStartable
{
    readonly UIConfigSO uiConfig;

    [Inject]
    public IntroEntryPoint(UIConfigSO uiConfig)
    {
        this.uiConfig = uiConfig;
    }

    public async UniTask StartAsync(CancellationToken cancellation)
    {
        var viewModel = await uiNavigator.LoadAsync<IntroView, IntroViewModel, IntroModel>(
            path:       uiConfig.uiIntroView,
            model:      new IntroModel(),
            canvasType: CanvasType.Hud);

        await uiNavigator.ShowAsync<IntroView>();
        AddUICache<IntroView>();

        await UniTask.WaitUntil(
            () => viewModel.View.VisibleState == VisibleState.Disappeared,
            cancellationToken: cancellation);
    }
}
