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
        PatchService _patchService;

        [Inject]
        void InjectServices(PatchService patchService)
            => _patchService = patchService;

        public override void Initialize(PatchView view, PatchModel model)
        {
            base.Initialize(view, model);

            view.ShowAsync(onComplete: async () =>
            {
                _patchService.Progress
                    .Subscribe(p => view.SetProgress(p))
                    .AddTo(ref disposableBag);

                _patchService.StatusText
                    .Subscribe(s => view.SetStatus(s))
                    .AddTo(ref disposableBag);

                await _patchService.CheckAndUpdateAsync();

                await view.HideAsync(onComplete: async () =>
                {
                    resourceFactory.DeleteInstance(ResourceType.Addressable, view.gameObject);
                    await sceneLoader.LoadIntroAsync();
                });

            }).Forget();
        }
    }
}
