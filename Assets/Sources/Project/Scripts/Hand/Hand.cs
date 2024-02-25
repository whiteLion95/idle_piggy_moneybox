using System;
using System.Collections.Generic;
using UnityEngine;

public class Hand : StaticCoinThrower
{
    [SerializeField] private uint baseMoneyPerThrow = 20000;

    public static System.Action OnThrow;
    public Action<ulong> OnMoneyPerThrowChanged;

    private const string FLIP_TRIGGER = "Flip";
    private const string SAVE_KEY = "Hand timer progress";

    public override CoinSource CoinSource => CoinSource.HAND;

    public static Hand Instance { get; private set; }

    protected override void Awake()
    {
        base.Awake();
        Instance = this;
    }

    protected override void Start()
    {
        base.Start();
        CalculateMoneyPerThrow();
        Invoke(nameof(SpawnCoins), 0.1f);
    }

    protected override void SetSaveKey()
    {
        _saveKey = SAVE_KEY;
    }

    public override void ThrowCoins()
    {
        base.ThrowCoins();
        _anim.SetTrigger(FLIP_TRIGGER);
        OnThrow?.Invoke();
    }

    protected override void CalculateMoneyPerThrow()
    {
        base.CalculateMoneyPerThrow();

        ulong prevMoneyPerThrow = MoneyPerThrow;
        ulong newMoneyPerThrow = baseMoneyPerThrow;

        float deltaMoneyPerThrow = 0;
        List<UpgradeData> handUpDatas = _upManager.GetUpgradeDatas(CoinSource.HAND);

        for (int i = 0; i < handUpDatas.Count; i++)
        {
            UpgradeType upgradeType = handUpDatas[i].UpgradeType;

            if (_upManager.GetCurrentLevel(upgradeType) > 0)
            {
                deltaMoneyPerThrow += _upManager.GetValue(upgradeType);
            }
        }

        newMoneyPerThrow += (ulong)deltaMoneyPerThrow;

        if (newMoneyPerThrow != prevMoneyPerThrow)
        {
            MoneyPerThrow = newMoneyPerThrow;
            OnMoneyPerThrowChanged?.Invoke(MoneyPerThrow);
        }
    }

    protected override void CalculateThrowRate()
    {
        _throwRate = baseThrowRate;
    }

    protected override void HandleUpgrade(UpgradeType upType, CoinSource coinSource)
    {
        base.HandleUpgrade(upType, coinSource);

        if (coinSource == CoinSource.HAND)
        {
            CalculateMoneyPerThrow();
            UpdateSpawnedCoins();
        }
    }

    protected override void HandleCountDownFinished()
    {
        base.HandleCountDownFinished();
        ThrowCoins();
    }
}
