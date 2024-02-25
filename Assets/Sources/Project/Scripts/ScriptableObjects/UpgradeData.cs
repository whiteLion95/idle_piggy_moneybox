using Sirenix.OdinInspector;
using System;
using UnityEngine;

[CreateAssetMenu(fileName = "UpgradesData", menuName = "ScriptableObjects/UpgradesData")]
public class UpgradeData : ScriptableObject
{
    [field: SerializeField] public CoinSource CoinSource { get; private set; }
    [field: SerializeField] public UpgradeType UpgradeType { get; private set; }
    [SerializeField] protected bool limitedLevels;
    [ShowIf("limitedLevels")][SerializeField] protected uint maxLevel;
    [SerializeField] protected ulong baseValue;
    [SerializeField] protected ulong baseCost;
    [SerializeField] protected bool needMultipliers;
    [ShowIf("needMultipliers")][SerializeField] protected float valueMultiplier;
    [ShowIf("needMultipliers")][SerializeField] protected float costMultiplier;

    private LevelData _levelData;

    //hand constants
    private const float KOEF_COST = 1.04999995f;
    private const float KOEF_PROFIT = 1f;

    public LevelData GetLevelData(int level)
    {
        _levelData.value = GetValue(level);
        _levelData.levelUpPrice = GetLevelUpPrice(level);

        if (limitedLevels)
            _levelData.maxLevel = maxLevel;
        else
            _levelData.maxLevel = uint.MaxValue;

        return _levelData;
    }

    public virtual float GetValue(int level)
    {
        float value = 0;

        switch (UpgradeType)
        {
            case UpgradeType.MONEY_JAR_COIN_VALUE:
                value = baseValue + (level * valueMultiplier);
                break;
            case UpgradeType.RUNNERS_COUNT:
                value = (int)baseValue + level;
                break;
            case UpgradeType.SKILLED_RUNNERS:
                if (level > 0)
                    value = 0.9f + level * 0.1f;
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
                if (level > 0)
                {
                    var koef = 1.0f;
                    if (level > 1) koef = 0.2f + level * 0.8f;
                    value = Mathf.RoundToInt(baseValue * koef * (float)(Math.Pow(KOEF_PROFIT, level - 1)));
                }
                break;
            case UpgradeType.GOLDEN_FROG:
                value = baseValue + 0.1f * Mathf.Pow(level, 1.15f);
                break;
            case UpgradeType.SPELLBOOK:
                if (level > 0)
                    value = 0.9f + level * 0.1f;
                break;
        }

        return value;
    }

    /// <summary>
    /// Price to level up from current level
    /// </summary>
    public virtual ulong GetLevelUpPrice(int level)
    {
        float price = 0f;

        switch (UpgradeType)
        {
            case UpgradeType.MONEY_JAR_COIN_VALUE:
                price = baseCost * Mathf.Pow(costMultiplier, level);
                break;
            case UpgradeType.RUNNERS_COUNT:
                if (level <= 25)
                    price = baseCost * Mathf.Pow(1.8f, level);
                else
                    price = 1000000000f + 100000000 * Mathf.Pow(level - 25, 1.5f);
                break;
            case UpgradeType.SKILLED_RUNNERS:
                price = baseCost * Mathf.Pow(1.8f, level);
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
                price = baseCost * Mathf.Pow(KOEF_COST, level);
                break;
            case UpgradeType.GOLDEN_FROG:
                price = baseCost * Mathf.Pow(1.06f, level);
                break;
            case UpgradeType.SPELLBOOK:
                price = baseCost * Mathf.Pow(1.8f, level);
                break;
        }

        return (ulong)price;
    }
}

public struct LevelData
{
    public float value;
    public float levelUpPrice;
    public uint maxLevel;
}

public enum UpgradeType
{
    //Money jar
    MONEY_JAR_COIN_VALUE,
    //Runners
    RUNNERS_COUNT,
    SKILLED_RUNNERS,
    //Hand
    WEAK_COIN_FLIP,
    MEDIUM_COIN_FLIP,
    THE_TRIPLE_COUP,
    ULTRA_STRONG_COIN_FLIP,
    PRECISE_COIN_TOSS,
    CHARGED_WEAK_COIN_FLIP,
    GOLDEN_RAIN,
    CHARGED_MEDIUM_COIN_FLIP,
    LUCK_IS_ON_YOUR_SIDE,
    CHARGED_STRONG_COIN_FLIP,
    MONETARY_BLESSING,
    CHARGED_ULTRA_STRONG_COIN_FLIP,
    //Frog
    GOLDEN_FROG,
    SPELLBOOK
}