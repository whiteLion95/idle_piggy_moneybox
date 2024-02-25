using System;
using Deslab.InApp;
using Deslab.RemoteConfig;
using Deslab.Scripts.Deslytics.Ads;
using UnityEngine;

public class InAppInterOfferShowChecker : MonoBehaviour
{
    [SerializeField] private InAppPurchaseBlock iapBlock;
    [SerializeField] private int showCount = 0;

    void OnEnable()
    {
        showCount++;
        if (iapBlock.productType == ProductType.NoInterstitialAds &&
            !AdsManager.Instance.interAdsRemoved &&
            RemoteConfigManager.Instance.GetData<int>(RemoteKeys.InterCounterBeforeSpecialOffer) > showCount)
            
            iapBlock.gameObject.SetActive(false);
        else
            iapBlock.gameObject.SetActive(true);
    }
}