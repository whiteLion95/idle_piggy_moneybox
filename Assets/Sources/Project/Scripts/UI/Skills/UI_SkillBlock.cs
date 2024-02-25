using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public abstract class UI_SkillBlock : MonoBehaviour
{
    [SerializeField] protected int levelToOpen;
    [SerializeField] protected Button purchaseButton;
    [SerializeField] protected Image buttonDim;
    [SerializeField] protected TMP_Text levelText;
    [SerializeField] protected TMP_Text currencyText;
    [SerializeField] protected List<StatUI> statsUI;
    [SerializeField] protected Image blockDim;
    [SerializeField] protected TMP_Text needLevelText;

    protected SkillsManager _skillManager;
    protected ExpController _expController;
    protected UpgradesManager _upManager;
    protected bool _isOpened;
    protected bool _isAvailableForPurchase;
    protected Skill _mySkill;

    protected virtual void Awake()
    {
        purchaseButton.onClick.AddListener(HandlePurchaseButtonClicked);
    }

    protected virtual void Start()
    {
        _upManager = UpgradesManager.Instance;

        Init();

        if (_mySkill.RefreshingSkill)
            InvokeRepeating(nameof(SetStats), _mySkill.RefreshRate, _mySkill.RefreshRate);

        _expController.OnLevelUp += HandleLevelUp;
        _skillManager.OnSkillPointsChanged += HandleSkillPointsChanged;
        _skillManager.OnSkillUpgrade += HandleSkillUpgrade;
        _skillManager.OnSkillsReset += HandleSkillsReset;
        _skillManager.OnSkillStatsRefresh += HandleSkillStatsRefreshed;
    }

    protected virtual void Init()
    {
        SetSkill();

        blockDim.gameObject.SetActive(true);
        buttonDim.gameObject.SetActive(true);

        CheckLevel(_expController.CurrentLevel);
        CheckPurchaseAvailability();
        SetStats();
        SetPriceText();
        SetLevelText();

        needLevelText.text = "NEED LVL " + (levelToOpen + 1).ToString();
    }

    private void CheckLevel(int level)
    {
        if (level >= levelToOpen)
        {
            _isOpened = true;
            blockDim.gameObject.SetActive(false);
        }
        else
        {
            _isOpened = false;
            blockDim.gameObject.SetActive(true);
        }
    }

    private void CheckPurchaseAvailability()
    {
        if (_isOpened)
        {
            if (_skillManager.CurSkillsCount >= _mySkill.GetPriceInSkillPoints())
            {
                _isAvailableForPurchase = true;
                buttonDim.gameObject.SetActive(false);
            }
            else
            {
                _isAvailableForPurchase = false;
                buttonDim.gameObject.SetActive(true);
            }
        }
    }

    private void SetStats()
    {
        foreach (var statUI in statsUI)
        {
            float curValue = _mySkill.GetValue(statUI.name);
            float nextValue = _mySkill.GetValue(statUI.name, _mySkill.CurLevel + 1);

            statUI.SetValues(curValue, nextValue);
        }
    }

    private void SetPriceText()
    {
        currencyText.text = _mySkill.GetPriceInSkillPoints().ToString("F0");
    }

    private void SetLevelText()
    {
        levelText.text = "Level " + (_mySkill.CurLevel + 1);
    }

    protected abstract void SetSkill();

    protected abstract void PurchaseUpgrade();

    private void HandleLevelUp(int level)
    {
        CheckLevel(level);
    }

    private void HandleSkillPointsChanged()
    {
        CheckPurchaseAvailability();
    }

    private void HandleSkillUpgrade(Skill skill)
    {
        CheckPurchaseAvailability();

        if (skill.Equals(_mySkill))
        {
            SetStats();
            SetPriceText();
            SetLevelText();
        }
    }

    private void HandlePurchaseButtonClicked()
    {
        if (_isOpened && _isAvailableForPurchase)
        {
            PurchaseUpgrade();
        }
    }

    private void HandleSkillsReset()
    {
        if (_isOpened)
        {
            CheckPurchaseAvailability();
            SetStats();
            SetPriceText();
            SetLevelText();
        }
    }

    private void HandleSkillStatsRefreshed(Skill skill)
    {
        if (skill.Equals(_mySkill))
        {
            SetStats();
        }
    }
}

[System.Serializable]
public class StatUI
{
    public Stat name;
    public TMP_Text curValueText;
    public TMP_Text nextValueText;
    public bool floatFormat;
    public bool hundredBasedText = true;
    public bool hundredPlus = true;
    public bool percentText = true;

    public void SetValues(float curValue, float nextValue)
    {
        string tempCurValueText;
        string tempNextValueText;

        if (hundredBasedText)
        {
            curValue *= 100f;
            nextValue *= 100f;
        }

        if (hundredPlus)
        {
            curValue += 100f;
            nextValue += 100f;
        }

        if (!floatFormat)
        {
            tempCurValueText = curValue.ToString("F0");
            tempNextValueText = nextValue.ToString("F0");
        }
        else
        {
            tempCurValueText = curValue.ToString("F2").TrimEnd('0').TrimEnd('.');
            tempNextValueText = nextValue.ToString("F2").TrimEnd('0').TrimEnd('.');
        }

        if (percentText)
        {
            tempCurValueText += " %";
            tempNextValueText += " %";
        }

        curValueText.text = tempCurValueText;
        nextValueText.text = tempNextValueText;
    }
}
