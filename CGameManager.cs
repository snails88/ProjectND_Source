using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Constants;

public class CGameManager : MonoBehaviour
{
    public static CGameManager _instance;

    [SerializeField] private GameObject[] _attackObjectPrototypes;
    private Queue<GameObject> _attackObjectPool = new Queue<GameObject>();

    private void Awake()
    {
        if (!_instance)
            _instance = this;
    }

    public bool AttackObjectPoolIsEmpty()
    {
        return _attackObjectPool.Count == 0 ? true : false;
    }

    public void AddAttackObjectInPool(in GameObject go)
    {
        _attackObjectPool.Enqueue(go);
    }

    public GameObject PopAttackObjectByPool()
    {
        return _attackObjectPool.Dequeue();
    }

    public GameObject GetAttackObjectPrototype(int index) 
    {
        return _attackObjectPrototypes[index];
    }

    public GameObject GetAttackObjectPrototype(PROTOTYPE_ATTACK Eindex)
    {
        return _attackObjectPrototypes[(int)Eindex];
    }

    public void SetTimeScale(float timeScale)
    {
        Time.timeScale = timeScale;
    }
}
