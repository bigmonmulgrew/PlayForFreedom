using UnityEngine;

public class Enemy: Character
{

    [SerializeField] float pickupDropChance = 0.5f;
    [SerializeField] Pickup[] pickups;

    protected override void Die()
    {
        base.Die();
        if (IsDead) DropPickup();
    }
    void DropPickup()
    {
        float rng = Random.Range(0f, 1f);
        if (rng < pickupDropChance)
        {
            if (pickups.Length == 0) return;

            Pickup p = pickups[Random.Range(0, pickups.Length)];
            Instantiate(p, transform.position, Quaternion.identity);
        }
    }
}
