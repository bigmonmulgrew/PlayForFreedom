using Unity.Netcode;
using UnityEngine;

public class EnemySpawner : NetworkBehaviour
{
   public void SpawnEnemy(Enemy enemyTempalte)
    {
        
        Enemy newEnemy = Instantiate(enemyTempalte, transform.position, transform.rotation);
        newEnemy.GetComponent<NetworkObject>().Spawn();

    }
}
