using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


// Parent Node => 3 Selectable Nodes => 2 Selectable Nodes


[Serializable]
public class PlayerSkillContainer
{
    public List<SkillTrees> SkillDataset;
}

[Serializable]
public class SkillTrees
{
    public int ID;
    public List<PlayerSkillScriptableObject> Skills;
}


public enum SlotType
{ 
    Normal,
    Skill1,
    Skill2,
}

// Communicate With TreeNodes
public class PlayerSkillManager : MonoBehaviour
{
    public static PlayerSkillManager Instance;

    public Dictionary<PlayerClassType, SkillTree> PlayerNormalSkillTreeDict;
    public Dictionary<PlayerClassType, SkillTree> PlayerActive1SkillTreeDict;
    public Dictionary<PlayerClassType, SkillTree> PlayerActive2SkillTreeDict;

    [Header("Knight")]
    [SerializeField] private int KnightCurSkillPoint; 
    [SerializeField] private int KnightTotalSkillPoint; 
    [SerializeField] private PlayerSkillContainer KnightNormalTree;
    [SerializeField] private PlayerSkillContainer KnightSkill1Tree;
    [SerializeField] private PlayerSkillContainer KnightSkill2Tree;
    [SerializeField] private SkillTreeMap KnightSkillTreeMap;


    [Header("Archer")]
    [SerializeField] private int ArcherCurSkillPoint;
    [SerializeField] private int ArcherTotalSkillPoint;
    [SerializeField] private PlayerSkillContainer ArcherSkillTreeMap;

    private void Awake()
    {
        if (Instance == null) { Instance = this; } else { Destroy(gameObject); }
    }

    private IEnumerator Start()
    {
        PlayerNormalSkillTreeDict = new Dictionary<PlayerClassType, SkillTree>();
        PlayerActive1SkillTreeDict = new Dictionary<PlayerClassType, SkillTree>();
        PlayerActive2SkillTreeDict = new Dictionary<PlayerClassType, SkillTree>();
        yield return null;

        foreach(PlayerClassType _class in LibraryManager.Instance.playerAnalyses.Keys)
        {
            PlayerNormalSkillTreeDict[_class] =  LibraryManager.Instance.playerAnalyses[_class].NormalSkillTree;
            PlayerActive1SkillTreeDict[_class] =  LibraryManager.Instance.playerAnalyses[_class].Active1SkillTree;
            PlayerActive2SkillTreeDict[_class] =  LibraryManager.Instance.playerAnalyses[_class].Active2SkillTree;
        }


        //buildSkillTreeNodes(KnightNormalTree, SlotType.Normal, KnightSkillTreeMap, PlayerClassType.Knight);
        //buildSkillTreeNodes(KnightSkill1Tree, SlotType.Skill1, KnightSkillTreeMap, PlayerClassType.Knight);
        //buildSkillTreeNodes(KnightSkill2Tree, SlotType.Skill2, KnightSkillTreeMap, PlayerClassType.Knight);
    }

    // Mapping 과 Tree Build를 구분하자.

    public void BuildSkillTree(PlayerClassType _class, SlotType _slot)
    {
        PlayerSkillContainer _container;
        SkillTree Tree;
        switch (_slot)
        {
            default:
            case SlotType.Normal:
                Tree= LibraryManager.Instance.playerAnalyses[_class].NormalSkillTree;
                _container = GetContainer(_class).Item1;
                break;
            case SlotType.Skill1:
                Tree = LibraryManager.Instance.playerAnalyses[_class].Active1SkillTree;
                _container = GetContainer(_class).Item2;
                break;
            case SlotType.Skill2:
                Tree = LibraryManager.Instance.playerAnalyses[_class].Active2SkillTree;
                _container = GetContainer(_class).Item3;
                break;
        }

        Tree.StartNode.isEarned = true;
        int SkillLength = _container.SkillDataset.Count;
        //PlayerSkillScriptableObject StartSkill = _container.SkillDataset[0].Skills[0];
        // Tree.StartNode.SkillObject = StartSkill;
        List<TreeNode> TempParents = new List<TreeNode>();
        TempParents.Add(Tree.StartNode);
        for (int i = 1; i < SkillLength; i++) // Tiers
        {
            int TierLength = _container.SkillDataset[i].Skills.Count;
            List<TreeNode> NextParents = new List<TreeNode>();
            for (int j = 0; j < TierLength; j++) // Child
            {
                PlayerSkillScriptableObject TierSkill = _container.SkillDataset[i].Skills[j];
                string NodeName = $"{i}_{j}";
                TreeNode treeNode = new TreeNode();
                treeNode.Name = NodeName;
                // treeNode.SkillObject = TierSkill;


                for (int t = 0; t < TempParents.Count; t++)
                {
                    TempParents[t].Add(treeNode);
                }
                NextParents.Add(treeNode);
            }
            TempParents = NextParents;
        }
    }

