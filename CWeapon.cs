using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Constants;

public class CWeapon : CEquipment
{
    [SerializeField] private PROTOTYPE_ATTACK _attackType;
    [SerializeField] private float _initDamage;
    [SerializeField] private float _damageGrowth;
    [SerializeField] private float _initAttackSpeed;
    [SerializeField] private float _attackSpeedGrowth;

    public PROTOTYPE_ATTACK AttackType { get { return _attackType; } }
    public float WeaponAttackSpeed { get; protected set; } 
    public float Damage { get; protected set; }

    protected override void Awake()
    {
        base.Awake();
        WeaponAttackSpeed = _initAttackSpeed;
        Damage = _initDamage;
    }

    public override void Enchant()
    {
        base.Enchant();
        WeaponAttackSpeed = _initAttackSpeed + (_lv * _attackSpeedGrowth);
        Damage = _initDamage + (_lv * _damageGrowth);
        ++_lv;
    }
}
