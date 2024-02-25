using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lean.Touch;
using Deslab.UI;

public class UI_UpgradePanelsManager : MonoBehaviour
{
    [SerializeField] private GameObject[] panels;

    public System.Action OnClose;

    private CanvasGroupWindow _window;

    public static UI_UpgradePanelsManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;

        _window = GetComponent<CanvasGroupWindow>();

        _window.HideWindow();
    }

    private void OnEnable()
    {
        LeanTouch.OnFingerDown += HandlerOnFingerDown;
        LeanTouch.OnFingerUp += HandlerOnFingerDown;
    }

    private void OnDisable()
    {
        LeanTouch.OnFingerDown -= HandlerOnFingerDown;
        LeanTouch.OnFingerUp -= HandlerOnFingerDown;
    }

    public void Show()
    {
        _window.ShowWindow();
    }

    public void Close()
    {
        _window.HideWindow(() => OnClose?.Invoke());
    }

    private void HandlerOnFingerDown(LeanFinger finger)
    {
        if (!finger.IsOverGui)
        {
            if (!TutorialsController.Instance.SavingData.completed && TutorialsController.Instance.SavingData.step == 2)
            {
                return;
            }

            Close();
        }
    }
}
