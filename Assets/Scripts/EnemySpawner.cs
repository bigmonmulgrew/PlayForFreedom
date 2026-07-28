using Unity.Netcode;
using UnityEngine;
using UnityEngine.AI;

public class EnemySpawner : NetworkBehaviour
{
    public void SpawnEnemy(Enemy enemyTempalte)
    {
        
        Enemy newEnemy = Instantiate(enemyTempalte, GetSpawnPosition(), transform.rotation);
        newEnemy.GetComponent<NetworkObject>().Spawn();

    }

    Vector3 GetSpawnPosition()
    {
        NavMesh.SamplePosition(transform.position, out NavMeshHit hit, 2f, NavMesh.AllAreas);
        return hit.position;
    }
}
