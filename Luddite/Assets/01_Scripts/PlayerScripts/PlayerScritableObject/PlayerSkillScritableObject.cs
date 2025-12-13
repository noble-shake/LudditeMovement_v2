using System;
using System.Reflection;
using UnityEngine;

[Serializable]
public enum PlayerSkillType
{ 
    Passive,
    Active,
}

[CreateAssetMenu(fileName = "PlayerSkillObject", menuName = "Lucide-Boundary/PlayerSkill", order = -1)]
public class PlayerSkillScriptableObject : ScriptableObject
{
    [SerializeField]public Sprite Icon;
    public string Name;
    [TextArea] public string Description;
    public bool isCharging;
    public int NodeIndex;
    public PlayerSkillType skillType;
    public float[] Params;
    public bool isSynergy;
    public PlayerSkillScriptableObject SynergySkill1;
    public PlayerSkillScriptableObject SynergySkill2;

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

    public string GetDescription(string _name)
    {
        if (_name == "전투 고조")
        {
            return $"[3번 사이클을 돌려 발동] {Params[0]} 초 동안 공격 속도가 {Params[1]}%, 이동 속도가 {Params[2]}% 증가합니다.";
        }
        else if (_name == "체력 증가")
        {
            return $"[현재 {Params[0]}% 증가] 체력 2% 증가";
        }
        else
        {
            return Description;
        }
    }
}