using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Text;
using Mamboo.Internal.Analytics;
using Mamboo.Internal;
using com.adjust.sdk;

public static class AdjustConversions
{
  private static string IS_FIRST_RUN = "is_first_run";
  private static string COLLECTED_POINTS_KEY = "collected_points";
  private static string SENT_POINTS_KEY = "sent_points";
  private static string COUNTRY_KEY = "country";
  private static string INTERSTITIAL_KEY = "full";
  private static string REWARD_VIDEO_KEY = "reward_video";
  private static string BANNER_KEY = "banner";
  private static string IAP_KEY = "iap";
  private static string PREFS_PREFIX = "AdjustConversions:";

  internal const string INTERSTITIAL_FORMAT = "fullscreen";
  internal const string REWARD_VIDEO_FORMAT = "rewarded video";
  internal const string BANNER_FORMAT = "banner";
  private const string IAP_FORMAT = "iap";
    
  static AdjustConversions()
  {

#if UNITY_IOS
    var first_run = AdjustPrefsHelper.GetString(IS_FIRST_RUN);

    if(first_run == "") {
      AdjustPrefsHelper.SetString(IS_FIRST_RUN, "not_first_run");
      SendValue(0);
    }
#endif
    }

    public static void IncValue()
    {
#if UNITY_IOS
        try
        {
            if (IDFATracking.IDFATracking.GetCurrentStatus() == IDFATracking.Status.Authorized)
                return;

            decimal target_conversion_value = GetSumRevenueByAdFormat(INTERSTITIAL_FORMAT) +
                                              GetSumRevenueByAdFormat(REWARD_VIDEO_FORMAT) +
                                              GetSumRevenueByAdFormat(BANNER_FORMAT) +
                                              GetSumRevenueByAdFormat(IAP_FORMAT);

            float result_points = (float)target_conversion_value / ConfRevenues.conversion_rate;
            Debug.Log($"AdjustConversions:IncValue:target_conversion_value: {target_conversion_value}; result_points: {result_points}");

            if (result_points > 0)
            {
                AdjustPrefsHelper.SetFloat(COLLECTED_POINTS_KEY, result_points);
                SyncValue();
            }
        }
        catch (Exception ex)
        {
            Debug.Log($"{nameof(IncValue)} error: {ex}");
        }
#endif
    }

    // TODO SDK: integrate the code below into your PurchaseManager
    /// <summary>
    /// Call this method from Purchase. NOTE: avoid calling that when Restore Purchases
    /// in Unity IAP ProcessPurchase() method also called for Restoring transactions, please add condition for that to call this method only for purchasing).
    /// </summary>
    /// <param name="value">Value should be pass in USD! NOTE: that Unity IAP sends localized currency!</param>
    public static void IncIAP(float value)
    {
#if UNITY_IOS
        try
        {
            if (IDFATracking.IDFATracking.GetCurrentStatus() == IDFATracking.Status.Authorized)
                return;

            Debug.Log("AdjustConversions:IncIAP:" + value);
            SaveRevenue(IAP_FORMAT, (decimal)value);
            IncValue();
        }
        catch (Exception ex)
        {
            Debug.Log($"{nameof(IncIAP)} error: {ex}");
        }
  #endif
    }

    public static void SyncValue()
    {
#if UNITY_IOS
        try
        {
            if ((DateTime.Now - Settings.GetStartGameDate()).TotalDays > 1)
                return;

            if (IDFATracking.IDFATracking.GetCurrentStatus() == IDFATracking.Status.Authorized)
                return;

            float collected_points = AdjustPrefsHelper.GetFloat(COLLECTED_POINTS_KEY);
            float sent_points = AdjustPrefsHelper.GetFloat(SENT_POINTS_KEY);
            if (collected_points > sent_points)
            {
                Debug.Log("AdjustConversions:SyncValue:collected_points: " + collected_points + "; sent_points: " + sent_points);
                SendValue((int)collected_points);
                AdjustPrefsHelper.SetFloat(SENT_POINTS_KEY, collected_points);
            }
        }
        catch (Exception ex)
        {
            Debug.Log($"{nameof(SyncValue)} error: {ex}");
        }
#endif
    }

