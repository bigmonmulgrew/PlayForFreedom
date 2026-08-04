using Unity.Netcode;
using UnityEngine;
using CharacterController = BMD.CharacterController;

public class Weapon : NetworkBehaviour
{
    [SerializeField] Projectile projectile;
    [SerializeField] Transform bulletSpawn;
    [SerializeField] float projectileForce = 10f;
    [SerializeField] float firingCooldown = 0.5f;

    #region Cached references
    CharacterController characterController;
    #endregion

    float nextFireTime;

    public float NextFireTime => nextFireTime;

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

        RequestFireRPC(bulletSpawn.position, transform.forward);

        // TODO this should really be after the animation ends. 
        characterController.NotifyFireWeaponEnded();
    }

    [Rpc(SendTo.Server)]
    private void RequestFireRPC(Vector3 spawnPosition, Vector3 direction)
    {
        Projectile newProjectile = Instantiate(projectile, spawnPosition, Quaternion.LookRotation(direction));
        if (newProjectile.TryGetComponent<NetworkObject>(out NetworkObject no)) no.Spawn();


        newProjectile.FireProjectile(bulletSpawn.forward, projectileForce);
    }


 
}
