using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;

public enum GameCondition
{ 
    Menu,
    Game,
}


public enum Difficulty
{ 
    Easy,
    Normal,
    Hard,
    Nightmare,
}

public class GameManager : MonoBehaviour
{ 
    public static GameManager Instance;
    public int TotalScore;
    public int CurrentStageScore;
    public Difficulty difficulty;
    public GameCondition currentCondition;
    private float PlayTime;
    public MapData CurrentMap;

    public float PlayTimeValue { get { return PlayTime; } set { PlayTime = value; PlayTimeEvent?.Invoke(value); } }

    public PlayerManager playerManager;
    public TopBorderUI topBorderUI;

    public Action SpawnAction = new Action(() => { }); // From Spawn Environment.
    public event Action<float> PlayTimeEvent;
    public event Action<int> StageScoreEvent;

    public void CharacterSpawn()
    {
        SpawnAction.Invoke();
    }

    private void Awake()
    {
        if (Instance == null) { Instance = this; } else { Destroy(gameObject); }
    }

    private void Start()
    {
        difficulty = Difficulty.Normal;
        currentCondition = GameCondition.Menu;
        Cursor.visible = true;
        MappingTopBorder();
    }

    private void MappingTopBorder()
    {
        topBorderUI.MaxHPValue = playerManager.MaxHPValue;
        topBorderUI.MaxSPValue = playerManager.MaxSoulValue;
        playerManager.OnHPEvent += topBorderUI.GetHPValue;
        playerManager.OnSPEvent += topBorderUI.GetSPValue;
        playerManager.OnMaxHPEvent += topBorderUI.GetMaxHPValue;
        playerManager.OnMaxSPEvent += topBorderUI.GetMaxSPValue;
        PlayTimeEvent += topBorderUI.GetPlayTime;
        StageScoreEvent += topBorderUI.GetScore;
    }

    // TEST
    public EnemyAttackScriptable attackScript;
    public EnemyMoveScriptable moveScript;

    public Difficulty GetDifficulty()
    {
        return difficulty;
    }

    public void SetDifficulty(Difficulty _diff)
    { 
        difficulty = _diff;
    }

    public void CursorVisible(bool isOn)
    {
        Cursor.visible = isOn;
    }

    public void TimeContinue()
    {
        Time.timeScale = 1f;
    }

    public void TimeStop()
    {
        Time.timeScale = 0f;
    }

    public void TimeSlow()
    {
        Time.timeScale = 0.3f;
    }


    List<StageData> EnemyProgression;
    private void BuildSpawnEnemy()
    {
        
        EnemyProgression = CurrentMap.EnemyProgression.ToList<StageData>();
    }


    public void GameStart()
    {
        currentCondition = GameCondition.Game;
        InputManager.Instance.PlayerInputBind();
        Cursor.visible = false;
        BuildSpawnEnemy();
    }

    private void Update()
    {
        if (currentCondition == GameCondition.Menu) return;

        PlayTimeValue += Time.deltaTime;


        if (EnemyProgression.Count <= 0) return;

        for (int i = EnemyProgression.Count - 1; i >= 0; i--)
        {
            StageData t = EnemyProgression[i];
            if (t.SpawnTime > PlayTimeValue) continue;

            GameObject enemy = ResourceManager.Instance.GerEnemyResource(t.MonsterType);
            enemy.transform.position = new Vector3(-7.5f + t.SpawnColumn, 0f, -4.0f + t.SpawnRow);
            EnemyProgression.Remove(t);
        }

    }
}