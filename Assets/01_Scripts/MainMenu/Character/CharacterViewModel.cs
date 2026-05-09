using VContainer;

using RottenNoble.Core;
using RottenNoble.Core.UI;

namespace RottenNoble.MainMenu.Character
{
    /// <summary>
    /// 캐릭터 탭 ViewModel — 영웅 선택 및 스킬 트리 관련 로직 담당
    /// </summary>
    public class CharacterViewModel : ViewModelBase<CharacterView, CharacterModel>
    {
        SaveDataService _saveData;

        [Inject]
        void InjectServices(SaveDataService saveData) => _saveData = saveData;

        public override void Initialize(CharacterView view, CharacterModel model)
        {
            base.Initialize(view, model);

            // TODO: 해금된 영웅 목록 표시, 선택 버튼 바인딩
        }

        public void OnHeroSelected(HeroId heroId) => Model.SelectedHero.Value = heroId;
    }
}
