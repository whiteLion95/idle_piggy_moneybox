using DG.Tweening;
using TMPro;
using UnityEngine;

public class MoneyValue : ChangingValueText
{
    private void Start()
    {
        InitValue();
    }

    protected override void InitChangingValue()
    {
        changingValue = BalanceManager.Instance.Money;
    }

    protected override void SubscribeToEvents()
    {
        BalanceManager.OnMoneyChanged += OnMoneyChangedHandler;
    }

    protected override void UnSubscribeFromEvents()
    {
        BalanceManager.OnMoneyChanged -= OnMoneyChangedHandler;
    }

    private void OnMoneyChangedHandler(float moneyValue)
    {
        ChangeValueText(moneyValue); //, () => {Debug.LogError(moneyValue); }
    }
}
