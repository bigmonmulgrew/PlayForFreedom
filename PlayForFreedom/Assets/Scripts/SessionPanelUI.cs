using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SessionPanelUI : UIBehaviour
{
    public static SessionPanelUI Instance;

    [SerializeField] Slider maxPlayersSlider;
    [SerializeField] TextMeshProUGUI statusText;
    [SerializeField] TMP_InputField joinCodeInput;
    [SerializeField] TextMeshProUGUI netcodeStateText;

    public int MaxPlayers => (int)maxPlayersSlider.value;
    public string JoinCode => joinCodeInput.text;

    protected override void Awake()
    {
        base.Awake();
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogWarning("Duplicate SessionPanelUI found, removing.");
            Destroy(gameObject);
        }
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        SessionManager.Instance.OnStatusChanged += UpdateStatus;
        SessionManager.Instance.OnHostCodeSet += UpdateHostCode;
        SessionManager.Instance.OnNetcodeStateChanged += UpdateNetcodeState;

    }

    protected override void OnDisable()
    {
        base.OnDisable();
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
        SessionManager.StartGame();
        
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
