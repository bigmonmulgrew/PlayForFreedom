
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Room : NetworkBehaviour
{
    public static List<Room> AllRooms = new();
   

    #region Cached reference
    RoomStartTrigger startTrigger;
    EnemyRoomSpawner enemyRoomSpawner;
    RoomCameraLocations cameraLocations;
    Door[] doorList;
    #endregion

    List<PlayerUI> playerUIList = new();

    public List<PlayerUI> PlayerUIList {  get { return playerUIList; }  set { playerUIList = value; } }

    private void Awake()
    {
        AllRooms.Add(this);
    }

    void FindReferences()
    {
        // This skips searching in children if already assigned.
        startTrigger = startTrigger != null ? startTrigger : GetComponentInChildren<RoomStartTrigger>();

        enemyRoomSpawner = enemyRoomSpawner != null ? enemyRoomSpawner : GetComponent<EnemyRoomSpawner>();

        doorList = GetComponentsInChildren<Door>();

        cameraLocations = GetComponentInChildren<RoomCameraLocations>();
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
        
        foreach (Door d in doorList)
        {
            d.Close();
        }

        Transform newCamerTransform = cameraLocations.GetTransformAtIndex(0);   // TODO add some smart transtion so we preserve relative transforms.
        ArenaCamera.Instance?.SetNewTransfrom(newCamerTransform);
    }

    public void FinishRoom()
    {
        if (!IsServer) return;

        foreach (Door d in doorList)
        {
            d.Open();
        }
    }
    public void LockOtherDoors(Door door)
    {
        foreach (Door d in doorList)
        {
            if (door != d) d.LockDoor();
        }
    }
}
