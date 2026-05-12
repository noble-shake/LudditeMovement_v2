using System;
using System.Collections.Generic;
using UnityEngine;

namespace RottenNoble.Core
{
    /// <summary>
    /// 영웅(Hero)별 Addressable 리소스 키 레지스트리 ScriptableObject
    ///
    /// Assets 우클릭 → Create → RottenNoble/Config/Resource → Hero Resource 로 생성
    /// AppLifetimeScope Inspector에 연결 → VContainer로 전역 주입
    /// </summary>
    [CreateAssetMenu(fileName = "HeroResource", menuName = "RottenNoble/Config/Resource/Hero Resource")]
    public class HeroResourceSO : ScriptableObject
    {
        [Serializable]
        public class Entry
        {
            public HeroId heroId;

            [Header("Prefab")]
            [Tooltip("InGame 필드에 생성할 영웅 프리팹 Addressable 키")]
            public string prefabKey;

            [Header("UI")]
            [Tooltip("스테이지 선택 등에서 사용하는 대형 초상화 키")]
            public string portraitKey;
            [Tooltip("파티 슬롯 등에서 사용하는 소형 아이콘 키")]
            public string thumbnailKey;

            [Header("Skills")]
            [Tooltip("CW 스킬 VFX Addressable 키")]
            public string skillCWEffectKey;
            [Tooltip("CCW 스킬 VFX Addressable 키")]
            public string skillCCWEffectKey;

            [Header("Audio")]
            [Tooltip("캐릭터 선택 테마 BGM 키 (없으면 공백)")]
            public string themeBgmKey;
        }

        public List<Entry> entries = new();

        // ── 조회 ──────────────────────────────────────────────────────────

        Dictionary<HeroId, Entry> lookup;

        void BuildLookup()
        {
            lookup = new Dictionary<HeroId, Entry>(entries.Count);
            foreach (var entry in entries)
                lookup[entry.heroId] = entry;
        }

        /// <summary>HeroId에 해당하는 Entry 반환. 없으면 null.</summary>
        public Entry GetEntry(HeroId heroId)
        {
            if (lookup == null) BuildLookup();
            return lookup.TryGetValue(heroId, out var entry) ? entry : null;
        }

        /// <summary>HeroId에 해당하는 프리팹 키 반환. 없으면 빈 문자열.</summary>
        public string GetPrefabKey(HeroId heroId)
            => GetEntry(heroId)?.prefabKey ?? string.Empty;

        /// <summary>HeroId에 해당하는 초상화 키 반환. 없으면 빈 문자열.</summary>
        public string GetPortraitKey(HeroId heroId)
            => GetEntry(heroId)?.portraitKey ?? string.Empty;

        /// <summary>HeroId에 해당하는 아이콘 키 반환. 없으면 빈 문자열.</summary>
        public string GetThumbnailKey(HeroId heroId)
            => GetEntry(heroId)?.thumbnailKey ?? string.Empty;

        // Inspector에서 데이터 변경 시 캐시를 무효화
        void OnValidate() => lookup = null;
    }
}
