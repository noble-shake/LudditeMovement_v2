using System.Threading;

using Cysharp.Threading.Tasks;
using VContainer;
using VContainer.Unity;

using RottenNoble.Core;
using RottenNoble.Core.Resource;
using RottenNoble.Core.UI;
using RottenNoble.Patch.UI;

/// <summary>
/// Patch 씬 EntryPoint — PatchView 로드 → PatchViewModel에 위임
/// </summary>
public class PatchEntryPoint : EntryPointBase, IAsyncStartable
{
    readonly GameConfig gameConfig;

    [Inject]
    public PatchEntryPoint(DataManager dataManager, UINavigator uiNavigator, GameConfig gameConfig)
    {
        this.dataManager  = dataManager;
        this.uiNavigator  = uiNavigator;
        this.gameConfig   = gameConfig;
    }

    public async UniTask StartAsync(CancellationToken cancellation)
    {
        var view = await uiNavigator.LoadAsync<PatchView>(CanvasType.Hud, gameConfig.uiPatchView);
        AddCache(ResourceType.Addressable, view.gameObject);

        view.InjectPresenter<PatchViewModel>()
            .Initialize(view, new PatchModel());

        await UniTask.WaitUntil(
            () => view.VisibleState == VisibleState.Disappeared,
            cancellationToken: cancellation);
    }
}
