using System.Threading;

using Cysharp.Threading.Tasks;
using VContainer.Unity;

using RottenNoble.Core;
using RottenNoble.Core.UI;
using RottenNoble.DataLoad.UI;

/// <summary>
/// DataLoad 씬 EntryPoint — DataLoadView 로드.
/// 리소스 로드 · 페이드아웃 · 씬 전환은 DataLoadViewModel이 처리합니다.
/// </summary>
public class DataLoadEntryPoint : EntryPointBase, IAsyncStartable
{
    public async UniTask StartAsync(CancellationToken cancellation)
    {
        await uiNavigator.LoadAsync<DataLoadView, DataLoadViewModel, DataLoadModel>(
            path:       dataManager.UIConfig.uiDataLoadView,
            model:      new DataLoadModel(),
            canvasType: CanvasType.Hud,
            onComplete: () => dataManager.ScenePath.LoadMainMenuAsync());
    }
}
