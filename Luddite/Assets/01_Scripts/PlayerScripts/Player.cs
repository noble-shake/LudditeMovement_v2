using UnityEngine;


// 공통점 : 이동 방식, 활성화, 획득, 스탯

public enum PlayerType
{ 
    Knight,
    Healer,
    Wizard,
    Ranger,
    Thief,
}

public abstract class Player : MonoBehaviour
{ 
    protected PlayerType playerType;
}