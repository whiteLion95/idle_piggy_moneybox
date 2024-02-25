using EZ_Pooling;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public abstract class StaticCoinThrower : MonoBehaviour, ISaveable
{
    [System.Serializable]
    public struct PersistentData
    {
        public float timerProgress;
        public int level;
        public float curExp;
    }

    [SerializeField] protected CoinsData coinsData;
    [SerializeField] protected Transform coinSpawnPlace;
    [SerializeField] protected int numberOfCoins;
    [SerializeField][Tooltip("Throw delay between coins in one pack")] protected float throwDelay;
    [SerializeField] protected float baseThrowRate;
    [SerializeField] protected int baseThrowCapacity = 50;
    [Header("TimeOut")]
    [SerializeField] protected Slider timeOutSlider;
    [SerializeField] protected TMP_Text timeOutText;

    public static System.Action<Coin> OnLastCoinGained;
    public static System.Action<StaticCoinThrower> OnSpawned;
    public Action OnCountDownFinished;

    protected Animator _anim;
    protected string _saveKey;
    protected Queue<Coin> _spawnedCoins;
    protected Coin _lastCoin;
    protected bool _readyToThrow;
    protected int _coinsCount = 0;
    protected UpgradesManager _upManager;
    protected PersistentData _persistentData;
    [SerializeField] protected float _throwRate;

    public ulong MoneyPerThrow { get; protected set; }
    public float ThrowRate { get => _throwRate; }
    public virtual CoinSource CoinSource { get; }
    public Transform CoinSpawnPlace { get => coinSpawnPlace; }

    protected virtual void Awake()
    {
        _anim = GetComponentInChildren<Animator>();
        SetSaveKey();
        LoadData();
    }

    protected virtual void Start()
    {
        _upManager = UpgradesManager.Instance;

        InitSlider();
        CalculateThrowRate();
        OnSpawned?.Invoke(this);

        _upManager.OnUpgrade += HandleUpgrade;
        OnCountDownFinished += HandleCountDownFinished;
    }

    protected virtual void OnEnable()
    {
        CoinsDespawnPlace.OnCoinTriggered += HandlerOnCoinGained;
    }

    protected virtual void OnDisable()
    {
        CoinsDespawnPlace.OnCoinTriggered -= HandlerOnCoinGained;
    }

    private void OnApplicationQuit()
    {
        SaveData();
    }

    private void OnApplicationPause(bool pause)
    {
        SaveData();
    }

    private void OnDestroy()
    {
        SaveData();
    }

    // Update is called once per frame
    private void Update()
    {
        CountDown();
        ChangeSlider();
    }

    private void CountDown()
    {
        _persistentData.timerProgress += GetThrowRate() * Time.deltaTime;

        if (_persistentData.timerProgress >= GetThrowCapacity())
        {
            OnCountDownFinished?.Invoke();
            _persistentData.timerProgress = 0f;
        }
    }

    protected virtual float GetThrowRate()
    {
        return _throwRate;
    }

    protected virtual float GetThrowCapacity()
    {
        return baseThrowCapacity;
    }

    protected virtual void HandleCountDownFinished()
    {

    }

    public virtual void ThrowCoins()
    {
        _readyToThrow = false;
        StartCoroutine(ThrowCoinsRoutine());
    }

    protected IEnumerator ThrowCoinsRoutine()
    {
        while (_spawnedCoins.Count > 0)
        {
            Coin coinToThrow = _spawnedCoins.Dequeue();
            CoinsManager.Instance.ThrowCoin(coinToThrow, coinsData);
            _coinsCount++;
            yield return new WaitForSeconds(throwDelay);
        }

        SpawnCoins();
    }

    protected virtual void SpawnCoins()
    {
        _spawnedCoins = new Queue<Coin>();
        ulong counter = 0;
        ulong divider = MoneyPerThrow / (ulong)numberOfCoins;

        while (counter < MoneyPerThrow)
        {
            ulong coinValue = ((counter + divider) < MoneyPerThrow) ? divider : (MoneyPerThrow - counter);
            Coin spawnedCoin = CoinsManager.Instance.SpawnCoin(coinSpawnPlace, (uint)coinValue, CoinSource);
            _spawnedCoins.Enqueue(spawnedCoin);
            counter += divider;
        }

        _readyToThrow = true;
    }

    public void UpdateSpawnedCoins()
    {
        Queue<Coin> oldSpawnedCoins = new Queue<Coin>();

        while (_spawnedCoins.Count > 0)
        {
            oldSpawnedCoins.Enqueue(_spawnedCoins.Dequeue());
        }

        SpawnCoins();

        while (oldSpawnedCoins.Count > 0)
        {
            Coin coinToDespawn = oldSpawnedCoins.Dequeue();
            EZ_PoolManager.Despawn(coinToDespawn.transform);
        }
    }

    protected virtual void HandlerOnCoinGained(Coin coin)
    {
        if (coin.Source == CoinSource)
        {
            _coinsCount--;

            if (_coinsCount == 0)
            {
                OnLastCoinGained?.Invoke(coin);
            }
        }
    }

    private void InitSlider()
    {
        timeOutSlider.maxValue = GetThrowCapacity();
        timeOutSlider.value = _persistentData.timerProgress;
        timeOutText.text = _persistentData.timerProgress.ToString("F0");
    }

    private void ChangeSlider()
    {
        timeOutSlider.value = _persistentData.timerProgress;
        timeOutText.text = (GetThrowCapacity() - (int)_persistentData.timerProgress).ToString("F0");

        if (timeOutSlider.maxValue != GetThrowCapacity())
        {
            timeOutSlider.maxValue = GetThrowCapacity();
        }
    }

    public ulong GetProfitPerMinute()
    {
        float profitPerSecond = (float)MoneyPerThrow / (GetThrowCapacity() / GetThrowRate());
        float profitPerMinute = profitPerSecond * 60;
        return (ulong)(profitPerMinute);
    }

    protected virtual void CalculateMoneyPerThrow()
    {

    }

    protected virtual void CalculateThrowRate()
    {

    }

    protected virtual void HandleUpgrade(UpgradeType upType, CoinSource coinSource)
    {

    }

    protected abstract void SetSaveKey();

    public virtual void SaveData()
    {
        SaveManager.Save(_saveKey, _persistentData);
    }

    public virtual void LoadData()
    {
        _persistentData = SaveManager.Load<PersistentData>(_saveKey);
    }
}
