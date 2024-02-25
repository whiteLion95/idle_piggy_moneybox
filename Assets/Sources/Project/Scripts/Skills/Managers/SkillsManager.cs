using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SkillsManager : MonoBehaviour
{
    [System.Serializable]
    public struct PersistentData
    {
        public int spentSkillPoints;
    }

    [SerializeField] protected UpgradeType skillsUpgradeType;

    public Action OnSkillPointsChanged;
    public Action<Skill> OnSkillUpgrade;
    public Action<Skill> OnSkillReset;
    public Action OnSkillsReset;
    public Action<Skill> OnSkillStatsRefresh;

    protected List<Skill> skillList;
    protected int _totalSkillPointsCount;
    protected int _curSkillPointsCount;
    protected UpgradesManager _upManager;
    protected string _saveKey;
    protected PersistentData _persistentData;
    protected ExpController _expController;

    public int TotalSkillsCount => _totalSkillPointsCount;
    public int CurSkillsCount => _curSkillPointsCount;
    public UpgradeType SkillsUpgradeType => skillsUpgradeType;

    protected virtual void Awake()
    {
        SetSaveKey();
        LoadData();
    }

    protected virtual void Start()
    {
        InitSkills();

        _upManager = UpgradesManager.Instance;

        _upManager.OnUpgrade += HandleUpgrade;
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    protected virtual void OnApplicationQuit()
    {
        SaveData();
    }

    protected virtual void OnDestroy()
    {
        SaveData();
    }

    protected virtual void OnApplicationPause(bool pause)
    {
        if (pause)
            SaveData();
    }

    public abstract void ResetSkills();

    protected bool PurchaseUpgrade(Skill skill)
    {
        if (_curSkillPointsCount > 0)
        {
            int price = skill.GetPriceInSkillPoints();

            if (_curSkillPointsCount >= price)
            {
                _curSkillPointsCount -= price;
                _persistentData.spentSkillPoints += price;
                OnSkillPointsChanged?.Invoke();
                return true;
            }
        }

        return false;
    }

    protected abstract void SetExpController();

    protected virtual void SetSkillsCount()
    {
        int prevTotalSkillPointsCount = _totalSkillPointsCount;
        float skillPointsPerLevel = _upManager.GetValue(skillsUpgradeType);
        _totalSkillPointsCount = Mathf.RoundToInt((_expController.CurrentLevel + 1) * skillPointsPerLevel);
        _curSkillPointsCount = _totalSkillPointsCount - _persistentData.spentSkillPoints;

        if (_totalSkillPointsCount != prevTotalSkillPointsCount)
            OnSkillPointsChanged?.Invoke();
    }

    protected abstract void InitSkills();

    protected IEnumerator RefreshSkillRoutine(Skill skill)
    {
        while (isActiveAndEnabled)
        {
            yield return new WaitForSeconds(skill.RefreshRate);
            OnSkillStatsRefresh?.Invoke(skill);
        }
    }

    protected virtual void HandleUpgrade(UpgradeType type, CoinSource coinSource)
    {
        if (type == skillsUpgradeType)
        {
            SetSkillsCount();
        }
    }

    protected void HandleLevelUp(int level)
    {
        SetSkillsCount();
    }

    protected abstract void SetSaveKey();

    #region Persistent Data
    protected void LoadData()
    {
        _persistentData = SaveManager.Load<PersistentData>(_saveKey);
    }

    protected virtual void SaveData()
    {
        SaveManager.Save(_saveKey, _persistentData);
    }
    #endregion
}
