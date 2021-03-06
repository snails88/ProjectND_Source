﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CCreature : MonoBehaviour, ICollisionObject
{
    [SerializeField] protected float _moveSpeed;
    [SerializeField] protected float _maxHP;
    [SerializeField] protected float _hitTime;
    [SerializeField] protected float _hitColorTime;
    [SerializeField] protected WaitForSeconds _waitHit;
    [SerializeField] protected WaitForSeconds _waitHitColor;
    protected float _hP;
    protected Color _hitColor = new Color(1f, 0f, 0f, 1f);

    public abstract void Hit(float dmg);
    public abstract void HitTarget(in ICollisionObject target);
    protected abstract void Die();
}
