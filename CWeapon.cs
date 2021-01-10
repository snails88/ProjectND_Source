using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Constants;

public class CWeapon : CEquipment
{
    [SerializeField]
    private PROTOTYPE_ATTACK _attackType;
    public PROTOTYPE_ATTACK AttackType
    {
        get { return _attackType; }
    }

    [SerializeField]
    private float _weaponAttackSpeed;
    public float WeaponAttackSpeed
    {
        get { return _weaponAttackSpeed; }
    }
}