    public void SkillTreeMapping(PlayerClassType _class)
    {
        SkillTree normalTree = LibraryManager.Instance.playerAnalyses[_class].NormalSkillTree;
        SkillTree Active1Tree = LibraryManager.Instance.playerAnalyses[_class].Active1SkillTree;
        SkillTree Active2Tree = LibraryManager.Instance.playerAnalyses[_class].Active2SkillTree;

        SkillTreeMap treeMap = GetSkillTreeMap(_class);

        Mapping(treeMap, normalTree, SlotType.Normal, _class);
        Mapping(treeMap, Active1Tree, SlotType.Skill1, _class);
        Mapping(treeMap, Active2Tree, SlotType.Skill2, _class);
    }

    private void Mapping(SkillTreeMap _treeMap, SkillTree _tree, SlotType _slot, PlayerClassType _class)
    {
        PlayerSkillContainer _container;
        switch (_slot)
        {
            default:
            case SlotType.Normal:
                _container = GetContainer(_class).Item1;
                break;
            case SlotType.Skill1:
                _container = GetContainer(_class).Item2;
                break;
            case SlotType.Skill2:
                _container = GetContainer(_class).Item3;
                break;
        }

        SkillSlotRow[] SkillRows = _treeMap.skillSlotRows;
       
        for (int row = 0; row < SkillRows.Length; row++)
        {
            List<PlayerSkillScriptableObject> Skills = _container.SkillDataset[row].Skills;
            List<SkillSlotButton> Slots;
            switch (_slot)
            {
                default:
                case SlotType.Normal:
                    Slots = SkillRows[row].NormalSlots;
                    break;
                case SlotType.Skill1:
                    Slots = SkillRows[row].Active1Slots;
                    break;
                case SlotType.Skill2:
                    Slots = SkillRows[row].Active2Slots;
                    break;
            }

            int btnCnt = 0;
            int EarnCnt = -1;
            foreach (SkillSlotButton btn in Slots)
            {
                if (btn.isEmpty) continue;
                
                btn.NodeValue = _tree.FindNode($"{row}_{btnCnt}");
                btn.NodeValue.SkillObject = Skills[btnCnt++];
                btn.SetSkillButton();

                if (row == 0) btn.SetStartNode(true);
                if (row == SkillRows.Length - 1) btn.SetEndNode(true);

                if (btn.NodeValue.isEarned)
                {
                    btn.ButtonEarned();
                    EarnCnt = btnCnt - 1;
                }
                else
                {
                    btn.ButtonNotActivatedYet();
                }
            }

            if (EarnCnt != -1)
            {
                btnCnt = 0;
                // Blocking UnEarned Tier Slots, As Earned Slot Exist.
                foreach (SkillSlotButton btn in Slots)
                {
                    if (btn.isEmpty) continue;
                    if (btnCnt == EarnCnt) continue;

                    btn.ButtonDeActivated();
                }
            }


        }
    }

