using System.Collections.Generic;

using R3;

using RottenNoble.Core;
using RottenNoble.Core.UI;

namespace RottenNoble.MainMenu.StageSelect
{
    /// <summary>
    /// 스테이지 선택 탭 데이터 모델
    /// </summary>
    public class StageSelectModel : ModelBase
    {
        public ReactiveProperty<int>          SelectedStageId { get; } = new(0);
        public ReactiveProperty<List<HeroId>> SelectedHeroes  { get; } = new(new());
        public ReactiveProperty<bool>         CanStartGame    { get; } = new(false);
    }
}
