using UnityEngine;

[CreateAssetMenu(fileName = "EnemyInfo", menuName = "Lucide-Boundary/EnemyInfo", order = -1)]
public class EnemyScriptableObject : ScriptableObject
{
    public string Name;
    [TextArea] public string EnemyDescription;

    [TextArea] public string Comment1;
    [TextArea] public string Comment2;
    [TextArea] public string Comment3;
    public AudioClip Sound;
    public Sprite Portrait;
    public GameObject EnemyPrefab;
    public bool isBoss;
}