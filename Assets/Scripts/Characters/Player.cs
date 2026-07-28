using System.Collections.Generic;
using UnityEngine;
using CharacterController = BMD.CharacterController;

public class Player : Character
{
    public readonly static List<Player> AllPlayers = new();
    public readonly static List<Player> LocalPlayers = new();
    public readonly static List<Player> RemotePlayers = new();

    



    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        weapon = GetComponentInChildren<Weapon>();
        SubscribeToSignals();
    }
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        AllPlayers.Add(this);

        if (IsOwner) LocalPlayers.Add(this);
        else RemotePlayers.Add(this);
    }
    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();
        AllPlayers.Remove(this);
        
        if (IsOwner) LocalPlayers.Remove(this);
        else RemotePlayers.Remove(this);
    }
    private void SubscribeToSignals()
    {
        if (!IsOwner) return;
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
        if (!IsOwner) return;
        weapon.Fire();
    }
}
