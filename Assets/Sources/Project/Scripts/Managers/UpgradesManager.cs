using Deslab.Deslytics;
using System.Collections.Generic;
using UnityEngine;

public class UpgradesManager : MonoBehaviour, ISaveable
{
    [SerializeField][Tooltip("All available upgrades")] private UpgradeData[] upgradesData;
    [SerializeField] private UnlocksData unlocksData;

    public static UpgradesManager Instance { get; private set; }

    public System.Action<UpgradeType, CoinSource> OnUpgrade;
    public System.Action<PurchaseType> OnPurchase;
    public System.Action<int> OnRunnersCountLevelChanged;

    public bool FrogPurchased { get { return _purchases.frogPurchased; } }
    public bool HandPurchased { get { return _purchases.handPurchased; } }

    private LevelsSaving _levels;
    private PurchaseData _purchases;
    private RunnersManager _runnersManager;

    private void Awake()
    {
        Instance = this;
        LoadData();
    }

    private void Start()
    {
        _runnersManager = RunnersManager.Instance;

        _runnersManager.OnRunnerDespawned += HandleRunnerDespawned;
    }

    private void OnApplicationQuit()
    {
        SaveData();
    }

    private void OnDestroy()
    {
        SaveData();
    }

    private void OnApplicationPause(bool pause)
    {
        SaveData();
    }

    public void Upgrade(UpgradeType type)
    {
        int level = 0;

        switch (type)
        {
            case UpgradeType.MONEY_JAR_COIN_VALUE:
                level = ++_levels.moneyJarLevel;
                if (_levels.moneyJarLevel == 1) UI_UnitButton.OnFirstPurchase?.Invoke();
                break;
            case UpgradeType.RUNNERS_COUNT:
                level = ++_levels.runnersCountLevel;
                RunnersManager.Instance.SpawnNewRunner();
                if (_levels.runnersCountLevel == 1) UI_UnitButton.OnFirstPurchase?.Invoke();
                break;
            case UpgradeType.SKILLED_RUNNERS:
                level = ++_levels.skilledRunnersLevel;
                break;
            case UpgradeType.WEAK_COIN_FLIP:
                level = ++_levels.weakCoinFlipLevel;
                break;
            case UpgradeType.MEDIUM_COIN_FLIP:
                level = ++_levels.mediumCoinFlipLevel;
                break;
            case UpgradeType.THE_TRIPLE_COUP:
                level = ++_levels.theTripleCoupLevel;
                break;
            case UpgradeType.ULTRA_STRONG_COIN_FLIP:
                level = ++_levels.ultraStrongCoinFlipLevel;
                break;
            case UpgradeType.PRECISE_COIN_TOSS:
                level = ++_levels.preciseCoinTossLevel;
                break;
            case UpgradeType.CHARGED_WEAK_COIN_FLIP:
                level = ++_levels.chargedWeakCoinFlipLevel;
                break;
            case UpgradeType.GOLDEN_RAIN:
                level = ++_levels.goldenRainLevel;
                break;
            case UpgradeType.CHARGED_MEDIUM_COIN_FLIP:
                level = ++_levels.chargedMediumCoinFlipLevel;
                break;
            case UpgradeType.LUCK_IS_ON_YOUR_SIDE:
                level = ++_levels.luckIsOnYourSideLevel;
                break;
            case UpgradeType.CHARGED_STRONG_COIN_FLIP:
                level = ++_levels.chargedStrongCoinFlipLevel;
                break;
            case UpgradeType.MONETARY_BLESSING:
                level = ++_levels.monetayBlessingLevel;
                break;
            case UpgradeType.CHARGED_ULTRA_STRONG_COIN_FLIP:
                level = ++_levels.chargedUltraStrongCoinFlipLevel;
                break;
            case UpgradeType.GOLDEN_FROG:
                level = ++_levels.goldenFrogLevel;
                break;
            case UpgradeType.SPELLBOOK:
                level = ++_levels.spellBookLevel;
                break;
        }

        CoinSource coinSource = CoinSource.TAP;

        switch (type)
        {
            case UpgradeType.RUNNERS_COUNT:
            case UpgradeType.SKILLED_RUNNERS:
                coinSource = CoinSource.RUNNER;
                break;
            case UpgradeType.WEAK_COIN_FLIP:
            case UpgradeType.MEDIUM_COIN_FLIP:
            case UpgradeType.THE_TRIPLE_COUP:
            case UpgradeType.ULTRA_STRONG_COIN_FLIP:
            case UpgradeType.PRECISE_COIN_TOSS:
            case UpgradeType.CHARGED_WEAK_COIN_FLIP:
            case UpgradeType.GOLDEN_RAIN:
            case UpgradeType.CHARGED_MEDIUM_COIN_FLIP:
            case UpgradeType.LUCK_IS_ON_YOUR_SIDE:
            case UpgradeType.CHARGED_STRONG_COIN_FLIP:
            case UpgradeType.MONETARY_BLESSING:
            case UpgradeType.CHARGED_ULTRA_STRONG_COIN_FLIP:
                coinSource = CoinSource.HAND;
                break;
            case UpgradeType.GOLDEN_FROG:
            case UpgradeType.SPELLBOOK:
                coinSource = CoinSource.FROG;
                break;
        }

        DeslyticsManager.LevelUp(type.ToString(), level);
        OnUpgrade?.Invoke(type, coinSource);
    }

