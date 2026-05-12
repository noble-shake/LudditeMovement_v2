using Cysharp.Threading.Tasks;
using R3;
using VContainer;

using RottenNoble.Core.Input;
using RottenNoble.Core.UI;

namespace RottenNoble.Intro.UI
{
    /// <summary>
    /// Intro 씬 ViewModel — 좌클릭 감지 → 페이드아웃 → Model.OnComplete 실행
    /// </summary>
    public class IntroViewModel : ViewModelBase<IntroView, IntroModel>
    {
        InputManager inputManager;

        [Inject]
        void InjectServices(InputManager inputManager)
            => this.inputManager = inputManager;

        public override async UniTask Initialize(IntroView view, IntroModel model)
        {
            await base.Initialize(view, model);

            // Intro 화면에서 좌클릭 감지를 위해 Game 모드 활성화
            inputManager.SetInputMode(InputManager.InputMode.Game);

            // 좌클릭 한 번 → 뷰 숨김 (Take(1)로 중복 발동 방지)
            inputManager.LeftClick
                .Where(pressed => pressed)
                .Take(1)
                .Subscribe(_ => OnClickedAsync().Forget())
                .AddTo(ref disposableBag);
        }

        async UniTaskVoid OnClickedAsync()
        {
            inputManager.SetInputMode(InputManager.InputMode.UI); // 추가 클릭 무시

            await View.HideAsync();                               // 페이드아웃 → Disappeared

            if (Model.OnComplete != null)
                await Model.OnComplete.Invoke();                  // EntryPoint 주입 액션 (씬 전환)
        }
    }
}
