using UnityEngine;

namespace RottenNoble.Core
{
    /// <summary>
    /// BGM / SFX 재생 및 볼륨 제어 서비스
    /// </summary>
    public class AudioService
    {
        AudioSource _bgmSource;
        AudioSource _sfxSource;

        public void SetBgmVolume(float volume)
        {
            if (_bgmSource != null)
                _bgmSource.volume = Mathf.Clamp01(volume);
        }

        public void SetSfxVolume(float volume)
        {
            if (_sfxSource != null)
                _sfxSource.volume = Mathf.Clamp01(volume);
        }

        public void PlayBgm(AudioClip clip, bool loop = true)
        {
            if (_bgmSource == null) return;
            _bgmSource.clip = clip;
            _bgmSource.loop = loop;
            _bgmSource.Play();
        }

        public void StopBgm() => _bgmSource?.Stop();

        public void PlaySfx(AudioClip clip) => _sfxSource?.PlayOneShot(clip);

        public void Setup(AudioSource bgm, AudioSource sfx)
        {
            _bgmSource = bgm;
            _sfxSource = sfx;
        }
    }
}