    private static void SendValue(int value)
    {
#if UNITY_IOS
        try
        {
            Debug.Log("AdjustConversions:SendValue:" + value);
            Adjust.updateConversionValue(value);

            // FB postbacks support
            // TODO SDK: enable once needed
            //instance.SendEvent($"cv_{value}");
        }
        catch (Exception ex)
        {
            Debug.Log($"{nameof(SendValue)} error: {ex}");
        }
#endif
    }

    private static string GetCountry()
  {
#if UNITY_IOS
    string country = AdjustPrefsHelper.GetString(COUNTRY_KEY);
    if (country != "")
      return country;
    
    try
    {
      country = new System.Net.WebClient().DownloadString("https://ipapi.co/country/");
            AdjustPrefsHelper.SetString(COUNTRY_KEY, country);
    
      Debug.Log($"AdjustConversions:Country:{country}");
      return country;
    }
    catch
    {
      return string.Empty;
    }
#else
    return string.Empty;
#endif
  }

  private static string GetKeyByAdFormat(string adFormat)
  {
    Debug.Log($"AdjustConversions:GetKeyByAdFormat:{adFormat}");
    switch (adFormat.ToLower())
    {
      case INTERSTITIAL_FORMAT :
        return INTERSTITIAL_KEY;
      case REWARD_VIDEO_FORMAT :
        return REWARD_VIDEO_KEY;
      case BANNER_FORMAT :
        return BANNER_KEY;
      case IAP_FORMAT :
        return IAP_KEY;
    }
    
    return BANNER_KEY;
  }
    public static void SaveRevenue(string ad_format, decimal value)
    {
#if UNITY_IOS
        try
        {
            if (IDFATracking.IDFATracking.GetCurrentStatus() == IDFATracking.Status.Authorized)
                return;

            Debug.Log($"AdjustConversions:SaveRevenue:{ad_format}, {value}");
            List<decimal> revenue_list = AdjustPrefsHelper.GetArray(GetKeyByAdFormat(ad_format));
            Debug.Log($"AdjustConversions: SaveRevenue:{revenue_list}");
            revenue_list.Add(value);
            AdjustPrefsHelper.SetArray(GetKeyByAdFormat(ad_format), revenue_list);
        }
        catch (Exception ex)
        {
            Debug.Log($"{nameof(SaveRevenue)} error: {ex}");
        }
#endif
    }

    public static decimal GetSumRevenueByAdFormat(string ad_format)
  {
#if UNITY_IOS
    Debug.Log($"AdjustConversions:GetSumRevenueByAdFormat:ad_format:{ad_format}");
    List<decimal> revenue_list = AdjustPrefsHelper.GetArray(GetKeyByAdFormat(ad_format));
    Debug.Log($"AdjustConversions:GetSumRevenueByAdFormat:revenue_list{revenue_list}");
    return revenue_list.DefaultIfEmpty(0).Sum();
#else
    return 0;
#endif
  }
  public static decimal GetAverageRevenueByAdFormat(string ad_format)
  {
#if UNITY_IOS
    Debug.Log($"AdjustConversions:GetAverageRevenueByAdFormat:ad_format:{ad_format}");
    List<decimal> revenue_list = AdjustPrefsHelper.GetArray(GetKeyByAdFormat(ad_format));
    Debug.Log($"AdjustConversions:GetAverageRevenueByAdFormat:revenue_list{revenue_list}");
    return revenue_list.DefaultIfEmpty(0).Average();
#else
    return 0;
#endif
  }
  
    public static decimal GetRevenueByCountry(string ad_format)
    {
#if UNITY_IOS
        var country = GetCountry();
        var revenues = GetRevenueList();
        var placement = GetPlacementString(ad_format);

        if (revenues == null || placement == null) return 0;

        var revenue_item = revenues.FirstOrDefault(x => x.country == country && x.placement == placement);

        if (revenue_item != null) return (decimal)revenue_item.revenue;

        return 0;
#else
        return 0;
#endif
    }

