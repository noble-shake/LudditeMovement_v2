using UnityEngine;

[CreateAssetMenu(fileName = "Effect", menuName ="Lucide-Boundary/Effect")]
public class EffectScriptableObject : ScriptableObject
{
    public string Name;
    public int NumbOfEffects = 0;
    public GameObject Prefab;
}