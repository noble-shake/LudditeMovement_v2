using UnityEngine;

namespace RottenNoble.Core
{
    /// <summary>
    /// 앱 전역 런타임 설정 ScriptableObject
    ///
    /// Assets 우클릭 → Create → RottenNoble/Config → App Config 로 생성
    /// AppLifetimeScope Inspector에 연결 → VContainer로 전역 주입
    ///
    /// ┌────────────────────────────────────────────────────────────────────┐
    /// │  Environment  │  어떤 CDN / 서버를 바라볼지 결정                  │
    /// │  CDN          │  Base URL + 경로 조합으로 최종 URL 계산            │
    /// │  Platform     │  빌드 타겟별 스토어 URL · 최소 버전 오버라이드    │
    /// │  Debug / QA   │  씬 스킵, 강제 패치, 로컬 더미 데이터 등         │
    /// └────────────────────────────────────────────────────────────────────┘
    /// </summary>
    [CreateAssetMenu(fileName = "AppConfig", menuName = "RottenNoble/Config/App Config")]
    public class AppConfigSO : ScriptableObject
    {
        // ── Environment ───────────────────────────────────────────────────

        [Header("[ Environment ]")]
        [Tooltip("현재 실행 환경. CDN URL 선택에 사용됩니다.")]
        public AppEnvironment environment = AppEnvironment.Development;

        // ── CDN ───────────────────────────────────────────────────────────

        [Header("[ CDN Base URLs ]")]
        public string devCdnUrl     = "https://dev-cdn.example.com";
        public string stagingCdnUrl = "https://stg-cdn.example.com";
        public string prodCdnUrl    = "https://cdn.example.com";

        [Header("[ CDN Paths ]")]
        public string patchPath   = "/patch";
        public string assetPath   = "/assets";
        public string catalogPath = "/catalog";

        // ── Platform ──────────────────────────────────────────────────────

        [Header("[ Platform ]")]
        public PlatformConfig android = new();
        public PlatformConfig ios     = new();
        public PlatformConfig pc      = new();

        // ── Debug / QA ────────────────────────────────────────────────────

        [Header("[ Debug / QA ]")]
        [Tooltip("Splash 씬을 건너뛰고 Patch 씬부터 시작합니다")]
        public bool skipSplash   = false;

        [Tooltip("Patch 씬을 건너뛰고 Intro 씬부터 시작합니다")]
        public bool skipPatch    = false;

        [Tooltip("Intro 씬을 건너뛰고 DataLoad 씬부터 시작합니다")]
        public bool skipIntro    = false;

        [Tooltip("패치 버전과 무관하게 항상 패치를 강제 실행합니다")]
        public bool forcePatch   = false;

        [Tooltip("CDN 대신 로컬 더미 데이터로 DataLoad를 대체합니다")]
        public bool mockDataLoad = false;

        // ── 계산 프로퍼티 ─────────────────────────────────────────────────

        /// <summary>현재 Environment에 해당하는 CDN Base URL</summary>
        public string CdnBaseUrl => environment switch
        {
            AppEnvironment.Staging    => stagingCdnUrl,
            AppEnvironment.Production => prodCdnUrl,
            _                         => devCdnUrl,
        };

        public string CdnPatchUrl   => CdnBaseUrl + patchPath;
        public string CdnAssetUrl   => CdnBaseUrl + assetPath;
        public string CdnCatalogUrl => CdnBaseUrl + catalogPath;

        /// <summary>현재 빌드 플랫폼에 해당하는 PlatformConfig</summary>
        public PlatformConfig CurrentPlatform
        {
            get
            {
#if UNITY_ANDROID
                return android;
#elif UNITY_IOS
                return ios;
#else
                return pc;
#endif
            }
        }

        /// <summary>Development 환경 여부</summary>
        public bool IsDevelopment => environment == AppEnvironment.Development;

        /// <summary>Production 환경 여부</summary>
        public bool IsProduction  => environment == AppEnvironment.Production;
    }

    // ── 보조 타입 ─────────────────────────────────────────────────────────

    public enum AppEnvironment
    {
        Development,
        Staging,
        Production,
    }

    [System.Serializable]
    public class PlatformConfig
    {
        [Tooltip("비어있으면 AppConfigSO.CdnBaseUrl 사용")]
        public string cdnBaseUrlOverride;

        [Tooltip("앱 스토어 / 스팀 페이지 URL")]
        public string storeUrl;

        [Tooltip("지원 최소 앱 버전 (예: 1.0.0). 서버 응답과 비교에 사용.")]
        public string minimumVersion = "1.0.0";

        /// <summary>오버라이드 URL이 있으면 반환, 없으면 null (호출부에서 CdnBaseUrl 사용)</summary>
        public string GetCdnBaseUrl(string fallback)
            => string.IsNullOrEmpty(cdnBaseUrlOverride) ? fallback : cdnBaseUrlOverride;
    }
}
