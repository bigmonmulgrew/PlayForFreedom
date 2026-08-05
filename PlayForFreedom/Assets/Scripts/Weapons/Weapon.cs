using System;
using Unity.Netcode;
using UnityEngine;
using CharacterController = BMD.CharacterController;

public abstract class Weapon : NetworkBehaviour
{
    #region Configuration
    [SerializeField] protected float firingCooldown = 0.5f;

    [Header("Particle Settings")]
    [SerializeField] float particleDamage;

    [Header("AoE Settings")]

    [Header("Stats Settings")]
    [SerializeField] float baseDamage;


    #endregion

    #region Cached references
    protected CharacterController characterController;
    #endregion

    protected float nextFireTime = 0;

    public float NextFireTime => nextFireTime;

    private void Awake()
    {
        characterController = GetComponentInParent<CharacterController>();
        
    }
    void Start()
    {
        nextFireTime = Time.time;
    }

    public abstract void Fire();

    [Rpc(SendTo.Server)]
    protected virtual void RequestFireRPC(RequestFireParameters requestfireParameters)
    {
        Debug.LogError($"{name}: Weapon used does not oveeride RequestFireRPC, please ensure it is overrided", this);
        throw new NotImplementedException();
        
    }

}
