#if mamboo_max_mediation
using System;
using Deslab.Deslytics;
using Deslab.Scripts.Deslytics.Ads;
using GameAnalyticsSDK;
using UnityEngine;

[RequireComponent(typeof(InternetChecker))]
[RequireComponent(typeof(DisconnectionAdRequester))]
public class MaxAdsManager : MonoBehaviour,IMediationAdManager
{
    public bool IsInternetAvailable
    {
        get =>internetChecker.IsInternetAvilable;
        set => IsInternetAvailable = value;
    }

    public bool IsAdWasRequestedAfterDisconnect { get; set; } = true;
    
    private InternetChecker internetChecker;

    private bool isConnectAfterDisconnect = false;

    
    private const string ADS_REMOVED = "ads_removed";

#if UNITY_IOS
    public const string bannerAdUnit = "your iOS banner unit id";
    public const string interstitialAdUnit = "your iOS interstitial unit id";
    public const string rewardedAdUnit = "your iOS rewarded unit id";
#elif UNITY_ANDROID || UNITY_EDITOR
    public const string bannerAdUnit = "1f10831a4a840660";
    public const string interstitialAdUnit = "dfd5d4b51a768acf";
    public const string rewardedAdUnit = "42b4891b535865a1";
#endif
    private const string SdkKey = "XWF_ioNmvn_9wO1ib062hHOl-uNQvtsmhmPMtliaMET6DvWoQq94rlo6a37DFrubuC2KfIKlW_bO4NW1hmqGnZ";

    [HideInInspector] public bool AdsInit { get; private set; }
    
    private Action rewardDelegate;

    bool ADSRemovedGet()
    {
        if (!PlayerPrefs.HasKey(ADS_REMOVED))
        {
            ADSRemovedSet(false);
            Debug.Log("ADS_REMOVED :: NOT FOUND. SET IT FALSE");
        }

        return PlayerPrefs.GetInt(ADS_REMOVED) == 1;
    }

    void ADSRemovedSet(bool value)
    {
        Debug.LogError("ADS_REMOVED :: SET VALUE :: " + value);
        PlayerPrefs.SetInt(ADS_REMOVED, value ? 1 : 0);
        PlayerPrefs.Save();
    }

    // TODO ADS: subscribe to this to hide banner for remove ads, to hide remove ads button etc.
    public Action OnRemoveAdsStateChanged;
    public bool InterIsDisabled { get; internal set; } = false;
    public bool IsAdsRemoved
    {
        get => ADSRemovedGet();
        set
        {
            ADSRemovedSet(value);
            OnRemoveAdsStateChanged?.Invoke();
            Debug.Log("ADS_REMOVED :: ADS REMOVED SET :: " + value);
        }
    }

    public void DestroyBanner(){}

    public static MaxAdsManager instance;

    private void Start()
    {
        if (instance == null)
            instance = this;

        var dubl = FindObjectsOfType<MaxAdsManager>();
        if (dubl.Length > 1)
        {
            Destroy(this.gameObject);
        }

        // Applovin MAX
        MaxSdkCallbacks.OnSdkInitializedEvent += (MaxSdkBase.SdkConfiguration sdkConfiguration) => {

            instance.AdsInit = true;

            // AppLovin SDK is initialized, start loading ads
            InitializeBannerAds();
            InitializeInterstitialAds();
            InitializeRewardedAds();
            if (AdsManager.Instance.debugMax)
                MaxSdk.ShowMediationDebugger();
#if UNITY_IOS
            SetFacebookAdTracking();
#endif
            
        };

        MaxSdk.SetSdkKey(SdkKey);
        MaxSdk.InitializeSdk();

        DontDestroyOnLoad(gameObject);
    }

    private void Awake()
    {
        if (instance == null)
            instance = FindObjectOfType<MaxAdsManager>();
    }

#region ApplovinMax

    int interRetryAttempt;

