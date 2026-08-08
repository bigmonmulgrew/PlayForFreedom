using UnityEngine;

public class PlayerAvatarDemo : MonoBehaviour
{
    [SerializeField] int playerUIIndex = 0;

    [SerializeField] Player defaultAvatar;
    [SerializeField] Transform avatarSpawn;

    PlayerSelectUI playerSelectUI;
    Player demoAvatar;

    ulong clientID;

    public int PlayerUIIndex => playerUIIndex;
    public Player DemoAvatar => demoAvatar;

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
        if (demoAvatar == null) return;
        demoAvatar.SetPlayerColour(col1, col2, col3);
    }

    public void PossessPlayer(PlayerConfig playerConfig)
    {
        if (demoAvatar != null) return;
        CreatePlayerAvatar(playerConfig);
    }

    public void PossessPlayer(PlayerConfig playerConfig, ulong clientID)
    {
        if (demoAvatar != null) return;
        CreatePlayerAvatar(playerConfig);
        this.clientID = clientID;
    }

    void CreatePlayerAvatar(PlayerConfig playerConfig)
    {
        demoAvatar = Instantiate(defaultAvatar, avatarSpawn.position, avatarSpawn.rotation);
        demoAvatar.SetPlayerData(playerConfig);
    }
}
