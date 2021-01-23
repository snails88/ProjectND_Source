using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Constants;

public abstract class CPlayer : CCreature
{
    [System.Serializable] protected struct AttackVars
    {
        public float AttackSpeed;
        public float AttackDistance;
        [HideInInspector] public bool Attackable;
        [HideInInspector] public Vector2 AttackPos;
        [HideInInspector] public float AttackAngle;
        [HideInInspector] public WaitForSeconds WaitAttack;
    }

    [System.Serializable] protected struct EvasionVars
    {
        public float EvasionSpeed;
        public float EvasionTime;
        public float EvasionCooldown;
        [HideInInspector] public bool Evasing;
        [HideInInspector] public bool Evasionable;
        [HideInInspector] public Vector3 EvasionDir;
        [HideInInspector] public WaitForSeconds WaitEvasion;
        [HideInInspector] public WaitForSeconds WaitEvasionCD;
    }
    
    [SerializeField] protected EvasionVars _evasion;
    [SerializeField] protected AttackVars _attack;
    [SerializeField] protected float _cameraShakeForce;
    protected Vector3 _mousePos;
    protected Vector3 _hitDir;
    protected Animator _animator;
    protected int _runParameterHash;
    protected int _hitParameterHash;
    protected SpriteRenderer _spriteRenderer;
    protected Transform _invenTransform;
    protected Transform _equipTransform;
    protected CHitStop _hitStop;
    protected CCamera _camera;

    public float HP { get { return _hP; } }
    public float MaxHP { get { return _maxHP; } }

