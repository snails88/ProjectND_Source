using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Constants;

public class CPlayer : MonoBehaviour, ICollisionObject
{
    [System.Serializable]
    struct AttackVars
    {
        public float AttackSpeed;
        public float AttackDistance;

        [HideInInspector]
        public bool Attackable;
        [HideInInspector]
        public Vector2 AttackPos;
        [HideInInspector]
        public float AttackAngle;
    }

    [SerializeField]
    private AttackVars _attack;
    [SerializeField]
    private float _moveSpeed;

    private WaitForSeconds _wait;

    private CEquipment[] _equip = new CEquipment[(int)EQUIP_SLOT.EQUIP_SLOT_END];
    public CEquipment[] Equip
    {
        get { return _equip; }
    }

    private ACItem[] _inventory = new ACItem[(int)INVENTORY.CAPACITY];
    public ACItem[] Inventory
    {
        get { return _inventory; }
    }

    private void Awake()
    {
        _wait = new WaitForSeconds(_attack.AttackSpeed);
    }

    void Start()
    {
        _attack.Attackable = true;
        StartCoroutine("CoroutineAttackable");
    }

    public void Move(in Vector2 dir)
    {
        transform.Translate(dir * Time.deltaTime * _moveSpeed);
    }

    public void Attack()
    {
        if(_attack.Attackable)
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = transform.position.z;

            _attack.AttackPos = (mousePos - transform.position).normalized * _attack.AttackDistance + transform.position;
            _attack.AttackAngle = Mathf.Atan2(mousePos.y - transform.position.y, mousePos.x - transform.position.x) * Mathf.Rad2Deg;
            GameObject attack;
            if (CGameManager._instance.AttackObjectPoolIsEmpty())
            {
                if (_equip[(int)EQUIP_SLOT.WEAPON])
                    attack = Instantiate(CGameManager._instance.GetAttackObjectPrototype((int)((CWeapon)_equip[(int)EQUIP_SLOT.WEAPON]).AttackType), _attack.AttackPos, Quaternion.AngleAxis(_attack.AttackAngle, Vector3.forward));
                else
                    attack = Instantiate(CGameManager._instance.GetAttackObjectPrototype(PROTOTYPE_ATTACK.BASIC), _attack.AttackPos, Quaternion.AngleAxis(_attack.AttackAngle, Vector3.forward));
                attack.GetComponent<CAttack>().Damage = 1f;
            }
            else
            {
                attack = CGameManager._instance.PopAttackObjectByPool();
                attack.SetActive(true);
                if (_equip[(int)EQUIP_SLOT.WEAPON])
                    attack.GetComponent<CAttack>().SetAttack(((CWeapon)_equip[(int)EQUIP_SLOT.WEAPON]).AttackType, 1f);
                else
                    attack.GetComponent<CAttack>().SetAttack(PROTOTYPE_ATTACK.BASIC, 1f);
                attack.transform.rotation = Quaternion.AngleAxis(_attack.AttackAngle, Vector3.forward);
                attack.transform.position = _attack.AttackPos;
            }
            attack.transform.SetParent(gameObject.transform);
            _attack.Attackable = false;
        }
    }

    public void Hit(float dmg)
    {
        print("으악");
    }

    // 추가 성공, 실패 반환
    public bool AddItemToInventory(in ACItem item)
    {
        int index = -1;
        for (int i = 0; i < (int)INVENTORY.CAPACITY; i++)
        {
            if (_inventory[i] == null)
            {
                index = i;
                break;
            }
        }

        if(index != -1)
        {
            _inventory[index] = item;
            item.transform.SetParent(gameObject.transform.Find("Inventory").transform);
            item.gameObject.SetActive(false);
            CUIManager._instance.RefreshInventory();
            return true;
        }
        return false;
    }


    public void UseItem(int InvenIdx)
    {
        _inventory[InvenIdx].UseItem(InvenIdx);
    }

    public void ReleaseEquip(int equipIdx)
    {
        if(AddItemToInventory(_equip[equipIdx]))
        {
            _equip[equipIdx] = null;
            CUIManager._instance.RefreshInventory();
            if (equipIdx == (int)EQUIP_SLOT.WEAPON)
                _wait = new WaitForSeconds(_attack.AttackSpeed);
        }
    }

    public void EquipItem(int InvenIdx)
    {
        int equipIdx = (int)((CEquipment)_inventory[InvenIdx]).EquipSlot;
        if (_equip[equipIdx])
        {
            if (_inventory[InvenIdx] is CEquipment)
            {
                ACItem temp = _equip[equipIdx];
                _equip[equipIdx] = (CEquipment)_inventory[InvenIdx];
                _equip[equipIdx].gameObject.transform.SetParent(transform.Find("Equip").transform);
                _inventory[InvenIdx] = temp;
                _inventory[InvenIdx].gameObject.transform.SetParent(transform.Find("Inventory").transform);
            }
        }
        else
        {
            _equip[equipIdx] = (CEquipment)_inventory[InvenIdx];
            _equip[equipIdx].gameObject.transform.SetParent(transform.Find("Equip").transform);
            _inventory[InvenIdx] = null;
        }

        // 무기바꾸면 무기공격속도 적용
        if (equipIdx == (int)EQUIP_SLOT.WEAPON)
            _wait = new WaitForSeconds(_attack.AttackSpeed + ((CWeapon)_equip[(int)EQUIP_SLOT.WEAPON]).WeaponAttackSpeed);
        CUIManager._instance.RefreshInventory();
    }

    IEnumerator CoroutineAttackable()
    {
        while (true)
        {
            if (!_attack.Attackable)
            {
                yield return _wait;
                _attack.Attackable = true;
            }
            else
                yield return null;
        }
    }
}
