using Unity.Netcode;
using System.Linq;
using UnityEngine;
using CharacterController = BMD.CharacterController;

public class UnarmedWeapon : Weapon
{

    [SerializeField] Transform damageHere;
    [SerializeField] float fistSize = 0.5f;

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
        // TODO needs to work on all characters incase we give player melee
        Player[] hitPlayers = 
            Physics.OverlapSphere(rfp.position, rfp.range)
            .Select( hit => GetComponent<Player>() )
            .Where( player => player != null )
            .Distinct()
            .ToArray();

        foreach (Player player in hitPlayers)
        {
            player.DealDamage(baseDamage);
        }
    }


 
}
