using UnityEngine;
using System.Collections.Generic;
using System;

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

    public Action SpawnAction = new Action(() => { }); // From Spawn Environment.

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
}