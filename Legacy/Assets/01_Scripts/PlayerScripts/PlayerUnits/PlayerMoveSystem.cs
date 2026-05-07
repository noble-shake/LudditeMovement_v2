using UnityEngine;

// 9, 16
public class PlayerMoveSystem : MonoBehaviour
{
    int width = 16;
    int height = 9;


    // Open List : Node Queue For Searching
    // Closed List : Already Been Searched

    // Keep Going Untill :  Current Node = End Node  or OpenList is Empty


    // A* Algorithm Cost Function (F) = G + H
    private float CostFunctionEvaluate(float PathFromStartCost, float HeuristicFuncCost)
    {
        return PathFromStartCost + HeuristicFuncCost; 
    }


}