using RottenNoble.Core.Resource;

namespace RottenNoble.Core
{
    /// <summary>
    /// 씬·리소스 접근을 하나의 진입점으로 묶는 관리자.
    ///
    ///   DataManager.Scene    — 씬 전환 (SceneLoader)
    ///   DataManager.Resource — 프리팹 생성/해제 (ResourceFactory)
    ///
    /// AppLifetimeScope에 Singleton 등록 → VContainer로 전역 주입
    /// </summary>
    public class DataManager
    {
        public SceneLoader     Scene    { get; }
        public ResourceFactory Resource { get; }

        public DataManager(SceneLoader scene, ResourceFactory resource)
        {
            Scene    = scene;
            Resource = resource;
        }
    }
}
