
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using CharacterController = BMD.CharacterController;

public class Player : Character
{
    public readonly static List<Player> AllPlayers = new();
    public readonly static List<Player> LocalPlayers = new();
    public readonly static List<Player> RemotePlayers = new();

    //public event Action<int> OnScoreChanged;
    [SerializeField] LayerMask pickupsLayers = (1 << 12);
    [SerializeField] Renderer customColourRenderer1;
    [SerializeField] int materialIndex1;
    [SerializeField] Renderer customColourRenderer2;
    [SerializeField] int materialIndex2;
    [SerializeField] Renderer customColourRenderer3;
    [SerializeField] int materialIndex3;

    NetworkVariable<int> cashScore = new(0);
    string playerName = "Dave"; // TODO add UI to change this.

    public NetworkVariable<int> CashScore => cashScore;

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
        if (!IsServer) return;

        if ((pickupsLayers.value & (1 << other.gameObject.layer)) != 0)
        {
            if (other.gameObject.transform.parent.TryGetComponent<Pickup>(out Pickup p))
            {
                cashScore.Value += p.CashValue;
                
                p.DestroyPickup();
            }
        }
    }

    public void SetPlayerData(PlayerConfig playerConfig)
    {
        name = playerConfig.name != "" ? playerConfig.name : $"Player {AllPlayers.Count}";
        cashScore.Value = playerConfig.startingMoney;
        customColourRenderer1.materials[materialIndex1].color = playerConfig.customColour1;
        customColourRenderer2.materials[materialIndex2].color = playerConfig.customColour2;
        customColourRenderer3.materials[materialIndex3].color = playerConfig.customColour3;

    }

    public void SetPlayerColour(Color col1, Color col2, Color col3)
    {
        customColourRenderer1.materials[materialIndex1].color = col1;
        customColourRenderer2.materials[materialIndex2].color = col2;
        customColourRenderer3.materials[materialIndex3].color = col3;
    }

}
