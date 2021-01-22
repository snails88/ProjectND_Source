using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CEnemy : CCreature
{
    [System.Serializable]
    protected struct AttackVars
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
    [SerializeField] protected AttackVars _attack;
    [SerializeField] protected float _hPBarAddYPos;
    protected CPlayer _player;
    protected GameObject _hPBar;
    protected Vector3 _moveDir;

    protected virtual void Start()
    {
        _player = GameObject.FindWithTag("Player").GetComponent<CPlayer>();
        _attack.Attacking = false;
        _attack.WaitAfter = new WaitForSeconds(_attack.DelayAfter);
        _attack.WaitBefore = new WaitForSeconds(_attack.DelayBefore);
        _hP = _maxHP;
    }

    public override void Hit(float dmg)
    {
        _hP -= dmg;

        if (_hP < 0)
        {
            Die();
            return;
        }

        if (!_hPBar)
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

        if (_player is CKnight)
        {
            ((CKnight)_player).GainFury(5f);
        }
    }

    public override void HitTarget(in ICollisionObject target)
    {
        if (target == (ICollisionObject)_player)
        {
            _player.SetHitDirection((transform.position - _player.transform.position).normalized);
        }
    }

    protected override void Die()
    {
        if (GetComponentInChildren<CAttack>())
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
}
