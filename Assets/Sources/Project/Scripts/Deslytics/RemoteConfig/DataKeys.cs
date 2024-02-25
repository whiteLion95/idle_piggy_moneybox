using System;
using System.Collections.Generic;
using UnityEngine;

namespace Deslab.RemoteConfig
{
    [Serializable]
    public class RemoteKeys
    {
        public static readonly string OfflineRewardDisabled = "Offline_Reward_Disabled";
        public static readonly string OnlineOnly = "Online_Only"; 
        public static readonly string TransformRewardToInter = "Transform_Reward_To_Inter";
        public static readonly string InterCounterBeforeSpecialOffer = "Inter_Offer";
    }

    [Serializable]
    public class RemoteData
    {
        public string Key;
        public object Value;
    }

    [CreateAssetMenu(fileName = "DataKeys", menuName = "Deslab/Remote Config/DataKeys", order = 52)]
    public class DataKeys : ScriptableObject
    {
        public List<RemoteData> Data;
    }
}