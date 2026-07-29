using Unity.Netcode;
using UnityEngine;

public class Room : NetworkBehaviour
{
    #region Cached reference
    RoomStartTrigger startTrigger;
    EnemyRoomSpawner enemyRoomSpawner;
    #endregion

    void FindReferences()
    {
        // This skips searching in children if already assigned.
        startTrigger = startTrigger != null ? startTrigger : GetComponentInChildren<RoomStartTrigger>();

        enemyRoomSpawner = enemyRoomSpawner != null ? enemyRoomSpawner : GetComponent<EnemyRoomSpawner>();
    }

    void SanityChecks()
    {
        if (!IsServer) return;
        if (startTrigger == null) Debug.LogError($"{name} cannot find start trigger. This is required.", this);
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (!IsServer) return;
        
        FindReferences();
        SanityChecks();
                
        startTrigger.RoomStartTriggered += StartRoom;
    }
    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();
        if (!IsServer) return;
        startTrigger.RoomStartTriggered -= StartRoom;
    }

    void StartRoom()
    {
        if (!IsServer) return;
        if(enemyRoomSpawner != null) enemyRoomSpawner.StartSpawning();
    }
}
