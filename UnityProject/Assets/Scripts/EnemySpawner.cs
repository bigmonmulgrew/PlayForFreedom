using Unity.Netcode;
using UnityEngine;
using UnityEngine.AI;

public class EnemySpawner : NetworkBehaviour
{
    public Enemy SpawnEnemy(Enemy enemyTempalte)
    {
        if (!IsServer) return null;

        Enemy newEnemy = Instantiate(enemyTempalte, GetSpawnPosition(), transform.rotation);
        
        newEnemy.GetComponent<NetworkObject>().Spawn();

        return newEnemy;

    }

    Vector3 GetSpawnPosition()
    {
        NavMesh.SamplePosition(transform.position, out NavMeshHit hit, 2f, NavMesh.AllAreas);
        return hit.position;
    }
}
