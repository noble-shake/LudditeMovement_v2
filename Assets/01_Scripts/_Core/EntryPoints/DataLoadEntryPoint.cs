using System.Threading;

using Cysharp.Threading.Tasks;
using VContainer;
using VContainer.Unity;

using RottenNoble.Core;
using RottenNoble.Core.UI;
using RottenNoble.DataLoad.UI;

/// <summary>
/// DataLoad 씬 EntryPoint — DataLoadView 로드 → ViewModel/Model 주입 → 표시
/// </summary>
public class DataLoadEntryPoint : EntryPointBase, IAsyncStartable
{
    readonly UIConfigSO uiConfig;

    [Inject]
    public DataLoadEntryPoint(UIConfigSO uiConfig)
    {
        this.uiConfig = uiConfig;
    }

    public async UniTask StartAsync(CancellationToken cancellation)
    {
        var viewModel = await uiNavigator.LoadAsync<DataLoadView, DataLoadViewModel, DataLoadModel>(
            path:       uiConfig.uiDataLoadView,
            model:      new DataLoadModel(),
            canvasType: CanvasType.Hud);

        await uiNavigator.ShowAsync<DataLoadView>();
        AddUICache<DataLoadView>();

        await UniTask.WaitUntil(
            () => viewModel.View.VisibleState == VisibleState.Disappeared,
            cancellationToken: cancellation);
    }
}
