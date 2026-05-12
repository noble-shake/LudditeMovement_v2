using Cysharp.Threading.Tasks;
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
        StageSelectService stageSelectService;
        SaveDataService    saveDataService;

        [Inject]
        void InjectServices(StageSelectService stageSelectService, SaveDataService saveDataService)
        {
            this.stageSelectService = stageSelectService;
            this.saveDataService    = saveDataService;
        }

        public override async UniTask Initialize(StageSelectView view, StageSelectModel model)
        {
            await base.Initialize(view, model);

            stageSelectService.Initialize(saveDataService.Data);

            view.OnStartButtonClicked()
                .Subscribe(_ => stageSelectService.StartGame())
                .AddTo(ref disposableBag);

            stageSelectService.CanStartGame
                .Subscribe(can => view.SetStartButtonInteractable(can))
                .AddTo(ref disposableBag);
        }

        public void OnHeroClicked(HeroId heroId)
            => stageSelectService.ToggleHero(heroId);

        public void OnStageSelected(int stageId)
            => stageSelectService.SelectedStageId.Value = stageId;

        public void OnSkillSlotChanged(HeroId h, SkillId cw, SkillId ccw)
            => stageSelectService.SetSkillLoadout(h, cw, ccw);
    }
}
