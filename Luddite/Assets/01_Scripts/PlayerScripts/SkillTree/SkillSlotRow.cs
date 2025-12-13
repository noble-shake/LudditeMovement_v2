using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillSlotRow : MonoBehaviour
{
    [SerializeField] private int Tier;
    [SerializeField] private TMP_Text TierLabel;

    [SerializeField] public List<SkillSlotButton> NormalSlots;
    [SerializeField] public List<SkillSlotButton> Active1Slots;
    [SerializeField] public List<SkillSlotButton> Active2Slots;

    public int TierValue { get { return Tier; } set { Tier = value; } }
    public string TierText{ get { return TierLabel.text; } set { TierLabel.text = value; } }
}