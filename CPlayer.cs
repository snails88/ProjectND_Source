using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Constants;

public abstract class CPlayer : CCreature
{
    [System.Serializable]
    protected struct AttackVars
    {
        public float AttackSpeed;
        public float AttackDistance;

        [HideInInspector]
        public bool Attackable;
        [HideInInspector]
        public Vector2 AttackPos;
        [HideInInspector]
        public float AttackAngle;
        [HideInInspector]
        public WaitForSeconds WaitAttack;
    }

    [System.Serializable]
    protected struct EvasionVars
    {
        public float EvasionSpeed;
        public float EvasionTime;
        public float EvasionCooldown;

        [HideInInspector]
        public bool Evasing;
        [HideInInspector]
        public bool Evasionable;
        [HideInInspector]
        public Vector3 EvasionDir;
        [HideInInspector]
        public WaitForSeconds WaitEvasion;
        [HideInInspector]
        public WaitForSeconds WaitEvasionCD;
    }
    
    [SerializeField]
    protected EvasionVars _evasion;
    [SerializeField]
    protected AttackVars _attack;
    protected Vector3 _mousePos;
    protected Animator _animator;
    protected int _runParameterHash;
    protected int _hitParameterHash;
    protected SpriteRenderer _spriteRenderer;
    protected Transform _invenTransform;
    protected Transform _equipTransform;
    protected float _resource;
    public float Resource
    {
        get { return _resource; }
    }
    protected float _maxResource;
    public float MaxResource
    {
        get { return _maxResource; }
    }
    protected Color _resourceColor;
    public Color ResourceColor
    {
        get { return _resourceColor; }
    }
    protected CEquipment[] _equips = new CEquipment[(int)EQUIP_SLOT.EQUIP_SLOT_END];
    public CEquipment[] Equips
    {
        get { return _equips; }
    }
    protected ACItem[] _inventory = new ACItem[(int)INVENTORY.CAPACITY];
    public ACItem[] Inventory
    {
        get { return _inventory; }
    }
    protected ACItem[] _quickSlots = new ACItem[(int)QUICK_SLOT.CAPACITY];
    public ACItem[] QuickSlots
    {
        get { return _quickSlots; }
    }

    public float HP
    {
        get { return _hP; }
    }

    public float MaxHP
    {
        get { return _maxHP; }
    }

    protected virtual void Awake()
    {
        _attack.WaitAttack = new WaitForSeconds(_attack.AttackSpeed);
        _evasion.Evasing = false;
        _evasion.Evasionable = true;
        _evasion.WaitEvasion = new WaitForSeconds(_evasion.EvasionTime);
        _evasion.WaitEvasionCD = new WaitForSeconds(_evasion.EvasionCooldown);
        _invenTransform = transform.Find("Inventory");
        _equipTransform = transform.Find("Equip");
        _spriteRenderer = transform.Find("Graphic").GetComponent<SpriteRenderer>();
        _animator = _spriteRenderer.gameObject.GetComponent<Animator>();
        _runParameterHash = Animator.StringToHash("Run");
        _hitParameterHash = Animator.StringToHash("Hit");
        _hP = _maxHP;
        _waitHit = new WaitForSeconds(_hitTime);
        _waitHitColor = new WaitForSeconds(_hitColorTime);
    }

    protected virtual void Start()
    {
        _attack.Attackable = true;
        StartCoroutine("CoroutineAttackable");
    }

    protected virtual void Update()
    {
        LookAtMousePointer();
    }

    public void Move(in Vector2 dir)
    {
        if (!_evasion.Evasing)
            transform.Translate(dir * Time.deltaTime * _moveSpeed);
    }

    public void SetRunAnim(bool run)
    {
        _animator.SetBool(_runParameterHash, run);
    }

    private void LookAtMousePointer()
    {
        if (!_evasion.Evasing)
        {
            _mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            _spriteRenderer.flipX = transform.position.x - _mousePos.x > 0 ? true : false;
        }
    }

