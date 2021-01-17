using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Constants;

public class CSlime : CCreature
{
    private CPlayer _player;

    [SerializeField]
    private float _hPBarAddYPos;

    private GameObject _hPBar;

    [System.Serializable]
    private struct AttackVars
    {
        public float DelayBefore;
        public float DelayAfter;
        public float AttackDistance;
        public float TargetDistance;

        public WaitForSeconds WaitAfter;
        public WaitForSeconds WaitBefore;

        [HideInInspector]
        public Vector3 AttackDir;
        [HideInInspector]
        public Vector2 AttackPos;
        [HideInInspector]
        public float AttackAngle;
        [HideInInspector]
        public bool Attacking;
    }

    [SerializeField]
    private AttackVars _attack;

    private Vector3 _moveDir;


    void Start()
    {
        _player = GameObject.FindWithTag("Player").GetComponent<CPlayer>();
        _attack.Attacking = false;
        _attack.WaitAfter = new WaitForSeconds(_attack.DelayAfter);
        _attack.WaitBefore = new WaitForSeconds(_attack.DelayBefore);

        _hP = _maxHP;
        // 이거안해주면 CoroutineAttack 시작할때 한번 때리고 시작함
        _moveDir = _player.transform.position - transform.position;
        StartCoroutine("CoroutineAttack");
    }

    void Update()
    {
        _moveDir = _player.transform.position - transform.position;

        if (!_attack.Attacking)
            Move();
    }

    public override void Hit(float dmg)
    {
        _hP -= dmg;

        if (_hP < 0)
        {
            Die();
            return;
        }

        if(!_hPBar)
        {
            if (CUIManager._instance.HPBarPoolIsEmpty())
                _hPBar = Instantiate(CUIManager._instance.HPBarPrefab, GameObject.Find("HPBarObjectPool").transform);
            else
            {
                _hPBar = CUIManager._instance.PopHPBarByPool();
                _hPBar.SetActive(true);
            }
            _hPBar.GetComponent<CHPBar>().Owner = gameObject;
            _hPBar.GetComponent<CHPBar>().AddYValue = _hPBarAddYPos;
        }
        _hPBar.GetComponent<CHPBar>().SetFillAmount(_hP / _maxHP);

        if(_player is CKnight)
        {
            ((CKnight)_player).GainFury(5f);
        }
    }

    protected override void Die()
    {
        if(GetComponentInChildren<CAttack>())
        {
            GameObject attack = GetComponentInChildren<CAttack>().gameObject;
            attack.transform.SetParent(CGameManager._instance.transform.Find("AttackObjectPool").transform);
            CGameManager._instance.AddAttackObjectInPool(attack);
            attack.SetActive(false);
        }

        CUIManager._instance.AddHPBarInPool(_hPBar);
        _hPBar.SetActive(false);
        _hPBar = null;

        Destroy(gameObject);
    }

    void Move()
    {
        transform.Translate(_moveDir.normalized * _moveSpeed * Time.deltaTime);
    }

    IEnumerator CoroutineAttack()
    {
        while (true)
        {
            if (_attack.Attacking)
            {
                _attack.AttackPos = _attack.AttackDir * _attack.AttackDistance + transform.position;

                CAttack attack;
                if (CGameManager._instance.AttackObjectPoolIsEmpty())
                {
                    attack = Instantiate(CGameManager._instance.GetAttackObjectPrototype(PROTOTYPE_ATTACK.SLIME), _attack.AttackPos, Quaternion.AngleAxis(_attack.AttackAngle, Vector3.forward)).GetComponent<CAttack>();
                    attack.Damage = 1f;
                }
                else
                {
                    attack = CGameManager._instance.PopAttackObjectByPool().GetComponent<CAttack>();
                    attack.gameObject.SetActive(true);
                    attack.SetAttack(PROTOTYPE_ATTACK.SLIME, 1f);
                    attack.transform.rotation = Quaternion.AngleAxis(_attack.AttackAngle, Vector3.forward);
                    attack.transform.position = _attack.AttackPos;
                }
                attack.LifeTimeCoroutineStart();
                attack.transform.SetParent(gameObject.transform);
                yield return new WaitForSeconds(_attack.DelayAfter);
                _attack.Attacking = false;
            }
            else
            {
                if (_moveDir.magnitude < _attack.TargetDistance)
                {
                    _attack.Attacking = true;
                    _attack.AttackDir = _moveDir.normalized;
                    _attack.AttackAngle = Mathf.Atan2(_player.transform.position.y - transform.position.y, _player.transform.position.x - transform.position.x) * Mathf.Rad2Deg;
                    yield return new WaitForSeconds(_attack.DelayBefore);
                }
                else
                    yield return null;
            }
        }
    }
}
