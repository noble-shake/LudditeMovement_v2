using System.Collections.Generic;
using UnityEngine;



public class TreeNode
{ 
    public string Name { get; set; }
    public int Tier { get; set; }
    public int Index { get; set; }
    public PlayerSkillScriptableObject SkillObject { get; set; }

    public List<TreeNode> Children { get; set; }
    public TreeNode Parent;
    
    public bool isEarned { get; set; }

    public TreeNode()
    {
        Children = new List<TreeNode>();
    }

    public TreeNode(string _name)
    {
        Name = _name;
        Children = new List<TreeNode>();
    }

    public TreeNode(int _tier, int _Index)
    {
        Tier = _tier;
        Index = _Index;
        Name = $"{_tier}_{_Index}";
    }

    public TreeNode(string _name, PlayerSkillScriptableObject _skill)
    {
        Name = _name;
        SkillObject = _skill;
    }

    public void Add(TreeNode node)
    { 
        Children.Add(node);
    }


    
}