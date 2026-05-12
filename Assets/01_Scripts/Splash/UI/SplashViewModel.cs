using Cysharp.Threading.Tasks;

using RottenNoble.Core.UI;

namespace RottenNoble.Splash.UI
{
    /// <summary>
    /// Splash 씬 ViewModel — 로고 노출(2s) → 페이드아웃 → Model.OnComplete 실행
    /// </summary>
    public class SplashViewModel : ViewModelBase<SplashView, SplashModel>
    {
        public override async UniTask Initialize(SplashView view, SplashModel model)
        {
            await base.Initialize(view, model);
            SplashSequenceAsync().Forget();
        }

        async UniTaskVoid SplashSequenceAsync()
        {
            await UniTask.Delay(2000);          // 로고 노출 대기
            await View.HideAsync();             // 페이드아웃 → Disappeared
            if (OnComplete != null)
                await OnComplete.Invoke();       // EntryPoint 주입 액션 (씬 전환)
        }
    }
}
