using Unity.Netcode;
using UnityEngine;

public class AvatarControlGate : NetworkBehaviour
{
    [SerializeField] MonoBehaviour inputController;
    private readonly NetworkVariable<bool> controlGranted = new(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    private void Awake()
    {
        if (inputController != null) inputController.enabled = false;
    }

    private void Update()
    {
        if (!IsSpawned || inputController == null) return;

        bool shouldHaveInput = IsOwner && controlGranted.Value;

        if (inputController.enabled != shouldHaveInput) inputController.enabled = shouldHaveInput;
    }

    public bool ControlGranted => controlGranted.Value;

    public void SetControlGranted(bool isGranted)
    {
        if (!IsServer) return;

        controlGranted.Value = isGranted;
    }

    
}
