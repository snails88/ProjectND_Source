using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Constants;

public class CAttack : MonoBehaviour
{
    [SerializeField]
    private float _lifeTime;
    private float _currentTime;

    [SerializeField]
    private string _ownerTag;

    private List<GameObject> _hitList = new List<GameObject>();

    private GameObject _objectPoolObject;
    private GameObject _graphicObject;
    private GameObject _prototypePrefab;
    private Transform _prototypeGraphicTransform;

    public float Damage
    {
        get;
        set;
    }

    private void Awake()
    {
        _objectPoolObject = GameObject.Find("AttackObjectPool");
        _graphicObject = transform.Find("Graphic").gameObject;
    }

    private void OnEnable()
    {
        _currentTime = 0f;
        _hitList.Clear();
    }


    void Update()
    {
        _currentTime += Time.deltaTime;
        
        if(_lifeTime <= _currentTime)
        {
            CGameManager._instance.AddAttackObjectInPool(gameObject);
            gameObject.transform.SetParent(_objectPoolObject.transform);
            gameObject.SetActive(false);
        }
    }

    public void SetAttack(PROTOTYPE_ATTACK prototype, float dmg)
    {
        _prototypePrefab = CGameManager._instance.GetAttackObjectPrototype((int)prototype);
        _prototypeGraphicTransform = _prototypePrefab.transform.Find("Graphic");
        gameObject.layer = _prototypePrefab.layer;
        _graphicObject.transform.localScale = _prototypeGraphicTransform.localScale;
        _graphicObject.GetComponent<SpriteRenderer>().color = _prototypeGraphicTransform.GetComponent<SpriteRenderer>().color;
        GetComponentInChildren<BoxCollider2D>().size = _prototypePrefab.GetComponent<BoxCollider2D>().size;
        GetComponentInChildren<BoxCollider2D>().offset = _prototypePrefab.GetComponent<BoxCollider2D>().offset;
        _lifeTime = _prototypePrefab.GetComponent<CAttack>()._lifeTime;
        _ownerTag = _prototypePrefab.GetComponent<CAttack>()._ownerTag;
        
        Damage = dmg;
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag(_ownerTag))
        {
            foreach (GameObject hitObject in _hitList)
            {
                if (collision.gameObject == hitObject)
                    return;
            }
            _hitList.Add(collision.gameObject);

            if (collision.gameObject.GetComponent<ICollisionObject>() != null)
            {
                collision.gameObject.GetComponent<ICollisionObject>().Hit(Damage);
            }
        }
    }
}
