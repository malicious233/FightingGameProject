using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Attack", menuName = "Combat/Attack")]

public class AttackObject : ScriptableObject
{
    public string animationState;
    public List<Hitbox> hitboxList = new List<Hitbox>();
}

[System.Serializable]
public class Hitbox
{
    

    [Header("Hitbox Timing:")]
    
    public int cluster; //The int of said cluster said attack this hitbox will align to. So you wont be hit twice by moves with the same cluster ID
    public float duration; //How long till hitbox expires
    public bool obeyDestroyEvent; //If events such as owner having their attack cancelled cancels this hitbox, such as projectiles

    [Header("Hitbox Position:")]
    public float hitboxRadius; //Radius of the hitbox detection
    public StaticEnums.BodyPart parentLimb;//What limb the hitbox will attach to

    [Header("Damage Properties:")]
    public float damage; //Damage of hitbox
    public float hitstunMultiplier; //The hitstun multiplier of this attack. 0 means 1 lol
    public Vector3 knockbackDirection; //Knockback direction relative to attacker direction
    public float knockbackStrength; //Knockback intensity

    public StaticEnums.HitboxType hitboxType;


}
