using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisconnectionAdRequester : MonoBehaviour
{
    public bool IsInternetAvailable
    {
        get =>internetChecker.IsInternetAvilable;
        set => IsInternetAvailable = value;
    }

    public bool IsAdWasRequestedAfterDisconnect { get; set; } = true;
    
    private InternetChecker internetChecker;

    private bool isConnectAfterDisconnect = false;

    private IMediationAdManager currentMediationAdManager;
    
    private void Start()
    {
        internetChecker = GetComponent<InternetChecker>();
        currentMediationAdManager = GetComponent<IMediationAdManager>();
    }

    private void FixedUpdate()
    {
        if (!internetChecker.IsInternetAvilable)
        {
            isConnectAfterDisconnect= false;
            IsAdWasRequestedAfterDisconnect = false;
        }

        if (!isConnectAfterDisconnect && IsInternetAvailable)
            isConnectAfterDisconnect = true;
                
        if (isConnectAfterDisconnect && !IsAdWasRequestedAfterDisconnect)
        {
            Debug.Log("Ads was requested after network connect");
            IsAdWasRequestedAfterDisconnect = true;
            currentMediationAdManager.RequestAllAds();
        }
    }
}
