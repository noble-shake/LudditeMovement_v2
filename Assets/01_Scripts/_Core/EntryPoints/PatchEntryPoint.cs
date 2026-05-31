using System.Threading;

using Cysharp.Threading.Tasks;
using VContainer.Unity;

using RottenNoble.Core;
using RottenNoble.Core.UI;
using RottenNoble.Patch.UI;

/// <summary>
/// Patch 씬 EntryPoint — PatchView 로드.
/// 패치 진행 · 페이드아웃 · 씬 전환은 PatchViewModel이 처리합니다.
/// </summary>
public class PatchEntryPoint : EntryPointBase, IAsyncStartable
{
    public async UniTask StartAsync(CancellationToken cancellation)
    {
        await uiNavigator.LoadAsync<PatchView, PatchViewModel, PatchModel>(
            path:       dataManager.UIConfig.uiPatchView,
            model:      new PatchModel(),
            canvasType: CanvasType.Hud,
            onComplete: () => dataManager.ScenePath.LoadIntroAsync());
    }
}
