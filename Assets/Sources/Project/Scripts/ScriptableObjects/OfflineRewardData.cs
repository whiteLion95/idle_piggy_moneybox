using UnityEngine;

namespace Deslab
{

    [System.Serializable]
    public struct SOfflineRewardData
    {
        public int TimeoutMin;
        public int TimeoutMax;
        public float Multiplier;
    }
    
    [CreateAssetMenu(fileName = "OfflineRewardData", menuName = "Deslab/Offline Reward Data", order = 52)]
    public class OfflineRewardData : ScriptableObject
    {
        public string PrefsKey = "LastTime";
        
        public SOfflineRewardData[] RewardData;
    }
}
