using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class GoalMenuButton : MonoBehaviour
{
    [SerializeField] protected Image[] goalIcons;
    [SerializeField] protected GameObject progressNumbers;
    [SerializeField] protected ReducedBigText unlockPriceText;
    [SerializeField] protected GameObject checkMark;
    [SerializeField] protected Slider slider;
    [SerializeField] private DOTweenAnimation blinking;
    [SerializeField] private Color completeColor;
    [SerializeField] private Color saveUpColor;
    [SerializeField] private Image saveUpIcon;
    [SerializeField] private Image saveUpBackground;

    protected GoalsManager _manager;
    protected BalanceManager _balanceManager;

    protected bool _readyToBuy;
    //private Tween _blinking;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        _manager = GoalsManager.Instance;
        _balanceManager = BalanceManager.Instance;
        Init();
        _manager.OnGoalChoose += Init;
    }

    private void OnEnable()
    {
        BalanceManager.OnMoneyChanged += OnMoneyChangedHandler;
    }

    private void OnDisable()
    {
        BalanceManager.OnMoneyChanged -= OnMoneyChangedHandler;
    }

    protected virtual void Init()
    {
        if (_manager.CurrentGoal != null)
        {
            Goal currentGoal = _manager.GetCurrentGoal();

            foreach (Image icon in goalIcons)
            {
                icon.sprite = currentGoal.icon;
            }

            slider.maxValue = currentGoal.price;
            unlockPriceText.SetValue(currentGoal.price);

            if (_balanceManager.Money < (ulong)currentGoal.price)
            {
                SetSavingUpState();
                UpdateState();
            }
            else
            {
                SetReadyToBuyState();
            }
        }

        if (_manager.AllGoalsComplete)
        {
            progressNumbers.SetActive(false);
            foreach (Image icon in goalIcons)
            {
                icon.color = completeColor;
            }
        }
    }

    protected virtual void SetReadyToBuyState()
    {
        progressNumbers.SetActive(false);
        slider.value = slider.maxValue;
        checkMark.SetActive(true);
        _readyToBuy = true;
        blinking.DOPlay();
        saveUpIcon.color = completeColor;
        saveUpBackground.color = completeColor;
    }

    protected virtual void SetSavingUpState()
    {
        progressNumbers.SetActive(true);
        checkMark.SetActive(false);
        _readyToBuy = false;
        blinking.DORewind();
        saveUpIcon.color = saveUpColor;
        saveUpBackground.color = saveUpColor;
    }

    private void UpdateState()
    {
        slider.value = _balanceManager.Money;
    }

    private void OnMoneyChangedHandler(float money)
    {
        if (_manager.CurrentGoal != null)
        {
            if (money < _manager.CurrentGoal.price)
            {
                if (_readyToBuy)
                    SetSavingUpState();

                UpdateState();
            }
            else
            {
                if (!_readyToBuy)
                    SetReadyToBuyState();
            }
        }
    }
}