using System;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Multiplayer;
using UnityEngine;

public class SessionManager : MonoBehaviour
{
    public static SessionManager Instance;
    static bool gameStarted = false;


    public event Action<string> OnStatusChanged;
    public event Action<string> OnHostCodeSet;
    public event Action<string> OnNetcodeStateChanged;

    ISession activeSession;

    bool servicesReady;
    bool busy;
    

    string hostJoinCode = "";
    string status = "Starting Unity Gaming Services...";

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Debug.LogWarning("Duplicate Session Manager found, removing.");
            Destroy(gameObject);
        }
    }
    private async void Start()
    {
        OnStatusChanged?.Invoke(status);
        OnNetcodeStateChanged?.Invoke(GetNetcodeState());
        await InitialiseServicesAsync();
    }
    private async Task InitialiseServicesAsync()
    {
        OnNetcodeStateChanged?.Invoke(GetNetcodeState());
        try
        {
            await UnityServices.InitializeAsync();

            if (!AuthenticationService.Instance.IsSignedIn)
            {
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
            }

            servicesReady = true;
            // TODO this should be a status object, not an arbitrary string.
            status = "Ready. Host a game or enter a join code.";
            OnStatusChanged?.Invoke(status);
        }
        catch (Exception exception)
        {
            RecordFailure("Service initialisation", exception);
        }
        OnNetcodeStateChanged?.Invoke(GetNetcodeState());
    }
    private void RecordFailure(string operation, Exception exception)
    {
        OnNetcodeStateChanged?.Invoke(GetNetcodeState());
        status = $"{operation} failed: {exception.Message}";
        OnStatusChanged?.Invoke(status);
        Debug.LogException(exception);
    }
    public async void HostGame()
    {

        if (!servicesReady || busy || activeSession != null) return;

        busy = true;
        status = "Creating Relay session...";
        OnStatusChanged?.Invoke(status);
        OnNetcodeStateChanged?.Invoke(GetNetcodeState());

        try
        {
            SessionOptions options = new SessionOptions
            {
                // The host counts as one of the two players.
                MaxPlayers = SessionPanelUI.Instance.MaxPlayers,
            }.WithRelayNetwork();

            activeSession = await MultiplayerService.Instance.CreateSessionAsync(options);

            hostJoinCode = activeSession.Code;
            OnHostCodeSet?.Invoke(hostJoinCode);
            status = "Session created. Give the code other players.";
            OnStatusChanged?.Invoke(status);

        }
        catch (Exception exception)
        {
            RecordFailure("Hosting", exception);
        }
        finally
        {
            busy = false;
        }
        OnNetcodeStateChanged?.Invoke(GetNetcodeState());
    }
    public async void JoinGame()
    {
        if (!servicesReady || busy || activeSession != null) return;
        OnNetcodeStateChanged?.Invoke(GetNetcodeState());

        string code = SessionPanelUI.Instance.JoinCode.Trim().ToUpperInvariant();

        if (string.IsNullOrWhiteSpace(code))
        {
            status = "Enter the host's join code.";
            OnStatusChanged?.Invoke(status);
            return;
        }

        busy = true;
        status = $"Joining session {code}...";
        OnStatusChanged?.Invoke(status);

        try
        {
            activeSession = await MultiplayerService.Instance.JoinSessionByCodeAsync(code);

            status = "Joined session.";
            OnStatusChanged?.Invoke(status);
        }
        catch (Exception exception)
        {
            RecordFailure("Joining", exception);
        }
        finally
        {
            busy = false;
        }
        OnNetcodeStateChanged?.Invoke(GetNetcodeState());
    }
    public async void LeaveGame()
    {
        // TODO, much of this isnt needed since we leave the scene that outputs status. Should probably rework this.
        if (activeSession == null || busy) return;
        OnNetcodeStateChanged?.Invoke(GetNetcodeState());

        busy = true;
        status = "Leaving session...";
        OnStatusChanged?.Invoke(status);

        try
        {
            await activeSession.LeaveAsync();

            activeSession = null;
            hostJoinCode = "";
            status = "Left session.";
            OnStatusChanged?.Invoke(status);
        }
        catch (Exception exception)
        {
            RecordFailure("Leaving", exception);
        }
        finally
        {
            busy = false;
        }
        OnNetcodeStateChanged?.Invoke(GetNetcodeState());
    }
    private static string GetNetcodeState()
    {
        NetworkManager manager = NetworkManager.Singleton;

        if (manager == null) return "Network Manager missing";

        if (manager.IsHost) return "Host";

        if (manager.IsClient) return "Client";

        if (manager.IsServer) return "Server";

        return "Offline";
    }

    public static void StartGame()
    {
        if (gameStarted) return;
        gameStarted = true;

        SessionPanelUI.Instance.gameObject.SetActive(false);

        PlayerCouch playerCouch = GetLocalCouch();

        if (playerCouch)
        {
            playerCouch.StartGame();
            playerCouch.RequestControl();
        }
        else Debug.Log("No couch found when starting session"); 


        foreach (PlayerSelectUI p in FindObjectsByType<PlayerSelectUI>())
        {
            p.gameObject.SetActive(false);
        }

        LevelManager.LoadFirstLevel();
    }

    public static PlayerCouch GetLocalCouch()
    {
        // TODO this is used in multiple scripts, move it to a helper
        NetworkManager nm = NetworkManager.Singleton;

        if (nm == null || !nm.IsClient) return null;

        NetworkObject playerObject = nm.LocalClient.PlayerObject;
        if (playerObject == null) return null;

        return playerObject.GetComponent<PlayerCouch>();
    }

}
