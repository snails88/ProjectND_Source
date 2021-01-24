using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Constants;

public class CSupplies : ACItem
{
    [SerializeField] private SUPPLIES _sort;
    private static bool[] _identified = new bool[(int)SUPPLIES.SUPPLIES_END];

    public SUPPLIES Sort{ get { return _sort; } }
    public int Count { get; set; }
    public override int InventoryExpress { get { return Count; } }
    public override bool Identified { get { return _identified[(int)_sort]; } set { _identified[(int)_sort] = value; } }

    public override void UseItem(int InvenIdx)
    {
        _player.UseSupplies(InvenIdx);
    }

    public override void Interaction()
    {
        _player.AddSuppliesToInventory(this);
    }
}
