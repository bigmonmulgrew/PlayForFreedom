using System;
using System.Collections;
using Unity.Netcode;
using UnityEngine;
using CharacterController = BMD.CharacterController;

public abstract class Weapon : NetworkBehaviour
{
    #region Configuration
    [SerializeField] protected float firingCooldown = 0.5f;
    [SerializeField] protected bool repeatFire;
    // TODO implement friendly fire option
    

    [Header("Stats Settings")]
    [Tooltip("Weapons with projectiles do not use base damage.\n" +
        "The damage is based on the projectile.")]
    [SerializeField] protected float baseDamage;


    #endregion

    #region Cached references
    protected CharacterController characterController;
    protected Character character;
    #endregion

    protected float nextFireTime = 0;
    protected int characterLayerIndex;
    protected Coroutine repeatFireCoroutine;

    public float NextFireTime => nextFireTime;

    protected virtual void Awake()
    {
        characterController = GetComponentInParent<CharacterController>();
        character = GetComponentInParent<Character>();
        characterLayerIndex = character.gameObject.layer;

        // Enemies manage repeat fire already so forcing this false.
        if (character is Enemy) repeatFire = false;
    }
    void Start()
    {
        nextFireTime = Time.time;
    }

    public abstract void Fire();

    public void StopFiring()
    {
        if (repeatFireCoroutine == null) return;

        characterController.NotifyFireWeaponEnded();
        StopCoroutine(repeatFireCoroutine);
    }

    [Rpc(SendTo.Server)]
    protected virtual void RequestFireRPC(RequestFireParameters requestfireParameters)
    {
        Debug.LogError($"{name}: Weapon used does not oveeride RequestFireRPC, please ensure it is overrided", this);
        throw new NotImplementedException();
        
    }
    
    protected IEnumerator RepeatFire(Action funciton)
    {
        while (true)
        {
            funciton?.Invoke();
            yield return new WaitForSeconds(firingCooldown);
        }
    }
    
}
