using System;
using RottenNoble.Core.UI;

namespace RottenNoble.Patch.UI
{
    /// <summary>
    /// Patch 씬 데이터 모델
    /// </summary>
    public class PatchModel : ModelBase
    {
        public Action PatchComplete {get; set;}
    }
}
