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

    public float Damage
    {
        get;
        set;
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
            gameObject.transform.SetParent(GameObject.Find("AttackObjectPool").transform);
            gameObject.SetActive(false);
        }
    }

    public void SetAttack(PROTOTYPE_ATTACK prototype, float dmg)
    {
        GameObject prototypePrefab = CGameManager._instance.GetAttackObjectPrototype((int)prototype);
        gameObject.layer = prototypePrefab.layer;
        transform.Find("Graphic").transform.localScale = prototypePrefab.transform.Find("Graphic").transform.localScale;
        transform.Find("Graphic").GetComponent<SpriteRenderer>().color = prototypePrefab.transform.Find("Graphic").GetComponent<SpriteRenderer>().color;
        GetComponentInChildren<BoxCollider2D>().size = prototypePrefab.GetComponent<BoxCollider2D>().size;
        GetComponentInChildren<BoxCollider2D>().offset = prototypePrefab.GetComponent<BoxCollider2D>().offset;
        _lifeTime = prototypePrefab.GetComponent<CAttack>()._lifeTime;
        _ownerTag = prototypePrefab.GetComponent<CAttack>()._ownerTag;
        
        Damage = dmg;
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag != _ownerTag)
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
