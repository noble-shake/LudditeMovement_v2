using Cysharp.Threading.Tasks;
using R3;
using VContainer;

using RottenNoble.Core;
using RottenNoble.Core.Services;
using RottenNoble.Core.UI;

namespace RottenNoble.DataLoad.UI
{
    /// <summary>
    /// DataLoad 씬 ViewModel — 리소스 로드 진행률 바인딩 → 완료 시 페이드아웃 → Model.OnComplete 실행
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

            DataLoadSequenceAsync().Forget();
        }

        async UniTaskVoid DataLoadSequenceAsync()
        {
            await dataLoadService.LoadAllAsync();   // 리소스 로드
            await saveDataService.LoadAsync();       // 세이브 데이터 로드
            await HideViewAsync();                   // Navigator 경유 → 페이드아웃 → Disappeared
            if (OnComplete != null)
                await OnComplete.Invoke();           // EntryPoint 주입 액션 (씬 전환)
        }
    }
}
