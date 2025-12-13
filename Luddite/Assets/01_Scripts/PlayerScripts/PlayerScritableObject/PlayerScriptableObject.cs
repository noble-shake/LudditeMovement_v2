using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEngine.UI.Extensions;

public enum PlayerClassType
{
    None,
    Knight,
    Buffer,
    Thief,
    SoulMaster,
    SpellMaster,
    Archer,

}

[Serializable]
public struct PlayerStatus
{
    public float HP;
    public float Speed;
    public float Attack;
    public float Critical;
    public float RequireAP;
    public int RequireCycle;
}

[CreateAssetMenu(fileName = "PlayerCharacter", menuName = "Lucide-Boundary/Player", order = -1)]
public class PlayerScriptableObject : ScriptableObject
{
    public string Name;
    public PlayerClassType classType;
    public string PlayerBattleType;

    [Header("Library")]
    [TextArea] public string Description;
    [TextArea] public string Memory1;
    [TextArea] public string Memory2;
    [TextArea] public string Memory3;
    public Player PlayerPrefab;
    public Sprite FullBodyPortrait;
    public Sprite FullBodySilouhettePortrait;
    public List<Sprite> FacePortrait;

    [Header("Status")]
    public float HP = 100;
    public float Speed = 1;
    public float Attack = 3;
    public float Critical = 0;
    public float RequireAP = 30;
    public int RequireCycle = 5;

    public PlayerStatus GetPlayerStatus()
    {
        return new PlayerStatus { HP = HP, Speed = Speed, Attack = Attack, Critical = Critical, RequireAP = RequireAP, RequireCycle = RequireCycle };
    }

    public float GetHP()
    {
        return HP;
    }

    public float GetSpeed()
    {
        return Speed;
    }

    public float GetAttack()
    {
        return HP;
    }

    public float GetCritical()
    {
        return Critical;
    }

    public float GetRequireAP()
    {
        return RequireAP;
    }

    public int GetRequireCycle()
    {
        return RequireCycle;
    }
}