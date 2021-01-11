using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CHPBar : MonoBehaviour
{
    private Vector3 _ownerPos;
    private float _addYValue = 0f;
    public float AddYValue
    {
        set { _addYValue = value; }
    }
    private Image _image;
    
    
    //RectTransform _rt;
    public GameObject Owner
    {
        get;
        set;
    }

    private void Awake()
    {
        //_rt = GetComponent<RectTransform>();
        _image = transform.Find("ProgressBar").GetComponent<Image>();
    }

    private void OnEnable()
    {
        _image.fillAmount = 1f;
    }

    private void Update()
    {
        if (Owner)
        {
            _ownerPos = Owner.transform.position;
            _ownerPos.y += _addYValue;
            transform.position = Camera.main.WorldToScreenPoint(_ownerPos);
        }
    }

    public void SetFillAmount(float amount)
    {
        _image.fillAmount = amount;
    }
}
