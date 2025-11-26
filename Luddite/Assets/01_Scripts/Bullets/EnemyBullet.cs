using UnityEngine;

public class EnemyBullet : Bullet
{
    [SerializeField] private int BulletHP;
    public IEnemyBullet pattern;

    private void Update()
    {
        if (pattern == null) return;
        pattern.Update();
    }

    public override void OnHit(GameObject _Triggered)
    {
        BulletHP--;
        if (BulletHP == 0) ResourceManager.Instance.ResourceRetrieve(this.gameObject);


    }

    private void OnBecameInvisible()
    {
        // Resource Manager Return
        ResourceManager.Instance.ResourceRetrieve(this.gameObject);
    }
}