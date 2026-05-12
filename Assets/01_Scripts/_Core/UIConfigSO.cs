using UnityEngine;

namespace RottenNoble.Core
{
    /// <summary>
    /// UI 프리팹 Addressable 키 설정 ScriptableObject
    ///
    /// Assets 우클릭 → Create → RottenNoble/Config → UI Config 로 생성
    /// AppLifetimeScope Inspector에 연결 → 각 EntryPoint에 주입
    /// </summary>
    [CreateAssetMenu(fileName = "UIConfig", menuName = "RottenNoble/Config/UI Config")]
    public class UIConfigSO : ScriptableObject
    {
        [Header("[ UI Prefab Keys (Addressable) ]")]
        public string uiSplashView   = "UI.Splash";
        public string uiPatchView    = "UI.Patch";
        public string uiIntroView    = "UI.Intro";
        public string uiDataLoadView = "UI.DataLoad";
        public string uiMainMenuView = "UI.MainMenu";
    }
}
