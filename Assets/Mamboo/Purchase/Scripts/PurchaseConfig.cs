using System;
using UnityEngine;
using UnityEngine.Purchasing;

namespace Mamboo.Internal.Purchase
{
    [CreateAssetMenu(menuName = "PurchaseConfig", fileName = "PurchaseConfig")]
    public class PurchaseConfig : ScriptableObject
    {
        private const string SETTING_RESOURCES_PATH = "Mamboo/PurchaseConfig";

        public static PurchaseConfig Load() => Resources.Load<PurchaseConfig>(SETTING_RESOURCES_PATH);

        [Serializable]
        public class PurchaseItem
        {
            [SerializeField]
            private double _priceIos;

            [SerializeField] 
            private double _priceAndroid;

            [SerializeField]
            private string _productId;

            [SerializeField]
            private ProductType _productType;

            public double Price => Application.platform == RuntimePlatform.Android || Application.isEditor ? _priceAndroid : _priceIos;

            public string ProductId => _productId;

            public ProductType ProductType => _productType;
        }

        [SerializeField]
        private PurchaseItem[] _products;

        public PurchaseItem[] Products => _products;
    }
}