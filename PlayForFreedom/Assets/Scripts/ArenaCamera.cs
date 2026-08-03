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

    RoomCameraLocations currentRoomCameraLocaitons;
    RoomCameraLocation currentCameraLocation;

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
        
        targetTransform ??= new()
        {
            Position = transform.position,
            Rotation = transform.rotation.eulerAngles
        };

       startTransform = new()
        {
            Position = transform.position,
            Rotation = transform.rotation.eulerAngles
        };
        

        if (currentCameraIndex == 0)
        {
            if      (cameraInput.y != 0) MoveToCamera(cameraInput);         // When top, move to lower camer in up or down position.
            else if (cameraInput.x != 0) RotateCameraInPlace(cameraInput);  // When top, horizontal input rotates in place
        }
        else
        {
            if      (cameraInput.y > 0)  MoveToCamera(cameraInput);          // When not top, y input can only move up, to the top camera
            else if (cameraInput.x != 0) MoveToSideCamera(cameraInput);     // When not top, x input moves to adjacent camera horizontally.
        }

    }

    void MoveToCamera(Vector2 cameraInput)
    {
        if (cameraInput.y < 0)  MoveToCameraLocaiton(currentCameraLocation.Down);
        else                    MoveToCameraLocaiton(currentCameraLocation.Up);

        if      (cameraInput.x > 0) currentCameraLocation.Up.RotateRight();
        else if (cameraInput.x < 0) currentCameraLocation.Up.RotateLeft();
    }

    void MoveToSideCamera(Vector2 cameraInput)
    {
        if      (cameraInput.x > 0) MoveToCameraLocaiton(currentCameraLocation.Right);
        else if (cameraInput.x < 0) MoveToCameraLocaiton(currentCameraLocation.Left);

        if      (cameraInput.x > 0) currentCameraLocation.Up.RotateRight();
        else if (cameraInput.x < 0) currentCameraLocation.Up.RotateLeft();
    }
    void MoveToCameraLocaiton(RoomCameraLocation newLocation)
    {
        if (newLocation == null) 
        {
            Debug.LogWarning("Moving to null location");
            return;
        }
        startTransform.Position = transform.position;
        startTransform.Rotation = transform.rotation.eulerAngles;

        targetTransform.Position = newLocation.transform.position;
        targetTransform.Rotation = newLocation.transform.rotation.eulerAngles;

        startTime = Time.time;
        transitionIsRoom = false;

        currentCameraLocation = newLocation;
        currentCameraIndex = currentRoomCameraLocaitons.GetIndexByTransform(currentCameraLocation.transform);
    }

    private void RotateCameraInPlace(Vector2 cameraInput)
    {
        targetTransform.Rotation = new(
                        targetTransform.Rotation.x + 0,
                        targetTransform.Rotation.y + 0,
                        targetTransform.Rotation.z + 90 * -cameraInput.x
                        );

        startTransform.Rotation = transform.rotation.eulerAngles;

        startTime = Time.time;
        transitionIsRoom = false;

        if      (cameraInput.x > 0) currentCameraLocation.RotateRight();
        else if (cameraInput.x < 0) currentCameraLocation.RotateLeft();

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
        currentRoomCameraLocaitons = cameraLocations;

        Transform newTransform = cameraLocations.GetTransformAtIndex(locationIndex);   // TODO add some smart transtion so we preserve relative transforms.

        currentCameraLocation = newTransform.gameObject.GetComponent<RoomCameraLocation>();

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
