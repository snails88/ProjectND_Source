using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CHitStop : MonoBehaviour
{
    [SerializeField] private float _hitStopTime;
    private WaitForSecondsRealtime _waitHitStop;

    private void Awake()
    {
        _waitHitStop = new WaitForSecondsRealtime(_hitStopTime);
    }

    public IEnumerator CoroutineHitStop()
    {
        CGameManager._instance.SetTimeScale(0f);
        yield return _waitHitStop;
        CGameManager._instance.SetTimeScale(1f);
    }
}