    public void Purchase(PurchaseType type)
    {
        switch (type)
        {
            case PurchaseType.FROG:
                _purchases.frogPurchased = true;
                StaticCoinThrowersManager.Instance.SpawnFrog();
                UI_UnitButton.OnFirstPurchase?.Invoke();
                break;
            case PurchaseType.HAND:
                _purchases.handPurchased = true;
                StaticCoinThrowersManager.Instance.SpawnHand();
                break;
            default:
                break;
        }

        OnPurchase?.Invoke(type);
    }

    public int GetCurrentLevel(UpgradeType type)
    {
        int level = 0;

        switch (type)
        {
            case UpgradeType.MONEY_JAR_COIN_VALUE:
                level = _levels.moneyJarLevel;
                break;
            case UpgradeType.RUNNERS_COUNT:
                level = _levels.runnersCountLevel;
                break;
            case UpgradeType.SKILLED_RUNNERS:
                level = _levels.skilledRunnersLevel;
                break;
            case UpgradeType.WEAK_COIN_FLIP:
                level = _levels.weakCoinFlipLevel;
                break;
            case UpgradeType.MEDIUM_COIN_FLIP:
                level = _levels.mediumCoinFlipLevel;
                break;
            case UpgradeType.THE_TRIPLE_COUP:
                level = _levels.theTripleCoupLevel;
                break;
            case UpgradeType.ULTRA_STRONG_COIN_FLIP:
                level = _levels.ultraStrongCoinFlipLevel;
                break;
            case UpgradeType.PRECISE_COIN_TOSS:
                level = _levels.preciseCoinTossLevel;
                break;
            case UpgradeType.CHARGED_WEAK_COIN_FLIP:
                level = _levels.chargedWeakCoinFlipLevel;
                break;
            case UpgradeType.GOLDEN_RAIN:
                level = _levels.goldenRainLevel;
                break;
            case UpgradeType.CHARGED_MEDIUM_COIN_FLIP:
                level = _levels.chargedMediumCoinFlipLevel;
                break;
            case UpgradeType.LUCK_IS_ON_YOUR_SIDE:
                level = _levels.luckIsOnYourSideLevel;
                break;
            case UpgradeType.CHARGED_STRONG_COIN_FLIP:
                level = _levels.chargedStrongCoinFlipLevel;
                break;
            case UpgradeType.MONETARY_BLESSING:
                level = _levels.monetayBlessingLevel;
                break;
            case UpgradeType.CHARGED_ULTRA_STRONG_COIN_FLIP:
                level = _levels.chargedUltraStrongCoinFlipLevel;
                break;
            case UpgradeType.GOLDEN_FROG:
                level = _levels.goldenFrogLevel;
                break;
            case UpgradeType.SPELLBOOK:
                level = _levels.spellBookLevel;
                break;
        }

        return level;
    }

