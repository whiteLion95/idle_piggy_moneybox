using DG.Tweening;
using Sirenix.OdinInspector;
using System;
using UnityEngine;
using UnityEngine.UI;

public class SliderBar : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private bool _speedBased;
    [ShowIf("_speedBased")][SerializeField][Range(0f, 5f)] private float _speedPct = 1f;
    [SerializeField] private float _changeSmoothness;
    [SerializeField] private bool _lookAtCamera;
    [ShowIf("_lookAtCamera")][SerializeField] private bool _updateLook;

    public Action OnZero = delegate { };

    private Camera _mainCam;

    private void Awake()
    {
        slider = GetComponent<Slider>();

        if (_lookAtCamera)
        {
            _mainCam = Camera.main;
            LookAtCamera();
        }
    }

    private void Update()
    {
        if (_updateLook)
        {
            LookAtCamera();
        }
    }

    public void SetMaxValue(float value)
    {
        if (slider != null)
        {
            slider.maxValue = value;
            SetValue(value);
        }
    }

    public void SetValue(float value)
    {
        slider.value = value;
    }

    public void ChangeValue(float value, Action onComplete = null)
    {
        if (_speedBased)
            _changeSmoothness = _speedPct * slider.maxValue;

        slider.DOValue(value, _changeSmoothness).SetUpdate(true).SetSpeedBased(_speedBased).onComplete +=
            () =>
            {
                if (value <= 0) OnZero?.Invoke();
                onComplete?.Invoke();
            };
    }

    private void LookAtCamera()
    {
        transform.parent.LookAt(transform.position + _mainCam.transform.rotation * Vector3.back);
    }
}
