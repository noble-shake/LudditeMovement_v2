using System.Threading;

using Cysharp.Threading.Tasks;
using VContainer.Unity;

using RottenNoble.Core;
using RottenNoble.Core.UI;
using RottenNoble.Splash.UI;

/// <summary>
/// Splash 씬 EntryPoint — SplashView 로드 · 표시.
/// 대기(2s) · 페이드아웃 · 씬 전환은 SplashViewModel이 처리합니다.
/// </summary>
public class SplashEntryPoint : EntryPointBase, IAsyncStartable
{
    public async UniTask StartAsync(CancellationToken cancellation)
    {
        await uiNavigator.LoadAsync<SplashView, SplashViewModel, SplashModel>(
            path:       dataManager.UIConfig.uiSplashView,
            model:      new SplashModel(),
            canvasType: CanvasType.Hud,
            onComplete: () => dataManager.ScenePath.LoadPatchAsync());
    }
}
