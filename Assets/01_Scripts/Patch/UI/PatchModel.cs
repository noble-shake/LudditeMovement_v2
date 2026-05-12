using System;

using Cysharp.Threading.Tasks;

using RottenNoble.Core.UI;

namespace RottenNoble.Patch.UI
{
    /// <summary>
    /// Patch 씬 데이터 모델.
    /// OnComplete — 패치 완료 후 실행할 작업 (EntryPoint에서 씬 전환 주입).
    /// </summary>
    public class PatchModel : ModelBase
    {
        /// <summary>ViewModel이 패치 완료 → HideAsync 후 호출합니다.</summary>
        public Func<UniTask> OnComplete { get; set; }
    }
}
