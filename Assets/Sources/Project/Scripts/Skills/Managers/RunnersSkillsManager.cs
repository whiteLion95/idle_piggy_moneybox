using System.Collections.Generic;
using System.Linq;
using Deslab.Deslytics;
using UnityEngine;

public class RunnersSkillsManager : SkillsManager
{
    [SerializeField] private List<RunnersSkill> skillList;

    private RunnersManager _runnersManager;

    private const string SAVE_KEY = "RunnersSkillsData";

    public static RunnersSkillsManager Instance { get; private set; }

    protected override void Awake()
    {
        Instance = this;
        base.Awake();
    }

    protected override void Start()
    {
        _runnersManager = RunnersManager.Instance;
        SetExpController();
        base.Start();
        SetSkillsCount();
    }

    public void PurchaseUpgrade(RunnersSkillName name)
    {
        if (name == RunnersSkillName.RunnerSacrifice)
        {
            if (_runnersManager.RunnersAmount == 0)
            {
                return;
            }
        }

        if (PurchaseUpgrade(GetSkill(name)))
        {
            UpgradeSkill(name);
        }
    }

    public RunnersSkill GetSkill(RunnersSkillName name)
    {
        RunnersSkill skill = skillList.First((s) => s.Name == name);
        return skill;
    }

    public override void ResetSkills()
    {
        if (_persistentData.spentSkillPoints > 0)
        {
            _persistentData.spentSkillPoints = 0;
            _curSkillPointsCount = _totalSkillPointsCount;

            foreach (Skill skill in skillList)
            {
                if (skill.CurLevel > 0)
                {
                    skill.Reset();
                    OnSkillReset?.Invoke(skill);
                }
            }

            OnSkillsReset?.Invoke();
        }
    }

    public float GetStatValue(Stat stat)
    {
        float value = 0f;

        foreach (RunnersSkill skill in skillList)
        {
            foreach (Stat s in skill.Stats)
            {
                if (s == stat)
                {
                    value += skill.GetValue(stat);
                }
            }
        }

        return value;
    }

    protected override void InitSkills()
    {
        if (skillList != null && skillList.Count > 0)
        {
            for (int i = 0; i < skillList.Count; i++)
            {
                Skill skill = skillList[i];
                skill.Init();

                if (skill.RefreshingSkill && skill.CurLevel > 0)
                    StartCoroutine(RefreshSkillRoutine(skill));
            }
        }
    }

    private void UpgradeSkill(RunnersSkillName name)
    {
        RunnersSkill skill = GetSkill(name);
        skill.Upgrade();
        OnSkillUpgrade?.Invoke(skill);
        
        DeslyticsManager.BuySkill(name.ToString(), skill.CurLevel);
        
        if (skill.RefreshingSkill && skill.CurLevel == 1)
            StartCoroutine(RefreshSkillRoutine(skill));
    }

    protected override void SetExpController()
    {
        _expController = _runnersManager.ExpController;
        _expController.OnLevelUp += HandleLevelUp;
    }

    #region PersistentData
    private void SaveSkills()
    {
        foreach (var skill in skillList)
        {
            skill.SaveData();
        }
    }

    protected override void SetSaveKey()
    {
        _saveKey = SAVE_KEY;
    }

    protected override void SaveData()
    {
        base.SaveData();
        SaveSkills();
    }
    #endregion
}

public enum RunnersSkillName
{
    CriticalStomp,
    Strength,
    WingFoot,
    BrainBoost,
    SuperCriticalStomp,
    RunnerMaturity,
    RunnerSacrifice,
    MoneyBags,
    HelpfulCarl
}
