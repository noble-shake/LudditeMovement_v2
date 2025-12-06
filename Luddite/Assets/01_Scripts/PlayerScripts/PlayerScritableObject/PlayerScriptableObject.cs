using UnityEngine;
using System.Collections.Generic;

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
    public List<Sprite> FacePortrait;

    [Header("Status")]
    public float HP;
    public float Speed;
}