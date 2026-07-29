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
    #endregion

    #region Runtime Variables
    float nextSpawnTime = 0;
    int enemyCount;
    bool startSpawning = false;
    bool spawningComplete = false;
    #endregion

    float StartSpawningTime => Time.time + initialSpawnDelay;

    private void Awake()
    {
        FindReferences();
        SanityChecks();
    }

    private void FindReferences()
    {
        spawners = GetComponentsInChildren<EnemySpawner>();
    }
    void SanityChecks()
    {
        if (spawners.Length == 0) Debug.LogError($"{name} cannot find any enemy spawner. Please places some on this room.", this);
        if (enemiesToSpawn == 0 && enemies.Length > 0) Debug.LogError($"{name} has enemies configured but is set to spawn 0 eneimes.", this);
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
        if (enemyCount > enemiesToSpawn) return;
        if (Time.time < nextSpawnTime) return;

        nextSpawnTime += spawnInterval;

        if (enemies.Length == 0 || spawners.Length == 0) 
        {
            Debug.LogError($"EnemyRoomSpawner: {gameObject.name} unable to spawn enemy. Enemies count: {enemies.Length} Spawners Count: {spawners.Length}", this);
            return; 
        }

        enemyCount++;
        if (enemyCount >= enemiesToSpawn) spawningComplete = true;

        Enemy newEnemy = enemies[Random.Range(0, enemies.Length)];
        EnemySpawner selectedSpawner = spawners[Random.Range(0, spawners.Length)];
        
        Enemy spawnedEnemy = selectedSpawner.SpawnEnemy(newEnemy);
    }

    public void StartSpawning()
    {
        if (!IsServer) return;
        if (spawningComplete) return;

        startSpawning = true;
        nextSpawnTime = StartSpawningTime;
    }
}
