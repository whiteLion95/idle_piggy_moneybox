using System;
using UnityEngine;

public class MopubImpressionHandler : MonoBehaviour
{
    private static MopubImpressionHandler instance;

    private void Start()
    {
        if (instance == null)
            instance = this;

        var dubl = FindObjectsOfType<MopubImpressionHandler>();
        if (dubl.Length > 1)
        {
            Destroy(this.gameObject);
        }

        DontDestroyOnLoad(gameObject);

#if tenjin_mopub_enabled
#if UNITY_IOS
        MoPubManager.OnImpressionTrackedEvent += OnImpressionTrackedEvent;
        MoPubManager.OnImpressionTrackedEventBg += OnImpressionTrackedEventBg;
#endif
#endif
    }

    private void Awake()
    {
        if (instance == null)
            instance = FindObjectOfType<MopubImpressionHandler>();
    }

#if tenjin_mopub_enabled
#if UNITY_IOS
    private void OnImpressionTrackedEvent(string adUnitId, MoPub.ImpressionData impressionData)
    {
        Debug.Log("OnImpressionTrackedEvent: " + impressionData.JsonRepresentation);
        HandleAdImpression(impressionData);
    }

    private void OnImpressionTrackedEventBg(string adUnitId, MoPub.ImpressionData impressionData)
    {
        Debug.Log("OnImpressionTrackedEventBg: " + impressionData.JsonRepresentation);
        HandleAdImpression(impressionData);
    }

    private void HandleAdImpression(MoPub.ImpressionData impressionData)
    {
        if (impressionData.AdUnitFormat == null)
            return;

        try
        {
            decimal value = impressionData.PublisherRevenue.HasValue ? Convert.ToDecimal(impressionData.PublisherRevenue.Value) : TryPredictRevenue(impressionData.AdUnitFormat);
            AdjustConversions.SaveRevenue(impressionData.AdUnitFormat, value);

            AdjustConversions.IncValue();
        }
        catch (Exception ex)
        {
            Debug.Log($"HandleAdImpression error: {ex}");
        }
    }

    private decimal TryPredictRevenue(string ad_format)
    {
        Debug.Log("TryPredictRevenue: " + ad_format);
        if (ad_format == null)
        {
            Debug.Log("TryPredictRevenue: AdUnitFormat is null");
            return 0;
        }

        decimal revenue = AdjustConversions.GetAverageRevenueByAdFormat(ad_format);
        Debug.Log("TryPredictRevenue:Average:" + revenue);
        if (revenue > 0)
            return revenue;

        revenue = AdjustConversions.GetRevenueByCountry(ad_format);
        Debug.Log("TryPredictRevenue:ByCountry:" + revenue);
        if (revenue > 0)
            return revenue;

        return 0;
    }
#endif
#endif

}
