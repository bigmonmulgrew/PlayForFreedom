using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class Projectile : NetworkBehaviour
{
    const float VELOCITY_CHECK_INTERVAL = 0.1f;

    #region Configuration
    [SerializeField] int maxBounces = 1;
    [SerializeField] float maxLifetime = 1.0f;
    [SerializeField] float minimumVelocity = 0.1f;
    [SerializeField] int damage = 100;
    #endregion 
    #region Cached references
    Rigidbody rb;
    #endregion

    #region Runtime Variables
    float spawnTime;
    Coroutine intermittenChecksCoroutine;
    int bounces;
    #endregion

    public int Damage => damage;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        intermittenChecksCoroutine = StartCoroutine(IntermittendChecks());
        spawnTime = Time.time;
    }

    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();
        if (intermittenChecksCoroutine != null) StopCoroutine(intermittenChecksCoroutine);
    }
    /// <summary>
    /// Destroys the projectile or applies pool cleanup
    /// </summary>
    void DisposeofProjectile()
    {
        if (intermittenChecksCoroutine != null) StopCoroutine(intermittenChecksCoroutine);

        // TODO imlement pooling logic
        // TODO add some destroy particles or effects.
        NetworkObject.Despawn();
        

    }

    IEnumerator IntermittendChecks()
    {
        if (!IsServer) yield break;

        bool destroyNow = false;
        while (true)
        {
            yield return new WaitForSeconds(VELOCITY_CHECK_INTERVAL);
            destroyNow = 
                rb.linearVelocity.magnitude < minimumVelocity ||
                (Time.time - spawnTime) > maxLifetime ;
            if (destroyNow)
            {
                DisposeofProjectile();
            }
        }
        
    }
    public void FireProjectile(Vector3 direction,  float strength)
    {
        rb.AddForce(strength * direction, ForceMode.Impulse);
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (!IsServer) return;
        // bounces is incremented AFTER the if statment accesses bounces.
        if (bounces++ == maxBounces) DisposeofProjectile();
    }
}
