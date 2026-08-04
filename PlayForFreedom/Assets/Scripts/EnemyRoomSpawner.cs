using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyRoomSpawner : NetworkBehaviour
{
    #region config
    [SerializeField] Enemy[] enemies;
    [SerializeField] float initialSpawnDelay = 3f;
    [SerializeField] float spawnInterval = 0.25f;
    [SerializeField] int enemiesToSpawn = 10;
    #endregion

    #region Cached references
    EnemySpawner[] spawners;
    Room room;
    #endregion

    #region Runtime Variables
    List<Enemy> spawnedEnemies = new();
    float nextSpawnTime = 0;
    int totalSpawnedEnemies;
    int totalKilledEnemies;
    bool startSpawning = false;
    bool spawningComplete = false;
    #endregion

    float StartSpawningTime => Time.time + initialSpawnDelay;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        FindReferences();
        SanityChecks();

    }
    private void FindReferences()
    {
        spawners = GetComponentsInChildren<EnemySpawner>();
        room = GetComponent<Room>();
    }
    void SanityChecks()
    {
        if (spawners.Length == 0) Debug.LogError($"{name} cannot find any enemy spawner. Please places some on this room.", this);
        if (enemiesToSpawn == 0 && enemies.Length > 0) Debug.LogError($"{name} has enemies configured but is set to spawn 0 eneimes.", this);
        if (room == null) Debug.LogError($"{name} has no Room script attached, please add one.", this);
    }

    void Update()
    {
        if (!IsServer) return;
        if (!IsHost) return;
        if (!startSpawning) return;

        SpawnEnemy();
    }

    void SpawnEnemy()
    {
        if (!IsServer) return;
        if (spawningComplete) return;
        if (totalSpawnedEnemies > enemiesToSpawn) return;
        if (Time.time < nextSpawnTime) return;

        nextSpawnTime += spawnInterval;

        if (enemies.Length == 0 || spawners.Length == 0) 
        {
            Debug.LogError($"EnemyRoomSpawner: {gameObject.name} unable to spawn enemy. Enemies count: {enemies.Length} Spawners Count: {spawners.Length}", this);
            return; 
        }

        totalSpawnedEnemies++;
        if (totalSpawnedEnemies >= enemiesToSpawn) spawningComplete = true;

        Enemy newEnemy = enemies[Random.Range(0, enemies.Length)];
        EnemySpawner selectedSpawner = spawners[Random.Range(0, spawners.Length)];
        
        Enemy spawnedEnemy = selectedSpawner.SpawnEnemy(newEnemy);
        spawnedEnemy.SetRoomSpawner(this);

        spawnedEnemies.Add(spawnedEnemy);

    }

    public void StartSpawning()
    {
        if (!IsServer) return;
        if (spawningComplete) return;

        startSpawning = true;
        nextSpawnTime = StartSpawningTime;
    }
    public void EnemyHasDiedOrRemoved(Enemy enemy)
    {
        if (spawnedEnemies.Remove(enemy)) totalKilledEnemies++;

        if (!IsServer) return;
        if (!spawningComplete) return;

        if (totalKilledEnemies < totalSpawnedEnemies) return;

        if (room) room.FinishRoomRPC();

    }
}
