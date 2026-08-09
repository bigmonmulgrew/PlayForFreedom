using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class PlayerSelectUI : MonoBehaviour
{
    [SerializeField] int playerUIIndex = -1;
    [SerializeField] TextMeshProUGUI playerLabel;
    [SerializeField] TMP_InputField nameInput;
    [SerializeField] Button possessButton;

    PlayerAvatarDemo playerAvatarDemo;
    UIColourSelector colourSelector;


    public int PlayerUIIndex => playerUIIndex;

    public UIColourSelector ColourSelector => colourSelector;
    public PlayerAvatarDemo PlayerAvatarDemo => playerAvatarDemo;

    private void Awake()
    {
        PlayerAvatarDemo[] allAvatarDemos = FindObjectsByType<PlayerAvatarDemo>();
        foreach (PlayerAvatarDemo avatarDemo in allAvatarDemos)
        {
            if (avatarDemo.PlayerUIIndex == playerUIIndex) playerAvatarDemo = avatarDemo;
        }
        if (playerAvatarDemo == null) Debug.LogError($"{name} unable to find character designer UI with matching player UI index.", this);

        colourSelector = GetComponentInChildren<UIColourSelector>();

        playerLabel.text = $"Player {playerUIIndex}";

    }

    public void SpawnPlayerOnPedestal()
    {
        if (playerAvatarDemo == null) return;

        PlayerCouch playerCouch = SessionManager.GetLocalCouch();
        playerCouch.SpawnAvatar(PlayerUIIndex);

        PlayerConfig newData = new PlayerConfig()
        {
            name = nameInput.text == "" ? $"Player {playerUIIndex}" : nameInput.text,
            startingMoney = -10000,
            customColour1 = colourSelector.PlayerColour1,
            customColour2 = colourSelector.PlayerColour2,
            customColour3 = colourSelector.PlayerColour3
        };
          
        foreach(PlayerSelectUI ui in FindObjectsByType<PlayerSelectUI>())
        {
            if (ui == this) possessButton.gameObject.SetActive(false);
            else ui.gameObject.SetActive(false);
        }


        playerCouch.SetPlayerData(playerAvatarDemo.PlayerUIIndex, newData);
        
    }

    
}
