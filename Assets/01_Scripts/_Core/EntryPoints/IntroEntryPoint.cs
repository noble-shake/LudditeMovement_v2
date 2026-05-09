using System.Threading;

using Cysharp.Threading.Tasks;
using VContainer;
using VContainer.Unity;

using RottenNoble.Core;
using RottenNoble.Core.Resource;
using RottenNoble.Core.UI;
using RottenNoble.Intro.UI;

/// <summary>
/// Intro 씬 EntryPoint — IntroView 로드 → IntroViewModel에 위임
/// </summary>
public class IntroEntryPoint : EntryPointBase, IAsyncStartable
{
    readonly GameConfig gameConfig;

    [Inject]
    public IntroEntryPoint(DataManager dataManager, UINavigator uiNavigator, GameConfig gameConfig)
    {
        this.dataManager  = dataManager;
        this.uiNavigator  = uiNavigator;
        this.gameConfig   = gameConfig;
    }

    public async UniTask StartAsync(CancellationToken cancellation)
    {
        var view = await uiNavigator.LoadAsync<IntroView>(CanvasType.Hud, gameConfig.uiIntroView);
        AddCache(ResourceType.Addressable, view.gameObject);

        view.InjectPresenter<IntroViewModel>()
            .Initialize(view, new IntroModel());

        await UniTask.WaitUntil(
            () => view.VisibleState == VisibleState.Disappeared,
            cancellationToken: cancellation);
    }
}
