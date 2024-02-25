using System;
using UnityEngine;
using Deslab.Deslytics.Provider;

namespace Deslab.Deslytics
{
    public class DeslyticsManager : MonoBehaviour
    {
        [SerializeField]
        private Providers providers;
        private static DeslyticsManager instance;

        public static event Action OnLevelStart = delegate { };
        public static event Action OnLevelFinish = delegate { };

        private static int _levelOriginalNumber;

        private void Awake()
        {
            if (instance != null) return;
            instance = this;
        }

        public static void LevelStart(int levelRealNumber, bool restart = false, 
            string levelType = "default", string levelDiff = "easy", bool bonus = false)
        {
            _levelOriginalNumber = levelRealNumber;
            OnLevelStart();

            object[] parameters = {levelRealNumber, restart, levelType, levelDiff, bonus};
            instance.providers.CallAction("LevelStart", parameters);
        }

        public static void LevelFailed(int progress = 0, int score = 0, bool bonus = false)
        {
            float levelTime = LevelLengthTimer.Value;
            OnLevelFinish();
            object[] parameters = {_levelOriginalNumber, levelTime, progress, score, bonus};
            instance.providers.CallAction("LevelFailed", parameters);
            
        }

        public static void LevelWin(bool bonus = false)
        {
            float levelTime = LevelLengthTimer.Value;
            OnLevelFinish();
            object[] parameters = {_levelOriginalNumber, levelTime, bonus};
            instance.providers.CallAction("LevelWin", parameters);
        }

        public static void LevelUp(string upgradeName, int upgradeLevel)
        {
            OnLevelFinish();
            object[] parameters = {upgradeName, upgradeLevel};
            instance.providers.CallAction("LevelUp", parameters);
        }

        public static void Tutorial(int currentStep)
        {
            object[] parameters = {currentStep};
            instance.providers.CallAction("Tutorial", parameters);
        }

        public static void CustomLog(string eventName, string parameterName, string parameterValue)
        {
            object[] parameters = {eventName, parameterName, parameterValue};
            instance.providers.CallAction("CustomLog", parameters);
        }

        public static void AdsShow(string adType, string placement, string result)
        {
            object[] parameters = {adType, placement, result};
            instance.providers.CallAction("AdsShow", parameters);
        }

        public static void ShopItemUnlock(string itemID, string unlockType)
        {
            object[] parameters = {itemID, unlockType};
            instance.providers.CallAction("ShopItemUnlock", parameters);
        }

        public static void AllLevelsComplete()
        {
            instance.providers.CallAction("AllLevelsComplete");
        }


        public static void InterstitialShow()
        {
            instance.providers.CallAction("InterstitialShow");
        }

        public static void BannerShow()
        {
            instance.providers.CallAction("BannerShow");
        }

        public static void RewardedShow(bool finished)
        {
            object[] parameters = {finished};
            instance.providers.CallAction("RewardedShow", finished);
        }
        
        public static void BuyProgressionItem(string itemID)
        {
            object[] parameters = {itemID};
            instance.providers.CallAction("BuyProgressionItem", parameters);
        }

        public static void ShowOfflineReward()
        {
            instance.providers.CallAction("ShowOfflineReward");
        }
        
        public static void OnlineOnly()
        {
            instance.providers.CallAction("OnlineOnly");
        }
        
        public static void BuyOfferInShop(string productType)
        {
            object[] parameters = {productType};
            instance.providers.CallAction("BuyOfferInShop", parameters);
        }
        public static void BuyOfferBeforeAds(string productType)
        {
            object[] parameters = {productType};
            instance.providers.CallAction("BuyOfferBeforeAds", parameters);
        }
        public static void BuyOfferAfterAds(string productType)
        {
            object[] parameters = {productType};
            instance.providers.CallAction("BuyOfferAfterAds", parameters);
        }
        
        public static void BuyOfferOfflineReward(string productType)
        {
            object[] parameters = {productType};
            instance.providers.CallAction("BuyOfferOfflineReward", parameters);
        }
        
        public static void BuySkill(string productType, int newLevel)
        {
            object[] parameters = {productType, newLevel};
            instance.providers.CallAction("BuySkill", parameters);
        }
        
        
        // private void Update()
        // {
        //     if (Input.GetKeyDown(KeyCode.L))
        //     {
        //         LevelStart(1,false, "aaa");
        //         LevelFailed();
        //         LevelWin();
        //         LevelUp(10);
        //         Tutorial(2);
        //         CustomLog("TestEventName1", "ParameterName1", "4222");
        //         CustomLog("TestEventName2", "ParameterName2", "fdf2");
        //         AdsShow("REWARDED", "Win_Screen_Get_Prize_Reward", "success");
        //         ShopItemUnlock("12", "REWARD");
        //         AllLevelsComplete();
        //         InterstitialShow();
        //         BannerShow();
        //         RewardedShow(true);
        //     }
        // }
    }
}