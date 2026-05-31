using System.Threading;

using Cysharp.Threading.Tasks;
using VContainer;
using VContainer.Unity;

using RottenNoble.Core;
using RottenNoble.Core.UI;
using RottenNoble.MainMenu.UI;

/// <summary>
/// MainMenu 씬 EntryPoint — 카메라 MapPan 모드 설정 후 MainMenuView 로드.
/// 탭 전환 및 스테이지 진입은 각 서브탭 ViewModel이 담당합니다.
/// </summary>
public class MainMenuEntryPoint : EntryPointBase, IAsyncStartable
{
    readonly CameraController cameraController;

    [Inject]
    public MainMenuEntryPoint(CameraController cameraController)
    {
        this.cameraController = cameraController;
    }

    public async UniTask StartAsync(CancellationToken cancellation)
    {
        cameraController.SetMode(CameraMode.MapPan);

        await uiNavigator.LoadAsync<MainMenuView, MainMenuViewModel, MainMenuModel>(
            path:       dataManager.UIConfig.uiMainMenuView,
            model:      new MainMenuModel(),
            canvasType: CanvasType.Hud);
    }
}
