using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CCreature : MonoBehaviour, ICollisionObject
{
    [SerializeField] protected float _moveSpeed;
    [SerializeField] protected float _maxHP;
    protected float _hP;
    [SerializeField] protected WaitForSeconds _waitHit;
    [SerializeField] protected float _hitTime;
    [SerializeField] protected WaitForSeconds _waitHitColor;
    [SerializeField] protected float _hitColorTime;
    protected Color _hitColor = new Color(255f, 0f, 0f, 100f);

    public abstract void Hit(float dmg);
    protected abstract void Die();
}
