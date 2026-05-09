using UnityEngine;

namespace RottenNoble.Core
{
    /// <summary>
    /// BGM / SFX 재생 및 볼륨 제어 서비스
    /// </summary>
    public class AudioService
    {
        AudioSource bgmSource;
        AudioSource sfxSource;

        public void SetBgmVolume(float volume)
        {
            if (bgmSource != null)
                bgmSource.volume = Mathf.Clamp01(volume);
        }

        public void SetSfxVolume(float volume)
        {
            if (sfxSource != null)
                sfxSource.volume = Mathf.Clamp01(volume);
        }

        public void PlayBgm(AudioClip clip, bool loop = true)
        {
            if (bgmSource == null) return;
            bgmSource.clip = clip;
            bgmSource.loop = loop;
            bgmSource.Play();
        }

        public void StopBgm() => bgmSource?.Stop();

        public void PlaySfx(AudioClip clip) => sfxSource?.PlayOneShot(clip);

        public void Setup(AudioSource bgm, AudioSource sfx)
        {
            bgmSource = bgm;
            sfxSource = sfx;
        }
    }
}
