using UnityEngine;

using Cysharp.Threading.Tasks;

using RottenNoble.Core.Resource;

namespace RottenNoble.Core.UI
{
    /// <summary>
    /// UI 로드 → Canvas 배치 → 초기화를 하나의 호출로 처리하는 중재자.
    ///
    /// EntryPoint / ViewModel 어디서든 Canvas 레이어를 지정해
    /// View를 올바른 위치에 올릴 수 있습니다.
    ///
    ///   var view = await ui.LoadAsync&lt;SplashView&gt;(CanvasType.Hud, config.uiSplashView);
    ///   view.InjectPresenter&lt;SplashViewModel&gt;().Initialize(view, new SplashModel());
    ///
    /// AppLifetimeScope에 Singleton 등록 → EntryPointBase / ViewModelCore 경유 주입
    /// </summary>
    public class UINavigator
    {
        readonly ResourceFactory resourceFactory;
        readonly UIManager       uiManager;

        public UINavigator(ResourceFactory resourceFactory, UIManager uiManager)
        {
            this.resourceFactory = resourceFactory;
            this.uiManager       = uiManager;
        }

        // ── Load ─────────────────────────────────────────────────

        /// <summary>
        /// Addressable 키로 View를 로드하고 지정 Canvas 레이어에 배치합니다.
        /// InjectPresenter / ShowAsync 호출은 호출자가 담당합니다.
        /// </summary>
        public UniTask<T> LoadAsync<T>(CanvasType layer, string addressableKey)
            where T : ViewBase
            => LoadAsync<T>(layer, ResourceType.Addressable, addressableKey);

        /// <summary>
        /// 리소스 타입을 지정해 View를 로드하고 지정 Canvas 레이어에 배치합니다.
        /// </summary>
        public async UniTask<T> LoadAsync<T>(CanvasType layer, ResourceType resourceType, string key)
            where T : ViewBase
        {
            var go   = await resourceFactory.CreateAsync<GameObject>(resourceType, key);
            uiManager.Attach(layer, go);

            var view = go.GetComponent<T>();
            view.Initialize();
            return view;
        }

        // ── Show ─────────────────────────────────────────────────

        /// <summary>
        /// LoadAsync 후 즉시 ShowAsync까지 실행합니다.
        /// ViewModel 없이 단순 표시만 필요한 팝업에 사용합니다.
        /// </summary>
        public async UniTask<T> ShowAsync<T>(CanvasType layer, string addressableKey)
            where T : ViewBase
        {
            var view = await LoadAsync<T>(layer, addressableKey);
            await view.ShowAsync();
            return view;
        }

        public async UniTask<T> ShowAsync<T>(CanvasType layer, ResourceType resourceType, string key)
            where T : ViewBase
        {
            var view = await LoadAsync<T>(layer, resourceType, key);
            await view.ShowAsync();
            return view;
        }
    }
}
