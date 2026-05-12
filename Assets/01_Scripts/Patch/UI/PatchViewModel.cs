using Cysharp.Threading.Tasks;
using R3;
using VContainer;

using RottenNoble.Core;
using RottenNoble.Core.Services;
using RottenNoble.Core.UI;

namespace RottenNoble.Patch.UI
{
    /// <summary>
    /// Patch 씬 ViewModel — PatchService 진행률 바인딩 및 완료 시 Intro 씬 전환
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
        }
    }
}