    public void InitializeInterstitialAds()
    {
        // Attach callback
        MaxSdkCallbacks.OnInterstitialLoadedEvent += OnInterstitialLoadedEvent;
        MaxSdkCallbacks.OnInterstitialLoadFailedEvent += OnInterstitialFailedEvent;
        MaxSdkCallbacks.OnInterstitialAdFailedToDisplayEvent += InterstitialFailedToDisplayEvent;
        MaxSdkCallbacks.OnInterstitialHiddenEvent += OnInterstitialDismissedEvent;

        // Load the first interstitial
        LoadInterstitial();
    }

    private void LoadInterstitial()
    {
        MaxSdk.LoadInterstitial(interstitialAdUnit);
    }

    private void OnInterstitialLoadedEvent(string adUnitId)
    {
        // Interstitial ad is ready to be shown. MaxSdk.IsInterstitialReady(adUnitId) will now return 'true'

        // Reset retry attempt
        interRetryAttempt = 0;
    }

    private void OnInterstitialFailedEvent(string adUnitId, int errorCode)
    {
        // Interstitial ad failed to load 
        // We recommend retrying with exponentially higher delays up to a maximum delay (in this case 64 seconds)

        interRetryAttempt++;
        double retryDelay = Math.Pow(2, Math.Min(6, interRetryAttempt));

        Invoke(nameof(LoadInterstitial), (float)retryDelay);
    }

    private void InterstitialFailedToDisplayEvent(string adUnitId, int errorCode)
    {
        // Interstitial ad failed to display. We recommend loading the next ad
        LoadInterstitial();
    }

    private void OnInterstitialDismissedEvent(string adUnitId)
    {
        // Interstitial ad is hidden. Pre-load the next ad
        LoadInterstitial();
    }

    public void ShowInter(string placement)
    {
        if (!AdsInit || IsAdsRemoved) return;

        if (MaxSdk.IsInterstitialReady(interstitialAdUnit))
        {
            MaxSdk.ShowInterstitial(interstitialAdUnit);
            GameAnalytics.NewAdEvent(GAAdAction.Show, GAAdType.Interstitial, "MAX", placement);
            DeslyticsManager.AdsShow("InterstitialVideo", placement, "true");
            DeslyticsManager.InterstitialShow();
        }
    }

    private void ShowInter(float pause,string placement)
    {
        if (!AdsInit || IsAdsRemoved) return;

        if (MaxSdk.IsInterstitialReady(interstitialAdUnit))
        {
            MaxSdk.ShowInterstitial(interstitialAdUnit);
            GameAnalytics.NewAdEvent(GAAdAction.Show, GAAdType.Interstitial, "MAX", placement);
        }
    }

    /// Rewards

    int rewardRetryAttempt = 0;

    public void InitializeRewardedAds()
    {
        // Attach callback
        MaxSdkCallbacks.OnRewardedAdLoadedEvent += OnRewardedAdLoadedEvent;
        MaxSdkCallbacks.OnRewardedAdLoadFailedEvent += OnRewardedAdFailedEvent;
        MaxSdkCallbacks.OnRewardedAdFailedToDisplayEvent += OnRewardedAdFailedToDisplayEvent;
        MaxSdkCallbacks.OnRewardedAdClickedEvent += OnRewardedAdClickedEvent;
        MaxSdkCallbacks.OnRewardedAdHiddenEvent += OnRewardedAdDismissedEvent;
        MaxSdkCallbacks.OnRewardedAdReceivedRewardEvent += OnRewardedAdReceivedRewardEvent;

        // Load the first rewarded ad
        LoadRewardedAd();
    }

    private void LoadRewardedAd()
    {
        MaxSdk.LoadRewardedAd(rewardedAdUnit);
    }

    private void OnRewardedAdLoadedEvent(string adUnitId)
    {
        // Rewarded ad is ready to be shown. MaxSdk.IsRewardedAdReady(adUnitId) will now return 'true'
        AdsManager.OnRewardedLoaded?.Invoke(true);
        // Reset retry attempt
        rewardRetryAttempt = 0;
    }

