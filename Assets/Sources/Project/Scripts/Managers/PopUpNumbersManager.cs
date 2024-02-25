using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EZ_Pooling;
using DG.Tweening;

public class PopUpNumbersManager : MonoBehaviour
{
    [SerializeField] private PopUpNumber popUpNumberPrefab;
    [SerializeField] private PopUpNumbersData data;

    private float _yLevel;
    private UpgradesManager _upManager;
    private BalanceManager _balanceManager;
    private AdsController _adsController;

    private void Start()
    {
        _yLevel = MoneyJar.Instance.CoinsHole.position.y + data.YOffset;
        _upManager = UpgradesManager.Instance;
        _balanceManager = BalanceManager.Instance;
        _adsController = AdsController.Instance;
    }

    private void OnEnable()
    {
        CoinsDespawnPlace.OnCoinTriggered += OnCoinGainedHandler;
        StaticCoinThrower.OnLastCoinGained += OnLastCoinGainedHandler;
        CoinsManager.OnLastAdsCoinGained += OnLastCoinGainedHandler;
    }

    private void OnDisable()
    {
        CoinsDespawnPlace.OnCoinTriggered -= OnCoinGainedHandler;
        StaticCoinThrower.OnLastCoinGained -= OnLastCoinGainedHandler;
        CoinsManager.OnLastAdsCoinGained -= OnLastCoinGainedHandler;
    }

    private void OnCoinGainedHandler(Coin coin)
    {
        if (coin.Source != CoinSource.FROG && coin.Source != CoinSource.HAND && coin.Source != CoinSource.ADS)
        {
            SpawnPopupNumber(coin);
        }
    }

    private void OnLastCoinGainedHandler(Coin coin)
    {
        SpawnPopupNumber(coin);
    }

    private void SpawnPopupNumber(Coin coin)
    {
        Vector3 spawnPos = GetRandomPosition();
        Transform numTrans = EZ_PoolManager.Spawn(popUpNumberPrefab.transform, spawnPos, Quaternion.identity);
        PopUpNumber num = numTrans.GetComponent<PopUpNumber>();
        NumberAttributes a = new NumberAttributes();

        switch (coin.Source)
        {
            case CoinSource.TAP:
                a = data.GetNumberAttributes(CoinSource.TAP);
                num.BigText.SetValue((int)(coin.Value * _balanceManager.boosterValue));
                break;
            case CoinSource.RUNNER:
                a = data.GetNumberAttributes(CoinSource.RUNNER);
                num.BigText.SetValue(coin.Value * _balanceManager.boosterValue, true);
                break;
            case CoinSource.HAND:
                a = data.GetNumberAttributes(CoinSource.HAND);
                num.BigText.SetValue(Hand.Instance.MoneyPerThrow);
                break;
            case CoinSource.FROG:
                a = data.GetNumberAttributes(CoinSource.FROG);
                num.BigText.SetValue(Frog.Instance.MoneyPerThrow);
                break;
            case CoinSource.ADS:
                a = data.GetNumberAttributes(CoinSource.ADS);
                num.BigText.SetValue(_adsController.RewardAmount);
                break;
        }

        num.Text.fontSize = a.fontSize;
        num.Text.color = a.color;

        StartCoroutine(TweenNumber(num, coin.Source));
    }

    private Vector3 GetRandomPosition()
    {
        float randX = Random.Range(data.XMin, data.XMax);
        float randZ = Random.Range(data.ZMin, data.ZMax);
        Vector3 randPos = new Vector3(randX, _yLevel, randZ);
        return randPos;
    }

    private IEnumerator TweenNumber(PopUpNumber number, CoinSource source)
    {
        if (source == CoinSource.RUNNER || source == CoinSource.TAP)
        {
            number.transform.DOMoveY(data.DeltaY, data.TweenDuration).SetRelative(true);
            number.Text.DOFade(0f, data.TweenDuration);
            yield return new WaitForSeconds(data.TweenDuration);
        }
        else
        {
            number.transform.position = new Vector3(number.transform.position.x, number.transform.position.y + 0.2f,
                number.transform.position.z);
            Vector3 origScale = number.transform.localScale;
            number.transform.localScale = Vector3.zero;
            number.transform.DOScale(origScale, 0.2f).onComplete +=
                () => number.transform.DOShakePosition(0.3f, 0.1f, 10, 90f);
            number.transform.DOMoveY(data.DeltaY, data.TweenDuration).SetRelative(true).SetDelay(0.5f);
            number.Text.DOFade(0f, data.TweenDuration).SetDelay(0.5f);
            yield return new WaitForSeconds(data.TweenDuration + 0.5f);
        }

        EZ_PoolManager.Despawn(number.transform);
    }
}