using System;

using Cysharp.Threading.Tasks;

using RottenNoble.Core.UI;

namespace RottenNoble.Splash.UI
{
    /// <summary>
    /// Splash 씬 데이터 모델.
    /// OnComplete — 로고 연출이 끝난 뒤 실행할 작업 (EntryPoint에서 씬 전환 주입).
    /// </summary>
    public class SplashModel : ModelBase
    {
        /// <summary>ViewModel이 Show → Delay → Hide 완료 후 호출합니다.</summary>
        public Func<UniTask> OnComplete { get; set; }
    }
}
