using System;
using TMPro;
using UnityEngine;

public class ExpBar : SliderBar
{
    [SerializeField] private TMP_Text _lvlText;
    [SerializeField] private ReducedBigText _curExpText;
    [SerializeField] private ReducedBigText _curExpToLevelUpText;

    public static Action<int> OnLevelUp;

    private ExpController _expController;
    private int _curLevel;

    public void Init(ExpController expController)
    {
        _expController = expController;
        InitSlider();

        _expController.OnExpGained += HandleExpGained;
    }

    public void InitSlider()
    {
        _curLevel = _expController.CurrentLevel;
        UpdateSlider();
        SetValue(_expController.CurExp);
    }

    private void UpdateSlider()
    {
        _lvlText.text = (_curLevel + 1).ToString();
        _curExpText.SetValue(_expController.CurExp);
        _curExpToLevelUpText.SetValue(_expController.GetCurExpToLevelUp());
        SetMaxValue(_expController.GetCurExpToLevelUp());
    }

    private void HandleExpGained(float curExp)
    {
        ChangeSliderValue(curExp);
        _curExpText.SetValue(_expController.CurExp);
        _curExpToLevelUpText.SetValue(_expController.GetCurExpToLevelUp());
    }

    private void ChangeSliderValue(float curExp)
    {
        if (curExp < _expController.GetCurExpToLevelUp())
        {
            ChangeValue(curExp);
            return;
        }

        Action onComplete = () =>
        {
            curExp -= _expController.GetCurExpToLevelUp();
            LevelUp();
            ChangeSliderValue(curExp);
            return;
        };

        ChangeValue(_expController.GetCurExpToLevelUp(), onComplete);
    }

    private void LevelUp()
    {
        _curLevel++;
        UpdateSlider();
        SetValue(0);
        OnLevelUp?.Invoke(_curLevel + 1);
    }
}
