using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityManager : MonoBehaviour
{
    /// <summary>
    /// EntityManager handles all things Hitbox and Hurtbox related
    /// </summary>

    [SerializeField] GameObject hurtboxPrefab;
    [SerializeField] GameObject hitboxPrefab;
    PlayerManager playerManager;
    Animator animator;
    [HideInInspector] public EntityEventManager entityEvents;

    public delegate void OnClusterDeletion(int cluster);
    public event OnClusterDeletion onClusterDeletion;

    private int instanceID;
    private int hitboxIndex; //Index of the current hitbox which is used to take the right hitbox data from AttackObjects hitboxlist during hitbox instantiation
    private AttackObject currentAttack; //ScriptableObject that holds data about what stats hitboxes will have

    public Transform[] bodyPartTransforms = new Transform[12]; //Drag and drop the desired limb transforms in order of the StaticEnum.BodyPart enum. Order is found in StaticEnums bodyparts
    //This stupid part is hardcoded and needs its arraysize manually set. Maybe make this into a list?

    Dictionary<StaticEnums.BodyPart, Transform> bodyPartDict = new Dictionary<StaticEnums.BodyPart, Transform>();
    Dictionary<int, int> clusterDictionary = new Dictionary<int, int>(); 
    //Key is clusterID which is instanceID + cluster, and the value is amount of hitboxes said cluster is meant to have
    //Once the value is below 0 it will remove the dictionary index and also tell all enemies who has this cluster in their cluster blacklist to forget about this entry

    public StaticEnums.Teams team;

    #region Coroutines
    IEnumerator GetupInvulnerability(float invulnTime)
    {
        entityEvents.Invoke_DisableHurtbox();
        yield return new WaitForSeconds(invulnTime);
        entityEvents.Invoke_EnableHurtbox();
    }
    #endregion

    #region Clusterdictionary Methods

    public void RemoveFromClusterDict(int _clusterID) 
        //Reduces value(hitboxes remaining for this cluster) in the clusterdictionary or removes the index if value is below zero, this is called from hitboxes upon their expiration
        //If it removes a clusterdict index it will fire off an event to all hurtboxmanagers who has been hit by this cluster to forget about this cluster
    {
        if (clusterDictionary.ContainsKey(_clusterID))
        {
            if (clusterDictionary[_clusterID] > 1)
            {
                clusterDictionary[_clusterID]--;
            }
            else
            {
                onClusterDeletion?.Invoke(_clusterID);
                clusterDictionary.Remove(_clusterID);
                
            }
        }

            
    }

    private void AddToClusterDict(AttackObject _attackObject) //Creates an index in the clusterdictionary, or adds value(hitboxes remaining) to the clusterDictionary if it already exists
    {
        int _clusterID = _attackObject.hitboxList[hitboxIndex].cluster + instanceID;
        if (clusterDictionary.ContainsKey(_clusterID))
        {
            clusterDictionary[_clusterID]++; //If the clusterID key does exist in the dictionary, add another to the hitbox count
            //Debug.Log(clusterDictionary[_clusterID]);
        }
        else
        {
            clusterDictionary.Add(_clusterID, 1); //If the clusterID key does not exist in the dictionary, add another index
            //Debug.Log(clusterDictionary[_clusterID]);
        }
    }

    #endregion

    #region Attack and Hitbox Methods
    public void StartAttackEvent(AttackObject _attackObject) //Initiates an attack sequence, must be put frame1 on the animation timeline
    {
        entityEvents.Invoke_OnAttack();
        currentAttack = _attackObject;
        playerManager.SetState(StaticEnums.States.attacking);
        hitboxIndex = 0; 
        
    }

    public void EndAttack() //Animation event to end the attack, when going back to idle state and become actionable
    {
        entityEvents.Invoke_OnAttackEnd();

        if (playerManager.grounded)
        {
            
          playerManager.SetState(StaticEnums.States.idle);

        }
        else
        {
            animator.Play("anim_Airborne");
            playerManager.SetState(StaticEnums.States.airborne);
        }


    }

    public void StartAttack(AttackObject _attackObject) //Plays the animation of the attack
    {
        animator.Play(_attackObject.animationState, 0, 0f);
    }

    




    public void CreateHitbox() //Spawns a hitbox
    {
        Transform hitboxBody = bodyPartDict[currentAttack.hitboxList[hitboxIndex].parentLimb];
        
        GameObject hitboxObj = Instantiate(hitboxPrefab, hitboxBody.position, Quaternion.identity);
        HitboxManager hitbox = hitboxObj.GetComponent<HitboxManager>();
        hitbox.entityManager = GetComponent<EntityManager>();
        entityEvents.OnAttackEnd += hitbox.DestroyHitbox;
        hitbox.onHitboxRemoval += RemoveFromClusterDict;
        
        //Feeds the hitbox relevant data to the hitbox from the AttackObject hitbox list

        int _clusterID = currentAttack.hitboxList[hitboxIndex].cluster + instanceID; 
        hitbox.cluster = _clusterID; //Give hitbox the key clusterID
        hitbox.team = team;
        hitbox.damage = currentAttack.hitboxList[hitboxIndex].damage;
        hitbox.duration = currentAttack.hitboxList[hitboxIndex].duration;
        hitbox.hitboxRadius = currentAttack.hitboxList[hitboxIndex].hitboxRadius;
        hitbox.hitstunMultiplier = currentAttack.hitboxList[hitboxIndex].hitstunMultiplier + 1;
        hitbox.knockbackDirection = currentAttack.hitboxList[hitboxIndex].knockbackDirection;
        hitbox.knockbackDirection.x *= (int)playerManager.direction;
        hitbox.knockbackStrength = currentAttack.hitboxList[hitboxIndex].knockbackStrength ;
        hitbox.hitboxType = currentAttack.hitboxList[hitboxIndex].hitboxType;
        hitbox.parentTransform = hitboxBody;

        AddToClusterDict(currentAttack);

        hitboxIndex ++;
    }

    #endregion

    #region Hurtbox Methods
    private void CreateHurtbox()
    {
        GameObject obj = Instantiate(hurtboxPrefab, transform.position, Quaternion.identity);
        HurtboxManager hurtboxManager = obj.GetComponent<HurtboxManager>();
        hurtboxManager.entityManager = GetComponent<EntityManager>(); 
        hurtboxManager.parentTransform = bodyPartDict[StaticEnums.BodyPart.lowertorso];
        hurtboxManager.team = team;
        hurtboxManager.OnHit += RecieveDamage; //Calls the Recievedamage method when hurtboxmanager comes in contact with a hitbox raycast
        entityEvents.DisableHurtbox += hurtboxManager.DisableCollider;
        entityEvents.EnableHurtbox += hurtboxManager.EnableCollider;
    }


    public void RecieveDamage(HitboxManager _hitboxManager, Vector3 _hitPosition)
    {
        ///I could've instead just passed through a class called "Hitbox damage" with only damage and knockback data instead of passing the entire Hitboxmanager
        Vector3 knockbackVector = _hitboxManager.knockbackDirection * (_hitboxManager.knockbackStrength);
        switch (_hitboxManager.hitboxType)
        {
            //It's a switch statement incase I were to add say, grab hitboxes
            case StaticEnums.HitboxType.normal:
                if (playerManager.state == StaticEnums.States.launched) //If already launched when hit by a normal attack, get launched with a bit of extra upward knockback instead of normal hitstun, aka a juggle
                { 
                    playerManager.SetState(StaticEnums.States.launched);
                    knockbackVector += new Vector3(0f, 0.1f, 0f);
                }
                else
                { 
                    playerManager.SetHitstun(_hitboxManager.damage * 0.1f * _hitboxManager.hitstunMultiplier);
                }
                break;
            case StaticEnums.HitboxType.launcher:
                playerManager.SetState(StaticEnums.States.launched);
                break;

        }
        playerManager.health -= _hitboxManager.damage;
        playerManager.SetKnockback(knockbackVector);
        entityEvents.Invoke_WhenHit(_hitPosition);
        entityEvents.Invoke_OnAttackEnd();
        
    }

    #endregion

    private void OnDisable()
    {
        entityEvents.Invoke_OnRemoval();
    }

    public void Awake()
    {
        playerManager = GetComponent<PlayerManager>();
        entityEvents = GetComponent<EntityEventManager>();
        animator = GetComponentInChildren<Animator>();

        

        //Loops through all enum spots things in the BodyPart enum and places the given transform from the bodyPartTransform array-
        //inside the bodyPartDictionary with the given enum as key! So you can access every bodypart with the right bodypart enum key
        Type bodyPartCount = typeof(StaticEnums.BodyPart);
        foreach (int i in Enum.GetValues(bodyPartCount))
        {
            bodyPartDict.Add((StaticEnums.BodyPart)i, bodyPartTransforms[i]);
        }

        CreateHurtbox();
        instanceID = GetInstanceID();

    }
}
