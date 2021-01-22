using System.Collections;
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
    private Text[] _inventoryTexts = new Text[(int)INVENTORY.CAPACITY];
    private Image[] _equipImages = new Image[(int)EQUIP_SLOT.EQUIP_SLOT_END];
    private Text[] _equipTexts = new Text[(int)EQUIP_SLOT.EQUIP_SLOT_END];
    private Image[] _quickSlotImages = new Image[(int)QUICK_SLOT.CAPACITY];
    private Text[] _quickSlotTexts = new Text[(int)QUICK_SLOT.CAPACITY];
    private Image _playerHPProgressBar;
    private Image _playerResourceProgressBar;
    private CPlayer _player;
    private Queue<GameObject> _hPBarPool = new Queue<GameObject>();
    
    public GameObject HPBarPrefab { get { return _hPBarPrefab; } }
    public int MouseOverIndex { get; protected set; } = -1;

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
        // _UIInventory.SetActive(!_UIInventory.activeSelf);
    }

    public void RefreshInventory()
    {
        for (int i = 0; i < (int)INVENTORY.CAPACITY; i++)
        {
            if (_player.Inventory[i])
            {
                _inventoryImages[i].sprite = _player.Inventory[i].gameObject.transform.Find("Graphic").GetComponent<SpriteRenderer>().sprite;
                _inventoryTexts[i].text = _player.Inventory[i].InventoryExpress.ToString();
            }
            else
            {
                _inventoryImages[i].sprite = null;
                _inventoryTexts[i].text = string.Empty;
            }
        }

        for (int i = 0; i < (int)EQUIP_SLOT.EQUIP_SLOT_END; i++)
        {
            if (_player.Equips[i])
            {
                _equipImages[i].sprite = _player.Equips[i].gameObject.transform.Find("Graphic").GetComponent<SpriteRenderer>().sprite;
                _equipTexts[i].text = _player.Equips[i].Lv.ToString();
            }
            else
            {
                _equipImages[i].sprite = null;
                _equipTexts[i].text = string.Empty;
            }
        }

        for (int i = 0; i < (int)QUICK_SLOT.CAPACITY; i++)
        {
            if (_player.QuickSlots[i])
            {
                _quickSlotImages[i].sprite = _player.QuickSlots[i].gameObject.transform.Find("Graphic").GetComponent<SpriteRenderer>().sprite;
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
        _player.UseItem(invenIdx);
    }

    public void UseQuickSlotItem(int quickSlotIdx)
    {
        _player.UseQuickSlotItem(quickSlotIdx);
    }

    public void ReleaseEquip(int equipIdx)
    {
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
