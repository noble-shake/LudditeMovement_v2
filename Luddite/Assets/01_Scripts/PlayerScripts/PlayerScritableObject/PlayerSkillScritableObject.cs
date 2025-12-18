using System;
using System.Reflection;
using UnityEngine;

[Serializable]
public enum PlayerSkillType
{ 
    Passive,
    Active,
}

[Serializable]
public enum PassiveSkill
{ 
    None,
    HP,
    HPRegen,
    Attack,
    AttackSpeed,
    AttackSoulDrop,
    AttackHPRegen,
    Speed,
    Critical,
    ReduceDamage,
    ReduceRequireAP,
    ReduceCycle,
    BuffDuration,
    ReduceCharging,
    ReduceCooldown,
    DamageSouldrop,

}

[CreateAssetMenu(fileName = "PlayerSkillObject", menuName = "Lucide-Boundary/PlayerSkill", order = -1)]
public class PlayerSkillScriptableObject : ScriptableObject
{
    [SerializeField]public Sprite Icon;
    public string Name;
    public string InstanceName;
    [TextArea] public string Description;
    public bool isCharging;
    public int NodeIndex;
    public PlayerSkillType skillType;
    public PassiveSkill passiveType;
    public float[] Params;
    public bool isSynergy;
    public PlayerSkillScriptableObject SynergySkill1;
    public PlayerSkillScriptableObject SynergySkill2;

    public IPlaySkill GetInstance()
    {
        Assembly assembly = Assembly.GetExecutingAssembly();
        System.Type t = assembly.GetType(InstanceName);
        object obj = System.Activator.CreateInstance(t);

        if (null == obj)
        {
            Debug.LogWarning($"PlayerSkillScriptableObject : There is not exist {Name}");
            return null;
        }
        return (obj as IPlaySkill);

    }

    public float GetPassiveValue()
    {
        switch (passiveType)
        {
            default:
            case PassiveSkill.None:
                return 0;
            case PassiveSkill.HP:
            case PassiveSkill.Attack:
            case PassiveSkill.AttackSpeed:
            case PassiveSkill.BuffDuration:
            case PassiveSkill.ReduceCharging:
            case PassiveSkill.ReduceCooldown:
            case PassiveSkill.Speed:
                return 2;
            case PassiveSkill.HPRegen:
            case PassiveSkill.AttackSoulDrop:
            case PassiveSkill.ReduceCycle:
            case PassiveSkill.ReduceDamage:
            case PassiveSkill.DamageSouldrop:
            case PassiveSkill.AttackHPRegen:
                return 1;
            case PassiveSkill.ReduceRequireAP:
                return 5;

        }
    }
}