using System.Collections.Generic;
using UnityEngine;
using Deslab.UI;
using Sirenix.OdinInspector;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private CanvasGroupWindow defaultScreen;

    [SerializeField] private CanvasGroupWindow goalsChooseScreen;

    [SerializeField] private CanvasGroupWindow dreamScreen;

    //[SerializeField] private CanvasGroupWindow dreamsScreen;
    [SerializeField] private List<CanvasGroupWindow> _screens;

    [Space] [FoldoutGroup("После завершения всех целей")] [SerializeField]
    private GameObject[] _listObjectsToHide;

    [FoldoutGroup("После завершения всех целей")] [SerializeField]
    private Image _goalButtonImage;

    [FoldoutGroup("После завершения всех целей")] [SerializeField]
    private Image _goalButtonIcon;

    [FoldoutGroup("После завершения всех целей")] [SerializeField]
    private Sprite _goalButtonIconSprite;

    private GoalsManager _goalsManager;

    public static bool DefaultScreenIsActive;
    public static UIManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    private void OnEnable()
    {
        CanvasGroupWindow.OnWindowStartShowing += HandlerWindowStartShowing;
        CanvasGroupWindow.OnWindowHid += HandlerWindowHid;
    }

    private void OnDisable()
    {
        CanvasGroupWindow.OnWindowStartShowing -= HandlerWindowStartShowing;
        CanvasGroupWindow.OnWindowHid -= HandlerWindowHid;
    }

    // Start is called before the first frame update
    void Start()
    {
        _goalsManager = GoalsManager.Instance;

        if (_goalsManager.CurrentGoal != null || _goalsManager.AmountGoalsComplete > 0)
            ShowOnlyOneScreen(defaultScreen);
        else
            ShowGoalsChooseScreen();

        _goalsManager.OnGoalChoose += () => goalsChooseScreen.HideWindow();
    }

    private void ShowOnlyOneScreen(CanvasGroupWindow showScreen)
    {
        foreach (CanvasGroupWindow screen in _screens)
        {
            if (!screen.Equals(showScreen))
            {
                CanvasGroup canvasGroup = screen.GetComponent<CanvasGroup>();
                if (canvasGroup.alpha != 0)
                    canvasGroup.alpha = 0;
                screen.DisableWindow();
            }
        }

        showScreen.ShowWindow();
        showScreen.EnableWindow();

        if (showScreen.Equals(defaultScreen))
            DefaultScreenIsActive = true;
    }

    public void ShowGoalsChooseScreen()
    {
        if (_goalsManager.AllGoalsComplete)
        {
            ShowOnlyOneScreen(dreamScreen);
            AfterCompleteAllGoals();
            Time.timeScale = 1;
        }
        else
        {
            ShowOnlyOneScreen(goalsChooseScreen);
            Time.timeScale = 0f;
            _goalsManager.InitGoalChooseBlocks();
        }
    }

    private void HandlerWindowStartShowing(CanvasGroupWindow screen)
    {
        if (_screens.Contains(screen))
        {
            if (!screen.Equals(defaultScreen))
            {
                defaultScreen.DisableWindow();
                DefaultScreenIsActive = false;
            }
        }
    }

    private void HandlerWindowHid(CanvasGroupWindow screen)
    {
        if (_screens.Contains(screen))
        {
            if (screen.Equals(goalsChooseScreen) && _goalsManager.AmountGoalsComplete > 0) return;

            if (!screen.Equals(defaultScreen))
            {
                defaultScreen.ShowWindow();
                defaultScreen.EnableWindow();
                DefaultScreenIsActive = true;
            }
        }
    }

    private void AfterCompleteAllGoals()
    {
        foreach (GameObject go in _listObjectsToHide)
        {
            go.SetActive(false);
        }

        _goalButtonImage.color = Color.white;
        _goalButtonImage.fillAmount = 1f;

        _goalButtonIcon.sprite = _goalButtonIconSprite;
        _goalButtonIcon.color = Color.white;
    }
}