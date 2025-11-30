using UnityEngine;
public class SoulItem : DropItem
{
    [SerializeField] private SpriteRenderer spriteRenderer;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

    }

    public override void SetSprite(Sprite _sprite)
    { 
        spriteRenderer.sprite = _sprite;
    }


}