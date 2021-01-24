using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ACItem : MonoBehaviour, IInteractionObject
{
    protected CPlayer _player;

    // 인벤에 무기레벨이랑 포션개수 묶어서 사용
    public abstract int InventoryExpress { get; }
    public abstract bool Identified { get; set; }

    public abstract void UseItem(int InvenIdx);
    public abstract void Interaction();

    protected void Awake()
    {
        _player = GameObject.FindWithTag("Player").GetComponent<CPlayer>();
    }

    protected void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject == _player.gameObject)
        {
            CInputManager._instance.AddObjectInteractionList(gameObject);
        }
    }

    protected void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject == _player.gameObject)
        {
            CInputManager._instance.RemoveObjectInteractionList(gameObject);
        }
    }
}
