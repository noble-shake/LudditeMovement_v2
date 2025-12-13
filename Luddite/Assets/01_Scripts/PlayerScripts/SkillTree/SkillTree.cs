
using System.Collections.Generic;
using TreeEditor;
using Unity.VisualScripting;

public class SkillTree
{
    public TreeNode StartNode { get; set; }

    public SkillTree()
    {
        StartNode = new TreeNode();
        StartNode.Name = "0_0";
    }

    public SkillTree(int _tier, int _Index)
    {
        StartNode = new TreeNode();
        StartNode.Name = $"{_tier}_{_Index}";
    }

    public SkillTree(string _start)
    {
        StartNode = new TreeNode();
        StartNode.Name = _start;

    }

    //public void Add(string _parent, string _node)
    //{
    //    var parentNode = FindNode(StartNode, _parent);
    //    if (parentNode != null)
    //    {
    //        var newTreeNode = new TreeNode(_node);
    //        parentNode.Add(newTreeNode);
    //    }

    //}

    public void Add(string _parent, TreeNode _node)
    {
        var parentNode = FindNode(StartNode, _parent);
        if (parentNode != null)
        {
            parentNode.Add(_node);
        }

    }

    public void Add(int _tier, int _Index, TreeNode _node)
    {
        var parentNode = FindNode(StartNode, $"{_tier}_{_Index}");
        if (parentNode != null)
        {
            parentNode.Add(_node);
        }

    }

    public TreeNode FindNode(TreeNode _currentNode, string _nodeName)
    {
        if (_currentNode.Name == _nodeName)
        {
            return _currentNode;
        }

        foreach (var child in _currentNode.Children)
        {
            var foundNode = FindNode(child, _nodeName);
            if (foundNode != null)
            {
                return foundNode;
            }
        }

        return null;
    }

    public TreeNode FindNode(TreeNode _currentNode, int _tier, int _Index)
    {
        string _nodeName = $"{_tier}_{_Index}";
        if (_currentNode.Name == _nodeName)
        {
            return _currentNode;
        }

        foreach (var child in _currentNode.Children)
        {
            var foundNode = FindNode(child, _nodeName);
            if (foundNode != null)
            {
                return foundNode;
            }
        }

        return null;
    }

    public TreeNode FindNode(int _tier, int _Index)
    {
        return FindNode(StartNode, _tier, _Index);
    }

    public TreeNode FindNode(string _nodeName)
    {
        return FindNode(StartNode, _nodeName);
    }

    public TreeNode FindSynergyNode(TreeNode _currentNode, PlayerSkillScriptableObject _skill)
    {
        if (_currentNode.SkillObject == _skill)
        {
            return _currentNode;
        }

        foreach (var child in _currentNode.Children)
        {
            var foundNode = FindSynergyNode(child, _skill);
            if (foundNode != null)
            {
                return foundNode;
            }
        }

        return null;
    }

    public bool FindSynergyEarned(PlayerSkillScriptableObject _skill)
    {
        TreeNode Target = FindSynergyNode(StartNode, _skill);
        if (Target != null)
        {
            return Target.isEarned;
        }

        return false;
    }
}