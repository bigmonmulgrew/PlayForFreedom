using UnityEngine;

public class PlayerAvatarDemo : MonoBehaviour
{
    [SerializeField] int playerUIIndex = 0;

    [SerializeField] Player defaultAvatar;
    [SerializeField] Transform avatarSpawn;

    PlayerSelectUI playerSelectUI;

    public int PlayerUIIndex => playerUIIndex;

    private void Awake()
    {
        PlayerSelectUI[] allPlayerSelectUI = FindObjectsByType<PlayerSelectUI>();
        foreach (PlayerSelectUI selectUI in allPlayerSelectUI)
        {
            if (selectUI.PlayerUIIndex == playerUIIndex) playerSelectUI = selectUI;
        }
        if (playerSelectUI == null) Debug.LogError($"{name} unable to find player select UI with matching player UI index.", this);
    }

    public void PossessPlayer(PlayerConfig playerConfig)
    {
        CreatePlayerAvatar(playerConfig);
    }
    void CreatePlayerAvatar(PlayerConfig playerConfig)
    {
        Player newPlayer = Instantiate(defaultAvatar, avatarSpawn.position, avatarSpawn.rotation);
        newPlayer.SetPlayerData(playerConfig);
    }
}
