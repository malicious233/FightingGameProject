using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleManager : MonoBehaviour
{
    EntityEventManager entityEvents;

    [SerializeField] public GameObject hitParticlePrefab;
    [SerializeField] public GameObject chargedParticlePrefab;
    [SerializeField] public GameObject chargedAttackPrefab;
    [SerializeField] public GameObject deathParticlePrefab;


    public void Awake()
    {
        entityEvents = GetComponent<EntityEventManager>();
    }

    public void OnEnable()
    {
        entityEvents.WhenHit += CreateParticle_HitEffect;
        entityEvents.WhenCharged += CreateParticle_ChargedEffect;
        entityEvents.WhenChargeAttack += CreateParticle_ChargeAttack;
    }

    public void OnDisable()
    {
        entityEvents.WhenHit -= CreateParticle_HitEffect;
        entityEvents.WhenCharged -= CreateParticle_ChargedEffect;
        entityEvents.WhenChargeAttack -= CreateParticle_ChargeAttack;
    }

    public void CreateParticle(GameObject particlePrefab, Vector3 spawnPosition, Quaternion rotation)
    {
        Instantiate(particlePrefab, spawnPosition, rotation);
    }

    public void CreateParticle_HitEffect(Vector3 spawnPosition)
    {
        CreateParticle(hitParticlePrefab, spawnPosition, Quaternion.identity);
    }

    public void CreateParticle_ChargedEffect()
    {
        CreateParticle(chargedParticlePrefab, transform.position, Quaternion.Euler(-90f, 0f, 0f));
    }

    public void CreateParticle_ChargeAttack()
    {
        CreateParticle(chargedAttackPrefab, transform.position, Quaternion.Euler(-90f, 0f, 0f));
    }

    public void CreateParticle_DeathEffect()
    {
        CreateParticle(deathParticlePrefab, transform.position, Quaternion.Euler(-90f, 0f, 0f));
    }
}
