using Cysharp.Threading.Tasks;

using RottenNoble.Core;
using RottenNoble.Core.UI;

namespace RottenNoble.Splash.UI
{
    /// <summary>
    /// Splash 씬 ViewModel — 로고 연출 완료 후 Patch 씬으로 전환
    /// </summary>
    public class SplashViewModel : ViewModelBase<SplashView, SplashModel>
    {
        public override async UniTask Initialize(SplashView view, SplashModel model)
        {
            await base.Initialize(view, model);

            
        }
    }
}
