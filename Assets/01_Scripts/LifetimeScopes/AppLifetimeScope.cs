using VContainer;
using VContainer.Unity;

using RottenNoble.Core;
using RottenNoble.Core.Resource;
using RottenNoble.Core.Services;
using RottenNoble.MainMenu.StageSelect;

/// <summary>
/// 앱 전체 생명주기 루트 스코프
/// VContainer ProjectSettings → RootLifetimeScope에 등록
/// 씬 전환 시에도 파괴되지 않고 유지됨
/// </summary>
public class AppLifetimeScope : LifetimeScope
{
    protected override void Configure(IContainerBuilder builder)
    {
        // ── UI ───────────────────────────────────────
        // UIManager는 이 LifetimeScope GameObject의 자식 또는 씬 내 컴포넌트로 배치
        builder.RegisterComponentInHierarchy<UIManager>();

        // ── 리소스 관리 ──────────────────────────────
        builder.Register<ResourceManager>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
        builder.Register<ResourceFactory>(Lifetime.Singleton);

        // ── 씬 전환 전반에 걸쳐 공유되는 서비스 ──────
        builder.Register<SceneLoader>(Lifetime.Singleton);
        builder.Register<SaveDataService>(Lifetime.Singleton);
        builder.Register<AudioService>(Lifetime.Singleton);

        // ── Addressables 기반 서비스 ─────────────────
        builder.Register<PatchService>(Lifetime.Singleton);
        builder.Register<DataLoadService>(Lifetime.Singleton);

        // ── MainMenu 서비스 ──────────────────────────
        // ViewBase.InjectPresenter<T>()가 AppLifetimeScope의 resolver를 사용하므로
        // ViewModel에서 주입받는 서비스는 여기에 등록합니다.
        builder.Register<StageSelectService>(Lifetime.Singleton);
    }
}
