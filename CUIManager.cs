using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Constants;

public class CUIManager : MonoBehaviour
{
    public static CUIManager _instance;

    private GameObject _UIInventory;
    private Image[] _inventoryImages = new Image[(int)INVENTORY.CAPACITY];
    private Image[] _equipImages = new Image[(int)EQUIP_SLOT.EQUIP_SLOT_END];
    private CPlayer _player;
    private Queue<GameObject> _hPBarPool = new Queue<GameObject>();
    [SerializeField]
    private GameObject _hPBarPrefab;
    public GameObject HPBarPrefab
    {
        get { return _hPBarPrefab; }
    }

    private void Awake()
    {
        if (!_instance)
            _instance = this;

        _UIInventory = GameObject.Find("InventoryUI");
        for (int i = 0; i < (int)INVENTORY.CAPACITY; i++)
        {
            _inventoryImages[i] = GameObject.Find("InventoryImage_" + i.ToString()).GetComponent<Image>();
        }
        for (int i = 0; i < (int)EQUIP_SLOT.EQUIP_SLOT_END; i++)
        {
            _equipImages[i] = GameObject.Find("EquipImage_" + i.ToString()).GetComponent<Image>();
        }
        _UIInventory.SetActive(false);
    }

    void Start()
    {
        _player = GameObject.Find("Player").GetComponent<CPlayer>();
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
                _inventoryImages[i].sprite = _player.Inventory[i].gameObject.transform.Find("Graphic").GetComponent<SpriteRenderer>().sprite;
            else
                _inventoryImages[i].sprite = null;
        }

        for (int i = 0; i < (int)EQUIP_SLOT.EQUIP_SLOT_END; i++)
        {
            if(_player.Equip[i])
                _equipImages[i].sprite = _player.Equip[i].gameObject.transform.Find("Graphic").GetComponent<SpriteRenderer>().sprite;
            else
                _equipImages[i].sprite = null;
        }
    }

    public void UseItem(int invenIdx)
    {
        _player.UseItem(invenIdx);
    }

    public void ReleaseEquip(int equipIdx)
    {
        _player.ReleaseEquip(equipIdx);
    }
}
