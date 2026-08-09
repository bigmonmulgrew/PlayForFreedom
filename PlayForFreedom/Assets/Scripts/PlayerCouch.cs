using BMD;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerCouch : NetworkBehaviour
{
    static PlayerCouch instance;
    static PlayerCouch instanceLocal;
    public static PlayerCouch Instance => instanceLocal != null ? instanceLocal : instance;
    public static int nextCouchindex = 0;

    #region Config
    [SerializeField] Player defaultAvatar;

    #endregion

    #region Cached References
    NetworkObject couchNetworkObject;

    #endregion

    #region Runtime Variables
    NetworkVariable<int> playerIndex = new(-1);

    int playerID = -1;
    int seats;
    
    // TODO switch from single avatar spawn to multiple
    // List<Player> avatars = new();
    NetworkObject spawnedAvatar;
    #endregion

    public int Seats => seats;

    private void Awake()
    {
        instance = this;
        couchNetworkObject = GetComponent<NetworkObject>();
    }

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

        if (instanceLocal == null)
        {
            instanceLocal = this;
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
    // Called by local UI
    public void SpawnAvatar(int playerUIIndex)
    {
        if (!IsOwner) return;
        RequestSpawnAvatarRPC(playerUIIndex);
    }

    public void SetPlayerData(int playerUIIndex, PlayerConfig playerConfig)
    {
        if (!IsOwner) return;

        SetPlayerDataRPC(playerUIIndex, playerConfig);
    }
    public void RequestControl()
    {
        // TODO expand to allow control of multiple avatars per couch
        if (!IsOwner) return;
        GrantControlRPC();
    }

    [Rpc(SendTo.Server)]
    void RequestSpawnAvatarRPC(int playerUIIndex, RpcParams rpcParams = default)
    {
        // Never trust the client to provide an ID
        ulong senderID = rpcParams.Receive.SenderClientId;

        if (senderID != OwnerClientId) return;

        if (playerUIIndex < 0) return;

        // TODO this pervents us adding a second player on the same couch, rework for couch co-op
        if (spawnedAvatar != null && spawnedAvatar.IsSpawned) return;
        if (spawnedAvatar != null) Destroy(spawnedAvatar);

        if (!TryGetSpawnTransformFromPedestal(playerUIIndex, out Transform targetSpawn, out PlayerAvatarDemo selectedPedestal)) return;

        Player newAvatar = Instantiate(defaultAvatar, targetSpawn.position, targetSpawn.rotation);
        
        spawnedAvatar = newAvatar.GetComponent<NetworkObject>();

        if (spawnedAvatar == null)
        {
            Debug.LogError("Avatar prefab requires NetworkObject.");
            Destroy(newAvatar);
            return;
        }

        // Avatar control gate
        if (newAvatar.GetComponent<PlayerController>() == null)
        {
            Debug.LogError("Avatar prefab requires PlayerController.");
            Destroy(newAvatar);
            spawnedAvatar = null;
            return;
        }

        // Important:
        // Spawn normally, WITHOUT giving the client ownership yet.
        //
        // At this point the avatar exists on the network but the
        // player has not been granted control.
        spawnedAvatar.Spawn();

        selectedPedestal.SetDemoAvatar(spawnedAvatar);

    }

    [Rpc(SendTo.Server)]
    void SetPlayerDataRPC(int playerUIIndex, PlayerConfig playerData)
    {
        PlayerAvatarDemo[] allPlayerAvatarDemos = FindObjectsByType<PlayerAvatarDemo>();

        foreach (PlayerAvatarDemo pad in allPlayerAvatarDemos)
        {
            if (pad.PlayerUIIndex == playerUIIndex) pad.DemoAvatar.SetPlayerData(playerData);
        }

            
    }
    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();
        if (!IsServer) return;
        if (spawnedAvatar != null && spawnedAvatar.IsSpawned) spawnedAvatar.Despawn();
    }

    [Rpc(SendTo.Server)]
    void GrantControlRPC(RpcParams rpcParams = default)
    {
        // Never trust the client to provide an ID
        ulong senderID = rpcParams.Receive.SenderClientId;

        if (senderID != OwnerClientId) return;

        if (spawnedAvatar == null || !spawnedAvatar.IsSpawned) return;

        PlayerController controlGate = spawnedAvatar.GetComponent<PlayerController>();

        if (controlGate == null) return;

        // Grant ownership of the avatar
        spawnedAvatar.ChangeOwnership(senderID);

        controlGate.SetControlGranted(true);
    }

    public void StartGame()
    {
        StartGameRPC();
    }

    [Rpc(SendTo.Everyone)]
    void StartGameRPC()
    {
        SessionManager.StartGame();
    }

    bool TryGetSpawnTransformFromPedestal(int playerUIIndex, out Transform foundTransform, out PlayerAvatarDemo selectedPedestal)
    {
        bool success = false;
        foundTransform = null;
        selectedPedestal = null;

        PlayerAvatarDemo[] allPlayerAvatarDemos = FindObjectsByType<PlayerAvatarDemo>();

        foreach(PlayerAvatarDemo pad in allPlayerAvatarDemos)
        {
            if (pad.PlayerUIIndex == playerUIIndex)
            {
                foundTransform = pad.AvatarSpawn;
                selectedPedestal = pad;
                success = true;
                break;
            }
        }

        return success;
    }
}
