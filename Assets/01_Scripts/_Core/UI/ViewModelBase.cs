using System;
using System.Linq;
using UnityEngine;

using Cysharp.Threading.Tasks;

namespace RottenNoble.Core.UI
{
    /// <summary>
    /// 제네릭 ViewModel 기반 클래스.
    /// TView : ViewBase 파생,  TModel : ModelBase 파생
    /// </summary>
    public class ViewModelBase<TView, TModel> : ViewModelCore
        where TView  : ViewBase
        where TModel : ModelBase
    {
        public    TView  View  { get; private set; }
        protected TModel Model { get; private set; }

        /// <summary>
        /// View 시퀀스 완료 후 실행할 액션 (씬 전환 등).
        /// UINavigator.LoadAsync의 onComplete 파라미터로 주입됩니다.
        /// Model이 아닌 ViewModel 레벨에서 관리합니다.
        /// </summary>
        public Func<UniTask> OnComplete { get; set; }

        public virtual async UniTask Initialize(TView view, TModel model)
        {
            View  = view;
            Model = model;
        }

        /// <summary>모델만 교체합니다. 구독 재설정 없이 Model 참조만 갱신합니다.</summary>
        public void UpdateModel(TModel model) => Model = model;

        public static T Get<T>(string objectName) where T : ViewModelBase<TView, TModel>
        {
            return FindObjectsByType<ViewModelBase<TView, TModel>>(
                       FindObjectsInactive.Include)
                   .FirstOrDefault(v => v.name == objectName) as T;
        }
    }
}
