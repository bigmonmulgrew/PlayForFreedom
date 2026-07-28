using UnityEngine;

public class Pickup : MonoBehaviour
{
    [SerializeField] int cashValue = 1000;

    public int CashValue => cashValue;
    
    public void DestroyPickup()
    {
        Destroy(gameObject);
    }
}
