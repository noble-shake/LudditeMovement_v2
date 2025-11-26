using UnityEngine;

public enum DropType
{ 
    Score,
    Soul,
    Item,
}

public abstract class ItemComponent : MonoBehaviour
{
    protected DropType dropType;
    protected int Score;

    public void SetDropType(DropType _type) => dropType = _type;
    public void SetScore(int _score) => Score = _score;

}