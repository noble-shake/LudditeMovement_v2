using R3;

using RottenNoble.Core;
using RottenNoble.Core.UI;

namespace RottenNoble.MainMenu.Character
{
    /// <summary>
    /// 캐릭터 탭 데이터 모델
    /// </summary>
    public class CharacterModel : ModelBase
    {
        public ReactiveProperty<HeroId> SelectedHero { get; } = new(HeroId.Apolonia);
    }
}
