using System;
using System.IO;
using UnityEngine;

using Cysharp.Threading.Tasks;

namespace RottenNoble.Core
{
    /// <summary>
    /// 세이브/로드 서비스
    /// </summary>
    public class SaveDataService
    {
        const string SaveFileName = "savedata.json";

        // Application.persistentDataPath는 메인 스레드에서만 접근 가능.
        // 최초 접근 시 메인 스레드에서 캐싱하고 이후 재사용합니다.
        string _savePath;
        string SavePath => _savePath ??= Path.Combine(Application.persistentDataPath, SaveFileName);

        public SaveData Data { get; private set; } = new();

        public async UniTask LoadAsync()
        {
            // 메인 스레드에서 경로 미리 캡처
            var path = SavePath;

            await UniTask.RunOnThreadPool(() =>
            {
                if (!File.Exists(path))
                {
                    Data = new SaveData();
                    return;
                }

                var json = File.ReadAllText(path);
                Data = JsonUtility.FromJson<SaveData>(json) ?? new SaveData();
            });
        }

        public async UniTask SaveAsync()
        {
            // 메인 스레드에서 경로 및 직렬화 미리 처리
            var path = SavePath;
            var json = JsonUtility.ToJson(Data, prettyPrint: true);

            await UniTask.RunOnThreadPool(() =>
            {
                File.WriteAllText(path, json);
            });
        }
    }

    [Serializable]
    public class SaveData
    {
        // ── 영웅 / 스테이지 / 도감 ──────────────────────
        public HeroId[]           UnlockedHeroes = { HeroId.Apolonia };
        public HeroProgressData[] HeroProgresses = Array.Empty<HeroProgressData>();
        public StageScoreData[]   StageScores    = Array.Empty<StageScoreData>();
        public AffinityData[]     Affinities     = Array.Empty<AffinityData>();

        // ── 오디오 설정 ───────────────────────────────
        public float BgmVolume = 1f;
        public float SfxVolume = 1f;
    }

    [Serializable]
    public class HeroProgressData
    {
        public HeroId HeroId;
        public int    Level;
        public int    Exp;
        public int[]  UnlockedSkillNodes = Array.Empty<int>();
    }

    [Serializable]
    public class StageScoreData
    {
        public int  StageId;
        public int  BestScore;
        public bool Cleared;
    }

    [Serializable]
    public class AffinityData
    {
        public HeroId HeroA;
        public HeroId HeroB;
        public int    Level;
    }
}
