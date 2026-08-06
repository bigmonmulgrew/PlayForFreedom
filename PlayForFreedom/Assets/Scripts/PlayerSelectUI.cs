using TMPro;
using UnityEngine;

public class PlayerSelectUI : MonoBehaviour
{
    [SerializeField] int playerUIIndex = 0;
    [SerializeField] TextMeshProUGUI playerLabel;
    [SerializeField] TMP_InputField nameInput;

    PlayerAvatarDemo playerAvatarDemo;
    UIColourSelector colourSelector;


    public int PlayerUIIndex => playerUIIndex;

    public UIColourSelector ColourSelector => colourSelector;


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

    public void PossessPlayer()
    {
        if (playerAvatarDemo == null) return;

        PlayerConfig newData = new PlayerConfig()
        {
            name = nameInput.text == "" ? $"Player {playerUIIndex}" : nameInput.text,
            startingMoney = -10000,
            customColour1 = colourSelector.PlayerColour1,
            customColour2 = colourSelector.PlayerColour2,
            customColour3 = colourSelector.PlayerColour3
        };
          
        playerAvatarDemo.PossessPlayer(newData);
    }
}
