using System.Threading;

using Cysharp.Threading.Tasks;
using VContainer.Unity;

using RottenNoble.Core;
using RottenNoble.Core.UI;
using RottenNoble.Intro.UI;

/// <summary>
/// Intro 씬 EntryPoint — IntroView 로드 · 표시.
/// 클릭 감지 · 페이드아웃 · 씬 전환은 IntroViewModel이 처리합니다.
/// </summary>
public class IntroEntryPoint : EntryPointBase, IAsyncStartable
{
    public async UniTask StartAsync(CancellationToken cancellation)
    {
        await uiNavigator.LoadAsync<IntroView, IntroViewModel, IntroModel>(
            path:       dataManager.UIConfig.uiIntroView,
            model:      new IntroModel(),
            canvasType: CanvasType.Hud,
            onComplete: () => dataManager.ScenePath.LoadDataLoadAsync());
    }
}
