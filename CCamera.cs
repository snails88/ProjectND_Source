using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CCamera : MonoBehaviour
{
    [SerializeField] private float _shakeTime;
    [SerializeField] private float _limitPlayerToCameraDistance;
    [SerializeField] private float _scrollSpeed;
    private WaitForSeconds _waitShake;
    private Transform _playerTransform;
    private Vector3 _shakeDir;
    private Vector3 _mousePos;
    private Vector3 _conditionalPositionVector;
    private Vector3 _playerToCameraDir;
    private Vector3 _zeroToMouseDir;
    private Vector3 _beforeShakePosition;
    private float _playerToMouseDistance;
    private float _shakeForce;
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
        Move();
    }

    private void Move()
    {
        _mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        _mousePos.z = _playerTransform.position.z;
        _conditionalPositionVector = _shake ? _beforeShakePosition : transform.position;
        _conditionalPositionVector.z = _playerTransform.position.z;
        _zeroToMouseDir = _mousePos - _conditionalPositionVector;
        _playerToMouseDistance = (_playerTransform.position - _mousePos).magnitude;
        if (_limitPlayerToCameraDistance < _playerToMouseDistance)
        {
            _conditionalPositionVector += _zeroToMouseDir.normalized * Time.deltaTime * _scrollSpeed;
            _playerToCameraDir = _conditionalPositionVector - _playerTransform.position;
            if (_playerToCameraDir.magnitude > _limitPlayerToCameraDistance)
            {
                _conditionalPositionVector = _playerTransform.position + _playerToCameraDir.normalized * _limitPlayerToCameraDistance;
            }
        }
        else
        {
            if ((_playerTransform.position - _conditionalPositionVector).magnitude > 0.1f)
                _conditionalPositionVector += -_playerToCameraDir.normalized * Time.deltaTime * _scrollSpeed;
        }
        _conditionalPositionVector.z = -10f;
        transform.position = _conditionalPositionVector;
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
        _shakeForce = force;
        _shakeDir = dir;
    }

    IEnumerator CoroutineShake()
    {
        _shake = true;
        _beforeShakePosition = transform.position;
        StartCoroutine("CoroutineShaking");
        yield return _waitShake;
        _shake = false;
    }

    IEnumerator CoroutineShaking()
    {
        while (_shake)
        {
            _beforeShakePosition = transform.position;
            transform.position += _shakeDir * _shakeForce;
            _shakeDir *= -1;
            _shakeForce -= _shakeForce * Time.deltaTime / _shakeTime;
            if (_shakeForce < 0f)
                _shakeForce = 0f;
            yield return null;
        }
        yield return null;
    }
}
