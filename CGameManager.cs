using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Constants;

public class CGameManager : MonoBehaviour
{
    public static CGameManager _instance;

    [SerializeField] private GameObject[] _attackObjectPrototypes;
    [SerializeField] private List<Sprite> _potionSpriteList;
    [SerializeField] private List<Sprite> _scrollSpriteList;
    private Queue<GameObject> _attackObjectPool = new Queue<GameObject>();
    public List<Sprite> PotionSpriteList { get { return _potionSpriteList; } }
    public List<Sprite> ScrollSpriteList { get { return _scrollSpriteList; } }

    private void Awake()
    {
        if (!_instance)
            _instance = this;
        Screen.SetResolution(1280, 720, false);

        RandomSupplySprites();
    }

    private void RandomSupplySprites()
    {
        int count = _potionSpriteList.Count;
        List<Sprite> potionSpriteList = new List<Sprite>();
        for (int i = 0; i < count; i++)
        {
            int random = Random.Range(0, _potionSpriteList.Count);
            potionSpriteList.Add(_potionSpriteList[random]);
            _potionSpriteList.RemoveAt(random);
        }
        _potionSpriteList = potionSpriteList;

        count = _scrollSpriteList.Count;
        List<Sprite> scrollSpriteList = new List<Sprite>();
        for (int i = 0; i < count; i++)
        {
            int random = Random.Range(0, _scrollSpriteList.Count);
            scrollSpriteList.Add(_scrollSpriteList[random]);
            _scrollSpriteList.RemoveAt(random);
        }
        _scrollSpriteList = scrollSpriteList;
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
