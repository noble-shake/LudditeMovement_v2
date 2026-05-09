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
    readonly SceneLoader     _sceneLoader;
    readonly SaveDataService _saveData;

    [Inject]
    public BootstrapEntryPoint(SceneLoader sceneLoader, SaveDataService saveData)
    {
        _sceneLoader = sceneLoader;
        _saveData    = saveData;
    }

    public async UniTask StartAsync(CancellationToken cancellation)
    {
        await _saveData.LoadAsync();
        await _sceneLoader.LoadSplashAsync();
    }
}
