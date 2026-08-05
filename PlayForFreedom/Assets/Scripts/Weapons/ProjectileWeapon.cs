using Unity.Netcode;
using UnityEngine;
using CharacterController = BMD.CharacterController;

public class ProjectileWeapon : Weapon
{
    #region Configuration
    [Header("Projectile Settings")]
    [SerializeField] Projectile projectile;
    [SerializeField] Transform bulletSpawn;
    [SerializeField] float projectileForce = 10f;
    [Tooltip("Projectile Damage is handled on the projectile, this multiplies the damage.")]
    [SerializeField] float damageMultipler = 1;

    #endregion

    #region Cached references
    
    #endregion

    public override void Fire()
    {
        if (Time.time < nextFireTime) return;

        characterController.NotifyFireWeaponPerformed();
        nextFireTime = Time.time + firingCooldown;

        RequestFireParameters rfp = new()
        {
            position = bulletSpawn.position,
            direction = transform.forward
        };

        RequestFireRPC(rfp);

        // TODO this should really be after the animation ends. 
        characterController.NotifyFireWeaponEnded();
    }

    [Rpc(SendTo.Server)]
    protected override void RequestFireRPC(RequestFireParameters rfp)
    {

        Projectile newProjectile = Instantiate(projectile, rfp.position, Quaternion.LookRotation(rfp.direction));
        newProjectile.ApplyDamageMultiplier(damageMultipler);
        if (newProjectile.TryGetComponent<NetworkObject>(out NetworkObject no)) no.Spawn();


        newProjectile.FireProjectile(bulletSpawn.forward, projectileForce);
    }


 
}
