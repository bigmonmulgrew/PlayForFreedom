using Unity.Netcode;
using UnityEngine;
using CharacterController = BMD.CharacterController;

public class Character : NetworkBehaviour
{

    [SerializeField] int maxHealth = 100;
    [SerializeField] LayerMask projectileLayers = (1 << 8) | (1 << 10) | (1 << 11);

    #region Cached references
    protected CharacterController controller;
    protected Weapon weapon;
    #endregion

    #region Runtime Variables
    float health;
    bool isDead;
    bool isDespawning;
    #endregion

    public bool IsDead => isDead;
    protected bool IsDespawning => isDespawning;

    protected virtual void Awake()
    {
        controller = GetComponent<CharacterController>();
        weapon = GetComponentInChildren<Weapon>();
        SubscribeToSignals();
    }
    private void SubscribeToSignals()
    {
        if (controller == null)
        {
            Debug.LogError($"No character controller found on {gameObject.name}", this);
            return;
        }

        controller.OnFireWeaponRequested += FireWeapon;
        controller.OnReleaseFireWeaponRequested += ReleaseFireWeapon;
    }
    protected virtual void OnEnable()
    {
        health = maxHealth;
    }
    public void DealDamage(float amount)
    {
        if (IsDead) return;

        if (health <= 0) return;
        health -= amount;

        if (health <= 0) Die();
    }
    protected virtual void Die()
    {
        if (IsDead) return;

        isDead = true;
        controller.RequestDie();
    }
    void OnCollisionEnter(Collision collision)
    {
        if ((projectileLayers.value & (1 << collision.gameObject.layer)) != 0)
        {
            Debug.Log($"{collision.gameObject.name} on correct collision layer");
            if (collision.gameObject.TryGetComponent<Projectile>(out Projectile p)) DealDamage(p.Damage); 
        }
    }
    private void OnDisable()
    {
        controller.OnFireWeaponRequested -= FireWeapon;
        controller.OnReleaseFireWeaponRequested -= ReleaseFireWeapon;
    }
    public void FireWeapon()
    {
        if (!IsOwner) return;
        RequestFire();
    }

    void ReleaseFireWeapon()
    {
        if (!IsOwner) return;
        weapon.StopFiring();
        controller.NotifyReleaseFireWeaponPerformed();
    }

    void RequestFire()
    {
        if (weapon == null) return;

        weapon.Fire();
    }

    /// <summary>
    /// Despans the character as if they have left the play area and not been destroy.
    /// </summary>
    protected virtual void RemoveCharacter()
    {
        if (IsDespawning) return;

        isDespawning = true;
        // TODO may need to deactive character controller just liek Die()
        NetworkObject.Despawn();
    }

}
