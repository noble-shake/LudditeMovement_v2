using System;
using System.Collections.Generic;

using Cysharp.Threading.Tasks;

using RottenNoble.Core.Resource;

namespace RottenNoble.Core.UI
{
    /// <summary>
    /// View 로드 · 캐싱 · 표시 · 숨김 · 삭제를 통합 관리합니다.
    ///
    /// ┌──────────────────────────────────────────────────────────────────────────────────┐
    /// │  단계              │  설명                                                       │
    /// ├──────────────────────────────────────────────────────────────────────────────────┤
    /// │  LoadAsync         │  로드 + Initialize + ShowAsync + OnReveal 콜백까지 처리.    │
    /// │                    │  캐시에 있으면 재초기화 없이 콜백만 갱신 후 즉시 반환.       │
    /// │  ShowAsync         │  숨겨진 View를 다시 표시 (LoadAsync 이후 re-show 전용).     │
    /// │  HideAsync(false)  │  view.HideAsync() → view.OnHide() → onHide 콜백.           │
    /// │  HideAsync(true)   │  view.HideImmediate() → view.OnHide() → onHide 콜백.       │
    /// │  Destroy           │  Addressable 해제 + 캐시 제거 (다음 Load 시 재Initialize). │
    /// └──────────────────────────────────────────────────────────────────────────────────┘
    ///
    /// EntryPoint 패턴:
    ///   await uiNavigator.LoadAsync&lt;SplashView, SplashViewModel, SplashModel&gt;(
    ///       path:       uiConfig.uiSplashView,
    ///       model:      new SplashModel { OnComplete = () => ... },
    ///       canvasType: CanvasType.Hud,
    ///       onReveal:   async () => { ... },   // ShowAsync 완료 직후 실행
    ///       onHide:     async () => { ... });  // HideAsync 완료 직후 실행
    ///
    ///   AddUICache&lt;SplashView&gt;();  // 씬 언로드 시 Destroy 예약
    /// </summary>
    public class UINavigator
    {
        // ── Inner cache entry ─────────────────────────────────────────────

        sealed class CachedEntry
        {
            public ResourceType  ResourceType { get; }
            public ViewBase      View         { get; }
            public Func<UniTask> OnReveal     { get; }
            public Func<UniTask> OnHide       { get; }

            public CachedEntry(
                ResourceType  resourceType,
                ViewBase      view,
                Func<UniTask> onReveal,
                Func<UniTask> onHide)
            {
                ResourceType = resourceType;
                View         = view;
                OnReveal     = onReveal;
                OnHide       = onHide;
            }
        }

        // ── Fields ────────────────────────────────────────────────────────

        readonly ResourceFactory               resourceFactory;
        readonly UIManager                     uiManager;
        readonly Dictionary<Type, CachedEntry> viewCache = new();

        public UINavigator(ResourceFactory resourceFactory, UIManager uiManager)
        {
            this.resourceFactory = resourceFactory;
            this.uiManager       = uiManager;
        }

        // ── Load + Show (View + ViewModel + Model) ───────────────────────

        /// <summary>
        /// Addressable에서 View를 로드하고 ViewModel · Model을 초기화한 뒤 ShowAsync까지 완료합니다.
        /// 캐시에 있으면 재로드 · 재초기화하지 않고 모델 · 콜백만 갱신 후 ShowAsync를 실행합니다.
        /// onComplete — View 시퀀스 완료 시 ViewModel이 호출하는 액션 (씬 전환 등).
        /// </summary>
        public async UniTask<TViewModel> LoadAsync<TView, TViewModel, TModel>(
            string        path,
            TModel        model,
            CanvasType    canvasType  = CanvasType.Hud,
            Func<UniTask> onComplete  = null,
            Func<UniTask> onReveal    = null,
            Func<UniTask> onHide      = null)
            where TView      : ViewBase
            where TViewModel : ViewModelBase<TView, TModel>
            where TModel     : ModelBase
        {
            if (TryGetEntry<TView>(out var cached))
            {
                // 모델·콜백 갱신 후 ShowAsync — Initialize는 건너뜀
                var cachedViewModel = cached.View.gameObject.GetComponent<TViewModel>();
                cachedViewModel.UpdateModel(model);
                cachedViewModel.OnComplete = onComplete;

                viewCache[typeof(TView)] = new CachedEntry(cached.ResourceType, cached.View, onReveal, onHide);

                await cached.View.ShowAsync();
                cached.View.OnReveal();
                if (onReveal != null) await onReveal.Invoke();

                return cachedViewModel;
            }

            var go = await resourceFactory.CreateAsync<UnityEngine.GameObject>(ResourceType.Addressable, path);
            uiManager.Attach(canvasType, go);

            var view = go.GetComponent<TView>();
            view.Initialize();

            var viewModel = view.InjectPresenter<TViewModel>();
            viewModel.OnComplete = onComplete;
            await viewModel.Initialize(view, model);

            var entry = new CachedEntry(ResourceType.Addressable, view, onReveal, onHide);
            viewCache[typeof(TView)] = entry;

            await view.ShowAsync();
            view.OnReveal();
            if (onReveal != null) await onReveal.Invoke();

            return viewModel;
        }

