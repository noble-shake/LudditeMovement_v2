using System.Threading;

using Cysharp.Threading.Tasks;
using VContainer;
using VContainer.Unity;

using RottenNoble.Core;
using RottenNoble.Core.Resource;
using RottenNoble.Core.UI;
using RottenNoble.DataLoad.UI;

/// <summary>
/// DataLoad 씬 EntryPoint — DataLoadView 로드 → DataLoadViewModel에 위임
/// </summary>
public class DataLoadEntryPoint : EntryPointBase, IAsyncStartable
{
    readonly GameConfig gameConfig;

    [Inject]
    public DataLoadEntryPoint(DataManager dataManager, UINavigator uiNavigator, GameConfig gameConfig)
    {
        this.dataManager  = dataManager;
        this.uiNavigator  = uiNavigator;
        this.gameConfig   = gameConfig;
    }

    public async UniTask StartAsync(CancellationToken cancellation)
    {
        var view = await uiNavigator.LoadAsync<DataLoadView>(CanvasType.Hud, gameConfig.uiDataLoadView);
        AddCache(ResourceType.Addressable, view.gameObject);

        view.InjectPresenter<DataLoadViewModel>()
            .Initialize(view, new DataLoadModel());

        await UniTask.WaitUntil(
            () => view.VisibleState == VisibleState.Disappeared,
            cancellationToken: cancellation);
    }
}