    public void Attack()
    {
        if(_attack.Attackable)
        {
            _mousePos.z = transform.position.z;
            _attack.AttackPos = (_mousePos - transform.position).normalized * _attack.AttackDistance + transform.position;
            _attack.AttackAngle = Mathf.Atan2(_mousePos.y - transform.position.y, _mousePos.x - transform.position.x) * Mathf.Rad2Deg;
            CAttack attack;
            if (CGameManager._instance.AttackObjectPoolIsEmpty())
            {
                if (_equips[(int)EQUIP_SLOT.WEAPON])
                    attack = Instantiate(CGameManager._instance.GetAttackObjectPrototype((int)((CWeapon)_equips[(int)EQUIP_SLOT.WEAPON]).AttackType), _attack.AttackPos, Quaternion.AngleAxis(_attack.AttackAngle, Vector3.forward)).GetComponent<CAttack>();
                else
                    attack = Instantiate(CGameManager._instance.GetAttackObjectPrototype(PROTOTYPE_ATTACK.BASIC), _attack.AttackPos, Quaternion.AngleAxis(_attack.AttackAngle, Vector3.forward)).GetComponent<CAttack>();
                attack.Damage = 1f;
            }
            else
            {
                attack = CGameManager._instance.PopAttackObjectByPool().GetComponent<CAttack>();
                attack.gameObject.SetActive(true);
                if (_equips[(int)EQUIP_SLOT.WEAPON])
                    attack.SetAttack(((CWeapon)_equips[(int)EQUIP_SLOT.WEAPON]).AttackType, 1f);
                else
                    attack.SetAttack(PROTOTYPE_ATTACK.BASIC, 1f);
                attack.transform.rotation = Quaternion.AngleAxis(_attack.AttackAngle, Vector3.forward);
                attack.transform.position = _attack.AttackPos;
            }
            attack.LifeTimeCoroutineStart();
            attack.transform.SetParent(gameObject.transform);
            _attack.Attackable = false;
        }
    }

    public override void Hit(float dmg)
    {
        if(!_evasion.Evasing)
        {
            _hP -= dmg;
            StartCoroutine(CoroutineHit());
            if(_hP < 0f)
            {
                Die();
            }
        }
    }

    protected override void Die()
    {
        print("꽤꼬닼");
    }

