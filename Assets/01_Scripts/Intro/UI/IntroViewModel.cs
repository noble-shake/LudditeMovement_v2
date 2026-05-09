using Cysharp.Threading.Tasks;
using R3;

using RottenNoble.Core;
using RottenNoble.Core.UI;

namespace RottenNoble.Intro.UI
{
    /// <summary>
    /// Intro 씬 ViewModel — 시작 버튼 구독 → DataLoad 씬 전환
    /// </summary>
    public class IntroViewModel : ViewModelBase<IntroView, IntroModel>
    {
        public override void Initialize(IntroView view, IntroModel model)
        {
            base.Initialize(view, model);

            view.ShowAsync(onComplete: () =>
            {
                view.OnStartClicked()
                    .Subscribe(_ => OnStartAsync().Forget())
                    .AddTo(ref disposableBag);

            }).Forget();
        }

        async UniTaskVoid OnStartAsync()
        {
            await View.HideAsync();
            resourceFactory.DeleteInstance(ResourceType.Addressable, View.gameObject);
            await sceneLoader.LoadDataLoadAsync();
        }
    }
}
