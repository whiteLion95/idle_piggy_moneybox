using System.Collections.Generic;
using UnityEngine;

public class UI_FrogSpells : MonoBehaviour
{
    private Frog _frog;
    private ExpBar _expBar;
    private FrogSpellsController _frogSpellsController;
    private List<UI_FrogSpellButton> _spellsButtons;
    private UI_FrogSpellButton _selectedSpellButton;

    private void Awake()
    {
        _expBar = GetComponentInChildren<ExpBar>();
    }

    private void Start()
    {
        _frog = Frog.Instance;
        _frogSpellsController = FrogSpellsController.Instance;

        _expBar.Init(_frog.ExpController);
        InitSpellsButtons();

        _frogSpellsController.OnSpellSelected += HandleSpellSelected;
    }

    private void InitSpellsButtons()
    {
        _spellsButtons = new List<UI_FrogSpellButton>();
        _spellsButtons.AddRange(GetComponentsInChildren<UI_FrogSpellButton>());
        _selectedSpellButton = _spellsButtons[_frogSpellsController.CurSpellIndex];
    }

    private void HandleSpellSelected(FrogSpell spell)
    {
        UI_FrogSpellButton prevSelectedButton = _selectedSpellButton;

        if (!_spellsButtons[_frogSpellsController.CurSpellIndex].Equals(prevSelectedButton))
        {
            prevSelectedButton.Deselect();
            _selectedSpellButton = _spellsButtons[_frogSpellsController.CurSpellIndex];
        }
    }
}