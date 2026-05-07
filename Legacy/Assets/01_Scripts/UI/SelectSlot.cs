using UnityEngine;
using UnityEngine.UI;

public class SelectSlot : MonoBehaviour
{
    [SerializeField] public PlayerClassType playerClass;
    [SerializeField] public Image Portrait;
    [SerializeField] public Button SlotButton;
    [SerializeField] public CanvasGroup EmptyCanvas;
    [SerializeField] public CanvasGroup BlockedCanvas;
}