    public bool SkillEarnableCheck(SlotType _slot, string _nodeName, PlayerClassType _class)
    {
        PlayerSkillContainer _container;
        SkillTree Tree;
        switch (_slot)
        {
            default:
            case SlotType.Normal:
                Tree = LibraryManager.Instance.playerAnalyses[_class].NormalSkillTree;
                _container = GetContainer(_class).Item1;
                break;
            case SlotType.Skill1:
                Tree = LibraryManager.Instance.playerAnalyses[_class].Active1SkillTree;
                _container = GetContainer(_class).Item2;
                break;
            case SlotType.Skill2:
                Tree = LibraryManager.Instance.playerAnalyses[_class].Active2SkillTree;
                _container = GetContainer(_class).Item3;
                break;
        }

        TreeNode TargetNode = Tree.FindNode(_nodeName);
        int Tier = TargetNode.Tier;
        int Index = TargetNode.Index;
        if (Tier == 0) return true;

        if (TargetNode.SkillObject.isSynergy)
        {
            SkillTree otherTree = LibraryManager.Instance.playerAnalyses[_class].Active2SkillTree;

            if (Tree.FindSynergyEarned(TargetNode.SkillObject.SynergySkill1) &&
                otherTree.FindSynergyEarned(TargetNode.SkillObject.SynergySkill1))
            {
                return true;
            }
        }

        int TierLength = _container.SkillDataset[Tier -1].Skills.Count;
        for (int j = 0; j < TierLength; j++) // Child
        {
            TreeNode targetNode = Tree.FindNode($"{Tier - 1}_{j}");
            if (targetNode.isEarned) return true;
        }

        return false;
    }

    public bool SkillEarnableCheck(SlotType _slot, TreeNode TargetNode, PlayerClassType _class)
    {
        PlayerSkillContainer _container;
        SkillTree Tree;
        switch (_slot)
        {
            default:
            case SlotType.Normal:
                Tree = LibraryManager.Instance.playerAnalyses[_class].NormalSkillTree;
                _container = GetContainer(_class).Item1;
                break;
            case SlotType.Skill1:
                Tree = LibraryManager.Instance.playerAnalyses[_class].Active1SkillTree;
                _container = GetContainer(_class).Item2;
                break;
            case SlotType.Skill2:
                Tree = LibraryManager.Instance.playerAnalyses[_class].Active2SkillTree;
                _container = GetContainer(_class).Item3;
                break;
        }

        int Tier = TargetNode.Tier;
        int Index = TargetNode.Index;
        
        // 1. 현재 티어가 0 == 레벨 1 스킬이면 획득 가능.
        if (Tier == 0) return true;

        // 2. 현재 티어에 해당하는 다른 스킬을 얻었을 경우, 획득 불가능.
        for (int j = 0; j < _container.SkillDataset[Tier].Skills.Count; j++) // Child
        {
            if (Index == j) continue;
            TreeNode targetNode = Tree.FindNode($"{Tier}_{j}");
            if (targetNode.isEarned) return false;
        }

        // 3. 현재 스킬 획득 조건이 시너지가 필요하다면, 다른 트리까지 참조하여 획득 체크
        if (TargetNode.SkillObject.isSynergy)
        {
            SkillTree otherTree = LibraryManager.Instance.playerAnalyses[_class].Active2SkillTree;

            if (Tree.FindSynergyEarned(TargetNode.SkillObject.SynergySkill1) &&
                otherTree.FindSynergyEarned(TargetNode.SkillObject.SynergySkill2))
            {
                return true;
            }
        }

        // 4. 마지막으로, 이전 티어 스킬이 찍혀 있다면 획득 가능
        int TierLength = _container.SkillDataset[Tier - 1].Skills.Count;
        for (int j = 0; j < TierLength; j++) // Child
        {
            TreeNode targetNode = Tree.FindNode($"{Tier - 1}_{j}");
            if (targetNode.isEarned) return true;
        }

        // 5. 획득 불가능 상태
        return false;
    }

    public bool SkillEarnedCheck(SlotType _slot, string _nodeName, PlayerClassType _class)
    {
        SkillTree Tree;
        switch (_slot)
        {
            default:
            case SlotType.Normal:
                Tree = LibraryManager.Instance.playerAnalyses[_class].NormalSkillTree;
                break;
            case SlotType.Skill1:
                Tree = LibraryManager.Instance.playerAnalyses[_class].Active1SkillTree;
                break;
            case SlotType.Skill2:
                Tree = LibraryManager.Instance.playerAnalyses[_class].Active2SkillTree;
                break;
        }

        TreeNode TargetNode = Tree.FindNode(_nodeName);
        if (TargetNode.isEarned) return true;

        return false;
    }

    #region LEGACY
    //private void buildSkillTreeNodes(PlayerSkillContainer _container, SlotType _slot, SkillTreeMap _treeMap, PlayerClassType _class)
    //{

    //    int SkillLength = _container.SkillDataset.Count;
    //    string StartNodeName = "0_0";
    //    SkillTree Tree = new SkillTree(StartNodeName);
    //    Tree.StartNode.isEarned = true;

