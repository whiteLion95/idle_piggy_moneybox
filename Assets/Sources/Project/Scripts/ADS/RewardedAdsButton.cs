using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RewardedAdsButton : RewardedAdsObject
{
    [SerializeField] private Slider _timeSlider;

    private Button _button;

    private void Awake()
    {
        _button = GetComponent<Button>();

        _button.onClick.AddListener(() => OnActivated?.Invoke());
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        InitSlider();
    }

    protected override void Update()
    {
        base.Update();
        UpdateSlider();
    }

    private void InitSlider()
    {
        _timeSlider.maxValue = _showingDuration;
        _timeSlider.value = _timeSlider.maxValue;
    }

    private void UpdateSlider()
    {
        _timeSlider.value -= Time.deltaTime;
    }
}
