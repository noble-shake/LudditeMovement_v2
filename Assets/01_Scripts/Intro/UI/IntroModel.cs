using System;

using Cysharp.Threading.Tasks;

using RottenNoble.Core.UI;

namespace RottenNoble.Intro.UI
{
    /// <summary>
    /// Intro 씬 데이터 모델.
    /// OnComplete — 클릭 후 페이드아웃이 끝난 뒤 실행할 작업 (EntryPoint에서 씬 전환 주입).
    /// </summary>
    public class IntroModel : ModelBase
    {
        /// <summary>ViewModel이 HideAsync 완료 후 호출합니다.</summary>
        public Func<UniTask> OnComplete { get; set; }
    }
}
