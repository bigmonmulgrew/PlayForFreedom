using Unity.Netcode;
using UnityEngine;
using UnityEngine.AI;

public class EnemySpawner : NetworkBehaviour
{
    public Enemy SpawnEnemy(Enemy enemyTempalte)
    {
        if (!IsServer) return null;

        if (enemyTempalte == null)
        {
            Debug.Log("Enemy Template was null");
            return null;
        }

        Debug.Log($"Enemy spawner {name} attempting to spawn enemy using prefab {enemyTempalte.name}");

        Vector3 spawnPosition = GetSpawnPosition();

        if (float.IsInfinity(spawnPosition.x)) spawnPosition = transform.position;

        Enemy newEnemy = Instantiate(enemyTempalte, spawnPosition, transform.rotation);

        newEnemy.GetComponent<NetworkObject>().Spawn();

        return newEnemy;

    }

    Vector3 GetSpawnPosition()
    {
        NavMesh.SamplePosition(transform.position, out NavMeshHit hit, 2f, NavMesh.AllAreas);
        return hit.position;
    }
}
