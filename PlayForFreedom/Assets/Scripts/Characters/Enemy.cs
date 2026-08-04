using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Networking;

public class Enemy: Character
{
    const float DESPAWN_INITIAL_COOLDOWN = 1.0f;

    #region Configuration
    [SerializeField] protected float pickupDropChance = 0.5f;
    [SerializeField] protected Pickup[] pickups;

    [Header("Despawn conditions")]
    [SerializeField] bool despawnOnTrigger = false;
    [SerializeField] bool despawnOnTimer = false;
    [SerializeField] float despawnTimer = 10.0f;
    #endregion

    #region Cached References
    EnemyRoomSpawner parentSpawner;
    #endregion

    #region Runtime Variables
    float timeToDespawn;
    bool finishedSpawning;
    #endregion

    public bool ReadyToFire => Time.time >= weapon.NextFireTime;

    public EnemyRoomSpawner ParentSpawner => parentSpawner;

    protected override void OnEnable()
    {
        base.OnEnable();
        timeToDespawn = Time.time + despawnTimer;
        StartCoroutine(DelayedEnable());
    }
    IEnumerator DelayedEnable()
    {
        yield return new WaitForSeconds(DESPAWN_INITIAL_COOLDOWN);
        finishedSpawning = true;
    }
    private void Update()
    {
        CheckDespawnTime();
        
    }

    void CheckDespawnTime()
    {
        if (!despawnOnTimer) return;
        if (Time.time < timeToDespawn) return;
        RemoveCharacter();
    }

    protected override void Die()
    {
        base.Die();

        if (!IsDead) return;
            
        DropPickup();
        if (parentSpawner != null) parentSpawner.EnemyHasDiedOrRemoved(this);
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

    private void OnTriggerEnter(Collider other)
    {
        if (finishedSpawning && despawnOnTrigger && other.CompareTag("EnemyDespawn")) RemoveCharacter();
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("EnemyDespawn")) finishedSpawning = true;
    }

    protected override void RemoveCharacter()
    {
        base.RemoveCharacter();

        if (!IsDespawning) return;

        if (parentSpawner != null) parentSpawner.EnemyHasDiedOrRemoved(this);
    }
}
