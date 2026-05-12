using UnityEngine;

namespace RottenNoble.Core
{
    /// <summary>
    /// 씬 이름 설정 ScriptableObject
    ///
    /// Assets 우클릭 → Create → RottenNoble/Config → Scene Config 로 생성
    /// AppLifetimeScope Inspector에 연결 → SceneLoader에 주입
    /// </summary>
    [CreateAssetMenu(fileName = "SceneConfig", menuName = "RottenNoble/Config/Scene Config")]
    public class SceneConfigSO : ScriptableObject
    {
        [Header("[ Scene Names ]")]
        public string sceneBootstrap = "BootStrapper";
        public string sceneSplash    = "Splash";
        public string scenePatch     = "Patch";
        public string sceneIntro     = "Intro";
        public string sceneDataLoad  = "DataLoad";
        public string sceneMainMenu  = "MainMenu";
        public string sceneInGame    = "InGame";
    }
}
