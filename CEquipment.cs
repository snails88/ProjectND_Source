using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Constants;

public class CEquipment : ACItem, IInteractionObject
{
    [SerializeField]
    private EQUIP_SLOT _equipSlot;

    private CPlayer _player;

    public EQUIP_SLOT EquipSlot
    {
        get { return _equipSlot; }
    }


    void Start()
    {
        _player = GameObject.Find("Player").GetComponent<CPlayer>();
    }

    protected void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject == _player.gameObject)
        {
            CInputManager._instance.AddObjectInteractionList(gameObject);
            print("인터랙션리스트등록");
        }
    }

    protected void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject == _player.gameObject)
        {
            CInputManager._instance.RemoveObjectInteractionList(gameObject);
            print("인터랙션리스트해제");
        }
    }

    public void Interaction()
    {
        _player.AddItemToInventory(this);
    }

    public override void UseItem(int InvenIdx)
    {
        _player.EquipItem(InvenIdx);
    }
}
