using R3;

using RottenNoble.Core.UI;

namespace RottenNoble.MainMenu.Settings
{
    /// <summary>
    /// 시스템 설정 탭 데이터 모델
    /// </summary>
    public class SettingsModel : ModelBase
    {
        public ReactiveProperty<float> BgmVolume { get; } = new(1f);
        public ReactiveProperty<float> SfxVolume { get; } = new(1f);
    }
}
