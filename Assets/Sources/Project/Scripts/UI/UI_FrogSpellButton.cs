using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_FrogSpellButton : MonoBehaviour
{
    [SerializeField] private FrogSpellName spellName;
    [SerializeField] private GameObject selectionOutline;
    [SerializeField] private GameObject lockImage;
    [SerializeField] private TMP_Text lockInfo;
    [SerializeField] private TMP_Text expRewardText;
    [SerializeField] private ReducedBigText moneyPerThrowText;
    [SerializeField] private GameObject description;

    public static Action<UI_FrogSpellButton> OnSelected;

    private Button _button;
    private FrogSpellsController _spellsController;
    private bool _isOpened;
    private bool _isSelected;
    private FrogSpell _mySpell;
    private ExpController _expController;
    private Frog _frog;

    public FrogSpell MySpell => _mySpell;

    private void Awake()
    {
        _button = GetComponentInChildren<Button>();

        _button.onClick.AddListener(HandleButtonClicked);
    }

    private void Start()
    {
        _spellsController = FrogSpellsController.Instance;
        _expController = Frog.Instance.ExpController;
        _frog = Frog.Instance;

        Init();

        _expController.OnLevelUp += HandleLevelUp;
        _frog.OnSkillUpgraded += HandleSkillUpgraded;
    }

    private void Init()
    {
        _mySpell = _spellsController.GetSpell(spellName);
        lockInfo.text = "NEED LVL \n" + (_mySpell.levelToOpen + 1).ToString();

        CheckAvailability(_expController.CurrentLevel);
        CheckSelection();
        SetStats();
    }

    public void Select()
    {
        _isSelected = true;
        selectionOutline.gameObject.SetActive(true);
        Debug.Log(selectionOutline.gameObject.name + "is active: " + selectionOutline.gameObject.activeSelf);
        description.SetActive(true);
        OnSelected?.Invoke(this);
        SetStats();
    }

    public void Deselect()
    {
        _isSelected = false;
        selectionOutline.gameObject.SetActive(false);
        description.SetActive(false);
    }

    private void CheckSelection()
    {
        if (_spellsController.CurSpell.Equals(_mySpell))
            Select();
        else
            Deselect();
    }

    private void CheckAvailability(int level)
    {
        if (level >= _mySpell.levelToOpen)
        {
            _isOpened = true;
            lockImage.gameObject.SetActive(false);
        }
        else
        {
            _isOpened = false;
            lockImage.gameObject.SetActive(true);
        }
    }

    private void SetStats()
    {
        if (expRewardText)
            expRewardText.text = (_spellsController.CurSpell.expReward * _frog.GetStatValue(Stat.EXP_GAIN)).ToString("F0");
        if (moneyPerThrowText)
            moneyPerThrowText.SetValue(_frog.MoneyPerThrow);
    }

    private void HandleButtonClicked()
    {
        if (_isOpened && !_isSelected)
        {
            Select();
        }
    }

    private void HandleLevelUp(int level)
    {
        CheckAvailability(level);
    }

    private void HandleSkillUpgraded(FrogSkill skill)
    {
        if (moneyPerThrowText && skill.Stats.Contains(Stat.POWER_BONUS))
            moneyPerThrowText.SetValue(_frog.MoneyPerThrow);
        if (expRewardText && skill.Stats.Contains(Stat.EXP_GAIN))
            expRewardText.text = (_spellsController.CurSpell.expReward * _frog.GetStatValue(Stat.EXP_GAIN)).ToString("F0");
    }
}
