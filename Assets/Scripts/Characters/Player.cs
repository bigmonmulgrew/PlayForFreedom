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
            Debug.LogError($"Nocharacter controller found on {gameObject.name}", this);
            return;
        }

        controller.OnFireWeaponPerformed += FireWeapon;
    }
    private void OnDisable()
    {
        controller.OnFireWeaponPerformed -= FireWeapon;
    }
    public void FireWeapon()
    {
        Debug.Log("Hello");
        weapon.Fire();
    }
}
