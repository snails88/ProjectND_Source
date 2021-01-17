using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CKnight : CPlayer
{
    private Transform _graphicTransform;
    private bool _furyRelease = true;
    private WaitForSeconds _waitFuryRelease;
    [SerializeField]
    private float _furyReleaseTime;
    [SerializeField]
    private float _furyDecreaseAmount;

    protected override void Awake()
    {
        base.Awake();
        _graphicTransform = transform.Find("Graphic");
        _resourceColor = Color.red;
        _maxResource = 100f;
        _waitFuryRelease = new WaitForSeconds(_furyReleaseTime);
    }

    protected override void Update()
    {
        base.Update();
        if(_furyRelease && _resource > 0f)
        {
            _resource -= _furyDecreaseAmount * Time.deltaTime;
            if (_resource < 0f)
                _resource = 0f;
        }    
    }

    public void GainFury(float fury)
    {
        StopCoroutine("CoroutineFury");
        StartCoroutine("CoroutineFury", fury);
    }

    public override void Evasion(in Vector3 dir)
    {
        if (_evasion.Evasionable && !_evasion.Evasing && dir != Vector3.zero)
        {
            _evasion.EvasionDir = dir.normalized;
            StartCoroutine("CoroutineEvasion");
        }
    }

    private IEnumerator CoroutineEvasion()
    {
        _evasion.Evasionable = false;
        _evasion.Evasing = true;
        _attack.Attackable = false;
        StartCoroutine("CoroutineEvade");
        yield return _evasion.WaitEvasion;
        _evasion.Evasing = false;
        _attack.Attackable = true;
        yield return _evasion.WaitEvasionCD;
        _evasion.Evasionable = true;
    }

    private IEnumerator CoroutineEvade()
    {
        while (_evasion.Evasing)
        {
            transform.Translate(_evasion.EvasionDir * _evasion.EvasionSpeed * Time.deltaTime);
            if (_evasion.EvasionDir.x == 0f)
            {
                if (_spriteRenderer.flipX)
                    _graphicTransform.Rotate(Vector3.forward, 360f / _evasion.EvasionTime * Time.deltaTime);
                else
                    _graphicTransform.Rotate(Vector3.forward, -360f / _evasion.EvasionTime * Time.deltaTime);
            }
            else
            {
                if(_evasion.EvasionDir.x > 0)
                    _graphicTransform.Rotate(Vector3.forward, -360f / _evasion.EvasionTime * Time.deltaTime);
                else
                    _graphicTransform.Rotate(Vector3.forward, 360f / _evasion.EvasionTime * Time.deltaTime);
            }
            yield return null;
        }
        _graphicTransform.rotation = Quaternion.Euler(Vector3.zero);
    }

    private IEnumerator CoroutineFury(float fury)
    {
        _resource += fury;
        _furyRelease = false;
        yield return _waitFuryRelease;
        _furyRelease = true;
    }
}
