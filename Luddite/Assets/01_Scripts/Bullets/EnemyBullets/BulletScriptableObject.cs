using System.Reflection;
using UnityEngine;

[CreateAssetMenu(fileName = "BulletInstance", menuName = "Lucide-Boundary/Bullet", order = -1)]
public class BulletScriptableObject : ScriptableObject
{
    public string Name;

    public IEnemyBullet GetInstance()
    {
        Assembly assembly = Assembly.GetExecutingAssembly();
        System.Type t = assembly.GetType(Name);
        object obj = System.Activator.CreateInstance(t);

        if (null == obj)
        {
            Debug.LogWarning($"Bullet Scriptable Object : There is not exist {Name}");
            return null;
        }
        return (obj as IEnemyBullet);

    }
}