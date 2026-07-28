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
        if(collision.gameObject.layer == projectileLayers)
        {
            if (collision.gameObject.TryGetComponent<Projectile>(out Projectile p)) DealDamage(p.Damage); 
        }
    }
}
