using UnityEngine;
using System.Collections.Generic;
using System;

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

    }

    // TEST
    public EnemyAttackScriptable attackScript;
    public EnemyMoveScriptable moveScript;

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