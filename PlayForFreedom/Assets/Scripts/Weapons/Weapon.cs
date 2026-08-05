using System;
using Unity.Netcode;
using UnityEngine;
using CharacterController = BMD.CharacterController;

public abstract class Weapon : NetworkBehaviour
{
    #region Configuration
    [SerializeField] protected float firingCooldown = 0.5f;
    // TODO implement friendly fire option
    

    [Header("Stats Settings")]
    [SerializeField] protected float baseDamage;


    #endregion

    #region Cached references
    protected CharacterController characterController;
    protected Character character;
    #endregion

    protected float nextFireTime = 0;
    protected int layerIndex;

    public float NextFireTime => nextFireTime;

    protected virtual void Awake()
    {
        characterController = GetComponentInParent<CharacterController>();
        character = GetComponentInParent<Character>();
        layerIndex = character.gameObject.layer;
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
