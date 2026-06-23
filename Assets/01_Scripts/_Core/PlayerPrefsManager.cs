using System;
using UnityEngine;

namespace RottenNoble.Core
{
    /// <summary>
    /// PlayerPrefs 중앙 관리자.
    ///
    /// 문자열 키 직접 사용 금지 — PlayerPrefKey enum 경유.
    /// PlayerPrefs 기본 타입(int / float / string) + bool + enum 지원.
    ///
    /// 사용 예시:
    ///   playerPrefsManager.SetEnum(PlayerPrefKey.Difficulty, DifficultyLevel.Hard);
    ///   var diff = playerPrefsManager.GetEnum(PlayerPrefKey.Difficulty, DifficultyLevel.Normal);
    ///
    ///   playerPrefsManager.SetFloat(PlayerPrefKey.MasterVolume, 0.8f);
    ///   float vol = playerPrefsManager.GetFloat(PlayerPrefKey.MasterVolume, 1f);
    /// </summary>
    public class PlayerPrefsManager
    {
        // ── 키 존재 여부 ──────────────────────────────────────────

        public bool HasKey(PlayerPrefKey key)
            => PlayerPrefs.HasKey(KeyOf(key));

        public void DeleteKey(PlayerPrefKey key)
            => PlayerPrefs.DeleteKey(KeyOf(key));

        public void DeleteAll()
            => PlayerPrefs.DeleteAll();

        /// <summary>변경사항을 디스크에 즉시 저장. 앱 종료 전 또는 중요 설정 변경 후 호출.</summary>
        public void Save()
            => PlayerPrefs.Save();

        // ── int ───────────────────────────────────────────────────

        public int GetInt(PlayerPrefKey key, int defaultValue = 0)
            => PlayerPrefs.GetInt(KeyOf(key), defaultValue);

        public void SetInt(PlayerPrefKey key, int value)
        {
            PlayerPrefs.SetInt(KeyOf(key), value);
            PlayerPrefs.Save();
        }

        // ── float ─────────────────────────────────────────────────

        public float GetFloat(PlayerPrefKey key, float defaultValue = 0f)
            => PlayerPrefs.GetFloat(KeyOf(key), defaultValue);

        public void SetFloat(PlayerPrefKey key, float value)
        {
            PlayerPrefs.SetFloat(KeyOf(key), value);
            PlayerPrefs.Save();
        }

        // ── string ────────────────────────────────────────────────

        public string GetString(PlayerPrefKey key, string defaultValue = "")
            => PlayerPrefs.GetString(KeyOf(key), defaultValue);

        public void SetString(PlayerPrefKey key, string value)
        {
            PlayerPrefs.SetString(KeyOf(key), value);
            PlayerPrefs.Save();
        }

        // ── bool (int 0/1 로 저장) ────────────────────────────────

        public bool GetBool(PlayerPrefKey key, bool defaultValue = false)
            => PlayerPrefs.GetInt(KeyOf(key), defaultValue ? 1 : 0) == 1;

        public void SetBool(PlayerPrefKey key, bool value)
        {
            PlayerPrefs.SetInt(KeyOf(key), value ? 1 : 0);
            PlayerPrefs.Save();
        }

        // ── Enum (int 로 저장) ────────────────────────────────────

        public TEnum GetEnum<TEnum>(PlayerPrefKey key, TEnum defaultValue = default)
            where TEnum : struct, Enum
        {
            if (!HasKey(key)) return defaultValue;
            int stored = PlayerPrefs.GetInt(KeyOf(key));
            return Enum.IsDefined(typeof(TEnum), stored)
                ? (TEnum)(object)stored
                : defaultValue;
        }

        public void SetEnum<TEnum>(PlayerPrefKey key, TEnum value)
            where TEnum : struct, Enum
        {
            PlayerPrefs.SetInt(KeyOf(key), Convert.ToInt32(value));
            PlayerPrefs.Save();
        }

        // ── 내부 유틸 ─────────────────────────────────────────────

        static string KeyOf(PlayerPrefKey key) => key.ToString();
    }
}
