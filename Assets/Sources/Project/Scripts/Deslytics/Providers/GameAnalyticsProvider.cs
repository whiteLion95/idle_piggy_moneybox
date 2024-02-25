using UnityEngine;
using GameAnalyticsSDK;

namespace Deslab.Deslytics.Provider
{
    public class GameAnalyticsProvider : MonoBehaviour, IProvider
    {
        public void LevelStart(int levelRealNumber, bool restart = false,
            string levelType = "default",
            string levelDiff = "easy", bool bonus = false)
        {
#if !UNITY_EDITOR
         GameAnalytics.NewProgressionEvent(GAProgressionStatus.Start, "Level_" + levelRealNumber.ToString(), "Restart_" + restart, "Bonus_"+ bonus);   
#endif
            Debug.Log("Analytics - " + this.GetType() + " LevelStart:"
                      + " currentLevel: " + levelRealNumber
                      + " isRestart: " + restart
                      + " levelType: " + levelType
                      + " levelDiff: " + levelDiff
                      + " bonus: " + bonus);
        }

        public void LevelFailed(int levelRealNumber, float levelTime = 0, int progress = 0, int score = 0, bool bonus = false)
        {
#if !UNITY_EDITOR
            GameAnalytics.NewProgressionEvent(GAProgressionStatus.Fail, "Level_" + levelRealNumber.ToString(), "Bonus_"+ bonus);
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
            GameAnalytics.NewProgressionEvent(GAProgressionStatus.Complete, "Level_" + levelRealNumber.ToString(), "Bonus_"+ bonus);
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
            GameAnalytics.NewProgressionEvent(GAProgressionStatus.Complete, upgradeName + "_Level_" + newLevel);
#endif
            Debug.Log("Analytics - UpgradeName : " + upgradeName + " | UpgradeLevel : " + newLevel);
        }

        public void Tutorial(int currentStep)
        {
#if !UNITY_EDITOR
            GameAnalytics.NewDesignEvent("tutorial_step", currentStep);
#endif
            Debug.Log("Analytics - Tutorial:" + " tutorial_step: " + currentStep);
        }

        public void CustomLog(string eventName, string parameterName, string parameterValue)
        {
            int val = 0;
            int.TryParse(parameterValue, out val);
#if !UNITY_EDITOR
            GameAnalytics.NewDesignEvent(eventName, val);
#endif
            Debug.Log("Analytics -" + eventName + ":" + " " + parameterName + ": " + parameterValue);
        }

        public void AdsShow(string adType, string placement, string result)
        {
#if !UNITY_EDITOR
            GameAnalytics.NewDesignEvent(adType + ":" + placement + ":" + result);
#endif
            Debug.Log("Analytics - " + this.GetType() + " AdsEvent:"
                      + " ads_type: " + adType
                      + " placement: " + placement
                      + " result: " + result);
        }

        public void ShopItemUnlock(string itemID, string unlockType)
        {
#if !UNITY_EDITOR
            GameAnalytics.NewDesignEvent("unlock_item:" + "item_id_" + itemID + ":unlockType" + unlockType);
#endif
            Debug.Log("Analytics - " + this.GetType() + " ShopItemUnlock:"
                      + " item_id: " + itemID
                      + " unlock_type: " + unlockType);
        }

        public void AllLevelsComplete()
        {
#if !UNITY_EDITOR
        GameAnalytics.NewDesignEvent("all_levels_complete", 1);
#endif
            Debug.Log("Analytics - " + this.GetType() + " AllLevelsComplete:"
                      + " complete: " + 1);
        }

        public void InterstitialShow()
        {
#if !UNITY_EDITOR
            GameAnalytics.NewDesignEvent("interstitial_show", 1);
#endif
            Debug.Log("Analytics - " + this.GetType() + " InterstitialShow:"
                      + " interstitial_show: " + 1);
        }

        public void BannerShow()
        {
#if !UNITY_EDITOR
             GameAnalytics.NewDesignEvent("banner_show", 1);
#endif
            Debug.Log("Analytics - " + this.GetType() + " BannerShow:"
                      + " banner_show: " + 1);
        }

        public void RewardedShow(bool finished)
        {
#if !UNITY_EDITOR
            GameAnalytics.NewDesignEvent("rewarded_show", 1);
#endif
            Debug.Log("Analytics - " + this.GetType() + " RewardedShow:"
                      + " rewarded_show: " + 1
                      + "finished: " + finished);
        }

        public void BuyProgressionItem(string itemID)
        {
#if !UNITY_EDITOR
            GameAnalytics.NewDesignEvent("Buy_Progression_Item:" + "item_id_" + itemID);
#endif
            Debug.Log("Analytics - " + this.GetType() + " BuyProgressionItem:"
                      + " item_id: " + itemID);    
        }

        public void ShowOfflineReward()
        {
#if !UNITY_EDITOR
             GameAnalytics.NewDesignEvent("Show_Offline_Reward", 1);
#endif
            Debug.Log("Analytics - " + this.GetType() + " ShowOfflineReward:"
                      + " Show_Offline_Reward: " + 1);
        }

        public void OnlineOnly()
        {
#if !UNITY_EDITOR
             GameAnalytics.NewDesignEvent("Online_Only", 1);
#endif
            Debug.Log("Analytics - " + this.GetType() + " OnlineOnly:"
                      + " Online_Only: " + 1);
        }

        public void BuyOfferInShop(string productType)
        {
#if !UNITY_EDITOR
             GameAnalytics.NewDesignEvent("Buy_Offer_In_Shop", 1);
#endif
            Debug.Log("Analytics - " + this.GetType() + " BuyOfferInShop:"
                      + " Buy_Offer_In_Shop: " + 1);
        }

        public void BuyOfferBeforeAds(string productType)
        {
#if !UNITY_EDITOR
             GameAnalytics.NewDesignEvent("Buy_Offer_Before_Ads", 1);
#endif
            Debug.Log("Analytics - " + this.GetType() + " BuyOfferBeforeAds:"
                      + " Buy_Offer_Before_Ads: " + 1);
        }

        public void BuyOfferAfterAds(string productType)
        {
#if !UNITY_EDITOR
             GameAnalytics.NewDesignEvent("Buy_Offer_After_Ads", 1);
#endif
            Debug.Log("Analytics - " + this.GetType() + " BuyOfferAfterAds:"
                      + " Buy_Offer_After_Ads: " + 1);
        }

        public void BuyOfferOfflineReward(string productType)
        {
#if !UNITY_EDITOR
             GameAnalytics.NewDesignEvent("Buy_Offer_Offline_Reward", 1);
#endif
            Debug.Log("Analytics - " + this.GetType() + " BuyOfferOfflineReward:"
                      + " Buy_Offer_Offline_Reward: " + 1);
        }
        
        public void BuySkill(string skillName, int newLevel)
        {
#if !UNITY_EDITOR
             GameAnalytics.NewDesignEvent("Buy_Skill", 1);
#endif
            Debug.Log("Analytics - " + this.GetType() + " BuySkill:"
                      + " Buy_Skill: " + 1);
        }

        private void Start()
        {
#if !UNITY_EDITOR && FLAG_GA
			GameAnalytics.Initialize();
#endif
        }
    }
}