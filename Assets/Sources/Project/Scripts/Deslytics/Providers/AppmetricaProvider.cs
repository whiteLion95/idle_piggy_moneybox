using System.Collections.Generic;
using UnityEngine;

namespace Deslab.Deslytics.Provider
{
    public class AppmetricaProvider : MonoBehaviour, IProvider
    {
        public void LevelStart(int levelRealNumber, bool restart = false,
            string levelType = "default",
            string levelDiff = "easy", bool bonus = false)
        {
#if !UNITY_EDITOR
        AppMetrica.Instance.ReportEvent("level_start",
            new Dictionary<string, object> {
                {"level_number", levelRealNumber},
                {"level_restart", restart},
                {"level_bonus", bonus}
                // ,{"level_type", levelType},
                // {"level_diff", levelDiff}
            });
        AppMetrica.Instance.SendEventsBuffer();
#endif
            Debug.Log("Analytics - " + this.GetType() + " LevelStart:"
                      + " currentLevel: " + levelRealNumber
                      + " isRestart: " + restart
                      + " levelType: " + levelType
                      + " levelDiff: " + levelDiff
                      + " bonus: " + bonus);
        }

        public void LevelFailed(int levelRealNumber, float levelTime = 0, int progress = 0, int score = 0,
            bool bonus = false)
        {
#if !UNITY_EDITOR
        AppMetrica.Instance.ReportEvent("level_finish",
            new Dictionary<string, object> {
                {"level_number", levelRealNumber},
                {"level_progress", progress},
                {"result", "lose"},
                {"level_bonus", bonus},
                {"level_score", score},
                {"time", levelTime}});
        AppMetrica.Instance.SendEventsBuffer();
#endif
            Debug.Log("Analytics - " + this.GetType() + " LevelFailed:"
                      + " currentLevel: " + levelRealNumber
                      + " level_progress: " + progress
                      + " result: " + "lose"
                      + " level_score: " + score
                      + " levelTime: " + levelTime
                      + " bonus: " + bonus);
        }

        public void LevelWin(int levelRealNumber, float levelTime = 0, bool bonus = false)
        {
#if !UNITY_EDITOR
            AppMetrica.Instance.ReportEvent("level_finish",
                new Dictionary<string, object> {
                    {"level_number", levelRealNumber},
                    {"result", "win"},
                    {"level_bonus", bonus},
                    {"time", levelTime},
                    {"bonus", bonus}
                });
            AppMetrica.Instance.SendEventsBuffer();
#endif
            Debug.Log("Analytics - LevelWin:"
                      + " currentLevel: " + levelRealNumber
                      + " result: win"
                      + " time: " + levelTime
                      + " bonus: " + bonus);
        }

        public void LevelUp(string upgradeName, int newLevel)
        {
#if !UNITY_EDITOR
            AppMetrica.Instance.ReportEvent("level_up",
                new Dictionary<string, object> {
                    {"upgrades", upgradeName + " : Level = " + newLevel}
                });
            AppMetrica.Instance.SendEventsBuffer();
#endif
            Debug.Log("Analytics - UpgradeName : " + upgradeName + " | UpgradeLevel : " + newLevel);
        }

        public void Tutorial(int currentStep)
        {
#if !UNITY_EDITOR
        AppMetrica.Instance.ReportEvent("tutorial",
            new Dictionary<string, object> {
                {"tutorial_step", currentStep}
            });
        AppMetrica.Instance.SendEventsBuffer();
#endif
            Debug.Log("Analytics - Tutorial:" + " tutorial_step: " + currentStep);
        }

        public void CustomLog(string eventName, string parameterName, string parameterValue)
        {
#if !UNITY_EDITOR
        AppMetrica.Instance.ReportEvent(eventName,
            new Dictionary<string, object> {
                {"parameterName", parameterValue}
            });
        AppMetrica.Instance.SendEventsBuffer();
#endif
            Debug.Log("Analytics -" + eventName + ":" + " " + parameterName + ": " + parameterValue);
        }

        public void AdsShow(string adType, string placement, string result)
        {
#if !UNITY_EDITOR
        AppMetrica.Instance.ReportEvent("ads_event",
            new Dictionary<string, object> {
                {"ads_type", adType},
                {"placement", placement},
                {"result", result}});
        AppMetrica.Instance.SendEventsBuffer();
#endif
            Debug.Log("Analytics - " + this.GetType() + " AdsEvent:"
                      + " ads_type: " + adType
                      + " placement: " + placement
                      + " result: " + result);
        }

        public void ShopItemUnlock(string itemID, string unlockType)
        {
#if !UNITY_EDITOR
        AppMetrica.Instance.ReportEvent("shop_item",
            new Dictionary<string, object> {
                {"item_id", itemID},
                {"unlock_type", unlockType}});
        AppMetrica.Instance.SendEventsBuffer();
#endif
            Debug.Log("Analytics - " + this.GetType() + " ShopItemUnlock:"
                      + " item_id: " + itemID
                      + " unlock_type: " + unlockType);
        }

        public void AllLevelsComplete()
        {
#if !UNITY_EDITOR
        AppMetrica.Instance.ReportEvent("all_levels_complete",
            new Dictionary<string, object> {
                {"all_levels_complete", 1}});
        AppMetrica.Instance.SendEventsBuffer();
#endif
            Debug.Log("Analytics - " + this.GetType() + " AllLevelsComplete:"
                      + " complete: " + 1);
        }

