using Unity.Netcode;
using System.Linq;
using UnityEngine;
using CharacterController = BMD.CharacterController;
using UnityEditor.ShaderKeywordFilter;

public class UnarmedWeapon : Weapon
{

    [SerializeField] Transform damageHere;
    [SerializeField] float fistSize = 0.5f;

    #region Preallocations
    Collider[] hitTargets = new Collider[8];
    #endregion

    protected override void Awake()
    {
        base.Awake();
        FallbackDamageHere();
    }

    public override void Fire()
    {
        if (Time.time < nextFireTime) return;
        
        

        characterController.NotifyFireWeaponPerformed();
        nextFireTime = Time.time + firingCooldown;

        RequestFireParameters rfp = new()
        {
            position = damageHere.position,
            range = fistSize
        };

        RequestFireRPC(rfp);

        // TODO this should really be after the animation ends. 
        characterController.NotifyFireWeaponEnded();
    }

    void FallbackDamageHere()
    {
        if (damageHere != null) return;

        Debug.LogWarning($"No damageHere Transform specified, defaulting to using self.", this);
        damageHere = transform;
    }

    [Rpc(SendTo.Server)]
    protected override void RequestFireRPC(RequestFireParameters rfp)
    {
        // TODO add a visual impact effect

        int hitCount = Physics.OverlapSphereNonAlloc(rfp.position, rfp.range, hitTargets);

        if (hitCount <= 0) return;

        for (int i = 0; i < hitCount; i++)
        {
            if (hitTargets[i].gameObject.layer == characterLayerIndex) continue;
            if (hitTargets[i].TryGetComponent<Character>(out Character c)) continue;
            c.DealDamage(baseDamage);

        }
    }


 
}
