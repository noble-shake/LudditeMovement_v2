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
        AudioService    _audio;
        SaveDataService _saveData;

        [Inject]
        void InjectServices(AudioService audio, SaveDataService saveData)
        {
            _audio    = audio;
            _saveData = saveData;
        }

        public override void Initialize(SettingsView view, SettingsModel model)
        {
            base.Initialize(view, model);

            // ── 저장된 볼륨 값 로드 ───────────────────────
            model.BgmVolume.Value = _saveData.Data.BgmVolume;
            model.SfxVolume.Value = _saveData.Data.SfxVolume;

            _audio.SetBgmVolume(model.BgmVolume.Value);
            _audio.SetSfxVolume(model.SfxVolume.Value);

            // ── 볼륨 변경 → 오디오 적용 + 자동 저장 ─────
            model.BgmVolume
                .Skip(1)
                .ThrottleLast(AppConstants.SliderThrottle)
                .Subscribe(v =>
                {
                    _audio.SetBgmVolume(v);
                    _saveData.Data.BgmVolume = v;
                    _saveData.SaveAsync().Forget();
                })
                .AddTo(ref disposableBag);

            model.SfxVolume
                .Skip(1)
                .ThrottleLast(AppConstants.SliderThrottle)
                .Subscribe(v =>
                {
                    _audio.SetSfxVolume(v);
                    _saveData.Data.SfxVolume = v;
                    _saveData.SaveAsync().Forget();
                })
                .AddTo(ref disposableBag);
        }
    }
}
