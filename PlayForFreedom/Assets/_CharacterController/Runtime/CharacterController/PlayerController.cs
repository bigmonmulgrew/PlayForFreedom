using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

namespace BMD
{
    public class PlayerController : BMD.CharacterController
    {
        [SerializeField] MonoBehaviour inputController;
        private readonly NetworkVariable<bool> controlGranted = new(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
        
        #region Cached references
        private PlayerControls playerControls;
        private InputAction move;
        private InputAction look;
        private InputAction zoom;
        private InputAction jump;
        private InputAction roll;
        private InputAction crouch;
        private InputAction sprint;
        private InputAction fire;
        private InputAction attack;
        private InputAction specialAttack;
        private InputAction lockLook;

        #endregion

        bool lookLocked;

        protected override void Awake()
        {
            base.Awake();
            SetupControls();

            if (Camera == null)
            {
                Debug.LogWarning("No camera defined by character controller, attempting to search children");
                RegisterCamera(GetComponentInChildren<Camera>());       // Attempt backup setup, find camera in child to assign to character controller.

                if (Camera == null) Debug.LogWarning("No camera found on the player. Please attach a camera module or child camera.");
                return;
            }
        }

        private void SetupControls()
        {
            playerControls = new PlayerControls();
            move        = playerControls.Player.Move;
            jump        = playerControls.Player.Jump;
            look        = playerControls.Player.Look;
            zoom        = playerControls.Player.Zoom;
            crouch      = playerControls.Player.Crouch;
            roll        = playerControls.Player.Roll;
            sprint      = playerControls.Player.Sprint;
            fire        = playerControls.Player.Fire;
            attack      = playerControls.Player.Attack;
            specialAttack = playerControls.Player.SpecialAttack;
            lockLook    = playerControls.Player.LockLook;
        }
        public override void OnNetworkSpawn()
        {
            if (IsOwner)
            {
                playerControls.Player.Enable();
                look.performed += ctx => HandleLookInput(ctx);
                look.canceled += ctx => HandleLookInput();
                zoom.performed += ctx => AdjustZoomLevel(-ctx.ReadValue<float>());
                zoom.canceled += ctx => AdjustZoomLevel(0f);
                crouch.performed += ctx => ToggleCrouch();
                roll.performed += ctx => PerformRoll();
                sprint.started += ctx => NotifySprintTriggered(true);
                sprint.canceled += ctx => NotifySprintTriggered(false);
            }
            

            base.OnNetworkSpawn();
        }
        public override void OnNetworkDespawn()
        {
            playerControls.Player.Disable();
        }
        protected override void Update()
        {
            if (!IsOwner) return;

            HandleLookLockinput();
            HandleJumpInput();
            HandleAttackInput();
            base.Update();
        }

        void HandleLookLockinput()
        {
            if (lockLook.WasPressedThisFrame()) lookLocked = !lookLocked;
        }
        private void HandleLookInput(InputAction.CallbackContext ctx)
        {
            if (lookLocked) return;
            lookInput = ctx.ReadValue<Vector2>();
        }
        
        private void HandleLookInput()
        {
            if (lookLocked) return;
            lookInput = Vector2.zero;
        } 
        private void AdjustZoomLevel(float zd) => NotifyZoomChanged(zd);
        private void HandleJumpInput()
        {
            if (jump.WasPressedThisFrame())
            {
                RequestJump();
            }
        }
        private void HandleAttackInput()
        {
            if (attack.WasPressedThisFrame()) RequestAttack();
            if (specialAttack.WasPressedThisFrame()) RequestSpecialAttack();
            if (fire.WasPressedThisFrame()) RequestFireWeapon();

            // Only fire needs release due to repeat fire.
            //if (attack.WasReleasedThisFrame()) RequestAttack();
            //if (specialAttack.WasReleasedThisFrame()) RequestSpecialAttack();
            if (fire.WasReleasedThisFrame()) RequestReleaseFireWeapon();
        }
        protected override void FixedUpdate()
        {
            if (!IsOwner) return;
            SetMoveDirection();

            base.FixedUpdate(); // controller.Tick() and FixedTick() will trigger module updates

        }
        private void SetMoveDirection()
        {
            Vector2 moveInput = move.ReadValue<Vector2>();
            float inputMagnitude = moveInput.magnitude;
            inputMagnitude = Mathf.Pow(inputMagnitude, 1.5f); // smoother start

            // TODO swapped this from camera root while look is frozen for demo.
            //Vector3 moveDir = (cameraRoot.forward * moveInput.y + cameraRoot.right * moveInput.x);
            //Vector3 moveDir = (Camera.transform.forward * moveInput.y + Camera.transform.right * moveInput.x);
            Vector3 moveDir = new Vector3(moveInput.x, 0, moveInput.y);
            //moveDir.y = 0f;
            moveDirection = moveDir.normalized * inputMagnitude;
        }
        protected override void ToggleCrouch()
        {
            if (crouch.WasPressedThisFrame())
            {
                base.ToggleCrouch();
            }
        }
        private void PerformRoll()
        {
            if (roll.WasPressedThisFrame())
            {
                RequestRoll();
            }
        }

    }
}
