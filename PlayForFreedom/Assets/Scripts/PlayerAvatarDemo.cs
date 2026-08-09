using Unity.Netcode;
using UnityEngine;

public class PlayerAvatarDemo : MonoBehaviour
{
    [SerializeField] int playerUIIndex = -1;

    [SerializeField] Player defaultAvatar;
    [SerializeField] Transform avatarSpawn;

    PlayerSelectUI playerSelectUI;

    NetworkVariable<Player> demoAvatar = new(null, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    
    public int PlayerUIIndex => playerUIIndex;
    public Player DemoAvatar => demoAvatar.Value;
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
        if (demoAvatar.Value == null) return;
        demoAvatar.Value.SetPlayerColour(col1, col2, col3);
    }
    public void SetDemoAvatar(Player newAvatar)
    {
        demoAvatar.Value = newAvatar;
    }

}
