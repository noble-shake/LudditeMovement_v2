using UnityEngine;

using R3;
using VContainer;

using RottenNoble.Core.Services;
using System.Resources;

namespace RottenNoble.Core.UI
{
    /// <summary>
    /// 모든 ViewModel의 공통 기반 클래스.
    /// 핵심 서비스를 VContainer로 주입받고 DisposableBag 수명을 관리합니다.
    /// </summary>
    public class ViewModelCore : MonoBehaviour
    {
        public DisposableBag disposableBag = new();

        protected DataManager  dataManager;
        protected UINavigator  uiNavigator;
        protected AudioService audioService;
        protected ResourceManager resourceManager;
        // TODO: InputManager 구현 후에 넣어서 등록 할 것.
        // protected InputManager inputManager;

        [Inject]
        void InjectCores(
            DataManager dataManager,
         UINavigator uiNavigator,
          AudioService audioService,
           ResourceManager resourceManager
        //    InputManager inputManager,
        )
        {
            this.dataManager  = dataManager;
            this.uiNavigator  = uiNavigator;
            this.audioService = audioService;
            this.resourceManager = resourceManager;
            // this.inputManager = inputManager;
        }

        protected virtual void OnDestroy()
        {
            disposableBag.Clear();
        }
    }
}
