using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine;
using Constants;

public class CItemDrag : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    private Vector3 _initPos;
    private Transform _initParent;
    private Transform _canvasTransform;
    [SerializeField]
    private Image _image;
    private CPlayer _player;

    private static int _startIndex;
    private static bool _drag = false;

    private void Awake()
    {
        _initPos = transform.position;
        _initParent = transform.parent;
        _canvasTransform = GameObject.Find("Canvas").transform;
    }

    private void Start()
    {
        _player = GameObject.Find("Player").GetComponent<CPlayer>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        _startIndex = CUIManager._instance.MouseOverIndex;

        if (_startIndex < 0)
            return;

        // 마우스가 장비슬롯에 오버일때
        if (_startIndex < (int)EQUIP_SLOT.EQUIP_SLOT_END)
        {
            if (_player.Equips[_startIndex])
                _drag = true;
        }
        // 인벤토리에 오버
        else if (_startIndex < (int)EQUIP_SLOT.EQUIP_SLOT_END + (int)INVENTORY.CAPACITY)
        {
            if(_player.Inventory[_startIndex - (int)EQUIP_SLOT.EQUIP_SLOT_END])
                _drag = true;
        }
        // 퀵슬롯에 오버
        else
        {
            if(_player.QuickSlots[_startIndex - (int)EQUIP_SLOT.EQUIP_SLOT_END - (int)INVENTORY.CAPACITY])
                _drag = true;
        }

        if (_drag)
        {
            _image.raycastTarget = false;
            transform.SetParent(_canvasTransform);
            transform.SetAsLastSibling();
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (_drag)
            transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if(_drag)
        {
            print(CUIManager._instance.MouseOverIndex);
            if (CUIManager._instance.MouseOverIndex > -1 && _startIndex != CUIManager._instance.MouseOverIndex)
            {
                if (_player.DragItemSlot(_startIndex, CUIManager._instance.MouseOverIndex))
                    CUIManager._instance.RefreshInventory();
            }
            else if (CUIManager._instance.MouseOverIndex == -1 && _startIndex >= (int)EQUIP_SLOT.EQUIP_SLOT_END + (int)INVENTORY.CAPACITY)
            {
                _player.RemoveItemFromQuickSlot(_startIndex - (int)EQUIP_SLOT.EQUIP_SLOT_END - (int)INVENTORY.CAPACITY);
                CUIManager._instance.RefreshInventory();
            }
            _drag = false;
            _image.raycastTarget = true;
            transform.position = _initPos;
            transform.SetParent(_initParent);
        }
    }
}
