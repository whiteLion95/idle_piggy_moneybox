#if tenjin_mopub_enabled

using System;
using GameAnalyticsSDK;
using UnityEngine;

[RequireComponent(typeof(InternetChecker))]
[RequireComponent(typeof(DisconnectionAdRequester))]
public class MoPubAdsManager : MonoBehaviour,IMediationAdManager
{
    private const string ADS_REMOVED = "ads_removed";

    public bool IsInterstitialVideoReady { get { return MoPub.IsInterstitialReady(interstitialAdUnitID); } }
    public bool IsRewardedVideoReady { get { return MoPub.HasRewardedVideo(rewardedAdUnitID); } }

    public string iosBannerAdUnitId;
    public string androidBannerAdUnitId;
    public string iosInterstitialAdUnitId;

    public string androidInterstitialAdUnitId;
    public string iosRewardedAdUnitId;
    public string androidRewardedAdUnitId;

    [HideInInspector]
    public string bannerAdUnitID, interstitialAdUnitID, rewardedAdUnitID;

    int interRetryAttempt = 0;
    int rewardRetryAttempt = 0;

    bool AdsRemovedGet()
    {
        if (!PlayerPrefs.HasKey(ADS_REMOVED))
            AdsRemovedSet(false);

        return PlayerPrefs.GetInt(ADS_REMOVED) == 1;
    }

    void AdsRemovedSet(bool value)
    {
        PlayerPrefs.SetInt(ADS_REMOVED, value ? 1 : 0);
    }

    // TODO ADS: subscribe to this to hide banner for remove ads, to hide remove ads button etc.
    private Action OnRemoveAdsStateChanged;
    public bool InterIsDisabled { get; internal set; } = true;
    public bool ADSIsRemoved
    {
        get => AdsRemovedGet();
        set
        {
            AdsRemovedSet(value);
            OnRemoveAdsStateChanged?.Invoke();
        }
    }

    private static MoPubAdsManager instance;

    private void Start()
    {
        if (instance == null)
            instance = this;

        var dubl = FindObjectsOfType<MoPubAdsManager>();
        if (dubl.Length > 1)
        {
            Destroy(this.gameObject);
        }

        DontDestroyOnLoad(gameObject);
        

        MoPubManager.OnRewardedVideoLoadedEvent += OnRewardedVideoLoadedEvent;
        MoPubManager.OnRewardedVideoFailedEvent += OnRewardedVideoFailedEvent;
        MoPubManager.OnRewardedVideoReceivedRewardEvent += OnRewardedVideoRecievedRewardEvent;
        MoPubManager.OnRewardedVideoClosedEvent += OnRewardedVideoClosedEvent;
        MoPubManager.OnRewardedVideoExpiredEvent += MoPubManager_OnRewardedVideoExpiredEvent;
        MoPubManager.OnRewardedVideoLeavingApplicationEvent += OnRewardedVideoClosedEvent;
        MoPubManager.OnRewardedVideoExpiredEvent += OnRewardedVideoClosedEvent;

        MoPubManager.OnInterstitialDismissedEvent += MoPubManager_OnInterstitialDismissedEvent;
        MoPubManager.OnInterstitialExpiredEvent += MoPubManager_OnInterstitialDismissedEvent;
        MoPubManager.OnInterstitialFailedEvent += MoPubManager_OnInterstitialFailedEvent;
        MoPubManager.OnInterstitialShownEvent += MoPubManager_OnInterstitialDismissedEvent;
        MoPubManager.OnInterstitialLoadedEvent += MoPubManager_OnInterstitialLoadedEvent;
    }

    private void Awake()
    {
        if (instance == null)
            instance = FindObjectOfType<MoPubAdsManager>();
    }
    

    // TODO ADS: add this callback to MopubManager prefab onInitialized event on Scene
    public void OnMopubInitialized()
    {
        Debug.Log("OnMopubInitialized Start");

#if UNITY_IOS
        MoPub.LoadBannerPluginsForAdUnits(new[] { iosBannerAdUnitId });
        MoPub.LoadInterstitialPluginsForAdUnits(new[] { iosInterstitialAdUnitId });
        MoPub.LoadRewardedVideoPluginsForAdUnits(new[] { iosRewardedAdUnitId });
#elif UNITY_ANDROID
        MoPub.LoadBannerPluginsForAdUnits(new[] { androidBannerAdUnitId });
        MoPub.LoadInterstitialPluginsForAdUnits(new[] { androidInterstitialAdUnitId });
        MoPub.LoadRewardedVideoPluginsForAdUnits(new[] { androidRewardedAdUnitId });
#endif

#if UNITY_IOS
        interstitialAdUnitID = iosInterstitialAdUnitId;
        rewardedAdUnitID = iosRewardedAdUnitId;
#elif UNITY_ANDROID
        interstitialAdUnitID = androidInterstitialAdUnitId;
        rewardedAdUnitID = androidRewardedAdUnitId;
#endif

#if UNITY_IOS
        bannerAdUnitID = iosBannerAdUnitId;
#elif UNITY_ANDROID
        bannerAdUnitID = androidBannerAdUnitId;
#endif

        MoPub.RequestInterstitialAd(interstitialAdUnitID);
        MoPub.RequestRewardedVideo(rewardedAdUnitID);
        MoPub.RequestBanner(bannerAdUnitID, MoPub.AdPosition.BottomCenter, MoPub.MaxAdSize.Width320Height50);
        Debug.Log("OnMopubInitialized Done");
    }

