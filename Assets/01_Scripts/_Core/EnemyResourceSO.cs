using System;
using System.Collections.Generic;
using UnityEngine;

namespace RottenNoble.Core
{
    /// <summary>
    /// 적(Enemy)별 Addressable 리소스 키 레지스트리 ScriptableObject
    ///
    /// Assets 우클릭 → Create → RottenNoble/Config/Resource → Enemy Resource 로 생성
    /// AppLifetimeScope Inspector에 연결 → VContainer로 전역 주입
    /// </summary>
    [CreateAssetMenu(fileName = "EnemyResource", menuName = "RottenNoble/Config/Resource/Enemy Resource")]
    public class EnemyResourceSO : ScriptableObject
    {
        [Serializable]
        public class Entry
        {
            public EnemyId enemyId;

            [Header("Prefab")]
            [Tooltip("InGame 필드에 생성할 적 프리팹 Addressable 키")]
            public string prefabKey;

            [Header("UI")]
            [Tooltip("도감·스테이지 정보 등에 사용하는 초상화 키")]
            public string portraitKey;
            [Tooltip("적 HP바 위에 표시할 소형 아이콘 키")]
            public string thumbnailKey;

            [Header("Audio")]
            [Tooltip("등장 SE 키")]
            public string spawnSfxKey;
            [Tooltip("사망 SE 키")]
            public string deathSfxKey;
        }

        public List<Entry> entries = new();

        // ── 조회 ──────────────────────────────────────────────────────────

        Dictionary<EnemyId, Entry> lookup;

        void BuildLookup()
        {
            lookup = new Dictionary<EnemyId, Entry>(entries.Count);
            foreach (var entry in entries)
                lookup[entry.enemyId] = entry;
        }

        /// <summary>EnemyId에 해당하는 Entry 반환. 없으면 null.</summary>
        public Entry GetEntry(EnemyId enemyId)
        {
            if (lookup == null) BuildLookup();
            return lookup.TryGetValue(enemyId, out var entry) ? entry : null;
        }

        /// <summary>EnemyId에 해당하는 프리팹 키 반환. 없으면 빈 문자열.</summary>
        public string GetPrefabKey(EnemyId enemyId)
            => GetEntry(enemyId)?.prefabKey ?? string.Empty;

        void OnValidate() => lookup = null;
    }

    // ── EnemyId ──────────────────────────────────────────────────────────

    /// <summary>
    /// 적 종류 식별자. 새 적을 추가할 때 여기에 항목을 추가하세요.
    /// </summary>
    public enum EnemyId
    {
        // ── 일반 몬스터 ────────────────────────
        Goblin,
        Orc,
        Skeleton,
        Slime,

        // ── 엘리트 ─────────────────────────────
        EliteGoblin,
        EliteOrc,

        // ── 보스 ───────────────────────────────
        BossGoblinKing,
        BossDragon,
    }
}
