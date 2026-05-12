using System.Linq;
using UnityEngine;

using Cysharp.Threading.Tasks;
using R3;
using VContainer;

namespace RottenNoble.Core.UI
{
    /// <summary>
    /// 모든 UI View의 기반 클래스.
    ///
    /// ┌─────────────────────────────────────────────────────────────────────┐
    /// │  VisibleState 전환은 ViewBase가 전담합니다.                         │
    /// │  서브클래스는 OnShowAsync / OnHideAsync 만 override 하세요.         │
    /// │                                                                     │
    /// │  ShowAsync()     → OnShowAsync() [기본: SetActive(true)] → Appeared │
    /// │  ShowImmediate() → OnShowImmediate() [기본: SetActive(true)]        │
    /// │  HideAsync()     → OnHideAsync() [기본: SetActive(false)] → Disapp. │
    /// │  HideImmediate() → OnHideImmediate() [기본: SetActive(false)]       │
    /// │                                                                     │
    /// │  페이드 등 커스텀 애니메이션이 있는 View는 OnShow/OnHideAsync를     │
    /// │  override하여 SetActive 타이밍을 직접 제어하세요.                   │
    /// └─────────────────────────────────────────────────────────────────────┘
    /// </summary>
    public abstract class ViewBase : MonoBehaviour
    {
        // ── 상태 ─────────────────────────────────────────────────────────

        /// <summary>현재 가시성 상태. ViewBase 내부에서만 변경됩니다.</summary>
        [field: SerializeField]
        public VisibleState VisibleState { get; private set; } = VisibleState.None;

        protected DisposableBag disposableBag = new();
        IObjectResolver _objectResolver;

        // ── Unity 생명주기 ────────────────────────────────────────────────

        [Inject]
        void InjectCores(IObjectResolver objectResolver)
            => _objectResolver = objectResolver;

        protected virtual void Awake()
        {
            var rt = transform as RectTransform;
            if (rt != null) rt.anchoredPosition = Vector2.zero;
            gameObject.SetActive(false);
        }

        protected virtual void OnDestroy()
            => disposableBag.Dispose();

        // ── Public API (sealed — VisibleState 전환 전담) ──────────────────

        /// <summary>View를 표시합니다. 완료 시 VisibleState = Appeared.</summary>
        public async UniTask ShowAsync()
        {
            VisibleState = VisibleState.Appearing;
            await OnShowAsync();
            VisibleState = VisibleState.Appeared;
        }

        /// <summary>View를 즉시 표시합니다.</summary>
        public void ShowImmediate()
        {
            VisibleState = VisibleState.Appearing;
            OnShowImmediate();
            VisibleState = VisibleState.Appeared;
        }

        /// <summary>View를 숨깁니다. 완료 시 VisibleState = Disappeared.</summary>
        public async UniTask HideAsync()
        {
            VisibleState = VisibleState.Disappearing;
            await OnHideAsync();
            VisibleState = VisibleState.Disappeared;
        }

        /// <summary>View를 즉시 숨깁니다.</summary>
        public void HideImmediate()
        {
            VisibleState = VisibleState.Disappearing;
            OnHideImmediate();
            VisibleState = VisibleState.Disappeared;
        }

        // ── 애니메이션 override 진입점 ────────────────────────────────────

        /// <summary>
        /// 등장 처리. SetActive(true) 포함.
        /// 기본 동작: gameObject 활성화만 수행.
        /// 페이드 등 커스텀 연출이 필요한 경우 override하여 SetActive 타이밍을 직접 제어하세요.
        /// </summary>
        protected virtual UniTask OnShowAsync()
        {
            gameObject.SetActive(true);
            return UniTask.CompletedTask;
        }

        /// <summary>즉시 표시 처리. 기본 동작: gameObject 활성화.</summary>
        protected virtual void OnShowImmediate()
            => gameObject.SetActive(true);

        /// <summary>
        /// 퇴장 처리. SetActive(false) 포함.
        /// 기본 동작: gameObject 비활성화만 수행.
        /// 페이드 등 커스텀 연출이 필요한 경우 override하여 SetActive 타이밍을 직접 제어하세요.
        /// </summary>
        protected virtual UniTask OnHideAsync()
        {
            gameObject.SetActive(false);
            return UniTask.CompletedTask;
        }

        /// <summary>즉시 숨김 처리. 기본 동작: gameObject 비활성화.</summary>
        protected virtual void OnHideImmediate()
            => gameObject.SetActive(false);

        // ── UINavigator 콜백 ──────────────────────────────────────────────

        /// <summary>ShowAsync / ShowImmediate 완료 후 UINavigator가 호출합니다.</summary>
        public virtual void OnReveal() { }

        /// <summary>HideAsync / HideImmediate 완료 후 UINavigator가 호출합니다.</summary>
        public virtual void OnHide() { }

        // ── 초기화 ───────────────────────────────────────────────────────

        /// <summary>UINavigator.LoadAsync 시 최초 1회 호출됩니다.</summary>
        public virtual void Initialize(params object[] parameters) { }

        // ── VContainer ───────────────────────────────────────────────────

        /// <summary>
        /// 이 GameObject에 T 컴포넌트를 추가(또는 가져와)하고 VContainer로 주입한 뒤 반환합니다.
        /// </summary>
        public T InjectPresenter<T>() where T : ViewModelCore
        {
            var vm = gameObject.GetComponent<T>() ?? gameObject.AddComponent<T>();
            _objectResolver.Inject(vm);
            return vm;
        }

        public static T Get<T>(string objectName) where T : ViewBase
        {
            return FindObjectsByType<ViewBase>(FindObjectsInactive.Include, FindObjectsSortMode.None)
                   .FirstOrDefault(v => v.name == objectName) as T;
        }
    }
}
