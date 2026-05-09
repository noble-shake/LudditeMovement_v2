using System;
using System.Collections.Generic;

using Cysharp.Threading.Tasks;
using R3;
using VContainer;

using RottenNoble.Core;

namespace RottenNoble.MainMenu.StageSelect
{
    /// <summary>
    /// 스테이지 선택 / 파티 구성 / 스킬 등록 상태 관리
    /// StageSelectViewModel의 비즈니스 로직 담당
    /// </summary>
    public class StageSelectService : IDisposable
    {
        readonly DataManager         dataManager;
        readonly CompositeDisposable disposables = new();

        public ReactiveProperty<int>           SelectedStageId { get; } = new(0);
        public ReactiveProperty<List<HeroId>>  SelectedHeroes  { get; } = new(new());
        public ReactiveProperty<bool>          CanStartGame    { get; } = new(false);

        readonly Dictionary<HeroId, HeroSkillLoadout> skillLoadouts = new();

        [Inject]
        public StageSelectService(DataManager dataManager)
        {
            this.dataManager = dataManager;

            SelectedHeroes
                .Subscribe(heroes => CanStartGame.Value = heroes.Count == AppConstants.MaxPartySize)
                .AddTo(disposables);
        }

        public void Initialize(SaveData saveData)
        {
            int nextStage = 0;
            foreach (var score in saveData.StageScores)
            {
                if (score.Cleared && score.StageId >= nextStage)
                    nextStage = score.StageId + 1;
            }
            SelectedStageId.Value = nextStage;
            SelectedHeroes.Value  = new List<HeroId>(saveData.UnlockedHeroes);
        }

        public void ToggleHero(HeroId heroId)
        {
            var heroes = new List<HeroId>(SelectedHeroes.Value);

            if (heroes.Contains(heroId))
                heroes.Remove(heroId);
            else if (heroes.Count < AppConstants.MaxPartySize)
                heroes.Add(heroId);

            SelectedHeroes.Value = heroes;
        }

        public void SetSkillLoadout(HeroId heroId, SkillId cw, SkillId ccw)
            => skillLoadouts[heroId] = new HeroSkillLoadout { CW = cw, CCW = ccw };

        public HeroSkillLoadout GetSkillLoadout(HeroId heroId)
            => skillLoadouts.TryGetValue(heroId, out var loadout)
                ? loadout
                : new HeroSkillLoadout { CW = SkillId.None, CCW = SkillId.None };

        public async void StartGame()
        {
            if (!CanStartGame.Value) return;

            var session = new SessionData
            {
                StageId        = SelectedStageId.Value,
                SelectedHeroes = new List<HeroId>(SelectedHeroes.Value),
                SkillLoadouts  = new Dictionary<HeroId, HeroSkillLoadout>(skillLoadouts),
            };

            await dataManager.Scene.LoadInGameAsync(session);
        }

        public void Dispose() => disposables.Dispose();
    }
}
