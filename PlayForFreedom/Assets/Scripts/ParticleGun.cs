using BMD;
using UnityEngine;
using UnityEngine.InputSystem;

public class ParticleGun : MonoBehaviour
{
    [SerializeField] ParticleSystem bulletParticleSystem;

    PlayerControls playerControls;
    InputAction fire;
    private void OnEnable()
    {
        playerControls = new PlayerControls();
        playerControls.Enable();
        fire = playerControls.Player.Fire;
    }
    private void Update()
    {
        if (fire.WasPerformedThisFrame())
        {
            Debug.Log("Firing");
            bulletParticleSystem.Play();
        }
    }
}
