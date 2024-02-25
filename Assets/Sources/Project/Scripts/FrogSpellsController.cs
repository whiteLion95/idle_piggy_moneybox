using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(ExpController))]
public class FrogSpellsController : MonoBehaviour
{
    [System.Serializable]
    public struct PersistentData
    {
        public int curSpell;
    }

    [SerializeField] private List<FrogSpell> spells;
    [SerializeField] private float goldenBlessingMultiplier = 10f;
    [SerializeField] private float goldenBlessingDuration = 10f;

    public Action<FrogSpell> OnSpellCast;
    public Action<FrogSpell> OnSpellSelected;

    private ExpController _expController;
    private PersistentData _persistentData;
    private Frog _frog;
    private ParticlesManager _particlesManager;
    private MoneyJar _moneyJar;
    private CoinsManager _coinsManager;

    private const string SAVE_KEY = "Frog spells data";

    public static FrogSpellsController Instance { get; private set; }
    public FrogSpell CurSpell { get => spells[_persistentData.curSpell]; }
    public int CurSpellIndex => _persistentData.curSpell;

    private void Awake()
    {
        Instance = this;
        LoadData();

        UI_FrogSpellButton.OnSelected += HandleSpellButtonSelected;
    }

    private void Start()
    {
        _expController = GetComponent<ExpController>();
        _frog = Frog.Instance;
        _particlesManager = ParticlesManager.Instance;
        _moneyJar = MoneyJar.Instance;
        _coinsManager = CoinsManager.Instance;

        InitSpells();

        _expController.OnLevelUp += HandleLevelUp;
    }

    private void OnDisable()
    {
        UI_FrogSpellButton.OnSelected -= HandleSpellButtonSelected;
        StopAllCoroutines();
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

    public void CastCurrentSpell()
    {
        switch (CurSpell.name)
        {
            case FrogSpellName.MoneyMagnet:
                _particlesManager.PlayParticleOnce(Particles.FrogMoneySpell, transform.position, transform.rotation);
                break;
            case FrogSpellName.BigCoin:
                break;
            case FrogSpellName.GoldenBlessing:
                StartCoroutine(GoldenBlessingRoutine());
                break;
        }

        OnSpellCast?.Invoke(CurSpell);
    }

    private void InitSpells()
    {

    }

    private void HandleLevelUp(int level)
    {

    }

    public FrogSpell GetSpell(FrogSpellName spellName)
    {
        return spells.First((s) => s.name == spellName);
    }

    private IEnumerator GoldenBlessingRoutine()
    {
        ParticleSystem particles = _particlesManager.PlayParticle(Particles.FrogGoldenBlessing, _moneyJar.transform.position, Quaternion.identity);
        float prevTapCoinValueMultiplier = _coinsManager.TapCoinValueMultiplier;
        _coinsManager.TapCoinValueMultiplier = goldenBlessingMultiplier;
        yield return new WaitForSeconds(goldenBlessingDuration);
        _coinsManager.TapCoinValueMultiplier = prevTapCoinValueMultiplier;
        _particlesManager.DespawnParticle(particles);
    }

    private void HandleSpellButtonSelected(UI_FrogSpellButton spellButton)
    {
        _persistentData.curSpell = spells.IndexOf(spellButton.MySpell);
        OnSpellSelected?.Invoke(CurSpell);
    }

    #region Persistent Data
    protected void LoadData()
    {
        _persistentData = SaveManager.Load<PersistentData>(SAVE_KEY);
    }

    protected virtual void SaveData()
    {
        SaveManager.Save(SAVE_KEY, _persistentData);
    }
    #endregion
}

public enum FrogSpellName
{
    MoneyMagnet,
    BigCoin,
    GoldenBlessing
}

[System.Serializable]
public class FrogSpell
{
    public FrogSpellName name;
    public int levelToOpen;
    public bool opened;
    public bool coinBased;
    [ShowIf("coinBased")] public Coin coinPrefab;
    [ShowIf("coinBased")] public int expReward;
    [ShowIf("coinBased")] public ulong coinsReward;
    [ShowIf("coinBased")] public int numberOfCoins;
}