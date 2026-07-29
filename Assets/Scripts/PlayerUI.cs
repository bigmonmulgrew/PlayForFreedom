using UnityEngine;
using TMPro;
using System.Collections.Generic;
using Unity.VisualScripting;

public class PlayerUI : MonoBehaviour
{
    static int count = 0;
    static List<PlayerUI> PlayerUIs = new();

    [SerializeField] TextMeshProUGUI scoreText;
    [SerializeField] TextMeshProUGUI playerIndicatorText;

    Player selectedPlayer;

    int playerIndex = 0;
    bool isActive = false;
   
    public bool IsActive {  get { return isActive; } set { isActive = value; } }

    private void Awake()
    {
        PlayerUIs.Add(this);
    }
    void Start()
    {
        SetInitiaReadouts();

    }

    void SetInitiaReadouts()
    {
        playerIndicatorText.text = new string('.', count);
        scoreText.text = "000";
    }

    void SetPlayerIndex()
    {
        playerIndex = count;
        count++;
    }

    public void Initialise()
    {
        SetPlayerIndex();

        SetInitiaReadouts();

        if (playerIndex >= Player.AllPlayers.Count) return;

        selectedPlayer = Player.AllPlayers[playerIndex];
        selectedPlayer.OnScoreChanged += UpdateScore;
    }

    private void OnDisable()
    {
        if (selectedPlayer) selectedPlayer.OnScoreChanged -= UpdateScore;
    }
    void UpdateScore(int newScore)
    {
        scoreText.text = newScore.ToString("N0");
    }
    public static void NewPlayerSpawned()
    {
        foreach (var p in PlayerUIs)
        {
            if (p.IsActive) continue;
            p.IsActive = true;
            p.Initialise();
            break;
        } 
            
    }
}
