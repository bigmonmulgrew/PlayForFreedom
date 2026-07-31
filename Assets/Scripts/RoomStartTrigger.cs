using System;
using Unity.Netcode;
using UnityEngine;

public class RoomStartTrigger : MonoBehaviour
{
    public event Action RoomStartTriggered;

    bool isTriggered;

    private void OnTriggerEnter(Collider other)
    {
        if (isTriggered) return;
        
        // Layer collision matrix shoudl be configured so only players can trigger this, and all players should have network object
        // This is a safety check, just in case.
        if (!other.gameObject.TryGetComponent<NetworkObject>(out NetworkObject no)) return;
        if (!no.IsOwner) return;

        RoomStartTriggered?.Invoke();
        isTriggered = true;
        Destroy(gameObject);
    }
}
