using UnityEngine;
using System;
using System.Collections;

namespace EZ_Pooling
{
    /// <summary>
    /// PrefabPool class (for holding data in the editor inspector)
    /// </summary>
    [Serializable]
    public class EZ_PrefabPoolOption
    {
        public Transform prefabTransform;
        public int instancesToPreload = 1;
        public bool isPoolExpanded = true;
        public bool showDebugLog = false;
        public bool poolCanGrow = false;
        public bool cullDespawned = false;
        public int cullAbove = 10;
        public float cullDelay = 2f;
        public int cullAmount = 1;
        public bool enableHardLimit = false;
        public int hardLimit = 10;
        public bool recycle = false;
    }

}
