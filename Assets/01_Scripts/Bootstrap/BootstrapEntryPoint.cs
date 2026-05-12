using System.Threading;

using Cysharp.Threading.Tasks;
using VContainer;
using VContainer.Unity;

using RottenNoble.Core;

/// <summary>
/// BootStrapper 씬 진입점 — 앱 최초 실행 시 한 번만 동작, 완료 후 Splash로 전환
/// </summary>
public class BootstrapEntryPoint : IAsyncStartable
{
    readonly DataManager    dataManager;
    readonly SaveDataService saveDataService;

    [Inject]
    public BootstrapEntryPoint(DataManager dataManager, SaveDataService saveDataService)
    {
        this.dataManager     = dataManager;
        this.saveDataService = saveDataService;
    }

    public async UniTask StartAsync(CancellationToken cancellation)
    {
        await saveDataService.LoadAsync();
        await dataManager.ScenePath.LoadSplashAsync();
    }
}
