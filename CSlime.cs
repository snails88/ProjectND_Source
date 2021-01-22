using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Constants;

public class CSlime : CEnemy
{
    protected override void Start()
    {
        base.Start();
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
                    attack.SetOwner((CCreature)this);
                }
                else
                {
                    attack = CGameManager._instance.PopAttackObjectByPool().GetComponent<CAttack>();
                    attack.gameObject.SetActive(true);
                    attack.SetAttack(PROTOTYPE_ATTACK.SLIME, 1f, (CCreature)this);
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