    private LevelData GetCurrentLevelData(UpgradeType type)
    {
        int level = GetCurrentLevel(type);
        LevelData levelData = default(LevelData);

        foreach (UpgradeData uData in upgradesData)
        {
            if (type == uData.UpgradeType)
            {
                levelData = uData.GetLevelData(level);
                break;
            }
        }

        return levelData;
    }

    public float GetLevelUpPrice(UpgradeType type)
    {
        float price = GetCurrentLevelData(type).levelUpPrice;
        return price;
    }

    public float GetLevelUpPrice(UpgradeType type, int level)
    {
        foreach (UpgradeData uData in upgradesData)
        {
            if (type == uData.UpgradeType)
                return uData.GetLevelUpPrice(level);
        }

        return 0;
    }

    public float GetValue(UpgradeType type)
    {
        float value = GetCurrentLevelData(type).value;
        return value;
    }

    public float GetValue(UpgradeType type, int level)
    {
        foreach (UpgradeData uData in upgradesData)
        {
            if (type == uData.UpgradeType)
                return uData.GetValue(level);
        }

        return 0;
    }

    public uint GetMaxLevel(UpgradeType type)
    {
        uint maxLevel = GetCurrentLevelData(type).maxLevel;
        return maxLevel;
    }

    public float GetUnlockPrice(UnitButtonType buttonType)
    {
        float unlockPrice = 0;

        switch (buttonType)
        {
            case UnitButtonType.MONEY_JAR:
                unlockPrice = GetLevelUpPrice(UpgradeType.MONEY_JAR_COIN_VALUE, 0);
                break;
            case UnitButtonType.RUNNER:
                unlockPrice = GetLevelUpPrice(UpgradeType.RUNNERS_COUNT, 0);
                break;
            case UnitButtonType.HAND:
                unlockPrice = unlocksData.HandUnlockPrice;
                break;
            case UnitButtonType.FROG:
                unlockPrice = unlocksData.FrogUnlockPrice;
                break;
        }

        return unlockPrice;
    }

    public List<UpgradeData> GetUpgradeDatas(CoinSource coinSource)
    {
        List<UpgradeData> upgradeDatas = new List<UpgradeData>();

        for (int i = 0; i < upgradesData.Length; i++)
        {
            if (upgradesData[i].CoinSource == coinSource)
            {
                upgradeDatas.Add(upgradesData[i]);
            }
        }

        return upgradeDatas;
    }

    private void HandleRunnerDespawned(Runner runner)
    {
        if (_levels.runnersCountLevel > 0)
        {
            _levels.runnersCountLevel--;
            OnRunnersCountLevelChanged?.Invoke(_levels.runnersCountLevel);
        }
    }

    public void LoadData()
    {
        _levels = SaveManager.Load<LevelsSaving>("Levels data");
        _purchases = SaveManager.Load<PurchaseData>("Purchase data");
    }

    public void SaveData()
    {
        SaveManager.Save("Levels data", _levels);
        SaveManager.Save("Purchase data", _purchases);
    }
}

[System.Serializable]
public struct LevelsSaving
{
    public int moneyJarLevel;
    public int runnersCountLevel;
    public int skilledRunnersLevel;
    public int weakCoinFlipLevel;
    public int mediumCoinFlipLevel;
    public int theTripleCoupLevel;
    public int ultraStrongCoinFlipLevel;
    public int preciseCoinTossLevel;
    public int chargedWeakCoinFlipLevel;
    public int goldenRainLevel;
    public int chargedMediumCoinFlipLevel;
    public int luckIsOnYourSideLevel;
    public int chargedStrongCoinFlipLevel;
    public int monetayBlessingLevel;
    public int chargedUltraStrongCoinFlipLevel;
    public int goldenFrogLevel;
    public int spellBookLevel;
}

[System.Serializable]
public struct PurchaseData
{
    public bool frogPurchased;
    public bool handPurchased;
}

public enum PurchaseType
{
    FROG,
    HAND
}