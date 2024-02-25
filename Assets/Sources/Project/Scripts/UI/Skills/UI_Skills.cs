using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public abstract class UI_Skills : MonoBehaviour
{
    [SerializeField] protected TMP_Text skillPointsText;
    [SerializeField] protected Button resetButton;

    protected SkillsManager _skillsManager;

    protected virtual void Start()
    {
        StartCoroutine(InitWithDelay());
    }

    private IEnumerator InitWithDelay()
    {
        yield return new WaitForSeconds(0.1f);
        Init();
    }

    protected virtual void Init() 
    {
        _skillsManager.OnSkillPointsChanged += HandleSkillPointsChanged;

        skillPointsText.text = _skillsManager.CurSkillsCount.ToString() + "/" + _skillsManager.TotalSkillsCount.ToString();
        resetButton.onClick.AddListener(HandleResetButtonClicked);
    }

    private void HandleResetButtonClicked()
    {
        _skillsManager.ResetSkills();
        skillPointsText.text = _skillsManager.CurSkillsCount.ToString() + "/" + _skillsManager.TotalSkillsCount.ToString();
    }

    private void HandleSkillPointsChanged()
    {
        skillPointsText.text = _skillsManager.CurSkillsCount.ToString() + "/" + _skillsManager.TotalSkillsCount.ToString();
    }
}
