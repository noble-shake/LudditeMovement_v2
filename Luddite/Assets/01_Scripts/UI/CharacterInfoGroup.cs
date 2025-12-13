using System;
using UnityEngine;
using UnityEngine.UI;

public class CharacterInfoGroup : MonoBehaviour
{
    public Action ClearAction = new Action(() => { });

    public void Clear()
    {
        ClearAction.Invoke();
    }
}