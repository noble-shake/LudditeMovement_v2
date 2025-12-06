using UnityEngine;
public class SoulItem : DropItem
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private float SoulGauge;
    float duration = 10f;

    public float SoulValue { get { return SoulGauge; } set { SoulGauge = value; } }

    protected override void Start()
    {
        base.Start();
        spriteRenderer = GetComponent<SpriteRenderer>();

    }

    private void Update()
    {
        duration -=Time.deltaTime;
        if (duration <= 0f)
        {
            duration = 10f;
            ResourceManager.Instance.SoulRetrieve(this);
        }
    }

    public override void SetSprite(Sprite _sprite)
    { 
        spriteRenderer.sprite = _sprite;
    }

    public override void ItemEarned()
    {
        base.ItemEarned();
        PlayerManager.Instance.SoulValue += SoulGauge;
        ResourceManager.Instance.SoulRetrieve(this);
    }

    private void OnBecameInvisible()
    {
        base.ItemEarned();
        if(gameObject.activeSelf) ResourceManager.Instance.SoulRetrieve(this);
    }

}