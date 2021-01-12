using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Constants;

public class CPotion : ACItem
{
    [SerializeField]
    private POTION _sort;
    
    private int _count;

    public POTION Sort
    {
        get { return _sort; }
    }

    public int Count
    {
        get { return _count; }
        set { _count = value; }
    }

    public override int InventoryExpress
    {
        get { return _count; }
    }

    public override void UseItem(int InvenIdx)
    {
        _player.UsePotion(InvenIdx);
    }

    public override void Interaction()
    {
        _player.AddPotionToInventory(this);
    }
}
