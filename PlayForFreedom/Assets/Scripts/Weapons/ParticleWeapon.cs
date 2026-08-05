using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using CharacterController = BMD.CharacterController;

public class ParticleWeapon : Weapon
{
    #region Configuration
    [Header("Particle Settings")]
    [SerializeField] ParticleSystem particleEmitter;
    [Range(0.5f, 50f)]
    [SerializeField] float particleRange = 20f;
    //TODO add ricochet beams
    [SerializeField] bool penetrateTargets;
    [Range(1, 10)]
    [SerializeField] int maxPentrationTargets = 2;
    [SerializeField] float damageMultiplierPerTarget = 0.5f;
    
    #endregion

    #region Cached references

    #endregion

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
        nextFireTime = Time.time + firingCooldown;

        RequestFireParameters rfp = new()
        {
            position = particleEmitter.transform.position,
            direction = transform.forward,
            range = particleRange,

        };

        RequestFireRPC(rfp);
    }

    [Rpc(SendTo.Server)]
    protected override void RequestFireRPC(RequestFireParameters rfp)
    {
        particleEmitter.Play();

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
