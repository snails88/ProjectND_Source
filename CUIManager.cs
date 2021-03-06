﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Constants;

public class CUIManager : MonoBehaviour
{
    public static CUIManager _instance;

    [SerializeField] private GameObject _hPBarPrefab;
    private GameObject _UIInventory;
    private Image[] _inventoryImages = new Image[(int)INVENTORY.CAPACITY];
    private Image[] _identifyImage = new Image[(int)INVENTORY.CAPACITY];
    private Text[] _inventoryTexts = new Text[(int)INVENTORY.CAPACITY];
    private Image[] _equipImages = new Image[(int)EQUIP_SLOT.EQUIP_SLOT_END];
    private Text[] _equipTexts = new Text[(int)EQUIP_SLOT.EQUIP_SLOT_END];
    private Image[] _quickSlotImages = new Image[(int)QUICK_SLOT.CAPACITY];
    private Text[] _quickSlotTexts = new Text[(int)QUICK_SLOT.CAPACITY];
    private Image _playerHPProgressBar;
    private Image _playerResourceProgressBar;
    private CPlayer _player;
    private Color _unidentifiedColor = new Color(1f, 0.5f, 0.5f, 0.5f);
    private Color _cursedColor = new Color(1f, 0f, 1f, 1f);
    private Queue<GameObject> _hPBarPool = new Queue<GameObject>();
    
    public GameObject HPBarPrefab { get { return _hPBarPrefab; } }
    public int MouseOverIndex { get; protected set; } = -1;

    // 식별, 저주해제 등 사용시
    public bool UseSupplies { get; set; }
    public SUPPLIES UsedSuplly { get; set; }

    private void Awake()
    {
        if (!_instance)
            _instance = this;
    }

    private void Start()
    {
        _player = GameObject.FindWithTag("Player").GetComponent<CPlayer>();
        _UIInventory = GameObject.Find("InventoryUI");
        for (int i = 0; i < (int)INVENTORY.CAPACITY; i++)
        {
            _inventoryImages[i] = GameObject.Find("InventoryImage_" + i.ToString()).GetComponent<Image>();
            _identifyImage[i] = GameObject.Find("IdentifyImage_" + i.ToString()).GetComponent<Image>();
            _inventoryTexts[i] = GameObject.Find("InventoryText_" + i.ToString()).GetComponent<Text>();
        }
        for (int i = 0; i < (int)EQUIP_SLOT.EQUIP_SLOT_END; i++)
        {
            _equipImages[i] = GameObject.Find("EquipImage_" + i.ToString()).GetComponent<Image>();
            _equipTexts[i] = GameObject.Find("EquipText_" + i.ToString()).GetComponent<Text>();
        }
        for (int i = 0; i < (int)QUICK_SLOT.CAPACITY; i++)
        {
            _quickSlotImages[i] = GameObject.Find("QuickSlotImage_" + i.ToString()).GetComponent<Image>();
            _quickSlotTexts[i] = GameObject.Find("QuickSlotText_" + i.ToString()).GetComponent<Text>();

        }
        _UIInventory.SetActive(false);
        _playerHPProgressBar = GameObject.Find("PlayerHPProgressBar").GetComponent<Image>();
        _playerResourceProgressBar = GameObject.Find("PlayerResourceProgressBar").GetComponent<Image>();
        _playerResourceProgressBar.color = _player.ResourceColor;
    }

    private void Update()
    {
        _playerHPProgressBar.fillAmount = _player.HP / _player.MaxHP;
        _playerResourceProgressBar.fillAmount = _player.Resource / _player.MaxResource;
    }

    public bool HPBarPoolIsEmpty()
    {
        return _hPBarPool.Count == 0 ? true : false;
    }

    public void AddHPBarInPool(in GameObject go)
    {
        _hPBarPool.Enqueue(go);
    }

    public GameObject PopHPBarByPool()
    {
        return _hPBarPool.Dequeue();
    }

    public void PopUpInventory()
    {
        if(_UIInventory.activeSelf)
        {
            _UIInventory.SetActive(false);
            CGameManager._instance.SetTimeScale(1f);
        }
        else
        {
            _UIInventory.SetActive(true);
            CGameManager._instance.SetTimeScale(0f);
        }
    }

