using UnityEngine;
using System.Collections.Generic;

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
}