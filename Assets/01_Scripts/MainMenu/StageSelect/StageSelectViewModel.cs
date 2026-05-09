using R3;
using VContainer;

using RottenNoble.Core;
using RottenNoble.Core.UI;

namespace RottenNoble.MainMenu.StageSelect
{
    /// <summary>
    /// 스테이지 선택 탭 ViewModel — StageSelectService에 UI 이벤트를 위임
    /// </summary>
    public class StageSelectViewModel : ViewModelBase<StageSelectView, StageSelectModel>
    {
        StageSelectService _service;
        SaveDataService    _saveData;

        [Inject]
        void InjectServices(StageSelectService service, SaveDataService saveData)
        {
            _service  = service;
            _saveData = saveData;
        }

        public override void Initialize(StageSelectView view, StageSelectModel model)
        {
            base.Initialize(view, model);

            _service.Initialize(_saveData.Data);

            view.OnStartButtonClicked()
                .Subscribe(_ => _service.StartGame())
                .AddTo(ref disposableBag);

            _service.CanStartGame
                .Subscribe(can => view.SetStartButtonInteractable(can))
                .AddTo(ref disposableBag);
        }

        public void OnHeroClicked(HeroId heroId)
            => _service.ToggleHero(heroId);

        public void OnStageSelected(int stageId)
            => _service.SelectedStageId.Value = stageId;

        public void OnSkillSlotChanged(HeroId h, SkillId cw, SkillId ccw)
            => _service.SetSkillLoadout(h, cw, ccw);
    }
}
