using UnityEngine;
using CharacterController = BMD.CharacterController;

public class Weapon : MonoBehaviour
{
    [SerializeField] Projectile projectile;
    [SerializeField] Transform bulletSpawn;
    [SerializeField] float projectileForce = 10f;
    [SerializeField] float firingCooldown = 0.5f;

    #region Cached references
    CharacterController characterController;
    #endregion

    float nextFireTime;

    private void Awake()
    {
        characterController = GetComponentInParent<CharacterController>();
    }
    void Start()
    {
        nextFireTime = Time.time;
    }
    public void Fire()
    {
        if (Time.time < nextFireTime) return;
        characterController.NotifyFireWeaponPerformed();

        nextFireTime = Time.time + firingCooldown;

        Projectile newProjectile = Instantiate(projectile, bulletSpawn.position, bulletSpawn.rotation);
        
        newProjectile.FireProjectile(bulletSpawn.forward, projectileForce);

        // TODO this should really be after the animation ends. 
        characterController.NotifyFireWeaponEnded();
    }
}
