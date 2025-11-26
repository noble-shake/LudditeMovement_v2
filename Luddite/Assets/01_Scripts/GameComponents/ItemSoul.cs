using UnityEngine;

public class ItemSoul : ItemComponent, IEarnHandler
{
    public void Drop()
    {
        throw new System.NotImplementedException();
    }

    public void Earn()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {

    }
}