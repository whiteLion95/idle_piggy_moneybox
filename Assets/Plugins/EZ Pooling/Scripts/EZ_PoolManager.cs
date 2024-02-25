using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace EZ_Pooling
{
    /// <summary>
    /// 
    /// </summary>
    [AddComponentMenu("EZ Pooling/EZ_PoolManager")]
    public class EZ_PoolManager : MonoBehaviour
    {
        /// <summary>
        /// Public list maintained for Unity editing. 
        /// </summary>
        public List<EZ_PrefabPoolOption> prefabPoolOptions = new List<EZ_PrefabPoolOption>();
        public bool showDebugLog = false;
        public bool isRootExpanded = true;
        public bool autoAddMissingPrefabPool = false;
        public bool usePoolManager = true;

        /// <summary>
        /// Dictionary that is used for the collection of pools. 
        /// Created from the prefabPoolOptions for fast look up in run time.
        /// </summary>
        private static Dictionary<string, EZ_PrefabPool> Pools = new Dictionary<string, EZ_PrefabPool>();
        private static Transform parentTransform;
        private static EZ_PoolManager instance;

        private List<EZ_PrefabPoolOption> itemsMarkedForDeletion = new List<EZ_PrefabPoolOption>();

        /// <summary>
        /// Static instance.
        /// </summary>
        public static EZ_PoolManager Instance
        {
            get
            {
                return instance;
            }
        }

        /// <summary>
        /// Initializes all the pools set up by user in the editor, and adds them to the Dictionary.
        /// </summary>
        void Awake()
        {
            instance = this;

            parentTransform = this.transform;

            Pools.Clear();
            itemsMarkedForDeletion.Clear();

            if (!usePoolManager)
                return;

            // loop through all the pre-allocated pools and initialize all the pool
            for (var i = 0; i < prefabPoolOptions.Count; ++i)
            {
                var item = prefabPoolOptions[i];
                var prefabTransform = item.prefabTransform;
                var name = prefabTransform.name;

                if (item.instancesToPreload <= 0 && !item.poolCanGrow)
                {
                    itemsMarkedForDeletion.Add(item);
                    continue; // no need to pre-allocate any game obj, nothing else to do
                }

                if (prefabTransform == null)
                {
                    Debug.LogWarning("Item at index " + (i + 1) + " in the Pool has no prefab !");
                    continue;
                }

                if (Pools.ContainsKey(name))
                {
                    Debug.LogWarning("Duplicates found in the Pool : " + name);
                }

                // pre-allocate the game objs
                var tmpList = new List<Transform>();

                for (var j = 0; j < item.instancesToPreload; ++j)
                {
                    var newTransform = GameObject.Instantiate(prefabTransform, Vector3.zero, prefabTransform.rotation) as Transform;
                    newTransform.name = name;
                    newTransform.parent = parentTransform;
                    newTransform.gameObject.SetActive(false);

                    tmpList.Add(newTransform);
                }

                var newPrefabPool = new EZ_PrefabPool(tmpList);
                newPrefabPool.showDebugLog = item.showDebugLog;
                newPrefabPool.poolCanGrow = item.poolCanGrow;
                newPrefabPool.parentTransform = parentTransform;

                newPrefabPool.cullDespawned = item.cullDespawned;
                newPrefabPool.cullAbove = item.cullAbove;
                newPrefabPool.cullDelay = item.cullDelay;
                newPrefabPool.cullAmount = item.cullAmount;

                newPrefabPool.enableHardLimit = item.enableHardLimit;
                newPrefabPool.hardLimit = item.hardLimit;

                newPrefabPool.recycle = item.recycle;

                Pools.Add(name, newPrefabPool); //add the pool to the Dictionary
            }

            foreach (var item in itemsMarkedForDeletion)
            {
                prefabPoolOptions.Remove(item);
            }

            itemsMarkedForDeletion.Clear();
        }

        /// <summary>
        /// Update the PoolManager.
        /// </summary>
        void Update()
        {
            foreach (var item in Pools)
            {
                var prefabPool = Pools[item.Key];

                prefabPool.Poll();
            }
        }

        /// <summary>
        /// Method to create a new pool during run time. autoAddMissingPrefabPool must be enabled
        /// </summary>
        private static void CreateMissingPrefabPool(Transform missingTrans, string name)
        {
            var newPrefabPool = new EZ_PrefabPool();

            //Set the new pool options here
            newPrefabPool.parentTransform = parentTransform;
            newPrefabPool.poolCanGrow = true;

            Pools.Add(name, newPrefabPool);

            // for the Inspector only
            var newPrefabPoolOption = new EZ_PrefabPoolOption();
            newPrefabPoolOption.prefabTransform = missingTrans;
            newPrefabPoolOption.poolCanGrow = true;
            EZ_PoolManager.Instance.prefabPoolOptions.Add(newPrefabPoolOption);

            if (EZ_PoolManager.Instance.showDebugLog)
            {
                Debug.Log("EZ_PoolManager created Pool Item for missing item : " + name);
            }
        }

        /// <summary>
        /// Spawn a GameObject from the specified pool, if the pool's hard limit
        /// has not been met. If the pool does not exist, the returned GameObject
        /// will be a null reference.
        /// The OnSpawned method will be called on the GameObject.
        /// </summary>
        public static Transform Spawn(Transform transToSpawn, Vector3 position, Quaternion rotation)
        {
            if (transToSpawn == null)
            {
                Debug.LogWarning("No Transform passed to Spawn() !");
                return null;
            }

            if (!EZ_PoolManager.Instance.usePoolManager)
            {
                var newTransform = GameObject.Instantiate(transToSpawn, Vector3.zero, transToSpawn.rotation) as Transform;
                newTransform.name = transToSpawn.name;
                newTransform.parent = parentTransform;

                return newTransform;
            }

            var name = transToSpawn.name;
            if (!Pools.ContainsKey(name))
            {
                if (EZ_PoolManager.Instance.autoAddMissingPrefabPool)
                {
                    CreateMissingPrefabPool(transToSpawn, name);
                }
                else
                {
                    Debug.LogWarning(name + " passed to Spawn() is not in the Pool Manager.");
                    return null;
                }
            }

            return Pools[name].Spawn(transToSpawn, position, rotation);
        }

        /// <summary>
        /// Despawn the specified GameObject
        /// The OnDespawned method will be called on the GameObject.
        /// </summary>
        public static void Despawn(Transform transToDespawn)
        {
            if (transToDespawn == null)
            {
                Debug.LogWarning("No Transform passed to Despawn() !");
                return;
            }

            if (!EZ_PoolManager.Instance.usePoolManager)
            {
                GameObject.Destroy(transToDespawn.gameObject);
                return;
            }

            if (!transToDespawn.gameObject.activeInHierarchy)
            {
                return;
            }

            var name = transToDespawn.name;
            if (!Pools.ContainsKey(name))
            {
                Debug.LogWarning(name + " passed to Despawn() is not in the Pool.");
                return;
            }

            Pools[name].Despawn(transToDespawn);
        }

        /// <summary>
        /// Get the pool of the item
        /// </summary>
        public static EZ_PrefabPool GetPool(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return null;
            }

            if (!Pools.ContainsKey(name))
            {
                return null;
            }

            return Pools[name];
        }
    }

}