using System;
using System.Collections.Generic;

using Cysharp.Threading.Tasks;

using RottenNoble.Core.Resource;

namespace RottenNoble.Core.UI
{
    /// <summary>
    /// View 로드 · 캐싱 · 표시 · 숨김 · 삭제를 통합 관리합니다.
    ///
    /// ┌────────────────────────────────────────────────────────────────────────────┐
    /// │  단계             │  설명                                                  │
    /// ├────────────────────────────────────────────────────────────────────────────┤
    /// │  LoadAsync        │  로드 + ViewModel/Model 주입 + Canvas 배치             │
    /// │                   │  캐시에 없을 때만 Initialize 호출. ViewModel 반환.     │
    /// │  ShowAsync(false) │  view.ShowAsync() 완료 → OnReveal() → onReveal cb     │
    /// │  ShowAsync(true)  │  view.ShowImmediate() → OnReveal() → onReveal cb      │
    /// │  HideAsync(false) │  view.HideAsync() 완료 → OnComplete() → onComplete cb │
    /// │  HideAsync(true)  │  view.HideImmediate() → OnComplete() → onComplete cb  │
    /// │  Destroy          │  Addressable 해제 + 캐시 제거 (다음 Load 시 재Initialize) │
    /// └────────────────────────────────────────────────────────────────────────────┘
    ///
    /// EntryPoint 패턴:
    ///   var vm = await uiNavigator.LoadAsync&lt;SplashView, SplashViewModel, SplashModel&gt;(
    ///       path:       gameConfig.uiSplashView,
    ///       model:      new SplashModel(),
    ///       canvasType: CanvasType.Hud,
    ///       onReveal:   async () => { ... },
    ///       onComplete: async () => { ... });
    ///
    ///   await uiNavigator.ShowAsync&lt;SplashView&gt;();
    ///   AddUICache&lt;SplashView&gt;();
    ///
    ///   await UniTask.WaitUntil(() => vm.View.VisibleState == VisibleState.Disappeared,
    ///       cancellationToken: cancellation);
    /// </summary>
    public class UINavigator
    {
        // ── Inner cache entry ─────────────────────────────────────────────

        sealed class CachedEntry
        {
            public ResourceType  ResourceType { get; }
            public ViewBase      View         { get; }
            public Func<UniTask> OnReveal     { get; }
            public Func<UniTask> OnComplete   { get; }

            public CachedEntry(
                ResourceType  resourceType,
                ViewBase      view,
                Func<UniTask> onReveal,
                Func<UniTask> onComplete)
            {
                ResourceType = resourceType;
                View         = view;
                OnReveal     = onReveal;
                OnComplete   = onComplete;
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

        // ── Load (View + ViewModel + Model) ──────────────────────────────

        /// <summary>
        /// Addressable path에서 View를 로드하고 ViewModel · Model을 주입합니다.
        /// 이미 캐시에 있으면 재로드하지 않고 기존 ViewModel을 반환합니다.
        /// </summary>
        public async UniTask<TViewModel> LoadAsync<TView, TViewModel, TModel>(
            string        path,
            TModel        model,
            CanvasType    canvasType  = CanvasType.Hud,
            Func<UniTask> onReveal    = null,
            Func<UniTask> onComplete  = null)
            where TView      : ViewBase
            where TViewModel : ViewModelBase<TView, TModel>
            where TModel     : ModelBase
        {
            if (TryGetEntry<TView>(out var cached))
            {
                // 콜백만 갱신
                viewCache[typeof(TView)] = new CachedEntry(
                    cached.ResourceType, cached.View, onReveal, onComplete);
                return cached.View.gameObject.GetComponent<TViewModel>();
            }

            var go   = await resourceFactory.CreateAsync<UnityEngine.GameObject>(ResourceType.Addressable, path);
            uiManager.Attach(canvasType, go);

            var view = go.GetComponent<TView>();
            view.Initialize();

            var viewModel = view.InjectPresenter<TViewModel>();
            await viewModel.Initialize(view, model);

            viewCache[typeof(TView)] = new CachedEntry(ResourceType.Addressable, view, onReveal, onComplete);
            return viewModel;
        }

        // ── Load (View only) ─────────────────────────────────────────────

        /// <summary>
        /// ViewModel 주입 없이 View만 로드합니다.
        /// 기존 EntryPoint 패턴 또는 ViewModel이 필요 없는 뷰에 사용합니다.
        /// </summary>
        public async UniTask<TView> LoadAsync<TView>(
            string        path,
            CanvasType    canvasType   = CanvasType.Hud,
            ResourceType  resourceType = ResourceType.Addressable,
            Func<UniTask> onReveal     = null,
            Func<UniTask> onComplete   = null)
            where TView : ViewBase
        {
            if (TryGetEntry<TView>(out var cached))
            {
                viewCache[typeof(TView)] = new CachedEntry(
                    cached.ResourceType, cached.View, onReveal, onComplete);
                return (TView)cached.View;
            }

            var go   = await resourceFactory.CreateAsync<UnityEngine.GameObject>(resourceType, path);
            uiManager.Attach(canvasType, go);

            var view = go.GetComponent<TView>();
            view.Initialize();

            viewCache[typeof(TView)] = new CachedEntry(resourceType, view, onReveal, onComplete);
            return view;
        }

        // ── Show ──────────────────────────────────────────────────────────

        /// <summary>
        /// 캐시된 View를 표시합니다.
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
        /// 캐시된 View를 숨깁니다. GameObject는 유지됩니다 (다시 ShowAsync로 재표시 가능).
        /// immediate = true 이면 HideImmediate(), false 이면 HideAsync() 완료를 기다립니다.
        /// 완료 후 view.OnComplete() → onComplete 콜백 순서로 호출합니다.
        /// </summary>
        public async UniTask HideAsync<TView>(bool immediate = false) where TView : ViewBase
        {
            if (!TryGetEntry<TView>(out var entry)) return;

            if (immediate)
                entry.View.HideImmediate();
            else
                await entry.View.HideAsync();

            entry.View.OnComplete();
            if (entry.OnComplete != null)
                await entry.OnComplete.Invoke();
        }

        // ── Destroy (캐시 제거 + Addressable 해제) ────────────────────────

        /// <summary>
        /// 캐시를 제거하고 Addressable 인스턴스를 해제합니다.
        /// 다음 LoadAsync 호출 시 새로 로드하고 Initialize부터 재실행합니다.
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
