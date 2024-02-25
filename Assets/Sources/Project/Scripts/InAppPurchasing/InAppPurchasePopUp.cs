using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Deslab.UI;
using TMPro;

public class InAppPurchasePopUp : MonoBehaviour
{
    [SerializeField] private CanvasGroupWindow shopScreen;
    [SerializeField] private TMP_Text text;
    [SerializeField] private GameObject coinsFrame;
    [SerializeField] private GameObject noadsFrame;
    
    private CanvasGroupWindow _myScreen;

    // Start is called before the first frame update
    void Start()
    {
        _myScreen = GetComponent<CanvasGroupWindow>();
        InAppPurchasesManager.Instance.OnRewarded += ShowPopUp;
        CanvasGroupWindow.OnWindowHid += OnCloseHandler;
    }

    private void ShowPopUp(ulong rewardAmount, bool noAds = false)
    {
        //shopScreen.DisableWindow();
        _myScreen.ShowWindow();
        if (!noAds)
        {
            text.SetText(ReducedBigText.GetText(rewardAmount));
            coinsFrame.SetActive(true);
            noadsFrame.SetActive(false);
        }
        else
        {
            coinsFrame.SetActive(false);
            noadsFrame.SetActive(true);
        }
    }

    private void OnCloseHandler(CanvasGroupWindow window)
    {
        // if (window.Equals(_myScreen))
        // {
        
        //     if (!extraNoAdsButton)
        //         shopScreen.EnableWindow();
        // }
    }
}