using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CurrentGoalMenuBlock : GoalMenuButton
{
    [SerializeField] private Image enoughMoneyBackground;
    [SerializeField] private Button purchaseButton;
    [SerializeField] private ReducedBigText purchaseText;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private GameObject _parentGameObject;

    protected override void Start()
    {
        base.Start();
        purchaseButton.onClick.AddListener(_manager.PurchaseCurrentGoal);
    }

    protected override void Init()
    {
        base.Init();

        if (_manager.CurrentGoal != null)
            nameText.text = _manager.CurrentGoal.name;
        
        if(_manager.AllGoalsComplete)
            _parentGameObject.SetActive(false);
    }

    protected override void SetReadyToBuyState()
    {
        progressNumbers.SetActive(false);
        _readyToBuy = true;
        enoughMoneyBackground.gameObject.SetActive(true);
        slider.gameObject.SetActive(false);
        purchaseButton.gameObject.SetActive(true);
        purchaseText.SetValue(_manager.CurrentGoal.price);
        checkMark.SetActive(false);
    }

    protected override void SetSavingUpState()
    {
        progressNumbers.SetActive(true);
        _readyToBuy = false;
        enoughMoneyBackground.gameObject.SetActive(false);
        slider.gameObject.SetActive(true);
        purchaseButton.gameObject.SetActive(false);
        checkMark.SetActive(false);
    }
}
