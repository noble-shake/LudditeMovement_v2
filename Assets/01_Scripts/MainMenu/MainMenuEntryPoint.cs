using System.Threading;

using Cysharp.Threading.Tasks;
using VContainer;
using VContainer.Unity;

using RottenNoble.Core;
using RottenNoble.Core.Resource;
using RottenNoble.Core.UI;
using RottenNoble.MainMenu.UI;

/// <summary>
/// MainMenu 씬 EntryPoint — MainMenuView 로드 → MainMenuViewModel에 위임
/// </summary>
public class MainMenuEntryPoint : EntryPointBase, IAsyncStartable
{
    readonly GameConfig gameConfig;

    [Inject]
    public MainMenuEntryPoint(DataManager dataManager, UINavigator uiNavigator, GameConfig gameConfig)
    {
        this.dataManager  = dataManager;
        this.uiNavigator  = uiNavigator;
        this.gameConfig   = gameConfig;
    }

    public async UniTask StartAsync(CancellationToken cancellation)
    {
        var view = await uiNavigator.LoadAsync<MainMenuView>(CanvasType.Hud, gameConfig.uiMainMenuView);
        AddCache(ResourceType.Addressable, view.gameObject);

        view.InjectPresenter<MainMenuViewModel>()
            .Initialize(view, new MainMenuModel());

        await UniTask.WaitWhile(
            () => view != null && view.VisibleState != VisibleState.Disappeared,
            cancellationToken: cancellation);
    }
}
