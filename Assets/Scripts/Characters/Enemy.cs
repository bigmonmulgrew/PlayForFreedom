using UnityEngine;

public class Enemy: Character
{
    #region Cofiguration
    [SerializeField] float pickupDropChance = 0.5f;
    [SerializeField] Pickup[] pickups;
    #endregion

    #region Cached References
    EnemyRoomSpawner parentSpawner;
    #endregion


    protected override void Die()
    {
        base.Die();

        if (!IsDead) return;
            
        DropPickup();
        if (parentSpawner != null) parentSpawner.EnemyHasDied(this);
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

    public void SetRoomSpawner(EnemyRoomSpawner enemyRoomSpawner)
    {
        parentSpawner = enemyRoomSpawner;
    }
}
