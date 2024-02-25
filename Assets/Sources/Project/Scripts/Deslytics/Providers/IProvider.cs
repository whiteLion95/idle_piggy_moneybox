namespace Deslab.Deslytics.Provider
{
    public interface IProvider
    {
        #region Deprecated For Project
        public void LevelStart(int levelRealNumber, bool restart = false,
            string levelType = "default", string levelDiff = "easy", bool bonus = false);
        public void LevelFailed(int levelRealNumber, float levelTime = 0, int progress = 0, int score = 0, bool bonus = false);
        public void LevelWin(int levelRealNumber, float levelTime = 0, bool bonus = false);
        public void CustomLog(string eventName, string parameterName, string parameterValue);
        public void ShopItemUnlock(string itemID, string unlockType);
        public void AllLevelsComplete();
        #endregion

        public void LevelUp(string upgradeName, int newLevel);
        public void Tutorial(int currentStep);
        public void AdsShow(string adType, string placement, string result);
        

        public void InterstitialShow();
        public void BannerShow();
        public void RewardedShow(bool finished);
        
        public void BuyProgressionItem(string itemID);

        public void ShowOfflineReward();
        public void OnlineOnly();
        
        public void BuyOfferInShop(string productType);
        public void BuyOfferBeforeAds(string productType);
        public void BuyOfferAfterAds(string productType);
        public void BuyOfferOfflineReward(string productType);
        public void BuySkill(string skillName, int newLevel);
    }
}