    // 추가 성공, 실패 반환
    public bool AddItemToInventory(in ACItem item)
    {
        if (!item)
            return false;

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
            item.transform.SetParent(_invenTransform);
            item.gameObject.SetActive(false);
            CUIManager._instance.RefreshInventory();
            return true;
        }
        return false;
    }

    public bool AddPotionToInventory(in CPotion item)
    {
        if (!item)
            return false;

        int index = -1;
        for (int i = 0; i < (int)INVENTORY.CAPACITY; i++)
        {
            if(_inventory[i] is CPotion)
            {
                if(((CPotion)_inventory[i]).Sort == item.Sort)
                {
                    ++((CPotion)_inventory[i]).Count;
                    Destroy(item.gameObject);
                    CUIManager._instance.RefreshInventory();
                    return true;
                }
            }
            if (_inventory[i] == null && index == -1)
                index = i;
        }
        if (index != -1)
        {
            _inventory[index] = item;
            ++item.Count;
            item.transform.SetParent(_invenTransform);
            item.gameObject.SetActive(false);
            CUIManager._instance.RefreshInventory();
            return true;
        }
        return false;
    }


    public void UseItem(int InvenIdx)
    {
        if (_inventory[InvenIdx])
            _inventory[InvenIdx].UseItem(InvenIdx);
    }

    public void UseQuickSlotItem(int quickSlotIdx)
    {
        if (_quickSlots[quickSlotIdx])
        {
            int index = -1;

            for (int i = 0; i < (int)INVENTORY.CAPACITY; i++)
            {
                if (_quickSlots[quickSlotIdx] == _inventory[i])
                {
                    index = i;
                    break;
                }
            }
            if (index != -1)
            {
                _quickSlots[quickSlotIdx].UseItem(index);
                if (!_quickSlots[quickSlotIdx] || _quickSlots[quickSlotIdx].InventoryExpress == 0)
                    _quickSlots[quickSlotIdx] = null;
                CUIManager._instance.RefreshInventory();
            }
        }
    }

    public void UsePotion(int InvenIdx)
    {
        switch (((CPotion)_inventory[InvenIdx]).Sort)
        {
            case POTION.HEALING:
                {
                    if (((CPotion)_inventory[InvenIdx]).Count > 0)
                    {
                        if (_hP < _maxHP)
                        {
                            _hP += _maxHP * 0.5f;
                            if (_hP > _maxHP)
                                _hP = _maxHP;
                            --((CPotion)_inventory[InvenIdx]).Count;
                            if (((CPotion)_inventory[InvenIdx]).Count == 0)
                            {
                                Destroy(_inventory[InvenIdx].gameObject);
                                _inventory[InvenIdx] = null;
                            }
                            CUIManager._instance.RefreshInventory();
                        }
                    }
                }
                break;
        }
    }

    public void ReleaseEquip(int equipIdx)
    {
        if(AddItemToInventory(_equips[equipIdx]))
        {
            _equips[equipIdx] = null;
            CUIManager._instance.RefreshInventory();
            if (equipIdx == (int)EQUIP_SLOT.WEAPON)
                _attack.WaitAttack = new WaitForSeconds(_attack.AttackSpeed);
        }
    }

    public void EquipItem(int invenIdx)
    {
        int equipIdx = (int)((CEquipment)_inventory[invenIdx]).EquipSlot;
        if (_equips[equipIdx])
        {
            if (_inventory[invenIdx] is CEquipment)
            {
                ACItem temp = _equips[equipIdx];
                _equips[equipIdx] = (CEquipment)_inventory[invenIdx];
                _equips[equipIdx].transform.SetParent(_equipTransform);
                _inventory[invenIdx] = null;
                AddItemToInventory(temp);
            }
        }
        else
        {
            _equips[equipIdx] = (CEquipment)_inventory[invenIdx];
            _equips[equipIdx].transform.SetParent(_equipTransform);
            _inventory[invenIdx] = null;
        }

        // 무기바꾸면 무기공격속도 적용
        if (equipIdx == (int)EQUIP_SLOT.WEAPON)
            _attack.WaitAttack = new WaitForSeconds(_attack.AttackSpeed + ((CWeapon)_equips[(int)EQUIP_SLOT.WEAPON]).WeaponAttackSpeed);
    }

    protected bool SwapEquipToInventory(int equipIdx, int invenIdx)
    {
        if (_inventory[invenIdx] is CEquipment)
        {
            if ((int)((CEquipment)_inventory[invenIdx]).EquipSlot == equipIdx)
            {
                ACItem temp = _equips[equipIdx];
                _equips[equipIdx] = (CEquipment)_inventory[invenIdx];
                _equips[equipIdx].transform.SetParent(_equipTransform);
                _inventory[invenIdx] = temp;
                _inventory[invenIdx].transform.SetParent(_invenTransform);
                if (_equips[equipIdx].EquipSlot == EQUIP_SLOT.WEAPON)
                    _attack.WaitAttack = new WaitForSeconds(_attack.AttackSpeed + ((CWeapon)_equips[(int)EQUIP_SLOT.WEAPON]).WeaponAttackSpeed);
                return true;
            }
            else
                return false;
        }
        else
            return false;
    }

    protected void RegisterItemToQuickSlot(int startItemIdx, int quickSlotIdx)
    {
        if(startItemIdx < (int)EQUIP_SLOT.EQUIP_SLOT_END)
            _quickSlots[quickSlotIdx] = _equips[startItemIdx];

        // 퀵슬롯에서 퀵슬롯 스왑은 DragItemSlot에서 처리함.
        else
            _quickSlots[quickSlotIdx] = _inventory[startItemIdx - (int)EQUIP_SLOT.EQUIP_SLOT_END];
    }

    public void RemoveItemFromQuickSlot(int quickSlotIdx)
    {
        _quickSlots[quickSlotIdx] = null;
    }

    public bool DragItemSlot(int startIdx, int destIdx)
    {
        if(startIdx >= (int)EQUIP_SLOT.EQUIP_SLOT_END + (int)INVENTORY.CAPACITY)
        {
            startIdx -= (int)EQUIP_SLOT.EQUIP_SLOT_END + (int)INVENTORY.CAPACITY;
            if (destIdx >= (int)EQUIP_SLOT.EQUIP_SLOT_END + (int)INVENTORY.CAPACITY)
            {
                destIdx -= (int)EQUIP_SLOT.EQUIP_SLOT_END + (int)INVENTORY.CAPACITY;
                ACItem temp = _quickSlots[destIdx];
                _quickSlots[destIdx] = _quickSlots[startIdx];
                _quickSlots[startIdx] = temp;
                return true;
            }
            // 버튼말고 밖으로 빼는 경우는 CItemDrag.OnEndDrag에서 바로 처리.
            return false;
        }
        else if (destIdx < (int)EQUIP_SLOT.EQUIP_SLOT_END)
        {
            // 목표아이템이 장비슬롯에있으면
            if (_equips[destIdx])
            {
                // 바꿀 시작 아이템이 장비슬롯에있으면 리턴 (의미없으니)
                if (startIdx < (int)EQUIP_SLOT.EQUIP_SLOT_END)
                    return false;

                // 시작 아이템이 인벤토리에있으면
                else if(startIdx < (int)EQUIP_SLOT.EQUIP_SLOT_END + (int)INVENTORY.CAPACITY)
                {
                    startIdx -= (int)EQUIP_SLOT.EQUIP_SLOT_END;
                    return SwapEquipToInventory(destIdx, startIdx);
                }
            }
            else
            {
                if (startIdx < (int)EQUIP_SLOT.EQUIP_SLOT_END + (int)INVENTORY.CAPACITY)
                {
                    startIdx -= (int)EQUIP_SLOT.EQUIP_SLOT_END;
                    if (_inventory[startIdx] is CEquipment && (int)((CEquipment)_inventory[startIdx]).EquipSlot == destIdx)
                    {
                        // EquipItem 들어가서 검사 중복으로 하는 문제있음
                        EquipItem(startIdx);
                        return true;
                    }
                }
                else
                    return false;
            }
        }
        // 목표아이템이 인벤토리슬롯에 있으면
        else if (destIdx < (int)EQUIP_SLOT.EQUIP_SLOT_END + (int)INVENTORY.CAPACITY)
        {
            destIdx -= (int)EQUIP_SLOT.EQUIP_SLOT_END;

            // 시작 아이템이 장비슬롯에있으면
            if (startIdx < (int)EQUIP_SLOT.EQUIP_SLOT_END)
            {
                if(!_inventory[destIdx])
                {
                    _inventory[destIdx] = _equips[startIdx];
                    _equips[startIdx] = null;
                    _inventory[destIdx].transform.SetParent(_invenTransform);
                    if (startIdx == (int)EQUIP_SLOT.WEAPON)
                        _attack.WaitAttack = new WaitForSeconds(_attack.AttackSpeed);
                    return true;
                }
                else
                    return SwapEquipToInventory(startIdx, destIdx);
            }
            // 시작 아이템이 인벤토리에있으면
            else if(startIdx < (int)EQUIP_SLOT.EQUIP_SLOT_END + (int)INVENTORY.CAPACITY)
            {
                startIdx -= (int)EQUIP_SLOT.EQUIP_SLOT_END;

                if(_inventory[destIdx])
                {
                    ACItem temp = _inventory[destIdx];
                    _inventory[destIdx] = _inventory[startIdx];
                    _inventory[startIdx] = temp;
                }
                else
                {
                    _inventory[destIdx] = _inventory[startIdx];
                    _inventory[startIdx] = null;
                }
                return true;
            }
        }
        // 목표아이템이 퀵슬롯에 있으면
        else
        {
            RegisterItemToQuickSlot(startIdx, destIdx - (int)EQUIP_SLOT.EQUIP_SLOT_END - (int)INVENTORY.CAPACITY);
            return true;
        }

        return false;
    }

    public abstract void Evasion(in Vector3 dir);

    protected IEnumerator CoroutineAttackable()
    {
        while (true)
        {
            if (!_attack.Attackable)
            {
                yield return _attack.WaitAttack;
                _attack.Attackable = true;
            }
            else
                yield return null;
        }
    }

    protected IEnumerator CoroutineHit()
    {
        _animator.SetBool(_hitParameterHash, true);
        _spriteRenderer.color = _hitColor;
        yield return _waitHitColor;
        _spriteRenderer.color = Color.white;
        yield return _waitHit;
        _animator.SetBool(_hitParameterHash, false);
    }
}
