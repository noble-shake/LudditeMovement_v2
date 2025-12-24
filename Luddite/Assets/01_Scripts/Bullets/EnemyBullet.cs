using System.Collections.Generic;
using UnityEngine;

public class EnemyBullet : Bullet
{
    [SerializeField] private int BulletHP;
    public IEnemyBullet pattern;
    public BulletSprite bulletSprite;

    private void Update()
    {
        if (pattern == null) return;
        pattern.Update();
    }

    // Player는 맞았을 때, 무적 상태가 될 것임.
    // PlayerOrb는 지속적으로 대미지를 입음. 
    public override void OnHit(GameObject _Triggered, bool isOrb=false)
    {
        if(isOrb == false) BulletHP--;
        if (BulletHP == 0) ResourceManager.Instance.ResourceRetrieve(this.gameObject);

        // Player Hit Function.
    }

    private void OnBecameInvisible()
    {
        // Resource Manager Return
        ResourceManager.Instance.ResourceRetrieve(this.gameObject);
    }

    public void SetAngle(Vector3 _Euler)
    {
        transform.rotation = Quaternion.Euler(_Euler);
    }
}