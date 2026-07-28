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
    int health;
    bool isDead;
    #endregion

    bool IsDead => isDead;

    private void Awake()
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
    }

    private void OnEnable()
    {
        health = maxHealth;
    }
    public void DealDamage(int amount)
    {
        if (IsDead) return;

        if (health <= 0) return;
        health -= amount;

        if (health <= 0) Die();
    }

    void Die()
    {
        if (IsDead) return;

        isDead = true;
        controller.RequestDie();
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log($"{name} was shot by {collision.gameObject.name}");
        if ((projectileLayers.value & (1 << collision.gameObject.layer)) != 0)
        {
            Debug.Log($"{collision.gameObject.name} on correct collision layer");
            if (collision.gameObject.TryGetComponent<Projectile>(out Projectile p)) DealDamage(p.Damage); 
        }
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
