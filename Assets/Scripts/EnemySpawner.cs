using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
   public void SpawnEnemy(Enemy enemyTempalte)
    {
        Enemy newEnemy = Instantiate(enemyTempalte, transform.position, transform.rotation);
    }
}
