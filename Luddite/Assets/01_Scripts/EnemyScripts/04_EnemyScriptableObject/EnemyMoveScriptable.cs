using System.Reflection;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyMoveObject", menuName = "Lucide-Boundary/EnemyMove", order = -1)]
public class EnemyMoveScriptable : ScriptableObject
{
    public string Name;

    public IEnemyMove GetInstance()
    {
        Assembly assembly = Assembly.GetExecutingAssembly();
        System.Type t = assembly.GetType(Name);
        object obj = System.Activator.CreateInstance(t);

        if (null == obj)
        {
            Debug.LogWarning($"EnemyAttackScriptable : There is not exist {Name}");
            return null;
        }
        return (obj as IEnemyMove);

    }
}