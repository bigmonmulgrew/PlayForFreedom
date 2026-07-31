using Unity.Netcode;
using UnityEngine;

public class CameraMoveTrigger : MonoBehaviour
{
    RoomCameraLocations cameraLocations;

    private void Awake()
    {
        cameraLocations = GetComponentInChildren<RoomCameraLocations>();
    }
    private void OnTriggerEnter(Collider other)
    {
        // Layer collision matrix should be configured so only players can trigger this, and all players should have network object
        // This is a safety check, just in case.
        if (!other.gameObject.TryGetComponent<NetworkObject>(out NetworkObject no)) return;
        if (!no.IsOwner) return;

        Transform newCamerTransform = cameraLocations.GetTransformAtIndex(0);   // TODO add some smart transtion so we preserve relative transforms.
        ArenaCamera.Instance?.SetNewTransfrom(newCamerTransform);
    }

}