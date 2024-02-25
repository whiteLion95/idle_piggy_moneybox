using System;
using UnityEngine;

public class MaxImpressionHandler : MonoBehaviour
{
    private static MaxImpressionHandler instance;

    private void Start()
    {
        if (instance == null) 
            instance = this;

        var dubl = FindObjectsOfType<MaxImpressionHandler>();
        if (dubl.Length > 1)
        {
            Destroy(this.gameObject);
        }

        DontDestroyOnLoad(gameObject);

#if mamboo_max_mediation

        MaxSdkCallbacks.OnSdkInitializedEvent += (MaxSdkBase.SdkConfiguration sdkConfiguration) => {

#if UNITY_IOS
            InitializeRevenueCallbacks();
#endif
        };

#endif
    }

    private void Awake()
    {
        if (instance == null)
            instance = FindObjectOfType<MaxImpressionHandler>();
    }

#if mamboo_max_mediation

#if UNITY_IOS


    public void InitializeRevenueCallbacks()
    {
        MaxSdkCallbacks.OnRewardedAdReceivedRewardEvent += MaxSdkCallbacks_OnRewardedAdReceivedRewardEvent; ;
        MaxSdkCallbacks.OnInterstitialDisplayedEvent += OnInterstitialDisplayedEvent;
        MaxSdkCallbacks.OnBannerAdLoadedEvent += (a) => HandleAdLoaded();
    }

    private void MaxSdkCallbacks_OnRewardedAdReceivedRewardEvent(string adUnitId, MaxSdkBase.Reward arg2)
    {
        Debug.Log("OnRewardedAdDisplayedEvent: " + adUnitId);
        HandleAdImpression(adUnitId, AdjustConversions.REWARD_VIDEO_FORMAT);
    }

    public void HandleAdLoaded()
    {
        HandleAdImpression(MaxAdsManager.bannerAdUnit, AdjustConversions.BANNER_FORMAT);
    }

    private void OnInterstitialDisplayedEvent(string adUnitId)
    {
        Debug.Log("OnInterstitialDisplayedEvent: " + adUnitId);
        HandleAdImpression(adUnitId, AdjustConversions.INTERSTITIAL_FORMAT);
    }

    private void HandleAdImpression(string adUnitId, string adFormat)
    {
        if (adUnitId == null)
            return;

        try
        {
            var adInfo = MaxSdk.GetAdInfo(adUnitId);
            if (adInfo == null)
            {
                Debug.Log($"MAXSdk ad info is null for ad unit id: {adUnitId}, adFormat: {adFormat}");
                return;
            }

            var revenue = adInfo.Revenue;
            Debug.Log($"MAXRevenue: {revenue}");

            var countryCode = MaxSdk.GetSdkConfiguration().CountryCode;
            Debug.Log($"MAXCountry: {countryCode}");

            decimal value = revenue > 0 ? Convert.ToDecimal(revenue) : TryPredictRevenue(adFormat, countryCode);
            AdjustConversions.SaveRevenue(adFormat, value);

            AdjustConversions.IncValue();
        }
        catch (Exception ex)
        {
            Debug.Log($"{nameof(HandleAdImpression)} error: {ex}");
        }
    }

    private decimal TryPredictRevenue(string ad_format, string country)
    {
        Debug.Log("TryPredictRevenue: " + ad_format);
        if (ad_format == null)
        {
            Debug.Log("TryPredictRevenue: AdUnitFormat is null");
            return 0;
        }

        try
        {
            decimal revenue = AdjustConversions.GetAverageRevenueByAdFormat(ad_format);
            Debug.Log("TryPredictRevenue:Average:" + revenue);
            if (revenue > 0)
                return revenue;

            revenue = AdjustConversions.GetRevenueByCountry(ad_format);
            Debug.Log("TryPredictRevenue:ByCountry:" + revenue);
            if (revenue > 0)
                return revenue;

            return 0;
        }
        catch (Exception ex)
        {
            Debug.Log($"{nameof(TryPredictRevenue)} error: {ex}");
        }

        return 0;
    }

#endif
#endif
}