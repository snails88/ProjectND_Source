using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Constants;

public class CEquipment : ACItem
{
    [SerializeField] private EQUIP_SLOT _equipSlot;
    private int _lv = 1;

    public int Lv
    {
        get { return _lv; }
        set { _lv = value; }
    }
    public override int InventoryExpress { get { return _lv; } }
    public EQUIP_SLOT EquipSlot { get { return _equipSlot; } }

    public override void Interaction()
    {
        _player.AddItemToInventory(this);
    }

    public override void UseItem(int InvenIdx)
    {
        _player.EquipItem(InvenIdx);
        CUIManager._instance.RefreshInventory();
    }
}
