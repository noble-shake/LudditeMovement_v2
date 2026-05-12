using System;
using System.Collections.Generic;
using UnityEngine;

namespace RottenNoble.Core
{
    /// <summary>
    /// 스테이지(Stage / Environment)별 Addressable 리소스 키 레지스트리 ScriptableObject
    ///
    /// Assets 우클릭 → Create → RottenNoble/Config/Resource → Stage Resource 로 생성
    /// AppLifetimeScope Inspector에 연결 → VContainer로 전역 주입
    ///
    /// stageId 는 SessionData.StageId 와 동일한 int 값을 사용합니다.
    /// </summary>
    [CreateAssetMenu(fileName = "StageResource", menuName = "RottenNoble/Config/Resource/Stage Resource")]
    public class StageResourceSO : ScriptableObject
    {
        [Serializable]
        public class Entry
        {
            public int stageId;

            [Header("Info")]
            [Tooltip("Inspector용 스테이지 이름 (코드에서는 미사용)")]
            public string stageName;

            [Header("Environment")]
            [Tooltip("InGame 배경 환경 프리팹 Addressable 키")]
            public string environmentPrefabKey;
            [Tooltip("라이팅 설정 에셋 Addressable 키 (없으면 공백)")]
            public string lightingSettingsKey;

            [Header("UI")]
            [Tooltip("스테이지 선택 화면의 썸네일 스프라이트 키")]
            public string thumbnailKey;
            [Tooltip("스테이지 선택 화면의 배경 이미지 키")]
            public string backgroundKey;

            [Header("Audio")]
            [Tooltip("InGame BGM Addressable 키")]
            public string bgmKey;
            [Tooltip("스테이지 클리어 BGM 키")]
            public string clearBgmKey;

            [Header("Enemies")]
            [Tooltip("이 스테이지에 등장하는 적 ID 목록")]
            public List<EnemyId> enemyIds = new();
        }

        public List<Entry> entries = new();

        // ── 조회 ──────────────────────────────────────────────────────────

        Dictionary<int, Entry> lookup;

        void BuildLookup()
        {
            lookup = new Dictionary<int, Entry>(entries.Count);
            foreach (var entry in entries)
                lookup[entry.stageId] = entry;
        }

        /// <summary>stageId에 해당하는 Entry 반환. 없으면 null.</summary>
        public Entry GetEntry(int stageId)
        {
            if (lookup == null) BuildLookup();
            return lookup.TryGetValue(stageId, out var entry) ? entry : null;
        }

        /// <summary>stageId에 해당하는 환경 프리팹 키 반환. 없으면 빈 문자열.</summary>
        public string GetEnvironmentPrefabKey(int stageId)
            => GetEntry(stageId)?.environmentPrefabKey ?? string.Empty;

        /// <summary>stageId에 해당하는 BGM 키 반환. 없으면 빈 문자열.</summary>
        public string GetBgmKey(int stageId)
            => GetEntry(stageId)?.bgmKey ?? string.Empty;

        /// <summary>stageId에 해당하는 적 ID 목록 반환. 없으면 빈 리스트.</summary>
        public List<EnemyId> GetEnemyIds(int stageId)
            => GetEntry(stageId)?.enemyIds ?? new List<EnemyId>();

        /// <summary>등록된 전체 스테이지 수</summary>
        public int StageCount => entries.Count;

        void OnValidate() => lookup = null;
    }
}