    private void OnRewardedVideoLoadedEvent(string adUnitId)
    {
        Debug.Log("Loaded Reward Event " + adUnitId);

        // Reset retry attempt
        rewardRetryAttempt = 0;
    }
    private void OnRewardedVideoFailedEvent(string adUnitId, string errorMsg)
    {
        Debug.Log("Failed Reward Event");

        // Rewarded ad failed to load 
        // We recommend retrying with exponentially higher delays up to a maximum delay (in this case 64 seconds)

        rewardRetryAttempt++;
        double retryDelay = Math.Pow(2, Math.Min(6, rewardRetryAttempt));

        Invoke(nameof(LoadRewardedAd), (float)retryDelay);
    }
    private void OnRewardedVideoRecievedRewardEvent(string adUnitId, string arg2, float arg3)
    {
        Debug.Log("Shown Reward Event");
    }

    private void OnRewardedVideoClosedEvent(string adUnitId)
    {
        MoPub.RequestRewardedVideo(rewardedAdUnitID);
    }
    private void MoPubManager_OnRewardedVideoExpiredEvent(string obj)
    {
        MoPub.RequestRewardedVideo(rewardedAdUnitID);
    }

    private void MoPubManager_OnInterstitialDismissedEvent(string obj)
    {
        LoadInterstitial();
    }

    private void MoPubManager_OnInterstitialFailedEvent(string arg1, string arg2)
    {
        // Interstitial ad failed to load 
        // We recommend retrying with exponentially higher delays up to a maximum delay (in this case 64 seconds)

        interRetryAttempt++;
        double retryDelay = Math.Pow(2, Math.Min(6, interRetryAttempt));

        Invoke(nameof(LoadInterstitial), (float)retryDelay);
    }

    private void MoPubManager_OnInterstitialLoadedEvent(string obj)
    {
        // Reset retry attempt
        interRetryAttempt = 0;
    }

    private void LoadInterstitial()
    {
        MoPub.RequestInterstitialAd(interstitialAdUnitID);
    }

    private void LoadRewardedAd()
    {
        MoPub.RequestRewardedVideo(rewardedAdUnitID);
    }

    public void ShowBanner(string placement)
    {
        MoPub.ShowBanner(bannerAdUnitID, true);
        GameAnalytics.NewAdEvent(GAAdAction.Show, GAAdType.Banner, "MoPub", placement);
    }

    public void ShowInterstitial(string placement)
    {
        if (MoPub.IsInterstitialReady(interstitialAdUnitID))
        {
            MoPub.ShowInterstitialAd(interstitialAdUnitID);
            GameAnalytics.NewAdEvent(GAAdAction.Show, GAAdType.Interstitial, "MoPub", placement);
        }
        else
        {
            LoadInterstitial();
            GameAnalytics.NewAdEvent(GAAdAction.FailedShow, GAAdType.Interstitial, "MoPub", placement);
        }
    }

    public void ShowRewarded(string placement)
    {
        if (MoPub.HasRewardedVideo(rewardedAdUnitID))
        {
            MoPub.ShowRewardedVideo(rewardedAdUnitID);
            GameAnalytics.NewAdEvent(GAAdAction.Show, GAAdType.RewardedVideo, "MoPub", placement);
        }
        else
        {
            LoadRewardedAd();
            GameAnalytics.NewAdEvent(GAAdAction.FailedShow, GAAdType.RewardedVideo, "MoPub", placement);
        }
    }

    public void RequestAllAds()
    {
        MoPub.RequestInterstitialAd(interstitialAdUnitID);
        MoPub.RequestRewardedVideo(rewardedAdUnitID);
        MoPub.RequestBanner(bannerAdUnitID, MoPub.AdPosition.BottomCenter, MoPub.MaxAdSize.Width320Height50);
    }
}

#endif
