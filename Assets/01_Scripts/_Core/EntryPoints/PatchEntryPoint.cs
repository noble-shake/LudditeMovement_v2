using System.Threading;

using Cysharp.Threading.Tasks;
using VContainer;
using VContainer.Unity;

using RottenNoble.Core;
using RottenNoble.Core.UI;
using RottenNoble.Patch.UI;

/// <summary>
/// Patch 씬 EntryPoint — PatchView 로드.
/// 패치 진행 · 페이드아웃 · 씬 전환은 PatchViewModel이 Model.OnComplete를 통해 처리합니다.
/// </summary>
public class PatchEntryPoint : EntryPointBase, IAsyncStartable
{
    readonly UIConfigSO uiConfig;

    [Inject]
    public PatchEntryPoint(UIConfigSO uiConfig)
    {
        this.uiConfig = uiConfig;
    }

    public async UniTask StartAsync(CancellationToken cancellation)
    {
        await uiNavigator.LoadAsync<PatchView, PatchViewModel, PatchModel>(
            path:       uiConfig.uiPatchView,
            model:      new PatchModel(),
            canvasType: CanvasType.Hud,
            onComplete: () => dataManager.ScenePath.LoadIntroAsync());

        // 이후 흐름은 PatchViewModel.PatchSequenceAsync가 담당합니다.
    }
}
