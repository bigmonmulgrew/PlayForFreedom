using UnityEngine;
using TMPro;
using System.Collections.Generic;
using Unity.VisualScripting;

public class PlayerUI : MonoBehaviour
{
    static List<PlayerUI> PlayerUIs = new();

    [SerializeField] TextMeshProUGUI scoreText;
    [SerializeField] TextMeshProUGUI playerIndicatorText;

    Player selectedPlayer;
    Room parentRoom;

    int playerIndex = 0;
    bool isActive = false;
   
    public bool IsActive {  get { return isActive; } set { isActive = value; } }

    private void Awake()
    {
        PlayerUIs.Add(this);
        parentRoom = GetComponentInParent<Room>();
        parentRoom.PlayerUIList.Add(this);

        
    }
    void Start()
    {
        SetInitiaReadouts();

    }

    void SetInitiaReadouts(int i = 0)
    {
        playerIndicatorText.text = new string('.', i);
        scoreText.text = "000";
    }

    void SetPlayerIndex(int i)
    {
        playerIndex = i;
    }

    public void Initialise(int i)
    {
        SetPlayerIndex(i);

        SetInitiaReadouts(i + 1);

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
        foreach (Room r in Room.AllRooms)
        {
            int i = 0;
            foreach(PlayerUI p in r.PlayerUIList)
            {
                i++;
                if (p.IsActive) continue;
                p.IsActive = true;
                p.Initialise(i - 1);
                
                break;
            }

        }
            
    }
}
