using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Deslab.UI;

public class TutorialStep : MonoBehaviour
{
    [SerializeField] private Image hand;
    [SerializeField] private Button activeButton;
    [SerializeField] private CanvasGroupWindow groupToDisable;

    private CanvasGroup _buttonGroup;

    public static System.Action<TutorialStep> OnHide;

    private void Awake()
    {
        groupToDisable.OnWindowShowed += OnGroupToDisableShowedHandler;
    }

    private void OnGroupToDisableShowedHandler()
    {
        groupToDisable.CanvasGroup.blocksRaycasts = false;
    }

    public void Show()
    {
        gameObject.SetActive(true);
        hand.gameObject.SetActive(true);

        if (groupToDisable != null)
        {
            groupToDisable.CanvasGroup.blocksRaycasts = false;
        }
        
        if (activeButton != null)
        {
            _buttonGroup = activeButton.gameObject.AddComponent<CanvasGroup>();
            _buttonGroup.blocksRaycasts = true;
            _buttonGroup.interactable = true;
            _buttonGroup.ignoreParentGroups = true;

            activeButton.onClick.AddListener(Hide);
        }
    }

    public void Hide()
    {
        gameObject.SetActive(false);
        hand.gameObject.SetActive(false);

        if (_buttonGroup != null)
            Destroy(_buttonGroup);

        if (groupToDisable != null)
        {
            groupToDisable.CanvasGroup.blocksRaycasts = true;
        }

        OnHide?.Invoke(this);

        groupToDisable.OnWindowShowed -= OnGroupToDisableShowedHandler;
        Destroy(gameObject);
    }
}
