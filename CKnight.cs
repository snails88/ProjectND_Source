using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CKnight : CPlayer
{
    private Transform _graphicTransform;

    protected override void Awake()
    {
        base.Awake();
        _graphicTransform = transform.Find("Graphic");
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
            _graphicTransform.Rotate(Vector3.forward, 360f / _evasion.EvasionTime * Time.deltaTime);
            yield return null;
        }
        _graphicTransform.rotation = Quaternion.Euler(Vector3.zero);
    }
}
