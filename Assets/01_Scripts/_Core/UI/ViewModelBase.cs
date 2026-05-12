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

        public virtual async UniTask Initialize(TView view, TModel model)
        {
            View  = view;
            Model = model;
        }

        public static T Get<T>(string objectName) where T : ViewModelBase<TView, TModel>
        {
            return FindObjectsByType<ViewModelBase<TView, TModel>>(
                       FindObjectsInactive.Include)
                   .FirstOrDefault(v => v.name == objectName) as T;
        }
    }
}
