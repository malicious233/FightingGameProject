using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityEventManager : MonoBehaviour
{
    public event Action OnRemoval;

    public event Action OnAttack;
    public event Action OnAttackEnd;

    public event Action DisableHurtbox;
    public event Action EnableHurtbox;

    public event Action WhenCharged;
    public event Action WhenChargeAttack;

    public event Action<Vector3> WhenHit;
    

    public void Invoke_OnAttack()
    {
        OnAttack?.Invoke();
    }

    public void Invoke_OnAttackEnd()
    {
        OnAttackEnd?.Invoke();
    }

    public void Invoke_DisableHurtbox()
    {
        DisableHurtbox?.Invoke();
    }

    public void Invoke_EnableHurtbox()
    {
        EnableHurtbox?.Invoke();
    }

    public void Invoke_WhenCharged()
    {
        WhenCharged?.Invoke();
    }

    public void Invoke_WhenChargeAttack()
    {
        WhenChargeAttack?.Invoke();
    }

    public void Invoke_OnRemoval()
    {
        OnRemoval?.Invoke();
    }


    public void Invoke_WhenHit(Vector3 hitPosition)
    {
        WhenHit?.Invoke(hitPosition);
    }


}
