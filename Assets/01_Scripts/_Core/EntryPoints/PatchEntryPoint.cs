using System.Threading;

using Cysharp.Threading.Tasks;
using VContainer;
using VContainer.Unity;

using RottenNoble.Core;
using RottenNoble.Core.Services;
using RottenNoble.Core.UI;
using RottenNoble.Patch.UI;

/// <summary>
/// Patch 씬 EntryPoint — PatchView 로드 → ViewModel/Model 주입 → 표시
/// </summary>
public class PatchEntryPoint : EntryPointBase, IAsyncStartable
{
    readonly UIConfigSO  uiConfig;
    readonly PatchService patchService;

    [Inject]
    public PatchEntryPoint(UIConfigSO uiConfig, PatchService patchService)
    {
        this.uiConfig     = uiConfig;
        this.patchService = patchService;
    }

    public async UniTask StartAsync(CancellationToken cancellation)
    {
        var viewModel = await uiNavigator.LoadAsync<PatchView, PatchViewModel, PatchModel>(
            path:       uiConfig.uiPatchView,
            model:      new PatchModel(),
            canvasType: CanvasType.Hud,
            onComplete: async () =>
            {
                await patchService.CheckAndUpdateAsync();
                await UniTask.Delay(1000);
                await dataManager.ScenePath.LoadIntroAsync();
            });

        await uiNavigator.ShowAsync<PatchView>();
        AddUICache<PatchView>();

        await UniTask.WaitUntil(
            () => viewModel.View.VisibleState == VisibleState.Disappeared,
            cancellationToken: cancellation);
    }
}
