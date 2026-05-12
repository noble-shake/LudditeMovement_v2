using UnityEngine;

using VContainer;
using VContainer.Unity;

using RottenNoble.Core;
using RottenNoble.Core.Input;
using RottenNoble.Core.Resource;
using RottenNoble.Core.Services;
using RottenNoble.Core.UI;
using RottenNoble.MainMenu.StageSelect;

/// <summary>
/// 앱 전체 생명주기 루트 스코프
/// VContainer ProjectSettings → RootLifetimeScope에 등록
/// 씬 전환 시에도 파괴되지 않고 유지됨
/// </summary>
public class AppLifetimeScope : LifetimeScope
{
    [Header("[ Config ]")]
    [SerializeField] SceneConfigSO sceneConfig;
    [SerializeField] UIConfigSO    uiConfig;
    [SerializeField] AppConfigSO   appConfig;

    [Header("[ Resource ]")]
    [SerializeField] HeroResourceSO  heroResource;
    [SerializeField] EnemyResourceSO enemyResource;
    [SerializeField] StageResourceSO stageResource;

    [Header("[ UI ]")]
    [SerializeField] UIManager uiManager;

    protected override void Configure(IContainerBuilder builder)
    {
        // ── Config SO ────────────────────────────────
        builder.RegisterInstance(sceneConfig);
        builder.RegisterInstance(uiConfig);
        builder.RegisterInstance(appConfig);

        // ── Resource SO ───────────────────────────────
        builder.RegisterInstance(heroResource);
        builder.RegisterInstance(enemyResource);
        builder.RegisterInstance(stageResource);

        // ── UI ───────────────────────────────────────
        builder.RegisterComponentInHierarchy<UIManager>();
        builder.Register<UINavigator>(Lifetime.Singleton);

        // ── 데이터 관리 ──────────────────────────────
        builder.Register<ResourceManager>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
        builder.Register<ResourceFactory>(Lifetime.Singleton);
        builder.Register<SceneLoader>(Lifetime.Singleton);
        builder.Register<DataManager>(Lifetime.Singleton);

        // ── 입력 ─────────────────────────────────────
        builder.Register<InputManager>(Lifetime.Singleton);

        // ── 공유 서비스 ───────────────────────────────
        builder.Register<SaveDataService>(Lifetime.Singleton);
        builder.Register<AudioService>(Lifetime.Singleton);

        // ── Addressables 기반 서비스 ─────────────────
        builder.Register<PatchService>(Lifetime.Singleton);
        builder.Register<DataLoadService>(Lifetime.Singleton);

        // ── MainMenu 서비스 ──────────────────────────
        // ViewBase.InjectPresenter<T>()가 AppLifetimeScope의 resolver를 사용하므로
        // ViewModel에서 주입받는 서비스는 여기에 등록합니다.
        builder.Register<StageSelectService>(Lifetime.Singleton);

        // ── Prefab 기반의 서비스 ──────────────────────────
        builder.RegisterComponentInNewPrefab<UIManager>(uiManager, Lifetime.Singleton).DontDestroyOnLoad();
    }
}
