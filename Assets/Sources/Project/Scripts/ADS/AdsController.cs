using System.Collections;
using System.Collections.Generic;
using Deslab.Deslytics;
using UnityEngine;
using UnityEngine.UI;
using Deslab.Scripts.Deslytics.Ads;
using Deslab.Utils;

public class AdsController : MonoBehaviour
{
    [SerializeField] private List<RewardedAdsObject> _rewardedObjects;
    [SerializeField] private float _minShowRate;
    [SerializeField] private float _maxShowRate;
    [Space] [SerializeField] private RewardedAdsObject _rewardedBoosterObject;
    [SerializeField] private float _rewardedBoosterTimer = 180f;

    public static System.Action<int> OnRewardedAdWatched;
    public static System.Action OnRewardedBoosterAdWatched;


    private int _rewardAmount;
    private BalanceManager _balanceManager;
    private TutorialsController _tutorialController;
    private RewardedAdsObject _curRewardedObj;
    private RandomNoRepeate _randNoReap;

    #region Persistent Data

    [System.Serializable]
    private struct PersistentData
    {
        public bool repeatedLaunch;
    }

    private PersistentData _persistentData;

    #endregion

    public static AdsController Instance { get; private set; }

    public int RewardAmount
    {
        get { return _rewardAmount; }
    }


    private void Awake()
    {
        Instance = this;
        LoadData();

        AdsManager.OnRewardedLoaded += HandlerOnRewardedAdsLoaded;
    }

    // Start is called before the first frame update
    void Start()
    {
        _balanceManager = BalanceManager.Instance;
        _tutorialController = TutorialsController.Instance;

        InitRandNoReap();
        InitInterstitialAds();
        InitRewardedObjects();
    }

#if UNITY_EDITOR
    private void OnApplicationQuit()
    {
        SaveData();
    }
#endif

#if PLATFORM_ANDROID
    private void OnApplicationPause(bool pause)
    {
        if (pause)
            SaveData();
    }
#endif

    private void InitRandNoReap()
    {
        _randNoReap = new RandomNoRepeate();
        _randNoReap.SetCount(_rewardedObjects.Count);
    }

    private void InitInterstitialAds()
    {
        if (!_persistentData.repeatedLaunch)
            AdsManager.Instance.ShowInterstitalWithDelay(true);
        else
            AdsManager.Instance.ShowInterstitalWithDelay(false);
    }

    private void InitRewardedObjects()
    {
        foreach (var rewardedObj in _rewardedObjects)
        {
            rewardedObj.OnActivated += WatchAds;
            rewardedObj.OnEnded += () => StartCoroutine(RewardedObjectTimer());
        }

        _rewardedBoosterObject.OnActivated += WatchAdsForBoost;
        _rewardedBoosterObject.OnEnded += () => StartCoroutine(RewardedBoosterTimerRoutine());
        _rewardedBoosterObject.OnActivated += () => StartCoroutine(RewardedBoosterTimerRoutine());

        InitRewardedObjTimer();
    }

    private void InitRewardedObjTimer()
    {
        if (_tutorialController.SavingData.completed)
            StartCoroutine(RewardedObjectTimer());
        else
            _tutorialController.OnCompleted += () => StartCoroutine(RewardedObjectTimer());

        StartCoroutine(RewardedBoosterTimerRoutine());
    }

    private void SetCurRewardedObject()
    {
        int curIndex;

        if (_curRewardedObj)
            curIndex = _randNoReap.GetAvailableExcept(_rewardedObjects.IndexOf(_curRewardedObj));
        else
            curIndex = _randNoReap.GetAvailable();

        _curRewardedObj = _rewardedObjects[curIndex];
    }

    private void HandlerOnRewardedAdsLoaded(bool opened)
    {
        if (!opened)
        {
            StartCoroutine(RewardedObjectTimer());
            Debug.Log("rewarded closed");
        }
    }

    private void WatchAds()
    {
        if (!MaxAdsManager.instance.IsAdsRemoved || AdsManager.Instance.allAdsRemoved)
            AdsManager.ShowRewardedAds("REWARDED_COINS", WatchAdsResponse);
        else
            WatchAdsResponse();
        //DeslyticsManager.CoinRewardedStart();
    }

    private void WatchAdsResponse()
    {
        AdsManager.Instance.ShowResulWindow(GetReward, _rewardAmount);
    }

    private void GetReward()
    {
        AdsManager.Instance.ShowInterstitalWithDelay(false);
        //DeslyticsManager.CoinRewardedFinished();
        DeslyticsManager.RewardedShow(true);
        OnRewardedAdWatched?.Invoke(_rewardAmount);
    }

    public void WatchAdsForDouble()
    {
        if (!MaxAdsManager.instance.IsAdsRemoved || AdsManager.Instance.allAdsRemoved)
            AdsManager.ShowRewardedAds("X2_REWARD", WatchAdsForDoubleResponse);
        else
            WatchAdsForDoubleResponse();
        //DeslyticsManager.CoinRewardedStart();
    }

    public void WatchAdsForDoubleResponse()
    {
        AdsManager.Instance.ShowResulWindow(GetRewardDouble, _rewardAmount * 2);
    }

    private void GetRewardDouble()
    {
        AdsManager.Instance.ShowInterstitalWithDelay(false);
        DeslyticsManager.RewardedShow(true);
        _rewardAmount *= 2;
        OnRewardedAdWatched?.Invoke(_rewardAmount);
    }


    private void WatchAdsForBoost()
    {
        if (!MaxAdsManager.instance.IsAdsRemoved || AdsManager.Instance.allAdsRemoved)
            AdsManager.ShowRewardedAds("X2_TIME_BOOST", WatchAdsForBoostResponse);
        else
            WatchAdsForBoostResponse();
    }

    public void WatchAdsForBoostResponse()
    {
        AdsManager.Instance.ShowResulWindow(GetRewardBooster, 0);
    }

    private void GetRewardBooster()
    {
        DeslyticsManager.RewardedShow(true);
        OnRewardedBoosterAdWatched?.Invoke();
    }

    private void GetAllRewards()
    {
    }


    private IEnumerator RewardedObjectTimer()
    {
        //Debug.LogError("RewardedObjectTimer started");
        float waitingTime = Random.Range(_minShowRate, _maxShowRate);
        yield return new WaitForSeconds(waitingTime);
        ShowRewardedObject();
        //Debug.LogError("RewardedObjectTimer finished");
    }

    private void ShowRewardedObject()
    {
        SetCurRewardedObject();

        _curRewardedObj.gameObject.SetActive(true);
        CalculateRewardAmount(_curRewardedObj.RewardMultiplier);
        _curRewardedObj.RewardText.SetValue(_rewardAmount);
    }

    public void CalculateRewardAmount(float multiplier = 1f, float interDemultiplier = 1f)
    {
        _rewardAmount = Mathf.RoundToInt((_balanceManager.AverageProfitPerMinute * multiplier) / interDemultiplier);
    }


    private IEnumerator RewardedBoosterTimerRoutine()
    {
        yield return new WaitForSeconds(_rewardedBoosterTimer);
        _rewardedBoosterObject.gameObject.SetActive(true);
    }

    #region Persistent Data

    private const string SAVE_DATA = "Ads_Data";

    private void LoadData()
    {
        _persistentData = SaveManager.Load<PersistentData>(SAVE_DATA);
    }

    private void SaveData()
    {
        if (!_persistentData.repeatedLaunch)
            _persistentData.repeatedLaunch = true;

        SaveManager.Save(SAVE_DATA, _persistentData);
    }

    #endregion
}