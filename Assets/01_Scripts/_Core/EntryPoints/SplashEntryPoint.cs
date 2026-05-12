using System.Threading;

using Cysharp.Threading.Tasks;
using VContainer;
using VContainer.Unity;

using RottenNoble.Core;
using RottenNoble.Core.UI;
using RottenNoble.Splash.UI;

/// <summary>
/// Splash 씬 EntryPoint — SplashView 로드 · 표시.
/// 대기(2s) · 페이드아웃 · 씬 전환은 SplashViewModel이 Model.OnComplete를 통해 처리합니다.
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
        var model = new SplashModel
        {
            OnComplete = () => dataManager.ScenePath.LoadPatchAsync()
        };

        await uiNavigator.LoadAsync<SplashView, SplashViewModel, SplashModel>(
            path:       uiConfig.uiSplashView,
            model:      model,
            canvasType: CanvasType.Hud);
        // LoadAsync 내부에서 ShowAsync까지 완료합니다.
        // 이후 흐름은 SplashViewModel.SplashSequenceAsync가 담당합니다.
    }
}
