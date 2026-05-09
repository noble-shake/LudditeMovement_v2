using UnityEngine;

namespace RottenNoble.Core
{
    /// <summary>
    /// 프로젝트 전역 설정 ScriptableObject
    ///
    /// Assets 우클릭 → Create → RottenNoble → Game Config 로 생성
    /// AppLifetimeScope Inspector에 연결 → VContainer로 전역 주입
    /// </summary>
    [CreateAssetMenu(fileName = "GameConfig", menuName = "RottenNoble/Game Config")]
    public class GameConfig : ScriptableObject
    {
        [Header("[ Scene Names ]")]
        public string sceneBootstrap = "BootStrapper";
        public string sceneSplash    = "Splash";
        public string scenePatch     = "Patch";
        public string sceneIntro     = "Intro";
        public string sceneDataLoad  = "DataLoad";
        public string sceneMainMenu  = "MainMenu";
        public string sceneInGame    = "InGame";

        [Header("[ UI Prefab Keys (Addressable) ]")]
        public string uiSplashView   = "UI.Splash";
        public string uiPatchView    = "UI.Patch";
        public string uiIntroView    = "UI.Intro";
        public string uiDataLoadView = "UI.DataLoad";
        public string uiMainMenuView = "UI.MainMenu";
    }
}
