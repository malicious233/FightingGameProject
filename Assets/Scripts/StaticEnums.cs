using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class StaticEnums 
{
    public enum BodyPart
    {
        head,
        torso,
        lowertorso,
        left_arm,
        left_hand,
        left_weapon,
        left_leg,
        left_foot,
        right_arm,
        right_hand,
        right_weapon,
        right_leg,
        right_foot,
    }

    public enum Teams
    {
        players,
        enemies,
    }

    public enum States
    {
        idle, //Normal state, walking, standing
        airborne, //Jumping
        crouching, //Crouched position, cannot walk but has lowered hurtbox
        hitstunned, //Is hit by a normal hitbox, cannot act
        launched, //Is hit by a launcher hitbox, cannot act
        attacking, //While attacking, cannot act
        dead, //Dead, cannot act, probably
    }

    public enum HitboxType
    {
        normal, //This hitbox simply inflicts hitstun and damage
        launcher, //This hitbox causes opponent to be sent into the helpless launched state
    }

    public enum InputType
    {
        None,
        WASD,
        Arrows,
        
    }

    public enum Direction
    {
        forward = 1,
        backward = -1,
    }
}
