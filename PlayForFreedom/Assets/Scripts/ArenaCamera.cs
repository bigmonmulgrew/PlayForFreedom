using BMD;
using UnityEngine;
using UnityEngine.InputSystem;
using System;
public class ArenaCamera : MonoBehaviour
{
    class BasicTransform
    {
        public Vector3 Position;
        public Vector3 Rotation;

        public Quaternion QuaternionRotation => Quaternion.Euler(Rotation);
    }

    public static ArenaCamera Instance;
    [SerializeField] float roomTransitionTime = 2.0f;
    [SerializeField] float locationTransitionTime = 1.0f;

    PlayerControls playerControls;
    InputAction cameraInputIA;


    BasicTransform targetTransform;
    BasicTransform startTransform;

    float startTime;
    int currentCameraIndex = 0;
    bool transitionIsRoom = false;
    float TransitionTime => transitionIsRoom ? roomTransitionTime : locationTransitionTime;
    private void Awake()
    {
        Instance = this;

        playerControls = new();
    }

    private void OnEnable()
    {
        cameraInputIA = playerControls.Player.Camera;
        playerControls.Enable();
    }
    private void OnDisable()
    {
        playerControls.Disable();
    }
    private void Update()
    {
        ReadCameraInputs();
        SmoothTransition();

    }

    void ReadCameraInputs()
    {
        if (!cameraInputIA.WasPerformedThisFrame()) return;

        Vector2 cameraInput = cameraInputIA.ReadValue<Vector2>();

        if (cameraInput == Vector2.zero) return;
        if (targetTransform == null)
        {
            targetTransform = new()
            {
                Position = transform.position,
                Rotation = transform.rotation.eulerAngles
            };

        }

        if (startTransform == null)
        {
            startTransform = new()
            {
                Position = transform.position,
                Rotation = transform.rotation.eulerAngles
            };

        }

        if (currentCameraIndex == 0)
        {

            targetTransform.Rotation = new(
                targetTransform.Rotation.x + 0,
                targetTransform.Rotation.y + 0,
                targetTransform.Rotation.z + 90 * -cameraInput.x
                );

            startTransform.Rotation = transform.rotation.eulerAngles;

            startTime = Time.time;
            transitionIsRoom = false;
        }

    }

    private void SmoothTransition()
    {
        if (targetTransform == null) return;

        float timeDifference = Time.time - startTime;
        float interpolatioRatio = timeDifference / TransitionTime;
                
        Vector3 newPositon = Vector3.Lerp(startTransform.Position, targetTransform.Position, interpolatioRatio);
        Quaternion newRotation = Quaternion.Lerp(startTransform.QuaternionRotation, targetTransform.QuaternionRotation, interpolatioRatio);
        
        transform.SetPositionAndRotation(newPositon, newRotation);
        
        if (interpolatioRatio >= 1) targetTransform = null;
    }

    public void SetNewTransfrom(RoomCameraLocations cameraLocations, int locationIndex = 0)
    {
        Transform newTransform = cameraLocations.GetTransformAtIndex(locationIndex);   // TODO add some smart transtion so we preserve relative transforms.

        targetTransform = new()
        {
            Position = newTransform.position,
            Rotation = newTransform.rotation.eulerAngles
        };

        startTransform = new()
        {
            Position = transform.position,
            Rotation = transform.rotation.eulerAngles
        };

        startTime = Time.time;
        transitionIsRoom = true;
    }
}
