using System.Reflection;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyAttackObject", menuName ="Lucide-Boundary/EnemyAttack", order =-1)]
public class EnemyAttackScriptable : ScriptableObject
{
    public string Name;
    [TextArea] public string Description;
    public bool isCharging;

    public IEnemyAttack GetInstance()
    {
        Assembly assembly = Assembly.GetExecutingAssembly();
        System.Type t = assembly.GetType(Name);
        object obj = System.Activator.CreateInstance(t);

        if (null == obj)
        {
            Debug.LogWarning($"EnemyAttackScriptable : There is not exist {Name}");
            return null;
        }
        return (obj as IEnemyAttack);

    }
}