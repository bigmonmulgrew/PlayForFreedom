using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class SessionPanelUI : MonoBehaviour
{
    public static SessionPanelUI Instance;

    [SerializeField] Slider maxPlayersSlider;
    [SerializeField] TextMeshProUGUI statusText;
    [SerializeField] TMP_InputField joinCodeInput;
    [SerializeField] TextMeshProUGUI netcodeStateText;

    public int MaxPlayers => (int)maxPlayersSlider.value;
    public string JoinCode => joinCodeInput.text;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Debug.LogWarning("Duplicate SessionPanelUI found, removing.");
            Destroy(gameObject);
        }
    }

    void OnEnable()
    {
        SessionManager.Instance.OnStatusChanged += UpdateStatus;
        SessionManager.Instance.OnHostCodeSet += UpdateHostCode;
        SessionManager.Instance.OnNetcodeStateChanged += UpdateNetcodeState;

    }

    void OnDisable()
    {
        SessionManager.Instance.OnStatusChanged -= UpdateStatus;
        SessionManager.Instance.OnHostCodeSet -= UpdateHostCode;
        SessionManager.Instance.OnNetcodeStateChanged -= UpdateNetcodeState;
    }

    void UpdateStatus(string status)
    {
        statusText.text = status;
        
    }
    void UpdateHostCode(string code)
    {
        joinCodeInput.text = code;
    }
    void UpdateNetcodeState(string state)
    {
        netcodeStateText.text = state;
    }

    public void StartGame()
    {
        
        foreach (PlayerSelectUI p in FindObjectsByType<PlayerSelectUI>())
        {
            if (p.gameObject.activeSelf)
            {
                ulong clientID = PlayerCouch.InstanceLocal.OwnerClientId; // TODO add a fallback for single player, where there is no instance.
                Player avatar = p.PlayerAvatarDemo.DemoAvatar;
                NetworkObject no = avatar.GetComponent<NetworkObject>();
                no.Spawn(false);
                DontDestroyOnLoad(no);
                no.ChangeOwnership(clientID);
            }
            p.gameObject.SetActive(false);
        }

        LevelManager.LoadFirstLevel();
        gameObject.SetActive(false);
    }

    public void JoinGame()
    {
        SessionManager.Instance.JoinGame();
    }
    public void HostGame()
    {
        SessionManager.Instance.HostGame();
    }

    public void LeaveGame()
    {
        SessionManager.Instance.LeaveGame();
        LevelManager.LoadMainMenu();
    }

    public void CopyJoinCode()
    {
        GUIUtility.systemCopyBuffer = joinCodeInput.text;
    }
}
