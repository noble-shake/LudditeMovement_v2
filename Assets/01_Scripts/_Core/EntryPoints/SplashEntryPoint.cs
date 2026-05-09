using System.Threading;

using Cysharp.Threading.Tasks;
using VContainer;
using VContainer.Unity;

using RottenNoble.Core;
using RottenNoble.Core.Resource;
using RottenNoble.Core.UI;
using RottenNoble.Splash.UI;

/// <summary>
/// Splash 씬 EntryPoint — SplashView 로드 → SplashViewModel에 위임
/// </summary>
public class SplashEntryPoint : EntryPointBase, IAsyncStartable
{
    readonly GameConfig gameConfig;

    [Inject]
    public SplashEntryPoint(DataManager dataManager, UINavigator uiNavigator, GameConfig gameConfig)
    {
        this.dataManager  = dataManager;
        this.uiNavigator  = uiNavigator;
        this.gameConfig   = gameConfig;
    }

    public async UniTask StartAsync(CancellationToken cancellation)
    {
        var view = await uiNavigator.LoadAsync<SplashView>(CanvasType.Hud, gameConfig.uiSplashView);
        AddCache(ResourceType.Addressable, view.gameObject);

        view.InjectPresenter<SplashViewModel>()
            .Initialize(view, new SplashModel());

        await UniTask.WaitUntil(
            () => view.VisibleState == VisibleState.Disappeared,
            cancellationToken: cancellation);
    }
}
