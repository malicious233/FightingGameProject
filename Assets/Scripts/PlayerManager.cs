using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    //References
    EntityManager entityManager;
    CharacterController charController;
    Animator animator;
    [SerializeField] Transform enemyTransform;

    [Header("Attacks:")]
    [SerializeField] AttackObject lightAttack;
    [SerializeField] AttackObject heavyAttack;
    [SerializeField] AttackObject lightAerialAttack;
    [SerializeField] AttackObject lightChargedAttack;
    [SerializeField] AttackObject heavyChargedAttack;



    //Changeable Variables
    [Header("General Stats:")]
    [SerializeField] float maxHealth;

    public float health;

    [Header("Movement Stats:")]
    [SerializeField] float movementSpeed;
    [SerializeField] float friction;
    [SerializeField] float gravity;
    [SerializeField] float jumpForce;

    [Header("Combat Stats:")]
    [SerializeField] float chargeRate;



    //Hidden Variables


    [HideInInspector] public float horizontalInput;
    [HideInInspector] public Vector3 inputDirection;
    [HideInInspector] public bool lightAttackPress;
    [HideInInspector] public bool heavyAttackPress;
    [HideInInspector] public bool jumpPress;
    [HideInInspector] public bool downInput;

    [HideInInspector] public StaticEnums.Direction direction = StaticEnums.Direction.forward;
    public Vector2 velocity;
    private float hitstunDuration;
    public bool grounded; //This stupid bool clashes a lot with the character controllers inherent groundcheck that is janky as fuck
    public bool doingAerialAttack;
    public float charge;
    private float maxCharge = 100f;
    private bool gotCharged;

    public StaticEnums.States state;


    public void Awake()
    {
        charController = GetComponent<CharacterController>();
        entityManager = GetComponent<EntityManager>();
        animator = GetComponent<Animator>();

        health = maxHealth;
    }

    

    private void Update()
    {
        if (health < 0)
        {
            SetState(StaticEnums.States.dead);
        }
        animator.SetBool("isCrouching", false);
        GroundCheck();
        CanCharge();
        switch (state)
        {

            case StaticEnums.States.idle:
                State_Idle();
                break;
            case StaticEnums.States.attacking:
                State_Attacking();
                break;
            case StaticEnums.States.crouching:
                State_Crouching();
                break;
            case StaticEnums.States.hitstunned:
                State_Hitstunned();
                break;
            case StaticEnums.States.launched:
                State_Launched();
                break;
            case StaticEnums.States.airborne:
                State_Airborne();
                break;
            case StaticEnums.States.dead:
                State_Dead();
                break;
            default:
                Debug.Log(state);
                break;
        }

    }

    #region State Methods
    private void State_Idle()
    {
        ApplyMovement();
        FaceEnemy();
        if (downInput)
        {
            SetState(StaticEnums.States.crouching);
        }
        CanGroundAttack();

        CanJump();
        ApplyGravity();
        ApplyVelocity();
    }

    private void State_Crouching()
    {
        animator.SetBool("isCrouching", true);
        FaceEnemy();
        CanGroundAttack();
        CanJump();
        ApplyGravity();
        ApplyFriction();
        ApplyVelocity();
        if (!downInput)
        {
            SetState(StaticEnums.States.idle);
        }
    }

    private void State_Airborne()
    {
        ApplyGravity();
        CanAerialAttack();
        ApplyVelocity();
        if (charController.isGrounded)
        {
            SetState(StaticEnums.States.idle);
        }
    }

    private void State_Attacking()
    {
        ApplyFriction();
        ApplyGravity();
        ApplyVelocity();
        if (doingAerialAttack && charController.isGrounded)
        {
            entityManager.EndAttack();
            //SetState(StaticEnums.States.idle);
        }
    }

    private void State_Hitstunned()
    {
        hitstunDuration -= Time.deltaTime;

        if (hitstunDuration < 0)
        {
            animator.SetBool("isHitstunned", false);
            SetState(StaticEnums.States.idle);
        }
        ApplyFriction();
        ApplyGravity();
        ApplyVelocity();
    }

    private void State_Launched()
    {
        ApplyFriction();
        ApplyGravity();
        ApplyVelocity();
        
    }

    private void State_Dead()
    {
        ApplyFriction();
        ApplyGravity();
        ApplyVelocity();
    }


    public void SetState(StaticEnums.States _state)
    {
        switch (_state)
        {
            case StaticEnums.States.idle:
                if (state == StaticEnums.States.attacking) {entityManager.entityEvents.Invoke_OnAttackEnd();}
                state = StaticEnums.States.idle;
                break;

            case StaticEnums.States.attacking:
                if (state == StaticEnums.States.idle || state == StaticEnums.States.crouching)
                {
                    doingAerialAttack = false;
                }
                if (state == StaticEnums.States.airborne)
                {
                    doingAerialAttack = true;
                }
                state = StaticEnums.States.attacking;
                break;

            case StaticEnums.States.crouching:
                state = StaticEnums.States.crouching;
                break;

            case StaticEnums.States.airborne:
                state = StaticEnums.States.airborne;
                break;
                 
            case StaticEnums.States.hitstunned:
                state = StaticEnums.States.hitstunned;
                animator.Play("anim_Hitstunned");
                animator.SetBool("isHitstunned", true);
                break;

            case StaticEnums.States.launched:
                state = StaticEnums.States.launched;
                animator.Play("anim_FallingHit");
                animator.SetBool("isHitstunned", false);
                break;

            case StaticEnums.States.dead:
                state = StaticEnums.States.dead;
                animator.Play("anim_Dead");
                break;
        }
    }

    public void SetHitstun(float _hitstunDuration)
    {
        SetState(StaticEnums.States.hitstunned);
        hitstunDuration = _hitstunDuration;
        animator.ResetTrigger("gotHitstunned");
        animator.SetTrigger("gotHitstunned");
    }

    public void SetKnockback(Vector3 _knockBackVector)
    {
        Debug.Log("Ouch");
        velocity = _knockBackVector;
    }

    #endregion

    #region Movement Methods

    public void ApplyMovement()

    {
        velocity.x = horizontalInput * movementSpeed;
        animator.SetFloat("MovementDirection", horizontalInput * (int)direction);
        transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y, (int)direction);
        //charController.Move(velocity);
    }

    public void AddMovement(float modifier)
    {
        velocity.x += horizontalInput * movementSpeed * Time.deltaTime * modifier;
    }

    public void AddVelocity(float modifier)
    {
        velocity.x += modifier * (int)direction;
    }

    public void CanJump()
    {

        if (jumpPress)
        {
            animator.Play("anim_Jump");
        }
    }

    public void Jump()
    {
        SetState(StaticEnums.States.airborne);
        velocity.y = jumpForce;
        
    }

    public void CanCharge()
    {   if (downInput)
        {
            charge += chargeRate * Time.deltaTime;
            charge = Mathf.Clamp(charge, 0, maxCharge);
            if (charge == maxCharge && !gotCharged)
            {
                gotCharged = true;
                entityManager.entityEvents.Invoke_WhenCharged();
            }
        }
        else
        {
            gotCharged = false;
            charge -= (chargeRate * 0.75f)* Time.deltaTime;
            charge = Mathf.Clamp(charge, 0, maxCharge);
        }
        
    }

    public void ApplyFriction()
    {
        if (charController.isGrounded) { velocity.x -= velocity.x * friction * Time.deltaTime; }
        else { velocity.x -= velocity.x * (friction * 0.5f) * Time.deltaTime; }
    }

    public void ApplyGravity()
    {
        velocity.y -= gravity * Time.deltaTime;
    }

    public void ApplyVelocity()
    {
        charController.Move(velocity * Time.deltaTime);
    }

    public void GroundCheck()
    {
        if (charController.isGrounded)
        {
            grounded = true;
            animator.SetBool("isGrounded", true);
        }
        else
        {
            grounded = false;
            animator.SetBool("isGrounded", false);
        }
    }

    public void FaceEnemy()
    {
        if (transform.position.x < enemyTransform.position.x)
        {
            direction = StaticEnums.Direction.forward;
        }
        else
        {
            direction = StaticEnums.Direction.backward;
        }
    }

    #endregion

    #region Attack Methods

    public void CanGroundAttack()
    {
        if (lightAttackPress)
        {
            if (charge == 100)
            {
                entityManager.StartAttack(lightChargedAttack);
                charge = 0;
                gotCharged = false;
                entityManager.entityEvents.Invoke_WhenChargeAttack();
            }
            else
            {
                entityManager.StartAttack(lightAttack);
            }
            
        }
        if (heavyAttackPress)
        {
            if (charge == 100)
            {
                entityManager.StartAttack(heavyChargedAttack);
                charge = 0;
                gotCharged = false;
                entityManager.entityEvents.Invoke_WhenChargeAttack();
            }
            else
            {
                entityManager.StartAttack(heavyAttack);
            }
        }
    }

    public void CanAerialAttack()
    {
        if (lightAttackPress)
        {
            entityManager.StartAttack(lightAerialAttack);
        }
    }

    #endregion
}
