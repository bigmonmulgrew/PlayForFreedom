using System;
using Unity.Netcode;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnmyRoomSpawner : NetworkBehaviour
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
    bool networkReady;
    bool spawningComplete = false;
    #endregion

    private void Awake()
    {
        spawners = GetComponentsInChildren<EnemySpawner>();
        nextSpawnTime = Time.time + initialSpawnDelay;
    }

    public override void OnNetworkSpawn()
    {
        nextSpawnTime = Time.time + initialSpawnDelay;
        networkReady = true;
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsHost || !networkReady) return;
        SpawnEnemy();
    }

    void SpawnEnemy()
    {
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

}