    private void OnRewardedAdFailedEvent(string adUnitId, int errorCode)
    {
        // Rewarded ad failed to load 
        // We recommend retrying with exponentially higher delays up to a maximum delay (in this case 64 seconds)

        rewardRetryAttempt++;
        double retryDelay = Math.Pow(2, Math.Min(6, rewardRetryAttempt));

        Invoke("LoadRewardedAd", (float)retryDelay);
    }

    private void OnRewardedAdFailedToDisplayEvent(string adUnitId, int errorCode)
    {
        // Rewarded ad failed to display. We recommend loading the next ad
        LoadRewardedAd();
    }

    private void OnRewardedAdClickedEvent(string adUnitId) { }

    private void OnRewardedAdDismissedEvent(string adUnitId)
    {
        // Rewarded ad is hidden. Pre-load the next ad
        AdsManager.Instance.RewardedClosed();
        LoadRewardedAd();
    }

    private void OnRewardedAdReceivedRewardEvent(string adUnitId, MaxSdk.Reward reward)
    {
        // Rewarded ad was displayed and user should receive the reward
        rewardDelegate?.Invoke();
    }

    /// Banner
    public void InitializeBannerAds()
    {
        // Banners are automatically sized to 320x50 on phones and 728x90 on tablets
        // You may use the utility method `MaxSdkUtils.isTablet()` to help with view sizing adjustments
        MaxSdk.CreateBanner(bannerAdUnit, MaxSdkBase.BannerPosition.BottomCenter);

        // Set background or background color for banners to be fully functional
        MaxSdk.SetBannerBackgroundColor(bannerAdUnit, Color.white);

        ShowBaner("MainScreenBanner");
    }

    public void ShowBaner(string placement)
    {
        if (!AdsInit || IsAdsRemoved) return;

        MaxSdk.ShowBanner(bannerAdUnit);
        GameAnalytics.NewAdEvent(GAAdAction.Show, GAAdType.Banner, "MAX", placement);
        DeslyticsManager.AdsShow("BannerAds", placement, "true");
        DeslyticsManager.BannerShow();
    }

    public void HideBanner()
    {
        MaxSdk.HideBanner(bannerAdUnit);
    }

#endregion

    public void RemoveAds()
    {
        IsAdsRemoved = true;
        HideBanner();
        Debug.Log("Ads removed");
    }

    public void ShowRewardVideo(string placement,Action func=null)
    {
        rewardDelegate = func;

        if (MaxSdk.IsRewardedAdReady(rewardedAdUnit))
        {
            MaxSdk.ShowRewardedAd(rewardedAdUnit, placement);
            GameAnalytics.NewAdEvent(GAAdAction.Show, GAAdType.RewardedVideo, "MAX", placement);
            DeslyticsManager.AdsShow("RewardedVideo", placement, "true");
        }
    }

    public void RequestAllAds()
    {
        LoadInterstitial();
        LoadRewardedAd();
        ShowBaner("MainScreen");
    }
    
    #region Facebook

#if UNITY_IOS
    private void SetFacebookAdTracking()
    {
        bool advertiserTrackingEnabled = IDFATracking.IDFATracking.GetCurrentStatus() == IDFATracking.Status.Authorized;
        
        Debug.Log($"Call: SetFacebookAdTracking({advertiserTrackingEnabled})");
        Facebook.Unity.FB.Mobile.SetAdvertiserIDCollectionEnabled (advertiserTrackingEnabled);
        // ATE FB Audience network
        FbAdSettings.SetAdvertiserTrackingEnabled(advertiserTrackingEnabled);
    }
    
    private static class FbAdSettings
    {
        [System.Runtime.InteropServices.DllImport("__Internal")] 
        private static extern void FBAdSettingsBridgeSetAdvertiserTrackingEnabled(bool advertiserTrackingEnabled);

        public static void SetAdvertiserTrackingEnabled(bool advertiserTrackingEnabled)
        {
            FBAdSettingsBridgeSetAdvertiserTrackingEnabled(advertiserTrackingEnabled);
        }

    }
#endif

    #endregion
}

#endif


