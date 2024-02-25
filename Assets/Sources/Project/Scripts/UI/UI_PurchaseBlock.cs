using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_PurchaseBlock : PurchaseBlock
{
    [SerializeField] private GameObject[] relativeBlocks;
    [SerializeField] private Button button;
    [SerializeField] private UnlocksData data;
    [SerializeField] private GameObject dim;
    [SerializeField] private PurchaseType purchaseType;
    [SerializeField] private ReducedBigText moneyText;

    private BalanceManager _balanceManager;
    private UpgradesManager _uManager;
    private ulong _purchasePrice;

    public override bool EnoughMoney { get; protected set; }

    // Start is called before the first frame update
    void Start()
    {
        _balanceManager = BalanceManager.Instance;
        _uManager = UpgradesManager.Instance;

        SetPrice();
        CheckPurchase();

        button.onClick.AddListener(() =>
        {
            ActionsOnButtonClick();
        });
    }

    private void FixedUpdate()
    {
        CheckMoney();
    }

    private void ActionsOnButtonClick()
    {
        switch (purchaseType)
        {
            case PurchaseType.FROG:
                _balanceManager.SpendMoney(data.FrogUnlockPrice);
                break;
            case PurchaseType.HAND:
                _balanceManager.SpendMoney(data.HandUnlockPrice);
                break;
        }

        _uManager.Purchase(purchaseType);
        TurnOffPurchase();
    }

    protected override void CheckMoney()
    {
        if (_balanceManager.Money >= _purchasePrice && !EnoughMoney)
        {
            EnoughMoney = true;
            dim.SetActive(false);
            button.interactable = true;
        }
        else if (_balanceManager.Money < _purchasePrice && EnoughMoney)
        {
            EnoughMoney = false;
            dim.SetActive(true);
            button.interactable = false;
        }
    }

    private void SetPrice()
    {
        switch (purchaseType)
        {
            case PurchaseType.FROG:
                _purchasePrice = data.FrogUnlockPrice;
                break;
            case PurchaseType.HAND:
                _purchasePrice = data.HandUnlockPrice;
                break;
        }

        moneyText.SetValue(_purchasePrice);
    }

    private void CheckPurchase()
    {
        bool purchased = false;

        switch (purchaseType)
        {
            case PurchaseType.FROG:
                purchased = _uManager.FrogPurchased;
                break;
            case PurchaseType.HAND:
                purchased = _uManager.HandPurchased;
                break;
        }

        if (purchased)
        {
            TurnOffPurchase();
        }
    }

    private void TurnOffPurchase()
    {
        gameObject.SetActive(false);
        button.interactable = false;
        TurnOnRelativeBlocks();
        EnoughMoney = false;
    }

    private void TurnOnRelativeBlocks()
    {
        foreach (var block in relativeBlocks)
        {
            block.SetActive(true);
        }
    }
}
