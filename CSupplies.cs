using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Constants;

public class CSupplies : ACItem
{
    [SerializeField] private SUPPLIES _sort;
    private static bool[] _identified = new bool[(int)SUPPLIES.SUPPLIES_END];

    public SUPPLIES Sort{ get { return _sort; } }
    public int Count { get; set; } = 1;
    public override int InventoryExpress { get { return Count; } }
    public override bool Identified { get { return _identified[(int)_sort]; } set { _identified[(int)_sort] = value; } }

    private void Start()
    {
        if ((int)_sort < (int)SUPPLIES.POTION_END)
            transform.Find("Graphic").GetComponent<SpriteRenderer>().sprite = CGameManager._instance.PotionSpriteList[(int)_sort];
        else
            transform.Find("Graphic").GetComponent<SpriteRenderer>().sprite = CGameManager._instance.ScrollSpriteList[(int)_sort - (int)SUPPLIES.POTION_END];
    }

    public override void UseItem(int InvenIdx)
    {
        _player.UseSupplies(InvenIdx);
    }

    public override void Interaction()
    {
        _player.AddSuppliesToInventory(this);
    }
}
