using System;
using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

public class MoneyBooster : MonoBehaviour
{
    [FoldoutGroup("Бустер прибыли")] [SerializeField]
    private int _boosterMultiplier = 2;

    [FoldoutGroup("Бустер прибыли")] [SerializeField]
    private float _boosterDuration = 60f;

    [FoldoutGroup("Бустер прибыли")] [SerializeField]
    private GameObject _boosterInfoGameObject;

    [FoldoutGroup("Бустер прибыли")] [SerializeField]
    private Slider _boosterSlider;

    private BalanceManager _balanceManager;

    private void OnEnable()
    {
        AdsController.OnRewardedBoosterAdWatched += ActivateBooster;
    }

    private void OnDisable()
    {
        AdsController.OnRewardedBoosterAdWatched -= ActivateBooster;
    }

    private void Start()
    {
        _boosterSlider.maxValue = _boosterDuration;
        _balanceManager = BalanceManager.Instance;
    }

    private void ActivateBooster()
    {
        _balanceManager.boosterValue = (ulong)_boosterMultiplier;
        StartCoroutine(BoosterRoutine());
    }

    private IEnumerator BoosterRoutine()
    {
        float timer = _boosterDuration;
        _boosterInfoGameObject.SetActive(true);
        _boosterSlider.value = timer;
        do
        {
            yield return new WaitForEndOfFrame();
            timer -= Time.deltaTime;
            _boosterSlider.value = _boosterDuration - timer;
        } while (timer > 0f);

        _boosterInfoGameObject.SetActive(false);

        _balanceManager.boosterValue = 1;
    }
}