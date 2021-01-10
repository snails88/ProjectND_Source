using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Constants;

public class CSlime : MonoBehaviour, ICollisionObject
{
    private CPlayer _player;

    [SerializeField]
    private float _moveSpeed;

    [System.Serializable]
    private struct AttackVars
    {
        public float DelayBefore;
        public float DelayAfter;
        public float AttackDistance;
        public float TargetDistance;
        public bool Attacking;

        public WaitForSeconds WaitAfter;
        public WaitForSeconds WaitBefore;

        [HideInInspector]
        public Vector3 AttackDir;
        [HideInInspector]
        public Vector2 AttackPos;
        [HideInInspector]
        public float AttackAngle;
    }

    [SerializeField]
    private AttackVars _attack;

    private Vector3 _moveDir;


    void Start()
    {
        _player = GameObject.Find("Player").GetComponent<CPlayer>();
        _attack.Attacking = false;
        _attack.WaitAfter = new WaitForSeconds(_attack.DelayAfter);
        _attack.WaitBefore = new WaitForSeconds(_attack.DelayBefore);

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

    public void Hit(float dmg)
    {
        print(gameObject.name + " : " + dmg + "데미지 입음");
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

                GameObject attack;
                if (CGameManager._instance.AttackObjectPoolIsEmpty())
                {
                    attack = Instantiate(CGameManager._instance.GetAttackObjectPrototype(PROTOTYPE_ATTACK.SLIME), _attack.AttackPos, Quaternion.AngleAxis(_attack.AttackAngle, Vector3.forward));
                    attack.GetComponent<CAttack>().Damage = 1f;
                }
                else
                {
                    attack = CGameManager._instance.PopAttackObjectByPool();
                    attack.SetActive(true);
                    attack.GetComponent<CAttack>().SetAttack(PROTOTYPE_ATTACK.SLIME, 1f);
                    attack.transform.rotation = Quaternion.AngleAxis(_attack.AttackAngle, Vector3.forward);
                    attack.transform.position = _attack.AttackPos;
                }
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
