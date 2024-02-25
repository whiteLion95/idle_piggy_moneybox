using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_UnitButtonsManager : MonoBehaviour
{
    [SerializeField] UI_UpgradePanelsManager upgradePanel;

    private UI_UnitButton[] _uButtons;
    private UI_UnitButton _activeButton;

    public static UI_UnitButtonsManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;

        _uButtons = GetComponentsInChildren<UI_UnitButton>(true);
    }

    private void OnEnable()
    {
        upgradePanel.OnClose += UnFocus;
    }

    private void OnDestroy()
    {
        upgradePanel.OnClose -= UnFocus;
    }

    // Start is called before the first frame update
    void Start()
    {
        InitButtons();
    }

    private void InitButtons()
    {
        foreach (UI_UnitButton b in _uButtons)
        {
            b.Button.onClick.AddListener(() =>
            {
                ActionsOnClick(b);
            });
        }

        for (int i = 0; i < _uButtons.Length; i++)
        {
            if (i < _uButtons.Length - 1)
            {
                _uButtons[i].NextButton = _uButtons[i + 1];
            }
        }
    }

    private void ActionsOnClick(UI_UnitButton uButton)
    {
        if (_activeButton != null && _activeButton.Equals(uButton))
        {
            upgradePanel.Close();
        }
        else
        {
            upgradePanel.Show();
            FocusOnButton(uButton);
        }
    }

    private void FocusOnButton(UI_UnitButton uButton)
    {
        if (_activeButton != null)
        {
            _activeButton.SetFocus(false);
        }
        
        _activeButton = uButton;
        _activeButton.SetFocus(true);
    }

    private void UnFocus()
    {
        if (_activeButton != null)
        {
            _activeButton.SetFocus(false);
            _activeButton = null;
        }
    }

    public void SimulateButtonClick(UnitButtonType type)
    {
        foreach (var button in _uButtons)
        {
            if (type == button.ButtonType)
            {
                ActionsOnClick(button);
                break;
            }
        }
    }
}
