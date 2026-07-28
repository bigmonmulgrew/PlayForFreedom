using BMD;
using UnityEngine;
using CharacterController = BMD.CharacterController;

public class Player : Character
{
    #region Cached references
    CharacterController controller;
    Weapon weapon;
    #endregion



    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        weapon = GetComponentInChildren<Weapon>();
        SubscribeToSignals();
    }
    private void SubscribeToSignals()
    {
        if(controller == null)
        {
            Debug.LogError($"No character controller found on {gameObject.name}", this);
            return;
        }

        controller.OnFireWeaponRequested += FireWeapon;
    }
    private void OnDisable()
    {
        controller.OnFireWeaponRequested -= FireWeapon;
    }
    public void FireWeapon()
    {
        weapon.Fire();
    }
}
