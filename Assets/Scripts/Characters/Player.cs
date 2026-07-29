using System;
using System.Collections.Generic;
using UnityEngine;
using CharacterController = BMD.CharacterController;

public class Player : Character
{
    public readonly static List<Player> AllPlayers = new();
    public readonly static List<Player> LocalPlayers = new();
    public readonly static List<Player> RemotePlayers = new();

    public event Action<int> OnScoreChanged;

    [SerializeField] LayerMask pickupsLayers = (1 << 12);

    int cashScore;
    string playerName = "Dave"; // TODO add UI to change this.

    public int CashScore => cashScore;

    public void SetPlayerName(string playerName)
    {
        this.playerName = playerName;
    }

    
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        AllPlayers.Add(this);
        PlayerUI.NewPlayerSpawned();

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

    private void OnTriggerEnter(Collider other)
    {
        if ((pickupsLayers.value & (1 << other.gameObject.layer)) != 0)
        {
            if (other.gameObject.transform.parent.TryGetComponent<Pickup>(out Pickup p))
            {
                cashScore += p.CashValue;
                OnScoreChanged?.Invoke(cashScore);
                p.DestroyPickup();
                Debug.Log($"{playerName} now has {cashScore}");
            }
        }
    }
}
