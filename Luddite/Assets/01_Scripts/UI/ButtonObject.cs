using UnityEngine;
using UnityEngine.EventSystems;

public abstract class ButtonObject : MonoBehaviour
{
    public virtual void OrbInteract(EventSystem _current)
    {
        Debug.Log("OrbInteract");
    }
}
