using System.Collections;
using UnityEngine;
using System;
using Mamboo.Internal.Purchase;

public class InAppPurchasesManager : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Умножается на доход в минуту для определения награды")]
    private float rewardMultiplier;
    [SerializeField] private InAppPurchaseBlock defaultPurchaseBlock;

    private bool _isInitialized;
    //private IAPProduct[] _products;

    public Action OnInitialized;
    public Action<ulong, bool> OnRewarded;

    public static InAppPurchasesManager Instance { get; private set; }
    public float RewardMultiplier { get { return rewardMultiplier; } }
    public InAppPurchaseBlock DefaultPurchaseBlock { get { return defaultPurchaseBlock; } }

    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(CheckInitialized());
    }

    private void OnEnable()
    {
        OnInitialized += Init;
        //InAppPurchasing.PurchaseFailed += PurchaseFailedHandler;
    }

    private void OnDisable()
    {
        //InAppPurchasing.PurchaseFailed -= PurchaseFailedHandler;
    }

    private int _initCount;
    private IEnumerator CheckInitialized()
    {
        _initCount = 0;

        while (!_isInitialized && _initCount < 20)
        {
            _initCount++;
            //_isInitialized = InAppPurchasing.IsInitialized();
            yield return new WaitForSeconds(0.1f);
        }

        if (_isInitialized)
            OnInitialized?.Invoke();
        else
            StartCoroutine(CheckInitialized());
    }

    private void Init()
    {
        //_products = InAppPurchasing.GetAllIAPProducts();
    }

    public void Purchase(string productName)
    {
        PurchaseController.instance.YourProductBuy(productName);
    }

    // private void PurchaseFailedHandler(IAPProduct product, string failureReason)
    // {
    //     Debug.LogWarning("The purchase of product " + product.Name + " has failed with reason: " + failureReason);
    // }
}

namespace Deslab.InApp
{
    public enum ProductType
    {
        NoAdsWithBonus,
        NoAds,
        CoinPack1,
        CoinPack2,
        CoinPack3,
        SpecialOffer,
        NoInterstitialAds
    }

    public enum OfferType
    {
        Shop,
        BeforeAds,
        AfterAds,
        OfflineReward
    }
}
