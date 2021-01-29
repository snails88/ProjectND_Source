using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CCamera : MonoBehaviour
{
    [SerializeField] private float _shakeTime;
    [SerializeField] private float _limitPlayerToCameraDistance;
    [SerializeField] private float _scrollSpeed;
    [SerializeField] private float _moveRatioPow;
    [SerializeField] private float _limitMoveRatio;
    private WaitForSeconds _waitShake;
    private Transform _playerTransform;
    private Vector3 _shakeDir;
    private Vector3 _beforeShakePosition;
    private float _shakeForce;
    private float _halfResolutionWidth;
    private bool _shake = false;

    private void Awake()
    {
        _waitShake = new WaitForSeconds(_shakeTime);
    }

    private void Start()
    {
        _playerTransform = GameObject.FindWithTag("Player").transform;
        _halfResolutionWidth = (float)Screen.width * 0.5f;
    }

    private void Update()
    {
        if (Time.timeScale != 0)
            Move();
    }

    private void Move()
    {
        Vector3 mousePos = Input.mousePosition;
        Vector3 playerScreenPosition = Camera.main.WorldToScreenPoint(_playerTransform.position);
        mousePos.z = playerScreenPosition.z;
        float moveRatio = (playerScreenPosition - mousePos).magnitude / (_halfResolutionWidth) * _moveRatioPow;
        mousePos = Camera.main.ScreenToWorldPoint(mousePos);
        Vector3 conditionalPositionVector = _shake ? _beforeShakePosition : transform.position;
        conditionalPositionVector.z = _playerTransform.position.z;
        Vector3 zeroToMouseDir = mousePos - conditionalPositionVector;

        if (moveRatio > _limitMoveRatio)
            moveRatio = _limitMoveRatio;

        conditionalPositionVector += zeroToMouseDir.normalized * Time.deltaTime * _scrollSpeed;
        Vector3 playerToCameraDir = conditionalPositionVector - _playerTransform.position;
        if (playerToCameraDir.magnitude / _limitPlayerToCameraDistance > moveRatio)
        {
            conditionalPositionVector = _playerTransform.position + playerToCameraDir.normalized * _limitPlayerToCameraDistance * moveRatio;
        }
        conditionalPositionVector.z = -10f;
        transform.position = conditionalPositionVector;
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
