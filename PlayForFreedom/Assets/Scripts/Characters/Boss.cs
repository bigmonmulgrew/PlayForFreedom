using Unity.Netcode;
using UnityEngine;

public class Boss : Enemy
{
    [SerializeField] int pickupsToDrop = 10;
    [SerializeField] float dropRange = 5.0f;

    protected override void Awake()
    {
        base.Awake();

    }

    protected override void DropPickup()
    {
        if (pickups.Length == 0) return;

        for (int i = 0; i < pickupsToDrop; i++)
        {
            Pickup p = pickups[Random.Range(0, pickups.Length)];

            Vector3 offset = new();
            offset.x = Random.Range(-dropRange, dropRange);
            offset.z = Random.Range(-dropRange, dropRange);


            Pickup newPickup = Instantiate(p, transform.position + offset, Quaternion.identity);
            newPickup.GetComponent<NetworkObject>().Spawn();
        }
    }
}
