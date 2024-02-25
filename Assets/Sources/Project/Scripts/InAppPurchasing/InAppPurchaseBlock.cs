using System;
using System.Text.RegularExpressions;
using Deslab.Deslytics;
using Deslab.InApp;
using Deslab.RemoteConfig;
using Deslab.Scripts.Deslytics.Ads;
using Deslab.Utils;
using Mamboo.Internal.Purchase;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Purchasing;
using ProductType = Deslab.InApp.ProductType;

public class InAppPurchaseBlock : MonoBehaviour
{
    public ProductType productType;

    [SerializeField] private bool isOffer = false;
    [EnableIf("isOffer")] public OfferType offerType;

    [SerializeField] private Button purchaseButton;
    [SerializeField] private ReducedBigText rewardText;
    [SerializeField] private TMP_Text priceText;
    [SerializeField] private float valueKoef;
    [SerializeField] private TMP_Text valueText;
    [SerializeField] [Range(0f, 1f)] private float addedValueKoef;

    [SerializeField] private bool useRelativeMultiplier = false;

    private InAppPurchasesManager _manager;
    private ulong _rewardAmount;
    private Product _product;
    private float _relativeMultiplier;

    public string ProductName; // { get; private set; }
    //public IAPProduct Product { get { return _product; } }

    private void Awake()
    {
        PurchaseController.InitializationCompleted += Init;
    }

    // Start is called before the first frame update
    void Start()
    {
        BalanceManager.OnAvProfitPerMinuteChanged += OnAvMoneyPerMinuteChanged;
        if (valueKoef < 1f) valueKoef = 1f;
        _manager = InAppPurchasesManager.Instance;
        //Invoke(nameof(Init), 0.05f);
        purchaseButton.onClick.AddListener(Purchase);
        SetValueText();

        if (productType == ProductType.NoAds)
            MaxAdsManager.instance.OnRemoveAdsStateChanged += OnAdsRemove;

        if (productType == ProductType.NoInterstitialAds)
            AdsManager.Instance.OnRemoveInterStateChanged += OnAdsRemove;
    }


    private void OnAdsRemove()
    {
        //Debug.LogError("Ads removed : " + MaxAdsManager.instance.IsAdsRemoved);
        //MaxAdsManager.instance.RemoveAds();

        if (MaxAdsManager.instance.IsAdsRemoved &&
            (productType == ProductType.NoAds ||
             productType == ProductType.NoAdsWithBonus))
            gameObject.SetActive(false);

        if ((MaxAdsManager.instance.IsAdsRemoved || AdsManager.Instance.interAdsRemoved) &&
            productType == ProductType.NoInterstitialAds)
            gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        PurchaseController.PurchaseCompleted += PurchaseCompletedHandler;
    }

    private void OnDisable()
    {
        PurchaseController.PurchaseCompleted -= PurchaseCompletedHandler;
        BalanceManager.OnAvProfitPerMinuteChanged -= OnAvMoneyPerMinuteChanged;
    }

    private void Init()
    {
        //Debug.LogError("Init for : " + gameObject.name);
        _product = PurchaseController.instance.storeController.products.all[(int) productType];
        ProductName = _product.definition.id;
        string icoCode = _product.metadata.isoCurrencyCode;
        icoCode = ClearString(icoCode);
        if (priceText)
            priceText.text = _product.metadata.localizedPrice + " " + icoCode;
        OnAdsRemove();

        string ClearString(string s)
        {
            return Regex.Replace(s, @"[^\w\.-]", "", RegexOptions.None, TimeSpan.FromSeconds(1.5));
        }

        if (useRelativeMultiplier)
            Invoke(nameof(SetRelativeMultiplier), 0.6f);
    }

    private void Purchase()
    {
        switch (productType)
        {
            case ProductType.NoAds:
                PurchaseController.instance.RemoveAds();
                break;
            case ProductType.NoInterstitialAds:
                PurchaseController.instance.RemoveInterAds();
                break;
            default:
                _manager.Purchase(ProductName);
                break;
        }
    }

    private void PurchaseCompletedHandler(PurchaseEventArgs eProduct)
    {
        Debug.Log("PurchaseCompletedHandler eProduct : + " + eProduct.purchasedProduct.definition.id +
                  " || ProductName : " + ProductName + " || ProductType : " + productType);
        if (eProduct.purchasedProduct.definition.id == ProductName)
        {
            switch (productType)
            {
                case ProductType.NoAds:
                    //MaxAdsManager.instance.RemoveAds();
                    AdsManager.AdsRemoved();
                    _manager.OnRewarded?.Invoke(0, true); // if buy no ads, show no ads info
                    break;
                case ProductType.NoAdsWithBonus:
                    _manager.OnRewarded?.Invoke(0, true);
                    AdsManager.AdsRemoved();
                    RunnersManager.Instance.SpawnNewSupermans();
                    break;
                case ProductType.NoInterstitialAds:
                    AdsManager.InterAdsRemoved();
                    _manager.OnRewarded?.Invoke(0, true);
                    break;
                default:
                    BalanceManager.Instance.GainMoney(_rewardAmount);
                    _manager.OnRewarded?.Invoke(_rewardAmount, false);
                    break;
            }

            VibrationExtention.SelectionVibrate();

            if (isOffer)
                switch (offerType)
                {
                    case OfferType.Shop:
                        DeslyticsManager.BuyOfferInShop(productType.ToString());
                        break;
                    case OfferType.BeforeAds:
                        DeslyticsManager.BuyOfferBeforeAds(productType.ToString());
                        break;
                    case OfferType.AfterAds:
                        DeslyticsManager.BuyOfferAfterAds(productType.ToString());
                        break;
                    case OfferType.OfflineReward:
                        DeslyticsManager.BuyOfferOfflineReward(productType.ToString());
                        break;
                    default:
                        DeslyticsManager.BuyOfferInShop(productType.ToString());
                        break;
                }

            AudioManager.Instance.PlayOneShot("In app");
        }
        else
        {
            Debug.Log("PurchaseCompletedHandler eProduct : + " + eProduct.purchasedProduct.definition.id + " != " +
                      ProductName);
        }
    }

    private void OnAvMoneyPerMinuteChanged(ulong value)
    {
        _rewardAmount = (ulong) (value * _manager.RewardMultiplier * _relativeMultiplier * valueKoef);
        _rewardAmount += (ulong) (_rewardAmount * addedValueKoef);
        if (rewardText)
            rewardText.SetValue(_rewardAmount);
    }

    private void SetRelativeMultiplier()
    {
        _relativeMultiplier = (float) (_product.metadata.localizedPrice /
                                       _manager.DefaultPurchaseBlock._product.metadata.localizedPrice);
        OnAvMoneyPerMinuteChanged(BalanceManager.Instance.AverageProfitPerMinute);
    }

    private void SetValueText()
    {
        if (!valueText) return;

        if (valueKoef == Mathf.Floor(valueKoef))
            valueText.text = valueKoef.ToString() + "x Value";
        else
            valueText.text = valueKoef.ToString("F1") + "x Value";
    }
}