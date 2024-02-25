using UnityEngine;

public class BalanceManager : MonoBehaviour, ISaveable
{
    public static System.Action<float> OnMoneyChanged;
    public static System.Action<ulong> OnAvProfitPerMinuteChanged;

    public static BalanceManager Instance { get; private set; }
    [field: SerializeField] public float Money { get; private set; }
    [field: SerializeField] public ulong AverageProfitPerMinute { get; private set; }

    private const string SAVING_KEY = "Money";
    private const string AV_PROFIT_PER_MINUTE_KEY = "Average profit per minute";
    private UpgradesManager _upManager;
    private RunnersManager _runnersManager;

    [HideInInspector]
    public ulong boosterValue;

    private void Awake()
    {
        Instance = this;
        LoadData();
        Runner.OnRunnerSpawned += OnRunnerSpawnedHandler;
    }

    private void Start()
    {
        _runnersManager = RunnersManager.Instance;
        _upManager = UpgradesManager.Instance;
        _upManager.OnUpgrade += ChangeAverageProfitPerMinute;

        boosterValue = 1;

        SetProfitsPerMinute();
    }

    private void OnEnable()
    {
        CoinsDespawnPlace.OnCoinTriggered += GainMoneyFromCoin;
        Runner.OnRunnerLevelUp += OnRunnerAutoUpdateHandler;
        StaticCoinThrower.OnSpawned += OnStaticThrowerSpawnedHandler;
    }

    private void OnDisable()
    {
        CoinsDespawnPlace.OnCoinTriggered -= GainMoneyFromCoin;
        Runner.OnRunnerLevelUp -= OnRunnerAutoUpdateHandler;
        StaticCoinThrower.OnSpawned -= OnStaticThrowerSpawnedHandler;
    }

    private void OnDestroy()
    {
        SaveData();
    }

    private void OnApplicationQuit()
    {
        SaveData();
    }

    private void OnApplicationPause(bool pause)
    {
        SaveData();
    }

    private void GainMoneyFromCoin(Coin coin)
    {
        Money += coin.Value * boosterValue;
        OnMoneyChanged?.Invoke(Money);
    }

    public void GainMoney(float money)
    {
        Money += money;
        OnMoneyChanged?.Invoke(Money);
    }

    public void SpendMoney(float money)
    {
        if (Money >= money)
        {
            Money -= money;
            OnMoneyChanged?.Invoke(Money);
        }
    }

    private void OnRunnerSpawnedHandler()
    {
        SetProfitsPerMinute();
        Runner.OnRunnerSpawned -= OnRunnerSpawnedHandler;
    }

    private const int AVERAGE_TAPS_PER_MINUTE = 265;
    [SerializeField] private ulong _tapsProfit, _runnersProfit, _handProfit, _frogProfit;

    private void SetProfitsPerMinute()
    {
        _tapsProfit = (ulong)_upManager.GetValue(UpgradeType.MONEY_JAR_COIN_VALUE) * AVERAGE_TAPS_PER_MINUTE;
        _runnersProfit = (ulong)RunnersManager.Instance.GetProfitPerMinute();
        if (Hand.Instance != null) _handProfit = Hand.Instance.GetProfitPerMinute();
        if (Frog.Instance != null) _frogProfit = Frog.Instance.GetProfitPerMinute();

        if (_runnersProfit == 0 && Hand.Instance == null && Frog.Instance == null)
            return;

        CalculateAverageProfitPerMinute();
    }

    private void CalculateAverageProfitPerMinute()
    {
        AverageProfitPerMinute = _tapsProfit + _runnersProfit + _handProfit + _frogProfit;
        OnAvProfitPerMinuteChanged?.Invoke(AverageProfitPerMinute);
    }

    private void OnRunnerAutoUpdateHandler()
    {
        _runnersProfit = (ulong)RunnersManager.Instance.GetProfitPerMinute();
        CalculateAverageProfitPerMinute();
    }

    private void ChangeAverageProfitPerMinute(UpgradeType type, CoinSource coinSource)
    {
        switch (coinSource)
        {
            case CoinSource.TAP:
                _tapsProfit = (ulong)_upManager.GetValue(type) * AVERAGE_TAPS_PER_MINUTE;
                break;
            case CoinSource.RUNNER:
                _runnersProfit = (ulong)RunnersManager.Instance.GetProfitPerMinute();
                break;
            case CoinSource.HAND:
                _handProfit = Hand.Instance.GetProfitPerMinute();
                break;
            case CoinSource.FROG:
                _frogProfit = Frog.Instance.GetProfitPerMinute();
                break;
        }

        CalculateAverageProfitPerMinute();
    }

    private void OnStaticThrowerSpawnedHandler(StaticCoinThrower thrower)
    {
        if (thrower is Hand)
            _handProfit = thrower.GetProfitPerMinute();
        if (thrower is Frog)
            _frogProfit = thrower.GetProfitPerMinute();

        CalculateAverageProfitPerMinute();
    }

    #region Data

    public void SaveData()
    {
        SaveManager.Save(SAVING_KEY, Money);
        SaveManager.Save(AV_PROFIT_PER_MINUTE_KEY, AverageProfitPerMinute);
    }

    public void LoadData()
    {
        Money = SaveManager.LoadULong(SAVING_KEY);
        AverageProfitPerMinute = SaveManager.LoadULong(AV_PROFIT_PER_MINUTE_KEY);
    }

    #endregion
}