using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField] Projectile projectile;
    [SerializeField] Transform bulletSpawn;
    [SerializeField] float projectileForce = 10f;
    [SerializeField] float firingCooldown = 0.5f;

    float nextFireTime;
    
    void Start()
    {
        nextFireTime = Time.time;
    }
    public void Fire()
    {
        if (Time.time < nextFireTime) return;
        nextFireTime = Time.time + firingCooldown;

        Projectile newProjectile = Instantiate(projectile, bulletSpawn.position, bulletSpawn.rotation);
        
        newProjectile.FireProjectile(bulletSpawn.forward, projectileForce);
    }
}
