using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

public abstract class Skill
{
    [System.Serializable]
    public struct PersistentData
    {
        public int level;
    }

    [SerializeField] protected List<Stat> stats;
    [SerializeField] protected bool refreshingSkill;
    [ShowIf("refreshingSkill")][SerializeField] protected float refreshRate = 1f;

    protected PersistentData _persistentData;
    protected string _saveKey;

    public int CurLevel => _persistentData.level;
    public List<Stat> Stats => stats;
    public bool RefreshingSkill => refreshingSkill;
    public float RefreshRate => refreshRate;

    public virtual void Init()
    {
        LoadData();
    }

    public void Upgrade()
    {
        _persistentData.level++;
    }

    public void Reset()
    {
        _persistentData.level = 0;
    }

    public int GetPriceInSkillPoints()
    {
        return Mathf.RoundToInt(1 + CurLevel / 10f);
    }

    public float GetValue(Stat statName)
    {
        foreach (var stat in stats)
        {
            if (stat == statName)
            {
                return CalculateValue(statName, _persistentData.level);
            }
        }

        return -1f;
    }

    public float GetValue(Stat statName, int level)
    {
        foreach (var stat in stats)
        {
            if (stat == statName)
            {
                return CalculateValue(statName, level);
            }
        }

        return -1f;
    }

    protected abstract float CalculateValue(Stat statName, int level);

    #region PersistentData
    protected void LoadData()
    {
        _persistentData = SaveManager.Load<PersistentData>(_saveKey);
    }

    public void SaveData()
    {
        SaveManager.Save(_saveKey, _persistentData);
    }
    #endregion
}

public enum Stat
{
    CRIT_CHANCE,
    CRIT_MULT,
    POWER_BONUS,
    SPEED,
    EXP_GAIN,
    SUPER_CRIT_CHANCE,
    SUPER_CRIT_MULT,
    MANA_REGEN,
    MANA_COST_REDUCTION,
    RUNNER_EXP,
    MANA_REGEN_ADDING,
    MANA_BOOST_DURATION,
    MANA_BOOST_COOLDOWN
}

[System.Serializable]
public class StatValue
{
    [SerializeField] private Stat name;
    [SerializeField] private float value;
    [SerializeField] private float baseValue;

    public Stat Name => name;
    public float Value => value;

    public void SetValue(float value)
    {
        this.value = baseValue + value;
    }
}