    public float Resource { get; protected set; }
    public float MaxResource { get; protected set;  }
    public Color ResourceColor { get; protected set; }
    public CEquipment[] Equips { get; } = new CEquipment[(int)EQUIP_SLOT.EQUIP_SLOT_END];
    public ACItem[] Inventory { get; } = new ACItem[(int)INVENTORY.CAPACITY];
    public ACItem[] QuickSlots { get; } = new ACItem[(int)QUICK_SLOT.CAPACITY];

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
        _hitStop = GetComponent<CHitStop>();
        _runParameterHash = Animator.StringToHash("Run");
        _hitParameterHash = Animator.StringToHash("Hit");
        _hP = _maxHP;
        _waitHit = new WaitForSeconds(_hitTime);
        _waitHitColor = new WaitForSeconds(_hitColorTime);
        _camera = Camera.main.GetComponent<CCamera>();
    }

    protected virtual void Start()
    {
        _attack.Attackable = true;
        StartCoroutine("CoroutineAttackable");
    }

    protected virtual void Update()
    {
        if (Time.timeScale != 0f)
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

    public void SetHitDirection(Vector3 dir)
    {
        _hitDir = dir;
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
                if (Equips[(int)EQUIP_SLOT.WEAPON])
                    attack = Instantiate(CGameManager._instance.GetAttackObjectPrototype((int)((CWeapon)Equips[(int)EQUIP_SLOT.WEAPON]).AttackType), _attack.AttackPos, Quaternion.AngleAxis(_attack.AttackAngle, Vector3.forward)).GetComponent<CAttack>();
                else
                    attack = Instantiate(CGameManager._instance.GetAttackObjectPrototype(PROTOTYPE_ATTACK.BASIC), _attack.AttackPos, Quaternion.AngleAxis(_attack.AttackAngle, Vector3.forward)).GetComponent<CAttack>();
                attack.Damage = 1f;
                attack.SetOwner((CCreature)this);
            }
            else
            {
                attack = CGameManager._instance.PopAttackObjectByPool().GetComponent<CAttack>();
                attack.gameObject.SetActive(true);
                if (Equips[(int)EQUIP_SLOT.WEAPON])
                    attack.SetAttack(((CWeapon)Equips[(int)EQUIP_SLOT.WEAPON]).AttackType, 1f, (CCreature)this);
                else
                    attack.SetAttack(PROTOTYPE_ATTACK.BASIC, 1f, (CCreature)this);
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

    public override void HitTarget(in ICollisionObject target)
    {
        if (target is CCreature)
            StartCoroutine(_hitStop.CoroutineHitStop());
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
            if (Inventory[i] == null)
            {
                index = i;
                break;
            }
        }

        if(index != -1)
        {
            Inventory[index] = item;
            item.transform.SetParent(_invenTransform);
            item.gameObject.SetActive(false);
            CUIManager._instance.RefreshInventory();
            return true;
        }
        return false;
    }

    public bool AddSuppliesToInventory(in CSupplies item)
    {
        if (!item)
            return false;

        int index = -1;
        for (int i = 0; i < (int)INVENTORY.CAPACITY; i++)
        {
            if(Inventory[i] is CSupplies)
            {
                if(((CSupplies)Inventory[i]).Sort == item.Sort)
                {
                    ++((CSupplies)Inventory[i]).Count;
                    Destroy(item.gameObject);
                    CUIManager._instance.RefreshInventory();
                    return true;
                }
            }
            if (Inventory[i] == null && index == -1)
                index = i;
        }
        if (index != -1)
        {
            Inventory[index] = item;
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
        if (Inventory[InvenIdx])
            Inventory[InvenIdx].UseItem(InvenIdx);
    }

    public void UseQuickSlotItem(int quickSlotIdx)
    {
        if (QuickSlots[quickSlotIdx])
        {
            int index = -1;

            for (int i = 0; i < (int)INVENTORY.CAPACITY; i++)
            {
                if (QuickSlots[quickSlotIdx] == Inventory[i])
                {
                    index = i;
                    break;
                }
            }
            if (index != -1)
            {
                QuickSlots[quickSlotIdx].UseItem(index);
                if (!QuickSlots[quickSlotIdx] || QuickSlots[quickSlotIdx].InventoryExpress == 0)
                    QuickSlots[quickSlotIdx] = null;
                CUIManager._instance.RefreshInventory();
            }
        }
    }

    public void UseSupplies(int InvenIdx)
    {
        switch (((CSupplies)Inventory[InvenIdx]).Sort)
        {
            case SUPPLIES.HEALING:
                {
                    if (((CSupplies)Inventory[InvenIdx]).Count > 0)
                    {
                        if (_hP < _maxHP)
                        {
                            _hP += _maxHP * 0.5f;
                            if (_hP > _maxHP)
                                _hP = _maxHP;
                            --((CSupplies)Inventory[InvenIdx]).Count;
                            if (((CSupplies)Inventory[InvenIdx]).Count == 0)
                            {
                                Destroy(Inventory[InvenIdx].gameObject);
                                Inventory[InvenIdx] = null;
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
        if(AddItemToInventory(Equips[equipIdx]))
        {
            Equips[equipIdx] = null;
            CUIManager._instance.RefreshInventory();
            if (equipIdx == (int)EQUIP_SLOT.WEAPON)
                _attack.WaitAttack = new WaitForSeconds(_attack.AttackSpeed);
        }
    }

    public void EquipItem(int invenIdx)
    {
        int equipIdx = (int)((CEquipment)Inventory[invenIdx]).EquipSlot;
        if (Equips[equipIdx])
        {
            if (Inventory[invenIdx] is CEquipment)
            {
                ACItem temp = Equips[equipIdx];
                Equips[equipIdx] = (CEquipment)Inventory[invenIdx];
                Equips[equipIdx].transform.SetParent(_equipTransform);
                Inventory[invenIdx] = null;
                AddItemToInventory(temp);
            }
        }
        else
        {
            Equips[equipIdx] = (CEquipment)Inventory[invenIdx];
            Equips[equipIdx].transform.SetParent(_equipTransform);
            Inventory[invenIdx] = null;
        }

        // 무기바꾸면 무기공격속도 적용
        if (equipIdx == (int)EQUIP_SLOT.WEAPON)
            _attack.WaitAttack = new WaitForSeconds(_attack.AttackSpeed + ((CWeapon)Equips[(int)EQUIP_SLOT.WEAPON]).WeaponAttackSpeed);
    }

    protected bool SwapEquipToInventory(int equipIdx, int invenIdx)
    {
        if (Inventory[invenIdx] is CEquipment)
        {
            if ((int)((CEquipment)Inventory[invenIdx]).EquipSlot == equipIdx)
            {
                ACItem temp = Equips[equipIdx];
                Equips[equipIdx] = (CEquipment)Inventory[invenIdx];
                Equips[equipIdx].transform.SetParent(_equipTransform);
                Inventory[invenIdx] = temp;
                Inventory[invenIdx].transform.SetParent(_invenTransform);
                if (Equips[equipIdx].EquipSlot == EQUIP_SLOT.WEAPON)
                    _attack.WaitAttack = new WaitForSeconds(_attack.AttackSpeed + ((CWeapon)Equips[(int)EQUIP_SLOT.WEAPON]).WeaponAttackSpeed);
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
            QuickSlots[quickSlotIdx] = Equips[startItemIdx];

        // 퀵슬롯에서 퀵슬롯 스왑은 DragItemSlot에서 처리함.
        else
            QuickSlots[quickSlotIdx] = Inventory[startItemIdx - (int)EQUIP_SLOT.EQUIP_SLOT_END];
    }

    public void RemoveItemFromQuickSlot(int quickSlotIdx)
    {
        QuickSlots[quickSlotIdx] = null;
    }

    public bool DragItemSlot(int startIdx, int destIdx)
    {
        if(startIdx >= (int)EQUIP_SLOT.EQUIP_SLOT_END + (int)INVENTORY.CAPACITY)
        {
            startIdx -= (int)EQUIP_SLOT.EQUIP_SLOT_END + (int)INVENTORY.CAPACITY;
            if (destIdx >= (int)EQUIP_SLOT.EQUIP_SLOT_END + (int)INVENTORY.CAPACITY)
            {
                destIdx -= (int)EQUIP_SLOT.EQUIP_SLOT_END + (int)INVENTORY.CAPACITY;
                ACItem temp = QuickSlots[destIdx];
                QuickSlots[destIdx] = QuickSlots[startIdx];
                QuickSlots[startIdx] = temp;
                return true;
            }
            // 버튼말고 밖으로 빼는 경우는 CItemDrag.OnEndDrag에서 바로 처리.
            return false;
        }
        else if (destIdx < (int)EQUIP_SLOT.EQUIP_SLOT_END)
        {
            // 목표아이템이 장비슬롯에있으면
            if (Equips[destIdx])
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
                    if (Inventory[startIdx] is CEquipment && (int)((CEquipment)Inventory[startIdx]).EquipSlot == destIdx)
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
                if(!Inventory[destIdx])
                {
                    Inventory[destIdx] = Equips[startIdx];
                    Equips[startIdx] = null;
                    Inventory[destIdx].transform.SetParent(_invenTransform);
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

                if(Inventory[destIdx])
                {
                    ACItem temp = Inventory[destIdx];
                    Inventory[destIdx] = Inventory[startIdx];
                    Inventory[startIdx] = temp;
                }
                else
                {
                    Inventory[destIdx] = Inventory[startIdx];
                    Inventory[startIdx] = null;
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
        _camera.StartCameraShake(_hitDir, _cameraShakeForce);
        _animator.SetBool(_hitParameterHash, true);
        _spriteRenderer.color = _hitColor;
        yield return _waitHitColor;
        _spriteRenderer.color = Color.white;
        yield return _waitHit;
        _animator.SetBool(_hitParameterHash, false);
    }
}
