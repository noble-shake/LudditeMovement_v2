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
        DataLoadService _dataLoader;
        SaveDataService _saveData;

        [Inject]
        void InjectServices(DataLoadService dataLoader, SaveDataService saveData)
        {
            _dataLoader = dataLoader;
            _saveData   = saveData;
        }

        public override void Initialize(DataLoadView view, DataLoadModel model)
        {
            base.Initialize(view, model);

            view.ShowAsync(onComplete: async () =>
            {
                _dataLoader.Progress
                    .Subscribe(p => view.SetProgress(p))
                    .AddTo(ref disposableBag);

                _dataLoader.StatusText
                    .Subscribe(s => view.SetStatus(s))
                    .AddTo(ref disposableBag);

                await _dataLoader.LoadAllAsync();
                await _saveData.LoadAsync();

                await view.HideAsync(onComplete: async () =>
                {
                    resourceFactory.DeleteInstance(ResourceType.Addressable, view.gameObject);
                    await sceneLoader.LoadMainMenuAsync();
                });

            }).Forget();
        }
    }
}
