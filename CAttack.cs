using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Constants;

public class CAttack : MonoBehaviour
{
    [SerializeField] private float _lifeTime;
    [SerializeField] private string _ownerTag;
    private List<GameObject> _hitList = new List<GameObject>();
    private GameObject _objectPoolObject;
    private GameObject _graphicObject;
    private GameObject _prototypePrefab;
    private CCreature _owner;
    private Transform _prototypeGraphicTransform;
    private WaitForSeconds _wait;
    private string _graphic = "Graphic";

    public float Damage { get; set; }

    private void Awake()
    {
        _objectPoolObject = GameObject.Find("AttackObjectPool");
        _graphicObject = transform.Find(_graphic).gameObject;
        _wait = new WaitForSeconds(_lifeTime);
    }

    private void OnEnable()
    {
        _hitList.Clear();
    }

    public void LifeTimeCoroutineStart()
    {
        StartCoroutine("CoroutineLifeTime");
    }

    public void SetOwner(in CCreature owner)
    {
        _owner = owner;
    }


    public void SetAttack(PROTOTYPE_ATTACK prototype, float dmg, in CCreature owner)
    {
        _prototypePrefab = CGameManager._instance.GetAttackObjectPrototype((int)prototype);
        _prototypeGraphicTransform = _prototypePrefab.transform.Find(_graphic);
        gameObject.layer = _prototypePrefab.layer;
        _graphicObject.transform.localScale = _prototypeGraphicTransform.localScale;
        _graphicObject.GetComponent<SpriteRenderer>().color = _prototypeGraphicTransform.GetComponent<SpriteRenderer>().color;
        GetComponentInChildren<BoxCollider2D>().size = _prototypePrefab.GetComponent<BoxCollider2D>().size;
        GetComponentInChildren<BoxCollider2D>().offset = _prototypePrefab.GetComponent<BoxCollider2D>().offset;
        _lifeTime = _prototypePrefab.GetComponent<CAttack>()._lifeTime;
        _wait = new WaitForSeconds(_lifeTime);
        _ownerTag = _prototypePrefab.GetComponent<CAttack>()._ownerTag;
        Damage = dmg;
        _owner = owner;
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

            ICollisionObject target = collision.gameObject.GetComponent<ICollisionObject>();
            if (target != null)
            {
                _owner.HitTarget(target);
                target.Hit(Damage);
            }
        }
    }

    IEnumerator CoroutineLifeTime()
    {
        yield return _wait;
        CGameManager._instance.AddAttackObjectInPool(gameObject);
        gameObject.transform.SetParent(_objectPoolObject.transform);
        gameObject.SetActive(false);
    }
}
