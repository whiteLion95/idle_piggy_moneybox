using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Deslab.UI;
using DG.Tweening;

public class GoalPurchasedCongratsWindow : CanvasGroupWindow
{
    [SerializeField] private DOTweenAnimation buttonAnim;
    [SerializeField] private ParticleSystem fireworkParticle;

    private void Start()
    {
        Init();
    }

    private void OnEnable()
    {
        OnWindowStartShowing += OnWindowShowHandler;
        OnWindowHid += OnWindowHidHandler;
    }

    private void OnDisable()
    {
        OnWindowStartShowing -= OnWindowShowHandler;
        OnWindowHid -= OnWindowHidHandler;
    }

    private void OnWindowShowHandler(CanvasGroupWindow window)
    {
        if (window.Equals(this))
        {
            buttonAnim.DOPlay();
            fireworkParticle.Play();
            Time.timeScale = 0f;
            AudioManager.Instance.PlayOneShot("Victory");
        }
    }

    private void OnWindowHidHandler(CanvasGroupWindow window)
    {
        if (window.Equals(this))
        {
            buttonAnim.DORewind();
            fireworkParticle.Stop();
        }
    }

    private void Init()
    {
        buttonAnim.GetComponent<Button>().onClick.AddListener(UIManager.Instance.ShowGoalsChooseScreen);
    }
}