    //    PlayerSkillScriptableObject StartSkill = _container.SkillDataset[0].Skills[0];
    //    Tree.StartNode.SkillObject = StartSkill;
    //    SkillSlotRow[] SkillRows = _treeMap.skillSlotRows;

    //    List<SkillSlotButton> btns;
    //    switch (_slot)
    //    {
    //        default:
    //        case SlotType.Normal:
    //            btns = SkillRows[0].NormalSlots;
    //            break;
    //        case SlotType.Skill1:
    //            btns = SkillRows[0].Active1Slots;
    //            break;
    //        case SlotType.Skill2:
    //            btns = SkillRows[0].Active2Slots;
    //            break;
    //    }

    //    foreach (SkillSlotButton btn in btns)
    //    { 
    //        if(btn.isEmpty) continue;
    //        btn.SetStartNode(true);
    //        btn.NodeValue = Tree.StartNode;
    //        btn.SetSkillButton();
    //        btn.ButtonClicked();
    //        break;
    //    }



    //    for (int i = 1; i < SkillLength; i++)
    //    {
    //        List<SkillSlotButton> SkillButtons;
    //        switch (_slot)
    //        {
    //            default:
    //            case SlotType.Normal:
    //                SkillButtons = SkillRows[i].NormalSlots;
    //                break;
    //            case SlotType.Skill1:
    //                SkillButtons = SkillRows[i].Active1Slots;
    //                break;
    //            case SlotType.Skill2:
    //                SkillButtons = SkillRows[i].Active2Slots;
    //                break;
    //        }


    //        int TierLength = _container.SkillDataset[i].Skills.Count;
    //        for (int j = 0; j < TierLength; j++)
    //        {
    //            SkillSlotButton btn;
    //            foreach (SkillSlotButton b in SkillButtons)
    //            {
    //                if (b.isEmpty) continue;
    //                if (b.NodeValue != null) continue;
    //                btn = b;
    //                btn.SetStartNode(true);
    //                PlayerSkillScriptableObject skills = _container.SkillDataset[i].Skills[j];
    //                string NodeName = $"{i}_{j}";
    //                TreeNode treeNode = new TreeNode(NodeName);
    //                treeNode.SkillObject = skills;
    //                for (int pre = 0; pre < _container.SkillDataset[i - 1].Skills.Count; pre++)
    //                {
    //                    Tree.Add($"{i - 1}_{pre}", treeNode);
    //                }

    //                btn.NodeValue = treeNode;
    //                btn.SetSkillButton();
    //                btn.ButtonClicked();
    //                break;
    //            }
    //        }
    //    }

    //    switch (_slot)
    //    {
    //        default:
    //        case SlotType.Normal:
    //            PlayerNormalSkillTreeDict[_class] = Tree;
    //            break;
    //        case SlotType.Skill1:
    //            PlayerActive1SkillTreeDict[_class] = Tree;
    //            break;
    //        case SlotType.Skill2:
    //            PlayerActive2SkillTreeDict[_class] = Tree;
    //            break;
    //    }
    //}
    #endregion

    public (PlayerSkillContainer, PlayerSkillContainer, PlayerSkillContainer) GetContainer(PlayerClassType _class)
    {
        switch (_class)
        {
            default:
            case PlayerClassType.Knight:
                return (KnightNormalTree, KnightSkill1Tree, KnightSkill2Tree);
            case PlayerClassType.Archer:
                return (null, null, null);
            case PlayerClassType.SpellMaster:
                return (null, null, null);
            case PlayerClassType.Buffer:
                return (null, null, null);
            case PlayerClassType.SoulMaster:
                return (null, null, null);
            case PlayerClassType.Thief:
                return (null, null, null);
        }
    }

    public SkillTreeMap GetSkillTreeMap(PlayerClassType _class)
    {
        switch (_class)
        {
            default:
            case PlayerClassType.Knight:
                return KnightSkillTreeMap;
            case PlayerClassType.Archer:
                return null;
            case PlayerClassType.SpellMaster:
                return null;
            case PlayerClassType.Buffer:
                return null;
            case PlayerClassType.SoulMaster:
                return null;
            case PlayerClassType.Thief:
                return null;
        }
    }
}