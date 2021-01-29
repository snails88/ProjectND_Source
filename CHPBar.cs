using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CHPBar : MonoBehaviour
{
    private Image _image;

    public float AddYValue { protected get; set; }
    public GameObject Owner { get; set; }

    private void Awake()
    {
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
            Vector3 ownerPos = Owner.transform.position;
            ownerPos.y += AddYValue;
            transform.position = Camera.main.WorldToScreenPoint(ownerPos);
        }
    }

    public void SetFillAmount(float amount)
    {
        _image.fillAmount = amount;
    }
}
