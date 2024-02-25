using System.Collections.Generic;
using System.Linq;
using Deslab.Deslytics;
using UnityEngine;

public class FrogSkillsManager : SkillsManager
{
    [SerializeField] protected List<FrogSkill> skillList;

    private Frog _frog;

    private const string SAVE_KEY = "FrogSkillsData";

    public static FrogSkillsManager Instance { get; private set; }

    protected override void Awake()
    {
        Instance = this;
        base.Awake();

        StaticCoinThrower.OnSpawned += HandleStaticCoinThrowerSpawned;
    }

    protected override void Start()
    {
        _frog = Frog.Instance;

        base.Start();
    }

    private void OnDisable()
    {
        StaticCoinThrower.OnSpawned -= HandleStaticCoinThrowerSpawned;
    }

    public void PurchaseUpgrade(FrogSkillName name)
    {
        if (PurchaseUpgrade(GetSkill(name)))
        {
            UpgradeSkill(name);
        }
    }

    public FrogSkill GetSkill(FrogSkillName name)
    {
        FrogSkill skill = skillList.First((s) => s.Name == name);
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

        foreach (FrogSkill skill in skillList)
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

    private void UpgradeSkill(FrogSkillName name)
    {
        FrogSkill skill = GetSkill(name);
        skill.Upgrade();
        OnSkillUpgrade?.Invoke(skill);
        
        DeslyticsManager.BuySkill(name.ToString(), skill.CurLevel);

        if (skill.RefreshingSkill && skill.CurLevel == 1)
            StartCoroutine(RefreshSkillRoutine(skill));
    }

    protected override void SetExpController()
    {
        _expController = _frog.ExpController;
        _expController.OnLevelUp += HandleLevelUp;
    }

    private void HandleStaticCoinThrowerSpawned(StaticCoinThrower coinThrower)
    {
        if (coinThrower is Frog)
        {
            _frog = Frog.Instance;
            SetExpController();
            SetSkillsCount();
        }
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

public enum FrogSkillName
{
    ArcanePower,
    ScrollsOfKnowledge,
    Foresight,
    FrogFocus,
    EtherealWisdom,
    MultiRain,
    PhantomCast,
    AscendAndWizardry
}