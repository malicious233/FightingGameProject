using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitboxManager : MonoBehaviour
{
    //The controller component for hitboxes
    public Transform parentTransform; //What transform this hitbox will follow, like a limb or a projectile
    public SphereCollider sphereCollider;
    public EntityManager entityManager;
    public LayerMask hurtboxMask;

    [Header("Hitbox Timing:")]
    public int cluster; //The int of said cluster said attack this hitbox will align to. So you wont be hit twice by moves with the same cluster ID
    public float duration; //How long till hitbox expires
    public bool obeyDestroyEvent; //If events such as owner having their attack cancelled cancels this hitbox, such as projectiles

    [Header("Hitbox Position:")]
    public Vector3 positioning; //The local offset of the hitbox
    public float hitboxRadius = 0.2f;

    [Header("Damage Properties:")]
    public StaticEnums.Teams team;
    public float hitstunMultiplier;
    public float damage; //Damage of hitbox
    public Vector3 knockbackDirection; //Knockback direction relative to attacker direction
    public float knockbackStrength; //Knockback intensity
    public StaticEnums.HitboxType hitboxType;

    public delegate void OnHitboxRemoval(int cluster);
    public event OnHitboxRemoval onHitboxRemoval;



    private void Awake()
    {
        sphereCollider = GetComponent<SphereCollider>();
    }

    private void Update()
    {
        HandleHitboxRayCast();
        transform.position = parentTransform.position;
        transform.rotation = parentTransform.rotation;
        sphereCollider.radius = hitboxRadius;
        //transform.localScale = hitboxScale; //Do we need this?

        duration -= Time.deltaTime;
        if (duration < 0)
        {
            Destroy(gameObject);
        }
        
        
    }

    private void HandleHitboxRayCast()
    {
        RaycastHit[] hits;
        hits = Physics.SphereCastAll(transform.position, hitboxRadius, Vector3.forward, 0.0f, hurtboxMask);
        foreach (RaycastHit hit in hits)
        {
            HurtboxManager hurt = hit.transform.GetComponent<HurtboxManager>();
            if (hurt.team != team)
            {
                if (!hurt.hitboxClusterHashset.Contains(cluster))
                {
                    hurt.hitboxClusterHashset.Add(cluster);
                    entityManager.onClusterDeletion += hurt.RemoveFromHitboxClusterHashset;
                    hurt.Invoke_OnHit(this, transform.position);
                    //Debug.Log(damage);
                    //TODO: Do that this calls an event in the hurtbox manager which returns the encapsulated damage hitbox data
                }
            }

        }
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, hitboxRadius);
    }

    public void OnDisable()
    {
        onHitboxRemoval?.Invoke(cluster);
        onHitboxRemoval -= entityManager.RemoveFromClusterDict;
        entityManager.entityEvents.OnAttack -= DestroyHitbox;
        entityManager.entityEvents.OnAttackEnd -= DestroyHitbox;
    }

    public void DestroyHitbox()
    {
        
        Destroy(gameObject);
    }

}
