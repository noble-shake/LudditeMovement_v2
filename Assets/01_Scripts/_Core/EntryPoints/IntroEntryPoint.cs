using System.Threading;

using Cysharp.Threading.Tasks;
using VContainer;
using VContainer.Unity;

using RottenNoble.Core;
using RottenNoble.Core.UI;
using RottenNoble.Intro.UI;

/// <summary>
/// Intro 씬 EntryPoint — IntroView 로드 · 표시.
/// 클릭 감지 · 페이드아웃 · 씬 전환은 IntroViewModel이 Model.OnComplete를 통해 처리합니다.
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
        await uiNavigator.LoadAsync<IntroView, IntroViewModel, IntroModel>(
            path:       uiConfig.uiIntroView,
            model:      new IntroModel(),
            canvasType: CanvasType.Hud,
            onComplete: () => dataManager.ScenePath.LoadDataLoadAsync());
        // LoadAsync 내부에서 ShowAsync까지 완료합니다.
        // 이후 흐름은 IntroViewModel.OnClickedAsync가 담당합니다.
    }
}
