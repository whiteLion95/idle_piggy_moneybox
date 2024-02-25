using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TutorialsController : MonoBehaviour, ISaveable
{
    [SerializeField] private TutorialStep[] steps;

    private GoalsManager _goalsManager;

    public Action OnCompleted;

    public static TutorialsController Instance { get; private set; }

    public TutorialData SavingData { get { return _saveData; } }

    private void Awake()
    {
        Instance = this;
        LoadData();
    }

    // Start is called before the first frame update
    void Start()
    {
        HideAllSteps();

        _goalsManager = GoalsManager.Instance;

        if (_saveData.step == 0)
            _goalsManager.OnGoalChoose += OnGoalChooseHandler;

        if (!_saveData.completed)
        {
            DoStep(_saveData.step);

            if (_saveData.step == 2)
            {
                UI_UnitButtonsManager.Instance.SimulateButtonClick(UnitButtonType.MONEY_JAR);
            }
        }
    }

    private void OnEnable()
    {
        if (_saveData.step == 0) UI_UnitButton.OnButtonActivated += OnUpgradeButtonActivated;
        TutorialStep.OnHide += OnStepHide;
    }

    private void OnDisable()
    {
        UI_UnitButton.OnButtonActivated -= OnUpgradeButtonActivated;
        TutorialStep.OnHide -= OnStepHide;
    }

    private void OnGoalChooseHandler()
    {
        DoStep(0);
    }

    private void HideAllSteps()
    {
        foreach (var step in steps)
        {
            step.gameObject.SetActive(false);
        }
    }

    private void DoStep(int step)
    {
        TutorialStep curStep = steps[step];
        curStep.Show();

        switch (step)
        {
            case 0:
                break;
            case 1:
                CoinsManager.Instance.CanThrowCoinsFromTap = false;
                break;
            case 2:
                CoinsManager.Instance.CanThrowCoinsFromTap = false;
                break;
            default:
                Debug.LogWarning("There are no steps with number " + step);
                break;
        }
    }

    private void OnUpgradeButtonActivated(UnitButtonType type)
    {
        if (type == UnitButtonType.MONEY_JAR)
        {
            steps[0].Hide();
        }
    }

    private void OnStepHide(TutorialStep hiddenStep)
    {
        if (hiddenStep.Equals(steps[steps.Length - 1]))
        {
            _saveData.completed = true;
            CoinsManager.Instance.CanThrowCoinsFromTap = true;
            UI_UpgradePanelsManager.Instance.Close();
            _goalsManager.OnGoalChoose -= OnGoalChooseHandler;
            OnCompleted?.Invoke();
        }
        else
        {
            _saveData.step++;
            DoStep(_saveData.step);
        }
    }

    #region Data management
    private TutorialData _saveData;

    private void OnDestroy()
    {
        SaveData();
    }

    private void OnApplicationPause(bool pause)
    {
        SaveData();
    }

    private void OnApplicationQuit()
    {
        SaveData();
    }

    private const string SAVE_KEY = "Tutorial data";
    public void LoadData()
    {
        _saveData = SaveManager.Load<TutorialData>(SAVE_KEY);
    }

    public void SaveData()
    {
        SaveManager.Save(SAVE_KEY, _saveData);
    }
    #endregion
}

[System.Serializable]
public struct TutorialData
{
    public bool completed;
    public int step;
}
