using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HurtboxManager : MonoBehaviour
{
    public Transform parentTransform; //What transform this hurtbox will follow around
    public EntityManager entityManager;
    public Collider collider;
    public Vector3 positioning; //what offset this hurtbox will have from origin
    public StaticEnums.Teams team;
    [SerializeField] LayerMask hitboxMask;

    public HashSet<int> hitboxClusterHashset = new HashSet<int>();

    public delegate void OnHitEvent(HitboxManager _hitboxManager, Vector3 hitPosition);
    public event OnHitEvent OnHit;

    public void Awake()
    {
        collider = GetComponent<Collider>();
    }

    public void Invoke_OnHit(HitboxManager _hitboxManager, Vector3 _hitPosition) //The entitymanager is subscribed to OnHit, and this returns the hitboxManager and hitposition with relevant damage data
    {
        OnHit?.Invoke(_hitboxManager, _hitPosition);
    }

    public void RemoveFromHitboxClusterHashset(int _clusterID)
    {
        if (hitboxClusterHashset.Contains(_clusterID))
        {
            hitboxClusterHashset.Remove(_clusterID);
        }
    }

    public void DisableCollider()
    {
        collider.enabled = false;
    }

    public void EnableCollider()
    {
        collider.enabled = true;
    }



    private void OnDisable()
    {
        entityManager.entityEvents.DisableHurtbox -= DisableCollider;
        entityManager.entityEvents.EnableHurtbox -= EnableCollider;
    }


    public void Update()
    {
        transform.position = parentTransform.position + positioning;
    }

}
