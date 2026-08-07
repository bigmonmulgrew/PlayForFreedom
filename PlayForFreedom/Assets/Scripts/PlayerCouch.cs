using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerCouch : NetworkBehaviour
{
    public static PlayerCouch InstanceLocal;
    public static int nextCouchindex = 0;

    NetworkVariable<int> playerIndex = new(-1);

    [SerializeField] Player defaultAvatar;

    int playerID = -1;
    int seats;
    List<Player> avatars = new();

    public int Seats => seats;

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            playerIndex.Value  = nextCouchindex;
            nextCouchindex++;
        }

        playerIndex.OnValueChanged += OnPlayerIndexChanged;

        OnPlayerIndexChanged(-1, playerIndex.Value);

        if (!IsOwner) return;

        if (InstanceLocal == null)
        {
            InstanceLocal = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Debug.LogWarning("Duplicate Couch found locally, destroying extra.");
            Destroy(gameObject);
        }

    }


    void OnPlayerIndexChanged(int oldValue, int newValue)
    {
        if (IsOwner) Debug.Log($"I amn the owner with cloud ID {playerIndex.Value}");
    }
    private void Update()
    {
        if (!IsOwner) return;
        SpawnAvatar();
    }

    private void SpawnAvatar()
    {
        RequestSpawnAvatarRPC();
    }

    [Rpc(SendTo.Server)]
    void RequestSpawnAvatarRPC()
    {
        
    }
}