    public void PopUpInventoryAlways()
    {
        _UIInventory.SetActive(true);
        CGameManager._instance.SetTimeScale(0f);
    }

    public void RefreshInventory()
    {
        for (int i = 0; i < (int)INVENTORY.CAPACITY; i++)
        {
            if (_player.Inventory[i])
            {
                _inventoryImages[i].sprite = _player.Inventory[i].transform.Find("Graphic").GetComponent<SpriteRenderer>().sprite;
                _inventoryTexts[i].text = _player.Inventory[i].InventoryExpress.ToString();
                if (_player.Inventory[i].Identified)
                {
                    _identifyImage[i].color = Color.clear;
                    if (_player.Inventory[i] is CEquipment)
                    {
                        if (((CEquipment)_player.Inventory[i]).Cursed)
                            _inventoryImages[i].color = _cursedColor;
                        else
                            _inventoryImages[i].color = Color.white;
                    }
                }
                else
                {
                    _identifyImage[i].color = _unidentifiedColor;
                }
            }
            else
            {
                _inventoryImages[i].sprite = null;
                _inventoryImages[i].color = Color.white;
                _inventoryTexts[i].text = string.Empty;
                _identifyImage[i].color = Color.clear;
            }
            
        }

        for (int i = 0; i < (int)EQUIP_SLOT.EQUIP_SLOT_END; i++)
        {
            if (_player.Equips[i])
            {
                _equipImages[i].sprite = _player.Equips[i].transform.Find("Graphic").GetComponent<SpriteRenderer>().sprite;
                _equipTexts[i].text = _player.Equips[i].Lv.ToString();
                if (_player.Equips[i].Cursed)
                    _equipImages[i].color = _cursedColor;
                else
                    _equipImages[i].color = Color.white;
            }
            else
            {
                _equipImages[i].sprite = null;
                _equipImages[i].color = Color.white;
                _equipTexts[i].text = string.Empty;
            }
        }

        for (int i = 0; i < (int)QUICK_SLOT.CAPACITY; i++)
        {
            if (_player.QuickSlots[i])
            {
                _quickSlotImages[i].sprite = _player.QuickSlots[i].transform.Find("Graphic").GetComponent<SpriteRenderer>().sprite;
                _quickSlotTexts[i].text = _player.QuickSlots[i].InventoryExpress.ToString();
            }
            else
            {
                _quickSlotImages[i].sprite = null;
                _quickSlotTexts[i].text = string.Empty;
            }
        }
    }

    public void UseItem(int invenIdx)
    {
        if (!UseSupplies)
            _player.UseItem(invenIdx);
        else
        {
            switch(UsedSuplly)
            {
                case SUPPLIES.IDENTIFY_SCROLL:
                    _player.IdentifyItem(invenIdx);
                    break;
                case SUPPLIES.ENCHANT_SCROLL:
                    _player.EnchantItem(invenIdx);
                    break;
                case SUPPLIES.REMOVECURSE_SCROLL:
                    _player.RemoveCurseItem(invenIdx);
                    break;
            }
        }
    }

    public void UseQuickSlotItem(int quickSlotIdx)
    {
        _player.UseQuickSlotItem(quickSlotIdx);
    }

    public void ReleaseEquip(int equipIdx)
    {
        if (UseSupplies)
        {
            switch (UsedSuplly)
            {
                case SUPPLIES.IDENTIFY_SCROLL:
                    _player.IdentifyItemOnEquip(equipIdx);
                    break;
                case SUPPLIES.ENCHANT_SCROLL:
                    _player.EnchantItemOnEquip(equipIdx);
                    break;
                case SUPPLIES.REMOVECURSE_SCROLL:
                    _player.RemoveCurseItemOnEquip(equipIdx);
                    break;
            }
        }
        else
            _player.ReleaseEquip(equipIdx);
    }

    public void OnMouseOverEquip(int index)
    {
        MouseOverIndex = index;
    }

    public void OnMouseOverInventory(int index)
    {
        MouseOverIndex = index + (int)EQUIP_SLOT.EQUIP_SLOT_END;
    }

    public void OnMouseOverQuickSlot(int index)
    {
        MouseOverIndex = index + (int)EQUIP_SLOT.EQUIP_SLOT_END + (int)INVENTORY.CAPACITY;
    }

    public void ExitMouse()
    {
        MouseOverIndex = -1;
    }
}
