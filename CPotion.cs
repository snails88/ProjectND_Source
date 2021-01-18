using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Constants;

public class CPotion : ACItem
{
    [SerializeField] private POTION _sort;

    public POTION Sort{ get { return _sort; } }
    public int Count { get; set; }
    public override int InventoryExpress { get { return Count; } }

    public override void UseItem(int InvenIdx)
    {
        _player.UsePotion(InvenIdx);
    }

    public override void Interaction()
    {
        _player.AddPotionToInventory(this);
    }
}
