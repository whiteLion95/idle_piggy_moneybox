using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_RunnersStats : MonoBehaviour
{
    [SerializeField] private ExpBar _expBar;
    [SerializeField] private ReducedBigText _coinValueText;
    [SerializeField] private TMP_Text _runnersAmountText;
    [SerializeField] private TMP_Text _runnersSpeedText;
    [SerializeField] private TMP_Text _runnersExpRewardText;

    private RunnersManager _runnersManager;

    void Start()
    {
        _runnersManager = RunnersManager.Instance;
        _expBar.Init(_runnersManager.ExpController);
        SetStats();

        _runnersManager.OnStatUpdated += HandleStatUpdated;
        _runnersManager.OnNewRunnerSpawned += HandleRunnersAmountChanged;
        _runnersManager.OnRunnerDespawned += HandleRunnersAmountChanged;
        _runnersManager.OnLevelUp += HandleLevelUp;
        Runner.OnSpeedSetStatic += HandleSpeedChanged;
    }

    private void OnDestroy()
    {
        Runner.OnSpeedSetStatic -= HandleSpeedChanged;
    }

    private void SetStats()
    {
        _coinValueText.SetValue(_runnersManager.CoinValue, true);
        _runnersAmountText.text = _runnersManager.RunnersAmount.ToString();
        _runnersSpeedText.text = Runner.CurrentSpeed.ToString();
        _runnersExpRewardText.text = _runnersManager.ExpReward.ToString("F2");
    }

    private void HandleStatUpdated(Stat stat)
    {
        switch (stat)
        {
            case Stat.POWER_BONUS:
                _coinValueText.SetValue(_runnersManager.CoinValue, true);
                break;
            case Stat.EXP_GAIN:
                _runnersExpRewardText.text = _runnersManager.ExpReward.ToString("F2");
                break;
        }
    }

    private void HandleRunnersAmountChanged(Runner runner)
    {
        _runnersAmountText.text = _runnersManager.RunnersAmount.ToString();
    }

    private void HandleSpeedChanged(float value)
    {
        _runnersSpeedText.text = Runner.CurrentSpeed.ToString();
    }

    private void HandleLevelUp()
    {
        _coinValueText.SetValue(_runnersManager.CoinValue, true);
    }
}