    private static string GetPlacementString(string ad_format)
    {
        if (ad_format.ToLower() == INTERSTITIAL_FORMAT)
            return "full";

        if (ad_format.ToLower() == REWARD_VIDEO_FORMAT)
            return "rewarded_video";

        if (ad_format.ToLower() == BANNER_FORMAT)
            return "banner";

        return null;
    }

    private static List<ConfRevenueItem> GetRevenueList()
    {

       throw new ArgumentNullException(nameof(GameAnalyticsSDK), "Error: Revenues should be set in Mamboo settings");
    }
}

public static class AdjustPrefsHelper
{
    public static string PREFS_PREFIX = "";
    public static int GetInt(string name)
      {
        string key_name = PREFS_PREFIX + name;
        if (!PlayerPrefs.HasKey(key_name))
          SetInt(name, 0);

        return PlayerPrefs.GetInt(key_name);
      }
  
  public static void SetInt(string name, int value)
  {
    PlayerPrefs.SetInt(PREFS_PREFIX + name, value);
  }
  
  public static float GetFloat(string name)
  {
    string key_name = PREFS_PREFIX + name;
    if (!PlayerPrefs.HasKey(key_name))
      SetFloat(name, 0.0f);

    return PlayerPrefs.GetFloat(key_name);
  }
  
  public static void SetFloat(string name, float value)
  {
    PlayerPrefs.SetFloat(PREFS_PREFIX + name, value);
  }
  
  public static string GetString(string name)
  {
    string key_name = PREFS_PREFIX + name;
    if (!PlayerPrefs.HasKey(key_name))
      SetString(name, "");

    return PlayerPrefs.GetString(key_name);
  }
  public static void SetString(string name, string value)
  {
    PlayerPrefs.SetString(PREFS_PREFIX + name, value);
  }

    public static List<decimal> GetArray(string name)
    {
        Debug.Log("AdjustConversions:GetArray:name:" + name);
        string key_name = PREFS_PREFIX + name;
        if (!PlayerPrefs.HasKey(key_name))
            SetString(key_name, "");

        string arr_str = PlayerPrefs.GetString(key_name);
        Debug.Log("AdjustConversions:GetArray:arr_str:" + arr_str);
        decimal[] arr_float = new decimal[] { };
        try
        {
            if (!string.IsNullOrEmpty(arr_str))
            {
                arr_float = Array.ConvertAll(arr_str.Split(';'), decimal.Parse);
                Debug.Log("AdjustConversions:GetArray:arr_float:" + arr_float);
            }
        }
        catch
        {
            Debug.Log("AdjustConversions:GetArray: error conversion");
        }

        if (arr_float.Length > 0)
        {
            decimal avg_value = arr_float.Where(x => x > 0).DefaultIfEmpty(0).Average();
            Debug.Log("AdjustConversions:GetArray:avg_value:" + avg_value);
            for (int i = 0; i < arr_float.Length - 1; i++)
            {
                if (arr_float[i] == 0)
                    arr_float[i] = avg_value;
            }
        }

        return arr_float.ToList();
    }
  
  public static void SetArray(string name, List<decimal> arr_decimal)
  {
    Debug.Log("AdjustConversions:SetArray:name:" + name + "; arr_decimal:" + arr_decimal);
    string arr_str = Join(arr_decimal, ";");
    Debug.Log("AdjustConversions:SetArray:arr_str:" + arr_str);
    PlayerPrefs.SetString(PREFS_PREFIX + name, arr_str);
  }

  private static string Join<T>(List<T> list, string separator = ",")
  {
      StringBuilder builder = new StringBuilder();
      for (int i = 0; i < list.Count; ++i)
      {
          builder.Append(list[i]);
          if (i < list.Count - 1) builder.Append(separator);
      }

      return builder.ToString();
  }
}