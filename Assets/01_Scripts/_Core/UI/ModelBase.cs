using System;

using R3;

namespace RottenNoble.Core.UI
{
    /// <summary>
    /// ViewModel이 구독할 수 있는 프로퍼티 변경 알림을 제공하는 Model 기반 클래스
    /// </summary>
    [Serializable]
    public abstract class ModelBase
    {
        [NonSerialized] public readonly Subject<string> OnPropertyChanged = new();

        protected void NotifyChanged(string propertyName)
            => OnPropertyChanged.OnNext(propertyName);

        public static implicit operator bool(ModelBase m) => m != null;
    }
}
