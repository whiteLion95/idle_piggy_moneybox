using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class UI_RunnersAutoLevelProgressBar : MonoBehaviour
{
    [SerializeField] private TMP_Text levelText;
    [SerializeField] private TMP_Text speedText;
    [SerializeField] private Image arrow;

    private Slider _slider;
    private RunnersManager _runnersManager;
    private ExpController _expController;

    private void Awake()
    {
        _slider = GetComponent<Slider>();
    }

    // Start is called before the first frame update
    private void Start()
    {
        _runnersManager = RunnersManager.Instance;
        _expController = _runnersManager.ExpController;

        UpdateSliderState(_runnersManager.ExpController.CurrentLevel);

        _expController.OnExpGained += SetSliderValue;
        _expController.OnLevelUp += HandleLevelUp;
    }

    private void OnDisable()
    {
        _expController.OnExpGained -= SetSliderValue;
        _expController.OnLevelUp -= HandleLevelUp;
    }

    private void SetSliderValue(float value)
    {
        _slider.value = value;
    }

    private void SetSliderMaxValue(int value)
    {
        _slider.maxValue = value;
    }

    private void UpdateSliderState(int level)
    {
        SetSliderMaxValue(Mathf.RoundToInt(_expController.GetCurExpToLevelUp()));

        SetSliderValue(_expController.CurExp);
        levelText.text = "LV. " + (level + 1);

        speedText.text = "Speed x" + _runnersManager.GetStatValue(Stat.SPEED).ToString("F2").TrimEnd('0').TrimEnd('.');
    }

    private void TurnOnArrow()
    {
        arrow.gameObject.SetActive(true);
    }

    private void HandleLevelUp(int level)
    {
        UpdateSliderState(level);
        TurnOnArrow();
    }
}
