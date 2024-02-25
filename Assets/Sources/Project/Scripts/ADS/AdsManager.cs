using System;
using System.Collections;
using System.Threading.Tasks;
using Deslab.Deslytics;
using Deslab.RemoteConfig;
using TMPro;
using UnityEngine;

namespace Deslab.Scripts.Deslytics.Ads
{
    public class AdsManager : MonoBehaviour
    {
        public static AdsManager Instance;
        [HideInInspector] public bool allAdsRemoved = false;

        public bool interAdsRemoved
        {
            get => InterRemovedGet();
            set
            {
                InterRemovedSet(value);
                OnRemoveInterStateChanged?.Invoke();
                Debug.Log("INTER_REMOVED :: INTER REMOVED SET :: " + value);
            }
        }

        public Action OnRemoveInterStateChanged;

        private static Action _interstitialClosed;

        public static Action<bool> OnRewardedLoaded;
        private static Action<bool> _rewardedClosed;

        private static bool _isShowingAds;
        public static bool _interstitialTimerOver;

        private int _rewardAmount;

        [SerializeField] private float interstitialFirstTimer = 60f;
        [SerializeField] private float interstitialLoopTimer = 40f;
        [SerializeField] private bool interstitialAutoShow;
        [SerializeField] private GameObject interLockerButton;
        [SerializeField] private ReducedBigText _amountMoneyLabel;

        [SerializeField] private GameObject _resultWindowReward;
        [SerializeField] private GameObject _resultWindowCoins;
        [SerializeField] private GameObject _resultWindowBooster;
        [SerializeField] private ReducedBigText _amountMoneyResultLabel;
        [SerializeField] private float _rewardMultiplier = 1.5f;

        public bool debugMax;

        private const string INTER_REMOVED = "int_removed";


        private void Awake()
        {
            if (Instance != null) return;
            Instance = this;
        }

        private void Start()
        {
            ShowBannerAds("BANNER");
        }

        public static void AdsRemoved()
        {
            Instance.allAdsRemoved = true;
            Instance.interAdsRemoved = true;
            MaxAdsManager.instance.IsAdsRemoved = true;
            // PlayerPrefs.SetInt("ads_removed", 1);
            // PlayerPrefs.Save();
            HideBannerAds();
        }

        public static void InterAdsRemoved()
        {
            Instance.interAdsRemoved = true;
            // PlayerPrefs.SetInt("ads_removed", 1);
            // PlayerPrefs.Save();
        }

        bool InterRemovedGet()
        {
            if (!PlayerPrefs.HasKey(INTER_REMOVED))
            {
                InterRemovedSet(false);
                Debug.Log("INTER_REMOVED :: NOT FOUND. SET IT FALSE");
            }

            return PlayerPrefs.GetInt(INTER_REMOVED) == 1;
        }

        void InterRemovedSet(bool value)
        {
            Debug.Log("INTER_REMOVED :: SET VALUE :: " + value);
            PlayerPrefs.SetInt(INTER_REMOVED, value ? 1 : 0);
            PlayerPrefs.Save();
        }

        public static void ShowRewardedAds(string placementName, Action action)
        {
            if (!RemoteConfigManager.Instance.GetData<bool>(RemoteKeys.TransformRewardToInter))
            {
                MaxAdsManager.instance.ShowRewardVideo(placementName, action);
            }
            else
            {
                MaxAdsManager.instance.ShowInter(placementName);
                action?.Invoke(); 
            }
        }

        public static void ShowInterstitialAds(string placementName)
        {
            //if (!Instance.adsRemoved)
            Instance.ShowResulWindow(Instance.GetReward, Instance._rewardAmount);
            Instance.InterstitialClosed();
            Instance.interLockerButton.SetActive(false);

            if (Instance.interAdsRemoved || Instance.allAdsRemoved) return;
            MaxAdsManager.instance.ShowInter(placementName);
        }

        public static void ShowBannerAds(string placementName)
        {
            if (!Instance.allAdsRemoved)
                MaxAdsManager.instance.ShowBaner(placementName);
            else HideBannerAds();
        }

        public static void HideBannerAds()
        {
            MaxAdsManager.instance.HideBanner();
        }

        #region Interstitial

        public void ShowInterstitalWithDelay(bool firstTime)
        {
            if (MaxAdsManager.instance.IsAdsRemoved || Instance.allAdsRemoved || Instance.interAdsRemoved)
                return;

            interAdsTimer.SetText("");
            //if (MaxAdsManager.instance.IsAdsRemoved) return;

            StopAllCoroutines();

            StartCoroutine(firstTime
                ? InterstitialShowCoroutine(interstitialFirstTimer)
                : InterstitialShowCoroutine(interstitialLoopTimer));
        }

        [SerializeField] private TMP_Text interAdsTimer;

        private IEnumerator InterstitialShowCoroutine(float delay)
        {
            interAdsTimer.SetText(delay.ToString());

            int timer = (int) delay;

            for (int i = 0; i < delay; i++)
            {
                timer--;
                interAdsTimer.SetText(timer.ToString());
                yield return new WaitForSeconds(1);
            }

            _interstitialTimerOver = true;

            if (!Instance.allAdsRemoved || !Instance.interAdsRemoved)
                interLockerButton.SetActive(true);
            //_rewardAmount = Mathf.RoundToInt(BalanceManager.Instance.AverageProfitPerMinute * _rewardMultiplier) / 10;
            AdsController.Instance.CalculateRewardAmount(_rewardMultiplier, 10);
            _rewardAmount = AdsController.Instance.RewardAmount;
            _amountMoneyLabel.SetValue(_rewardAmount);

            if (interstitialAutoShow)
            {
                yield return new WaitForSeconds(2);
                ShowInterAds();
            }
        }

        public void ShowInterAds()
        {
            if (_interstitialTimerOver)
            {
                ShowInterstitialAds("AutoInter");
            }
        }

        public void InterstitialClosed()
        {
            Debug.Log("[MyAds] InterstitialClosed");
            _interstitialClosed?.Invoke();
            _interstitialClosed = null;
            _isShowingAds = false;
            _interstitialTimerOver = false;
            StopAllCoroutines();
            StartCoroutine(InterstitialShowCoroutine(interstitialLoopTimer));
        }

        #endregion

        #region Rewarded

        public void GetDoubleRewardForWatchAds()
        {
            Instance.InterstitialClosed();
            interLockerButton.SetActive(false);
        }

        private void GetReward()
        {
            //DeslyticsManager.CoinRewardedFinished();
            DeslyticsManager.RewardedShow(true);
            AdsController.OnRewardedAdWatched?.Invoke(_rewardAmount);
        }

        public void RewardedClosed()
        {
            _rewardedClosed = null;
            OnRewardedLoaded(false);
            _isShowingAds = false;
            _interstitialTimerOver = false;
            StopAllCoroutines();
            StartCoroutine(InterstitialShowCoroutine(interstitialLoopTimer));
        }

        #endregion

        public delegate void Callback();

        private Callback _callback;

        public void ShowResulWindow(Callback callback, float rewardAmount)
        {
            _callback = callback;

            _resultWindowReward.SetActive(true);

            _resultWindowCoins.SetActive(rewardAmount != 0);
            _resultWindowBooster.SetActive(rewardAmount == 0);

            _amountMoneyResultLabel.SetValue(rewardAmount);
        }

        public void CloseResultWindow()
        {
            _resultWindowReward.SetActive(false);

            _callback?.Invoke();
            _callback = null;
        }
    }
}