using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CInputManager : MonoBehaviour
{
    public static CInputManager _instance;

    private CPlayer _player;
    private Vector2 _moveDir;
    private string _horizon = "Horizontal";
    private string _vertical = "Vertical";

    private LinkedList<GameObject> _interactionList = new LinkedList<GameObject>();

    private void Awake()
    {
        if (!_instance)
            _instance = this;
    }

    void Start()
    {
        _player = GameObject.Find("Player").GetComponent<CPlayer>();
    }

    void Update()
    {
        PlayerMove();
        PlayerAttack();
        PlayerEvasion();
        PopUpInventory();
        Interaction();
    }

    void PlayerMove()
    {
        _moveDir.x = Input.GetAxisRaw(_horizon);
        _moveDir.y = Input.GetAxisRaw(_vertical);

        if(_moveDir.magnitude != 0f)
        {
            _moveDir.Normalize();
            _player.Move(_moveDir);
        }
    }

    void PlayerAttack()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0) && Time.timeScale != 0f && !EventSystem.current.IsPointerOverGameObject()) 
            _player.Attack();
    }

    void PlayerEvasion()
    {
        if(Input.GetKeyDown(KeyCode.Mouse1))
        {
            _player.Evasion(_moveDir);
        }
    }

    void PopUpInventory()
    {
        if(Input.GetKeyDown(KeyCode.I))
        {
            CUIManager._instance.PopUpInventory();
        }
    }

    void Interaction()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if(_interactionList.Count != 0)
            {
                _interactionList.First.Value.GetComponent<IInteractionObject>().Interaction();
            }
        }
    }

    public void AddObjectInteractionList(in GameObject go)
    {
        _interactionList.AddLast(go);
    }

    public void RemoveObjectInteractionList(in GameObject go)
    {
        _interactionList.Remove(go);
    }
}
