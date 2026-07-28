using System.Collections;
using UnityEngine;

public class Projectile : MonoBehaviour
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
    Coroutine velocityCheck;
    int bounces;
    #endregion

    public int Damage => damage;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void OnEnable()
    {
        velocityCheck = StartCoroutine(IntermittendChecks());
        spawnTime = Time.time;
    }
    void OnDisable()
    {
        if (velocityCheck != null) StopCoroutine(velocityCheck);
    }
    /// <summary>
    /// Destroys the projectile or applies pool cleanup
    /// </summary>
    void DisposeofProjectile()
    {
        if (velocityCheck != null) StopCoroutine(velocityCheck);

        // TODO imlement pooling logic
        // TODO add some destroy particles or effects.
        Destroy(gameObject);
    }

    IEnumerator IntermittendChecks()
    {
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
        // bounces is incremented AFTER the if statment accesses bounces.
        if (bounces++ == maxBounces) DisposeofProjectile();
    }
}
