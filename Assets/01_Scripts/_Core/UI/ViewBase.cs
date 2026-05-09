using System;
using System.Linq;
using UnityEngine;

using Cysharp.Threading.Tasks;
using R3;
using VContainer;

namespace RottenNoble.Core.UI
{
    /// <summary>
    /// 모든 UI View의 기반 클래스.
    /// ShowAsync / HideAsync 애니메이션 계약 + VContainer 인젝션 진입점 제공.
    /// </summary>
    public abstract class ViewBase : MonoBehaviour
    {
        [field: SerializeField]
        public VisibleState VisibleState { get; protected set; } = VisibleState.None;

        protected DisposableBag disposableBag = new();

        IObjectResolver _objectResolver;

        [Inject]
        void InjectCores(IObjectResolver objectResolver)
            => _objectResolver = objectResolver;

        protected virtual void Awake()
        {
            var rt = transform as RectTransform;
            if (rt != null) rt.anchoredPosition = Vector2.zero;
            gameObject.SetActive(false);
        }

        protected virtual void OnDestroy() { }

        public abstract UniTask ShowAsync(Action onComplete = null);
        public abstract void    ShowImmediate();
        public abstract UniTask HideAsync(Action onComplete = null);
        public abstract void    HideImmediate();

        public virtual void Initialize(params object[] parameters) { }

        /// <summary>
        /// 이 GameObject에 T 컴포넌트를 추가(또는 가져와)하고 VContainer로 인젝션한 뒤 반환합니다.
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
