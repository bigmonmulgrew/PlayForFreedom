using System.Collections.Generic;
using UnityEngine;
using CharacterController = BMD.CharacterController;

public class Player : Character
{
    public readonly static List<Player> AllPlayers = new();
    public readonly static List<Player> LocalPlayers = new();
    public readonly static List<Player> RemotePlayers = new();

    
    void Update()
    {
        Debug.Log($"{name} is looking at {controller.LookInput}");
    }


    
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        AllPlayers.Add(this);

        if (IsOwner) LocalPlayers.Add(this);
        else RemotePlayers.Add(this);
    }
    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();
        AllPlayers.Remove(this);
        
        if (IsOwner) LocalPlayers.Remove(this);
        else RemotePlayers.Remove(this);
    }
    
    
    
}
