using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Audio;

public class Projectile : NetworkBehaviour
{
    const float VELOCITY_CHECK_INTERVAL = 0.1f;

    #region Configuration
    [SerializeField] int maxBounces = 1;
    [SerializeField] float maxLifetime = 1.0f;
    [SerializeField] float minimumVelocity = 0.1f;
    [SerializeField] int baseDamage = 100;

    [Header("Audio")]
    [SerializeField] AudioClip fireSound;
    [SerializeField] AudioClip impactSound;
    [Range(0, 0.8f)]
    [SerializeField] float pitchVariance = 0.1f;
    #endregion 
    #region Cached references
    Rigidbody rb;
    AudioSource audioSource;
    #endregion

    #region Runtime Variables
    float spawnTime;
    float damage;
    Coroutine intermittenChecksCoroutine;
    int bounces;
    #endregion

    public float Damage => damage;

    void Awake()
    {
        FindReferences();
        ResetAudio();
        audioSource.Play();
        SetRandomPitch();

        damage = baseDamage;
    }

    void ResetAudio()
    {
        audioSource.clip = fireSound;
        
    }

    void SetRandomPitch()
    {
        audioSource.pitch = Random.Range(1 - pitchVariance, 1 + pitchVariance);
    }
    private void FindReferences()
    {
        rb = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
        
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
        damage = baseDamage;
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
        PlayImpactSound();

        if (!IsServer) return;
        // bounces is incremented AFTER the if statment accesses bounces.
        if (bounces++ == maxBounces) DisposeofProjectile();
    }
    void PlayImpactSound()
    {
        
        if (audioSource.isPlaying) AudioSource.PlayClipAtPoint(impactSound, transform.position);        // TODO move this to pooled instanced audio
        else
        {
            SetRandomPitch();
            audioSource.clip = impactSound;
            audioSource.Play();
        }
    }

    public void ApplyDamageMultiplier(float multiplier)
    {
        if (multiplier == 0)
        {
            Debug.LogWarning($"{name} was provided a damage multipler of 0, defaulting to 1");
            multiplier = 1;
        }
            
        damage = baseDamage * multiplier;
    }
}
