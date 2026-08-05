using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;
using CharacterController = BMD.CharacterController;

public class ParticleWeapon : Weapon
{
    #region Configuration
    [Header("Particle Settings")]
    [SerializeField] ParticleSystem particleEmitter;
    [SerializeField] AudioClip fireSound;
    [Range(0, 0.8f)]
    [SerializeField] float pitchVariance = 0.1f;
    [Range(0.5f, 50f)]
    [SerializeField] float particleRange = 20f;
    //TODO add ricochet beams
    [SerializeField] bool penetrateTargets;
    [Range(1, 10)]
    [SerializeField] int maxPentrationTargets = 2;
    [SerializeField] float damageMultiplierPerTarget = 0.5f;


    #endregion

    #region Cached references
    AudioSource audioSource;
    #endregion

    protected override void Awake()
    {
        base.Awake();

        audioSource = GetComponent<AudioSource>();

        ResetAudio();
        SetRandomPitch();
    }
    void ResetAudio()
    {
        audioSource.clip = fireSound;

    }

    void SetRandomPitch()
    {
        audioSource.pitch = Random.Range(1 - pitchVariance, 1 + pitchVariance);
    }

    public override void Fire()
    {
        if (Time.time < nextFireTime) return;

        characterController.NotifyFireWeaponPerformed();

        if (repeatFire) repeatFireCoroutine = StartCoroutine(RepeatFire(FireSingleShot));
        else
        {
            FireSingleShot();
            // TODO this should really be after the animation ends. 
            characterController.NotifyFireWeaponEnded();
        }

    }

    void FireSingleShot()
    {
        PlayShotSound();
        particleEmitter.Play();

        nextFireTime = Time.time + firingCooldown;

        RequestFireParameters rfp = new()
        {
            position = particleEmitter.transform.position,
            direction = transform.forward,
            range = particleRange,

        };

        RequestFireRPC(rfp);
    }

    void PlayShotSound()
    {

        if (audioSource.isPlaying) AudioSource.PlayClipAtPoint(fireSound, transform.position);        // TODO move this to pooled instanced audio
        else
        {
            SetRandomPitch();
            audioSource.Play();
        }
    }

    [Rpc(SendTo.Server)]
    protected override void RequestFireRPC(RequestFireParameters rfp)
    {
        

        RaycastHit[] hitTargets = Physics.RaycastAll(rfp.position, rfp.direction, rfp.range);

        if (hitTargets.Length <= 0) return;

        float currentDamage = baseDamage;
        int hitCount = 0;

        foreach (RaycastHit hit in hitTargets) 
        {
            if (hit.collider.gameObject.layer == characterLayerIndex) continue;
            if (!hit.collider.TryGetComponent<Character>(out Character c)) continue;

            c.DealDamage(currentDamage);
            if (!penetrateTargets) break;
            hitCount++;
            if (hitCount >= maxPentrationTargets) break;
            currentDamage *= damageMultiplierPerTarget;
        }

    }


 
}
