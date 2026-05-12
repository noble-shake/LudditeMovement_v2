using Cysharp.Threading.Tasks;

using RottenNoble.Core;
using RottenNoble.Core.UI;

namespace RottenNoble.Intro.UI
{
    /// <summary>
    /// Intro 씬 ViewModel — 시작 버튼 구독 → DataLoad 씬 전환
    /// </summary>
    public class IntroViewModel : ViewModelBase<IntroView, IntroModel>
    {
        public override async UniTask Initialize(IntroView view, IntroModel model)
        {
            await base.Initialize(view, model);

            view.ShowAsync(onComplete: () =>
            {
                // TODO : InputManager 구현 후에 진행 할 것.
                // Observable.EveryUpdate()
                //     .Where(_ => Keyboard.current.anyKey.wasPressedThisFrame
                //             || Mouse.current.leftButton.wasPressedThisFrame)
                //     .Take(1)
                //     .Subscribe(_ => OnAnyButtonPressed())
                //     .AddTo(this);

            }).Forget();
        }

        async UniTaskVoid OnStartAsync()
        {
            await View.HideAsync();

            await dataManager.ScenePath.LoadDataLoadAsync();
        }
    }
}
