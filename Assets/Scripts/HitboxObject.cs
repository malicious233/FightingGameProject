using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class HitboxObject : MonoBehaviour
{
    

    [Header("Hitbox Timing")]
    public int cluster; //The int of said cluster said attack this hitbox will align to. So you wont be hit twice by moves with the same cluster ID
    public float duration; //How long till hitbox expires
    public bool obeyDestroyEvent; //If events such as owner having their attack cancelled cancels this hitbox, such as projectiles

    [Header("Hitbox Position")]
    public Vector3 positioning;
    public Vector3 hitboxScale;
    public Transform limb;
    //What limb the hitbox will be parented to. If this field is own transform it will be parented to you, if this field is empty you can assign it to projectile

    [Header("Damage Properties")]
    public float damage; //Damage of hitbox
    public Vector3 knockbackDirection; //Knockback direction relative to attacker direction
    public float knockbackStrength; //Knockback intensity


}
