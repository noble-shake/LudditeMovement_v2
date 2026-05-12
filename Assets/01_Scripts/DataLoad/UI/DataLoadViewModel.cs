using Cysharp.Threading.Tasks;
using R3;
using VContainer;

using RottenNoble.Core;
using RottenNoble.Core.Services;
using RottenNoble.Core.UI;

namespace RottenNoble.DataLoad.UI
{
    /// <summary>
    /// DataLoad 씬 ViewModel — DataLoadService 바인딩 후 완료 시 MainMenu 씬 전환
    /// </summary>
    public class DataLoadViewModel : ViewModelBase<DataLoadView, DataLoadModel>
    {
        DataLoadService dataLoadService;
        SaveDataService saveDataService;

        [Inject]
        void InjectServices(DataLoadService dataLoadService, SaveDataService saveDataService)
        {
            this.dataLoadService = dataLoadService;
            this.saveDataService = saveDataService;
        }

        public override async UniTask Initialize(DataLoadView view, DataLoadModel model)
        {
            await base.Initialize(view, model);

            dataLoadService.Progress
                .Subscribe(p => view.SetProgress(p))
                .AddTo(ref disposableBag);

            dataLoadService.StatusText
                .Subscribe(s => view.SetStatus(s))
                .AddTo(ref disposableBag);

            await dataLoadService.LoadAllAsync();
            await saveDataService.LoadAsync();

            await dataManager.ScenePath.LoadMainMenuAsync();
        }
    }
}
