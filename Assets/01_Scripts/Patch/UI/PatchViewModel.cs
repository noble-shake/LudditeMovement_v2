using Cysharp.Threading.Tasks;
using R3;
using VContainer;

using RottenNoble.Core.Services;
using RottenNoble.Core.UI;

namespace RottenNoble.Patch.UI
{
    /// <summary>
    /// Patch 씬 ViewModel — 패치 진행률 바인딩 → 완료 시 페이드아웃 → Model.OnComplete 실행
    /// </summary>
    public class PatchViewModel : ViewModelBase<PatchView, PatchModel>
    {
        PatchService patchService;

        [Inject]
        void InjectServices(PatchService patchService)
            => this.patchService = patchService;

        public override async UniTask Initialize(PatchView view, PatchModel model)
        {
            await base.Initialize(view, model);

            patchService.Progress
                .Subscribe(p => view.SetProgress(p))
                .AddTo(ref disposableBag);

            patchService.StatusText
                .Subscribe(s => view.SetStatus(s))
                .AddTo(ref disposableBag);

            PatchSequenceAsync().Forget();
        }

        async UniTaskVoid PatchSequenceAsync()
        {
            await patchService.CheckAndUpdateAsync();  // 패치 진행
            await UniTask.Delay(1000);                  // 완료 후 짧은 대기
            await View.HideAsync();                     // 페이드아웃 → Disappeared
            if (OnComplete != null)
                await OnComplete.Invoke();              // EntryPoint 주입 액션 (씬 전환)
        }
    }
}
