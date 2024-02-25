using System.Collections;
using System.Collections.Generic;
using Deslab.Scripts.Deslytics.Ads;
using Deslab.Utils;
using UnityEngine;
using UnityEngine.UI;

public class UI_UnitButton : MonoBehaviour, ISaveable
{
    [field: SerializeField] public UI_UpgradePanel MyPanel;
    [field: SerializeField] public UnitButtonType ButtonType;

    [SerializeField] private GameObject progressNumbers;
    [SerializeField] private ReducedBigText curProgText;
    [SerializeField] private ReducedBigText unlockPriceText;

    [field: SerializeField] public GameObject RedDot { get; private set; }

    public static System.Action OnFirstPurchase;
    public static System.Action<UnitButtonType> OnButtonActivated;

    private Slider _slider;
    private UpgradesManager _upManager;
    private float _unlockPrice;

    public Button Button { get; private set; }
    public Mask FocusMark { get; private set; }
    public UnitButtonData Data { get; private set; }
    public UI_UnitButton NextButton { get; set; }

    private void Awake()
    {
        Button = GetComponent<Button>();
        FocusMark = GetComponentInChildren<Mask>(true);
        _slider = GetComponentInChildren<Slider>(true);

        LoadData();
    }

    private void Start()
    {
        _upManager = UpgradesManager.Instance;
        _unlockPrice = _upManager.GetUnlockPrice(ButtonType);

        Init();
    }

    private void OnEnable()
    {
        if (Data.enabled)
        {
            OnFirstPurchase += HandlerOnFirstPurchase;
            if (!Data.activated)
                BalanceManager.OnMoneyChanged += HandlerOnMoneyChanged;
            else
                MyPanel.OnEnoughMoney += (x) => RedDot.SetActive(x);
        }
    }

    private void OnDisable()
    {
        if (Data.enabled)
        {
            OnFirstPurchase -= HandlerOnFirstPurchase;
            if (!Data.activated) 
                BalanceManager.OnMoneyChanged -= HandlerOnMoneyChanged;
        }
    }

    private void Init()
    {
        if (Data.enabled)
        {
            gameObject.SetActive(true);
            _slider.maxValue = _unlockPrice;

            if (!Data.activated)
            {
                Data.activatingProgress = BalanceManager.Instance.Money;
                SetNotActiveState();
                CheckIfActivated();
            }
            else
            {
                SetActiveState();
            }
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    private void HandlerOnMoneyChanged(float money)
    {
        ProgressToActivate(money);
    }

    private void ProgressToActivate(float value)
    {
        if (!Data.activated)
        {
            Data.activatingProgress = value;

            CheckIfActivated();
        }
    }

    private void CheckIfActivated()
    {
        if (Data.activatingProgress >= _unlockPrice)
        {
            Data.activated = true;
            SetActiveState();
            BalanceManager.OnMoneyChanged -= HandlerOnMoneyChanged;
            MyPanel.OnEnoughMoney += (x) => RedDot.SetActive(x);
        }

        _slider.value = Data.activatingProgress;
        curProgText.SetValue(Data.activatingProgress);
    }

    private void SetActiveState()
    {
        Button.interactable = true;
        progressNumbers.SetActive(false);
        Data.activatingProgress = _unlockPrice;
        _slider.value = Data.activatingProgress;
        OnButtonActivated?.Invoke(ButtonType);
    }

    private void SetNotActiveState()
    {
        Button.interactable = false;
        progressNumbers.SetActive(true);
        unlockPriceText.SetValue(_unlockPrice);
        RedDot.SetActive(false);
    }

    public void SetFocus(bool value)
    {
        FocusMark.gameObject.SetActive(value);

        if (value)
        {
            MyPanel.Window.ShowWindow();
            CameraMover.MoveToEnd();
            VibrationExtention.LightVibrate();
        }
        else
        {
            MyPanel.Window.HideWindow();
            //CameraMover.MoveToStart();
        }
    }

    private void HandlerOnFirstPurchase()
    {
        if (NextButton != null && !NextButton.isActiveAndEnabled)
        {
            NextButton.Enable();
        }
    }

    public void Enable()
    {
        Data.enabled = true;
        Init();
    }

    #region Data management
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

    private void SetSavingKey()
    {
        switch (ButtonType)
        {
            case UnitButtonType.MONEY_JAR:
                _savingKey = "Money jar button";
                break;
            case UnitButtonType.RUNNER:
                _savingKey = "Runner button";
                break;
            case UnitButtonType.HAND:
                _savingKey = "Hand button";
                break;
            case UnitButtonType.FROG:
                _savingKey = "Frog button";
                break;
        }
    }

    private string _savingKey;

    public void SaveData()
    {
        SaveManager.Save(_savingKey, Data);
    }

    public void LoadData()
    {
        SetSavingKey();

        Data = SaveManager.Load<UnitButtonData>(_savingKey);

        if (Data == null) Data = new UnitButtonData();

        if (ButtonType == UnitButtonType.MONEY_JAR)
            Data.enabled = true;
    }
    #endregion
}

public enum UnitButtonType
{
    MONEY_JAR,
    RUNNER,
    HAND,
    FROG
}

[System.Serializable]
public class UnitButtonData
{
    public bool enabled;
    public float activatingProgress;
    public bool activated;
}
