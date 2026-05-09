using Cysharp.Threading.Tasks;
using R3;
using VContainer;

using RottenNoble.Core;
using RottenNoble.Core.UI;

namespace RottenNoble.MainMenu.Settings
{
    /// <summary>
    /// 시스템 설정 탭 ViewModel — AudioService에 볼륨 변경을 위임
    /// </summary>
    public class SettingsViewModel : ViewModelBase<SettingsView, SettingsModel>
    {
        AudioService    audioService;
        SaveDataService saveDataService;

        [Inject]
        void InjectServices(AudioService audioService, SaveDataService saveDataService)
        {
            this.audioService    = audioService;
            this.saveDataService = saveDataService;
        }

        public override void Initialize(SettingsView view, SettingsModel model)
        {
            base.Initialize(view, model);

            // ── 저장된 볼륨 값 로드 ───────────────────────
            model.BgmVolume.Value = saveDataService.Data.BgmVolume;
            model.SfxVolume.Value = saveDataService.Data.SfxVolume;

            audioService.SetBgmVolume(model.BgmVolume.Value);
            audioService.SetSfxVolume(model.SfxVolume.Value);

            // ── 볼륨 변경 → 오디오 적용 + 자동 저장 ─────
            model.BgmVolume
                .Skip(1)
                .ThrottleLast(AppConstants.SliderThrottle)
                .Subscribe(v =>
                {
                    audioService.SetBgmVolume(v);
                    saveDataService.Data.BgmVolume = v;
                    saveDataService.SaveAsync().Forget();
                })
                .AddTo(ref disposableBag);

            model.SfxVolume
                .Skip(1)
                .ThrottleLast(AppConstants.SliderThrottle)
                .Subscribe(v =>
                {
                    audioService.SetSfxVolume(v);
                    saveDataService.Data.SfxVolume = v;
                    saveDataService.SaveAsync().Forget();
                })
                .AddTo(ref disposableBag);
        }
    }
}
