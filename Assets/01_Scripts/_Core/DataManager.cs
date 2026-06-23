using RottenNoble.Core.Resource;

namespace RottenNoble.Core
{
    /// <summary>
    /// 앱 전역 데이터 진입점.
    ///
    ///   DataManager.ScenePath    — 씬 전환 (SceneLoader)
    ///   DataManager.ResourcePath — 프리팹 생성/해제 (ResourceFactory)
    ///   DataManager.UIConfig     — UI Prefab Addressable 키
    ///   DataManager.SceneConfig  — 씬 이름 목록
    ///   DataManager.AppConfig    — CDN URL, 환경, 디버그 플래그
    ///   DataManager.HeroResource / EnemyResource / StageResource — 리소스 SO
    ///   DataManager.Color — 난이도 색상 테마 (ColorManager)
    ///
    /// SO 직접 주입 지양 — 항상 DataManager를 통해 접근
    /// </summary>
    public class DataManager
    {
        // ── 씬 / 리소스 ──────────────────────────────────
        public SceneLoader     ScenePath    { get; }
        public ResourceFactory ResourcePath { get; }

        // ── Config SO ────────────────────────────────────
        public SceneConfigSO   SceneConfig  { get; }
        public UIConfigSO      UIConfig     { get; }
        public AppConfigSO     AppConfig    { get; }

        // ── Resource SO ──────────────────────────────────
        public HeroResourceSO  HeroResource  { get; }
        public EnemyResourceSO EnemyResource { get; }
        public StageResourceSO StageResource { get; }

        // ── 색상 테마 ─────────────────────────────────────
        public ColorManager       Color       { get; }

        // ── 설정 저장 ─────────────────────────────────────
        public PlayerPrefsManager PlayerPrefs { get; }

        public DataManager(
            SceneLoader     scene,
            ResourceFactory resource,
            SceneConfigSO   sceneConfig,
            UIConfigSO      uiConfig,
            AppConfigSO     appConfig,
            HeroResourceSO  heroResource,
            EnemyResourceSO enemyResource,
            StageResourceSO stageResource,
            ColorManager      colorManager,
            PlayerPrefsManager playerPrefsManager)
        {
            ScenePath    = scene;
            ResourcePath = resource;
            SceneConfig  = sceneConfig;
            UIConfig     = uiConfig;
            AppConfig    = appConfig;
            HeroResource  = heroResource;
            EnemyResource = enemyResource;
            StageResource = stageResource;
            Color         = colorManager;
            PlayerPrefs   = playerPrefsManager;
        }
    }
}
