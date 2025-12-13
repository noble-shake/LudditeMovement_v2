using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public enum CharacterInfoMenu
{ 
    Info,
    Soul,
    Skill,
}

public abstract class CharacterInfoIcon : MonoBehaviour
{
    [SerializeField] public PlayerClassType CurrentClassType = PlayerClassType.Knight;
    [SerializeField] public CharacterInfoMenu menu;
    [SerializeField] protected Button Icon;
    protected CharacterInfoGroup GroupManager;
    private Color OriginColor = new Color(0.5f, 0.5f, 0.5f);

    protected virtual void OnDisable()
    {
        DisSelect();
    }

    public void Start()
    {
        CurrentClassType = PlayerClassType.Knight;
        GroupManager = GetComponentInParent<CharacterInfoGroup>();
        GroupManager.ClearAction += DisSelect;
        Icon = GetComponent<Button>();
        Icon.onClick.AddListener(OnIconClicked);
        if (menu == CharacterInfoMenu.Info)
        {
            OnIconClicked();
        }
    }

    public virtual void OnIconClicked()
    {
        GroupManager = GetComponentInParent<CharacterInfoGroup>();
        GroupManager.Clear();
        ColorBlock cb = Icon.colors;
        cb.normalColor = Color.white;
        Icon.colors = cb;
    }

    public virtual void DisSelect()
    {
        ColorBlock cb = Icon.colors;
        cb.normalColor = OriginColor;
        Icon.colors = cb;
    }

}