        public void InterstitialShow()
        {
#if !UNITY_EDITOR
        AppMetrica.Instance.ReportEvent("interstitial_show",
            new Dictionary<string, object> {
                {"interstitial_show", 1}});
        AppMetrica.Instance.SendEventsBuffer();
#endif
            Debug.Log("Analytics - " + this.GetType() + " InterstitialShow:"
                      + " interstitial_show: " + 1);
        }

        public void BannerShow()
        {
#if !UNITY_EDITOR
        AppMetrica.Instance.ReportEvent("banner_show",
            new Dictionary<string, object> {
                {"banner_show", 1}});
        AppMetrica.Instance.SendEventsBuffer();
#endif
            Debug.Log("Analytics - " + this.GetType() + " BannerShow:"
                      + " banner_show: " + 1);
        }

        public void RewardedShow(bool finished)
        {
#if !UNITY_EDITOR
        AppMetrica.Instance.ReportEvent("rewarded_show",
            new Dictionary<string, object> {
                {"rewarded_show", 1},
                {"finished", finished}});
        AppMetrica.Instance.SendEventsBuffer();
#endif
            Debug.Log("Analytics - " + this.GetType() + " RewardedShow:"
                      + " rewarded_show: " + 1
                      + "finished: " + finished);
        }

        public void BuyProgressionItem(string itemID)
        {
#if !UNITY_EDITOR
        AppMetrica.Instance.ReportEvent("Buy_Progression_Item",
            new Dictionary<string, object> {
                {"item_id", itemID}});
        AppMetrica.Instance.SendEventsBuffer();
#endif
            Debug.Log("Analytics - " + this.GetType() + " BuyProgressionItem:"
                      + " item_id: " + itemID);
        }

        public void ShowOfflineReward()
        {
#if !UNITY_EDITOR
        AppMetrica.Instance.ReportEvent("Show_Offline_Reward",
            new Dictionary<string, object> {
                {"Show_Offline_Reward", 1}});
        AppMetrica.Instance.SendEventsBuffer();
#endif
            Debug.Log("Analytics - " + this.GetType() + " ShowOfflineReward:"
                      + " Show_Offline_Reward: " + 1);
        }

        public void OnlineOnly()
        {
#if !UNITY_EDITOR
        AppMetrica.Instance.ReportEvent("Online_Only",
            new Dictionary<string, object> {
                {"Online_Only", 1}});
        AppMetrica.Instance.SendEventsBuffer();
#endif
            Debug.Log("Analytics - " + this.GetType() + " OnlineOnly:"
                      + " Online_Only: " + 1);
        }

        public void BuyOfferInShop(string productType)
        {
#if !UNITY_EDITOR
            AppMetrica.Instance.ReportEvent("Offer",
                new Dictionary<string, object> {
                    {"Buy_Offer_In_Shop", 1},
                    {"Product_Type", productType}
                });
            AppMetrica.Instance.SendEventsBuffer();
#endif
            Debug.Log("Analytics - " + this.GetType() + " Offer:"
                      + " Buy_Offer_In_Shop: " + 1);
        }

        public void BuyOfferBeforeAds(string productType)
        {
#if !UNITY_EDITOR
        AppMetrica.Instance.ReportEvent("Offer",
            new Dictionary<string, object> {
                {"Buy_Offer_Before_Ads", 1},
                {"Product_Type", productType}
            });
        AppMetrica.Instance.SendEventsBuffer();
#endif
            Debug.Log("Analytics - " + this.GetType() + " Offer:"
                      + " Buy_Offer_Before_Ads: " + 1);
        }

        public void BuyOfferAfterAds(string productType)
        {
#if !UNITY_EDITOR
        AppMetrica.Instance.ReportEvent("Offer",
            new Dictionary<string, object> {
                {"Buy_Offer_After_Ads", 1},
                {"Product_Type", productType}
            });
        AppMetrica.Instance.SendEventsBuffer();
#endif
            Debug.Log("Analytics - " + this.GetType() + " Offer:"
                      + " Buy_Offer_After_Ads: " + 1);
        }

        public void BuyOfferOfflineReward(string productType)
        {
#if !UNITY_EDITOR
        AppMetrica.Instance.ReportEvent("Offer",
            new Dictionary<string, object> {
                {"Buy_Offer_Offline_Reward", 1},
                {"Product_Type", productType}
            });
        AppMetrica.Instance.SendEventsBuffer();
#endif
            Debug.Log("Analytics - " + this.GetType() + " Offer:"
                      + " Buy_Offer_Offline_Reward: " + 1);
        }

        public void BuySkill(string skillName, int newLevel)
        {
#if !UNITY_EDITOR
        AppMetrica.Instance.ReportEvent("Skills",
            new Dictionary<string, object> {
                {"Skill", skillName  + " : Level = " + newLevel}
            });
        AppMetrica.Instance.SendEventsBuffer();
#endif
            Debug.Log("Analytics - " + this.GetType() + " BuySkill:"
                      + " Buy_Skill: " + skillName);
        }
    }
}