using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(ExpController))]
public class Frog : StaticCoinThrower
{
    [SerializeField] private List<StatValue> statsValues;
    [SerializeField] private float minThrowCapacity = 15f;

    public static Action OnThrow;
    public Action<float> OnThrowRateChanged;
    public Action<FrogSkill> OnSkillUpgraded;
    public Action<Stat> OnStatUpdated;

    private ExpController _expController;
    private FrogSpellsController _spellsController;
    private RunnersManager _runnersManager;
    private FrogSkillsManager _skillsManager;
    private bool _boostedManaRegen;
    private bool _manaBoostIsAvailable = true;

    private const string THROW_TRIGGER = "Throw";
    private const string SAVE_KEY = "Frog timer progress";

    public override CoinSource CoinSource => CoinSource.FROG;
    public static Frog Instance { get; private set; }
    public ExpController ExpController => _expController;

    protected override void Awake()
    {
        Instance = this;
        base.Awake();

        _expController = GetComponent<ExpController>();

        OnLastCoinGained += HandleLastCoinGained;
    }

    protected override void Start()
    {
        _runnersManager = RunnersManager.Instance;
        _spellsController = FrogSpellsController.Instance;
        _expController.Init(_persistentData.level, _persistentData.curExp);
        _skillsManager = FrogSkillsManager.Instance;

        _skillsManager.OnSkillUpgrade += HandleSkillUpgraded;
        _skillsManager.OnSkillReset += HandleSkillReset;
        _spellsController.OnSpellCast += HandleSpellCasted;
        _spellsController.OnSpellSelected += HandleSpellSelected;
        InitStats();

        base.Start();
    }

    protected override void OnEnable()
    {
        base.OnEnable();

    }

    protected override void OnDisable()
    {
        base.OnDisable();

        StopAllCoroutines();

        OnLastCoinGained -= HandleLastCoinGained;
    }

    protected override void SetSaveKey()
    {
        _saveKey = SAVE_KEY;
    }

    protected override void HandleUpgrade(UpgradeType upType, CoinSource coinSource)
    {
        base.HandleUpgrade(upType, coinSource);

        if (upType == UpgradeType.GOLDEN_FROG)
        {
            CalculateThrowRate();
        }
    }

    protected override void CalculateThrowRate()
    {
        base.CalculateThrowRate();

        float prevThrowRate = _throwRate;
        float newThrowRate = _upManager.GetValue(UpgradeType.GOLDEN_FROG);

        if (newThrowRate != prevThrowRate)
        {
            _throwRate = newThrowRate;
            OnThrowRateChanged?.Invoke(ThrowRate);
        }
    }

    protected override float GetThrowRate()
    {
        float throwRate = base.GetThrowRate() + _runnersManager.GetStatValue(Stat.MANA_REGEN);

        if (_boostedManaRegen)
            throwRate += GetStatValue(Stat.MANA_REGEN_ADDING);

        return throwRate;
    }

    protected override float GetThrowCapacity()
    {
        float throwCapacity = base.GetThrowCapacity() - GetStatValue(Stat.MANA_COST_REDUCTION);

        if (throwCapacity < minThrowCapacity)
            throwCapacity = minThrowCapacity;

        return throwCapacity;
    }

    private void InitStats()
    {
        for (int i = 0; i < statsValues.Count; i++)
        {
            StatValue stat = statsValues[i];
            stat.SetValue(_skillsManager.GetStatValue(stat.Name));
        }

        MoneyPerThrow = (ulong)(_spellsController.CurSpell.coinsReward + _spellsController.CurSpell.coinsReward * GetStatValue(Stat.POWER_BONUS));
    }

    public float GetStatValue(Stat name)
    {
        return statsValues.First((s) => s.Name == name).Value;
    }

    private void UpdateStats(Skill skill)
    {
        foreach (var stat in skill.Stats)
        {
            StatValue statValue = statsValues.First((s) => s.Name == stat);
            statValue.SetValue(_skillsManager.GetStatValue(stat));

            switch (stat)
            {
                case Stat.POWER_BONUS:
                    MoneyPerThrow = (ulong)(_spellsController.CurSpell.coinsReward + _spellsController.CurSpell.coinsReward * GetStatValue(Stat.POWER_BONUS));
                    break;
                case Stat.MANA_REGEN:
                    break;
            }

            OnStatUpdated?.Invoke(stat);
        }
    }

    private void HandleSpellCasted(FrogSpell spell)
    {
        switch (spell.name)
        {
            case FrogSpellName.MoneyMagnet:
            case FrogSpellName.BigCoin:
                _coinsCount = spell.numberOfCoins;
                break;
            case FrogSpellName.GoldenBlessing:
                break;
        }

        CheckManaBoost();
        _runnersManager.ExpController.GetExp(GetStatValue(Stat.RUNNER_EXP));
    }

    private void CheckManaBoost()
    {
        if (_skillsManager.GetSkill(FrogSkillName.AscendAndWizardry).CurLevel > 0)
        {
            if (_manaBoostIsAvailable)
            {
                StartCoroutine(ManaBoostRoutine());
                StartCoroutine(ManaBoostCooldown());
            }
        }
    }

    private IEnumerator ManaBoostRoutine()
    {
        _boostedManaRegen = true;
        yield return new WaitForSeconds(GetStatValue(Stat.MANA_BOOST_DURATION));
        _boostedManaRegen = false;
    }

    private IEnumerator ManaBoostCooldown()
    {
        _manaBoostIsAvailable = false;
        yield return new WaitForSeconds(GetStatValue(Stat.MANA_BOOST_COOLDOWN));
        _manaBoostIsAvailable = true;
    }

    //protected override void HandlerOnCoinGained(Coin coin)
    //{
    //    if (coin.Source == CoinSource.FROG)
    //    {

    //    }
    //}

    protected override void HandleCountDownFinished()
    {
        base.HandleCountDownFinished();
        _spellsController.CastCurrentSpell();
    }

    private void HandleLastCoinGained(Coin coin)
    {
        if (coin.Source == CoinSource.FROG)
        {
            _expController.GetExp(_spellsController.CurSpell.expReward * GetStatValue(Stat.EXP_GAIN));
        }
    }

    private void HandleSkillUpgraded(Skill skill)
    {
        UpdateStats(skill);

        OnSkillUpgraded?.Invoke(skill as FrogSkill);
    }

    private void HandleSkillReset(Skill skill)
    {
        HandleSkillUpgraded(skill);
    }

    private void HandleSpellSelected(FrogSpell spell)
    {
        MoneyPerThrow = (ulong)(_spellsController.CurSpell.coinsReward + _spellsController.CurSpell.coinsReward * GetStatValue(Stat.POWER_BONUS));
    }

    public override void SaveData()
    {
        if (_expController)
        {
            _persistentData.level = _expController.CurrentLevel;
            _persistentData.curExp = _expController.CurExp;
        }

        base.SaveData();
    }
}
