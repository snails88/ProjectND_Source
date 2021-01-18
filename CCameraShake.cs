using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CCameraShake : MonoBehaviour
{
    [SerializeField] private float _shakeTime;
    private WaitForSeconds _waitShake;
    private Transform _playerTransform;
    private Vector3 _dir;
    private Vector3 _initPos;
    private float _force;
    private bool _shake = false;

    private void Awake()
    {
        _waitShake = new WaitForSeconds(_shakeTime);
    }

    private void Start()
    {
        _playerTransform = GameObject.FindWithTag("Player").transform;
    }

    private void Update()
    {
        transform.position = _playerTransform.position;
        transform.Translate(Vector3.forward * -10f);
    }

    public void SetShakeTime(float shakeTime)
    {
        if(_shakeTime != shakeTime)
        {
            _shakeTime = shakeTime;
            _waitShake = new WaitForSeconds(_shakeTime);
        }
    }

    public void StartCameraShake(Vector3 dir, float force)
    {
        if (_shake)
            StopCoroutine("CoroutineShaking");
        StartCoroutine(CoroutineShake());
        _force = force;
        _dir = dir;
    }

    IEnumerator CoroutineShake()
    {
        _shake = true;
        StartCoroutine("CoroutineShaking");
        yield return _waitShake;
        _shake = false;
    }

    IEnumerator CoroutineShaking()
    {
        while (_shake)
        {
            transform.position += _dir * _force;
            _dir *= -1;
            _force -= _force * Time.deltaTime / _shakeTime;
            if (_force < 0f)
                _force = 0f;
            yield return null;
        }
        yield return null;
    }
}
