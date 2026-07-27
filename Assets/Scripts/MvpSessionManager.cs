using System;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Multiplayer;
using UnityEngine;

public sealed class MvpSessionManager : MonoBehaviour
{
    [SerializeField] int maxPlayers = 2;

    private ISession activeSession;

    private bool servicesReady;
    private bool busy;

    private string joinCodeInput = "";
    private string hostJoinCode = "";
    private string status = "Starting Unity Gaming Services...";

    private async void Start()
    {
        await InitialiseServicesAsync();
    }

    private async Task InitialiseServicesAsync()
    {
        try
        {
            await UnityServices.InitializeAsync();

            if (!AuthenticationService.Instance.IsSignedIn)
            {
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
            }

            servicesReady = true;
            status = "Ready. Host a game or enter a join code.";

            Debug.Log($"Authenticated as player {AuthenticationService.Instance.PlayerId}");
        }
        catch (Exception exception)
        {
            RecordFailure("Service initialisation", exception);
        }
    }

    private async void HostGame()
    {
        if (!servicesReady || busy || activeSession != null) return;

        busy = true;
        status = "Creating Relay session...";

        try
        {
            SessionOptions options = new SessionOptions
            {
                // The host counts as one of the two players.
                MaxPlayers = maxPlayers
            }.WithRelayNetwork();

            activeSession = await MultiplayerService.Instance.CreateSessionAsync(options);

            hostJoinCode = activeSession.Code;
            status = "Session created. Give the code other players.";

            Debug.Log($"Join code: {hostJoinCode}");
        }
        catch (Exception exception)
        {
            RecordFailure("Hosting", exception);
        }
        finally
        {
            busy = false;
        }
    }

    private async void JoinGame()
    {
        if (!servicesReady || busy || activeSession != null) return;

        string code = joinCodeInput.Trim().ToUpperInvariant();

        if (string.IsNullOrWhiteSpace(code))
        {
            status = "Enter the host's join code.";
            return;
        }

        busy = true;
        status = $"Joining session {code}...";

        try
        {
            activeSession = await MultiplayerService.Instance.JoinSessionByCodeAsync(code);

            status = "Joined session.";
        }
        catch (Exception exception)
        {
            RecordFailure("Joining", exception);
        }
        finally
        {
            busy = false;
        }
    }

    private async void LeaveGame()
    {
        if (activeSession == null || busy) return;

        busy = true;
        status = "Leaving session...";

        try
        {
            await activeSession.LeaveAsync();

            activeSession = null;
            hostJoinCode = "";
            status = "Left session.";
        }
        catch (Exception exception)
        {
            RecordFailure("Leaving", exception);
        }
        finally
        {
            busy = false;
        }
    }

    private void RecordFailure(string operation, Exception exception)
    {
        status = $"{operation} failed: {exception.Message}";
        Debug.LogException(exception);
    }

    private void OnGUI()
    {
        GUILayout.BeginArea(
            new Rect(20f, 20f, 420f, 300f),
            GUI.skin.box
        );

        GUILayout.Label("<b>Multiplayer MVP</b>");
        GUILayout.Space(8f);

        GUILayout.Label($"Status: {status}");
        GUILayout.Label($"Netcode state: {GetNetcodeState()}");

        GUILayout.Space(12f);

        GUI.enabled = servicesReady && !busy;

        if (activeSession == null)
        {
            if (GUILayout.Button("Host Game", GUILayout.Height(40f)))
            {
                HostGame();
            }

            GUILayout.Space(12f);
            GUILayout.Label("Join code:");

            joinCodeInput = GUILayout.TextField(
                joinCodeInput,
                12,
                GUILayout.Height(30f)
            );

            if (GUILayout.Button("Join Game", GUILayout.Height(40f)))
            {
                JoinGame();
            }
        }
        else
        {
            if (!string.IsNullOrEmpty(hostJoinCode))
            {
                GUILayout.Label($"Host join code: <b>{hostJoinCode}</b>");

                if (GUILayout.Button("Copy Join Code"))
                {
                    GUIUtility.systemCopyBuffer = hostJoinCode;
                    status = "Join code copied.";
                }
            }

            GUILayout.Space(12f);

            if (GUILayout.Button("Leave Game", GUILayout.Height(40f)))
            {
                LeaveGame();
            }
        }

        GUI.enabled = true;

        GUILayout.Space(12f);
        GUILayout.Label("Movement: WASD or arrow keys");

        GUILayout.EndArea();
    }

    private static string GetNetcodeState()
    {
        NetworkManager manager = NetworkManager.Singleton;

        if (manager == null) return "NetworkManager missing";

        if (manager.IsHost) return "Host";

        if (manager.IsClient) return "Client";

        if (manager.IsServer) return "Server";

        return "Offline";
    }
}
