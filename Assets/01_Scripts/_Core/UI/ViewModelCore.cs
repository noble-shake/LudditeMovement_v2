using UnityEngine;

using R3;
using VContainer;

using RottenNoble.Core.Resource;

namespace RottenNoble.Core.UI
{
    /// <summary>
    /// 모든 ViewModel의 공통 기반 클래스.
    /// 핵심 서비스를 VContainer로 주입받고 DisposableBag 수명을 관리합니다.
    /// </summary>
    public class ViewModelCore : MonoBehaviour
    {
        public DisposableBag disposableBag = new();

        protected SceneLoader     sceneLoader;
        protected ResourceFactory resourceFactory;
        protected AudioService    audioService;

        [Inject]
        void InjectCores(
            SceneLoader     sceneLoader,
            ResourceFactory resourceFactory,
            AudioService    audioService)
        {
            this.sceneLoader     = sceneLoader;
            this.resourceFactory = resourceFactory;
            this.audioService    = audioService;
        }

        protected virtual void OnDestroy()
        {
            disposableBag.Clear();
        }
    }
}