        // ── Load + Show (View only) ──────────────────────────────────────

        /// <summary>
        /// ViewModel 없이 View만 로드하고 ShowAsync까지 완료합니다.
        /// </summary>
        public async UniTask<TView> LoadAsync<TView>(
            string        path,
            CanvasType    canvasType   = CanvasType.Hud,
            ResourceType  resourceType = ResourceType.Addressable,
            Func<UniTask> onReveal     = null,
            Func<UniTask> onHide       = null)
            where TView : ViewBase
        {
            if (TryGetEntry<TView>(out var cached))
            {
                // 콜백 갱신 후 ShowAsync — 캐시 히트도 LoadAsync와 동일한 흐름
                viewCache[typeof(TView)] = new CachedEntry(cached.ResourceType, cached.View, onReveal, onHide);

                await cached.View.ShowAsync();
                cached.View.OnReveal();
                if (onReveal != null) await onReveal.Invoke();

                return (TView)cached.View;
            }

            var go = await resourceFactory.CreateAsync<UnityEngine.GameObject>(resourceType, path);
            uiManager.Attach(canvasType, go);

            var view = go.GetComponent<TView>();
            view.Initialize();

            var entry = new CachedEntry(resourceType, view, onReveal, onHide);
            viewCache[typeof(TView)] = entry;

            await view.ShowAsync();
            view.OnReveal();
            if (onReveal != null) await onReveal.Invoke();

            return view;
        }

        // ── Show (re-show: 숨겨진 View를 다시 표시) ──────────────────────

        /// <summary>
        /// HideAsync로 숨겨진 View를 다시 표시합니다. (LoadAsync 이후 re-show 전용)
        /// immediate = true 이면 ShowImmediate(), false 이면 ShowAsync() 완료를 기다립니다.
        /// 완료 후 view.OnReveal() → onReveal 콜백 순서로 호출합니다.
        /// </summary>
        public async UniTask ShowAsync<TView>(bool immediate = false) where TView : ViewBase
        {
            if (!TryGetEntry<TView>(out var entry)) return;

            if (immediate)
                entry.View.ShowImmediate();
            else
                await entry.View.ShowAsync();

            entry.View.OnReveal();
            if (entry.OnReveal != null)
                await entry.OnReveal.Invoke();
        }

        // ── Hide (캐시 유지) ──────────────────────────────────────────────

        /// <summary>
        /// 캐시된 View를 숨깁니다. GameObject는 유지됩니다 (ShowAsync로 재표시 가능).
        /// 완료 후 view.OnHide() → onHide 콜백 순서로 호출합니다.
        /// </summary>
        public async UniTask HideAsync<TView>(bool immediate = false) where TView : ViewBase
        {
            if (!TryGetEntry<TView>(out var entry)) return;

            if (immediate)
                entry.View.HideImmediate();
            else
                await entry.View.HideAsync();

            entry.View.OnHide();
            if (entry.OnHide != null)
                await entry.OnHide.Invoke();
        }

        // ── Destroy (캐시 제거 + Addressable 해제) ────────────────────────

        /// <summary>
        /// 캐시를 제거하고 Addressable 인스턴스를 해제합니다.
        /// 다음 LoadAsync 시 새로 로드하여 Initialize부터 재실행합니다.
        /// </summary>
        public void Destroy<TView>() where TView : ViewBase
        {
            var type = typeof(TView);
            if (!viewCache.TryGetValue(type, out var entry)) return;
            if (entry.View != null)
                resourceFactory.DeleteInstance(entry.ResourceType, entry.View.gameObject);
            viewCache.Remove(type);
        }

        public void DestroyAll()
        {
            foreach (var (_, entry) in viewCache)
                if (entry.View != null)
                    resourceFactory.DeleteInstance(entry.ResourceType, entry.View.gameObject);
            viewCache.Clear();
        }

        // ── Get ───────────────────────────────────────────────────────────

        public bool TryGet<TView>(out TView view) where TView : ViewBase
            => TryGetCached(out view);

        // ── Helper ────────────────────────────────────────────────────────

        bool TryGetCached<TView>(out TView view) where TView : ViewBase
        {
            if (viewCache.TryGetValue(typeof(TView), out var entry) && entry.View != null)
            {
                view = (TView)entry.View;
                return true;
            }
            view = null;
            return false;
        }

        bool TryGetEntry<TView>(out CachedEntry entry) where TView : ViewBase
        {
            if (viewCache.TryGetValue(typeof(TView), out entry) && entry.View != null)
                return true;
            entry = null;
            return false;
        }
    }
}
