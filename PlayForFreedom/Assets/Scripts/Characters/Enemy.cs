using Unity.Netcode;
using UnityEngine;
using UnityEngine.Networking;

public class Enemy: Character
{
    #region Cofiguration
    [SerializeField] protected float pickupDropChance = 0.5f;
    [SerializeField] protected Pickup[] pickups;
    #endregion

    #region Cached References
    EnemyRoomSpawner parentSpawner;
    #endregion

    public bool ReadyToFire => Time.time >= weapon.NextFireTime;

    protected override void Die()
    {
        base.Die();

        if (!IsDead) return;
            
        DropPickup();
        if (parentSpawner != null) parentSpawner.EnemyHasDied(this);
    }
    protected virtual void DropPickup()
    {
        float rng = Random.Range(0f, 1f);
        if (rng < pickupDropChance)
        {
            if (pickups.Length == 0) return;

            Pickup p = pickups[Random.Range(0, pickups.Length)];
            Pickup newPickup = Instantiate(p, transform.position, Quaternion.identity);
            newPickup.GetComponent<NetworkObject>().Spawn();
        }
    }

    public void SetRoomSpawner(EnemyRoomSpawner enemyRoomSpawner)
    {
        parentSpawner = enemyRoomSpawner;
    }
}
