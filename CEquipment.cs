using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Constants;

public class CEquipment : ACItem
{
    [SerializeField] private EQUIP_SLOT _equipSlot;
    private int _lv = 1;
    private bool _identified = false;

    public int Lv
    {
        get { return _lv; }
        set { _lv = value; }
    }
    public EQUIP_SLOT EquipSlot { get { return _equipSlot; } }
    public bool Cursed { get; protected set; }
    public override int InventoryExpress { get { return _lv; } }
    public override bool Identified { get { return _identified; } set { _identified = value; } }

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
