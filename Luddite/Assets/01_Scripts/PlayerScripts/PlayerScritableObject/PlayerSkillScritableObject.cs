using System.Reflection;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerSkillObject", menuName = "Lucide-Boundary/PlayerSkill", order = -1)]
public class PlayerSkillScriptableObject : ScriptableObject
{
    public string Name;
    [TextArea] public string Description;
    public bool isCharging;

    public IPlaySkill GetInstance()
    {
        Assembly assembly = Assembly.GetExecutingAssembly();
        System.Type t = assembly.GetType(Name);
        object obj = System.Activator.CreateInstance(t);

        if (null == obj)
        {
            Debug.LogWarning($"PlayerSkillScriptableObject : There is not exist {Name}");
            return null;
        }
        return (obj as IPlaySkill);

    }
}