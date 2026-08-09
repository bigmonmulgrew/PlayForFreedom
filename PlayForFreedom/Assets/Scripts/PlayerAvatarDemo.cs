using Unity.Netcode;
using UnityEngine;

public class PlayerAvatarDemo : NetworkBehaviour
{
    [SerializeField] int playerUIIndex = -1;

    [SerializeField] Player defaultAvatar;
    [SerializeField] Transform avatarSpawn;

    PlayerSelectUI playerSelectUI;

    readonly NetworkVariable<NetworkObjectReference> demoAvatarNetworkRef = new(
        default, 
        NetworkVariableReadPermission.Everyone, 
        NetworkVariableWritePermission.Server
        );
    Player demoAvatar;
    
    public int PlayerUIIndex => playerUIIndex;
    public Player DemoAvatar => demoAvatar != null ? defaultAvatar : GetDemoAvatar();
    public Transform AvatarSpawn => avatarSpawn;
    

    private void Awake()
    {
        PlayerSelectUI[] allPlayerSelectUI = FindObjectsByType<PlayerSelectUI>();
        foreach (PlayerSelectUI selectUI in allPlayerSelectUI)
        {
            if (selectUI.PlayerUIIndex == playerUIIndex) playerSelectUI = selectUI;
        }
        if (playerSelectUI == null) Debug.LogError($"{name} unable to find player select UI with matching player UI index.", this);
    }

    private void OnEnable()
    {
        UIColourSelector col = playerSelectUI.ColourSelector;
        col.OnColourChanged += UpdateColour;
    }

    private void OnDisable()
    {
        playerSelectUI.ColourSelector.OnColourChanged -= UpdateColour;
    }

    void UpdateColour(Color col1, Color col2, Color col3)
    {
        if (DemoAvatar == null) return;
        demoAvatar.SetPlayerColour(col1, col2, col3);
    }
    public void SetDemoAvatar(NetworkObject newAvatarNO)
    {

        Debug.Log($"newAvatarNO null: {newAvatarNO == null}");
        Debug.Log($"demoAvatarNetworkRef null: {demoAvatarNetworkRef == null}");

        if (newAvatarNO == null)
        {
            Debug.LogError("Not a valid network object");
            return;
        }
        NetworkObjectReference reference = new(newAvatarNO);

        demoAvatarNetworkRef.Value = reference;
    }

    public Player GetDemoAvatar()
    {
        if (demoAvatarNetworkRef.Value.TryGet(out NetworkObject networkObject))
        {
            return networkObject.GetComponent<Player>();
        }

        return null;
    }
}
