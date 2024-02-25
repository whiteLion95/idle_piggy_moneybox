using System;
using System.Collections.Generic;
using System.Linq;
using com.adjust.sdk;
using com.adjust.sdk.purchase;
using Deslab.Scripts.Deslytics.Ads;
using Mamboo.Analytics.Adjust.Internal;
using Firebase.Analytics;
using GameAnalyticsSDK;
using Mamboo.Internal.Analytics;
using Mamboo.Purchase.Scripts;
using UnityEngine;
using UnityEngine.Purchasing;

namespace Mamboo.Internal.Purchase
{
    public class PurchaseController : MonoBehaviour, IStoreListener
    {
        public const string Version = "1.8.0";
        public static PurchaseController instance;
        public IStoreController storeController;
        private IExtensionProvider extensionsController;

        [SerializeField] private PurchaseConfig _purchaseConfig;

        private static bool purchaseCalled = false;

        public PurchaseConfig PurchaseConfig => _purchaseConfig;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(this);
            }
            else DestroyImmediate(this);
        }

        private void Start()
        {
            var standardPurchasingModule = StandardPurchasingModule.Instance();

#if UNITY_EDITOR
            standardPurchasingModule.useFakeStoreAlways = true;
            standardPurchasingModule.useFakeStoreUIMode = FakeStoreUIMode.StandardUser;
#endif

            _purchaseConfig = PurchaseConfig.Load();

            var builder = ConfigurationBuilder.Instance(standardPurchasingModule);

            foreach (var product in PurchaseConfig.Products)
            {
                builder.AddProduct(product.ProductId, product.ProductType);
            }

            UnityPurchasing.Initialize(this, builder);
        }

        #region IStoreListener
        public static event Action InitializationCompleted;

        public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
        {
            storeController = controller;
            extensionsController = extensions;

            // TODO PURCHASE: uncomment this if you need automatic restore purchases on start
            // WARNING: enabling this can be the reason of build rejection because restore button will be pressed
            // when the restore has already happened and from UI it looks like it does not work
            InitializationCompleted?.Invoke();
            extensions.GetExtension<IAppleExtensions>().RestoreTransactions(RestoreTransactions);
            
            // Debug.LogError("Products count : " + storeController.products.all.Length);
            //
            // foreach (var t in storeController.products.all)
            // {
            //     Debug.LogError(t.definition.id);
            // }
        }

        private void RestoreTransactions(bool result)
        {
            if (result)
            {
                Debug.LogError("RestorePurchases success");
            }
        }

        public void OnInitializeFailed(InitializationFailureReason error)
        {
            Debug.LogError($"Store initialization faild! Error: {error.ToString()}");
        }

        public void OnPurchaseFailed(Product i, PurchaseFailureReason p)
        {
            Debug.Log($"{i.transactionID} failed becuse of {p.ToString()}");
        }

        /// <summary>
        /// This method called automatically by Unity when purchase or restore successfully completed
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs e)
        {
            Debug.Log("ProcessPurchase called for :: " + e.purchasedProduct.definition.id);

            // TODO PURCHASE: compare you e.purchasedProduct.definition.id to call needed logic in the game
            if (e.purchasedProduct.definition.id == "removeads_id")
            {
                Debug.Log("ProcessPurchase: + removeads_id = success");
                MaxAdsManager.instance.RemoveAds();
            }
            if (PurchaseCompleted != null)
            {
                Debug.Log("ProcessPurchase: + " + e.purchasedProduct.definition.id + " success");
                PurchaseCompleted.Invoke(e);
            }
            else
            {
                Debug.LogError("ProcessPurchase NOT success");
            }

            if (purchaseCalled)
            {
                var product = e.purchasedProduct;
                var meta = product.metadata;
                var definition = product.definition;
                var itemType = definition.type.ToString();
                var itemId = definition.id;
                string receipt = product.receipt;
                var cartType = "shop";
                string currency = product.metadata.isoCurrencyCode;
                int amount = decimal.ToInt32(product.metadata.localizedPrice * 100);
                
#if !UNITY_EDITOR
                NotifyAdjust(e);
#endif

#if UNITY_IOS && !UNITY_EDITOR
                Receipt receiptiOS = JsonUtility.FromJson<Receipt>(receipt);
                string receiptPayload = receiptiOS.Payload;
                GameAnalytics.NewBusinessEventIOS(currency, amount, itemType, itemId, cartType, receiptPayload);
#elif UNITY_ANDROID && !UNITY_EDITOR
                Receipt receiptAndroid = JsonUtility.FromJson<Receipt>(receipt);
                PayloadAndroid receiptPayload = JsonUtility.FromJson<PayloadAndroid>(receiptAndroid.Payload);
                GameAnalytics.NewBusinessEventGooglePlay(currency, amount, itemType, itemId, cartType, receiptPayload.json, receiptPayload.signature);          
#endif
            }

            purchaseCalled = false;

            return PurchaseProcessingResult.Complete;
        }

        #endregion

        private void RemoveAdsInternal()
        {
            // TODO PURCHASE: add the logic for your AD manager to exclude needed ads from showing + save this state to PlayerPrefs
            AdsManager.AdsRemoved();
        }

        // TODO PURCHASE: call this method from UI on Remove ads button click
        public void RemoveAds()
        {
            purchaseCalled = true;
            var removeAdsId = "removeads";
            storeController?.InitiatePurchase(removeAdsId);
        }
        
        public void RemoveInterAds()
        {
            purchaseCalled = true;
            var removeAdsId = "remove_inter_ads";
            storeController?.InitiatePurchase(removeAdsId);
        }

        // TODO PURCHASE: call this method and pass your product id
        public void YourProductBuy(string yourProductId)
        {
            // TODO PURCHASE: important to set 'purchaseCalled = true' when you click on UI button to buy something
            purchaseCalled = true;
            storeController?.InitiatePurchase(yourProductId);
        }

        // TODO PURCHASE: call this from UI restore purchase button
        public void RestorePurchase()
        {
            extensionsController.GetExtension<IAppleExtensions>().RestoreTransactions(RestoreTransactions);
        }

        private void NotifyAdjust(PurchaseEventArgs e)
        {
            var price = e.purchasedProduct.metadata.localizedPrice;
            double lPrice = decimal.ToDouble(price);
            var currencyCode = e.purchasedProduct.metadata.isoCurrencyCode;

            var wrapper = (Dictionary<string, object>) MiniJson.JsonDecode(e.purchasedProduct.receipt);
            if (null == wrapper)
            {
                return;
            }

            var payload = (string) wrapper["Payload"]; // For Apple this will be the base64 encoded ASN.1 receipt
            var productId = e.purchasedProduct.definition.id;
            var type = e.purchasedProduct.definition.type;

#if UNITY_ANDROID

            var gpDetails = (Dictionary<string, object>) MiniJson.JsonDecode(payload);
            Debug.Log($"gpDetails: {payload}");
            
            var gpSig = (string) gpDetails["signature"];
            Debug.Log($"signature: {gpSig}");
            
            var gpJson = (Dictionary<string, object>) MiniJson.JsonDecode(gpDetails["json"].ToString());
            Debug.Log($"json: {gpJson}");

            var skuListDetails = ((List<object>) gpDetails["skuDetails"])[0];
            Debug.Log($"skuDetails: {skuListDetails}");
            
            var skuDetails = (Dictionary<string, object>) MiniJson.JsonDecode(skuListDetails.ToString());
            Debug.Log($"skuDetails: {skuDetails}");

            var purchaseTime = gpJson.TryGetValue("purchaseTime", out var timeValue)
                ? ((long) timeValue).ToString()
                : "";
            var pricemicros = skuDetails.TryGetValue("price_amount_micros", out var micros)
                ? ((long) micros).ToString()
                : "";
            var orderId = (string) gpJson["orderId"];
            var purchaseToken = (string) gpJson["purchaseToken"];
            var developerPayload = gpJson.TryGetValue("developerPayload", out var value) ? (string) value : "";

            CompletedAndroidPurchase(new PurchaseMetadata
            {
                ProductId = productId,
                Platform = PurchasePlatform.ANDROID,
                Type = type,
                PurchaseTime = purchaseTime,
                PurchaseToken = purchaseToken,
                CurrencyCode = currencyCode,
                UnitPrice = lPrice,
                OrderId = orderId,
                DeveloperPayload = developerPayload,
                Signature = gpSig,
                PriceMicros = pricemicros
            });
#elif UNITY_IOS
            var receipt = e.purchasedProduct.receipt;
            var transactionId = e.purchasedProduct.transactionID;
            
            CompletedIosPurchase(new PurchaseMetadata
            {
                ProductId = productId,
                Platform = PurchasePlatform.IOS,
                Type = type,
                CurrencyCode = currencyCode,
                UnitPrice = lPrice,
                TransactionId = transactionId,
                Receipt = receipt
            });
#endif
        }

        private static void CompletedAndroidPurchase(PurchaseMetadata metadata)
        {
            AdjustPurchase.VerifyPurchaseAndroid(metadata.ProductId, metadata.PurchaseToken, metadata.DeveloperPayload,
                VerificationInfoDelegate);

            void VerificationInfoDelegate(ADJPVerificationInfo verificationInfo)
            {
                switch (verificationInfo.VerificationState)
                {
                    case ADJPVerificationState.ADJPVerificationStatePassed:
                    {
                        TrackPurchase(metadata);
                        break;
                    }
                    case ADJPVerificationState.ADJPVerificationStateFailed:
                    {
                        var adjustEvent = new AdjustEvent(AdjustConstants.Purchase_failed);
                        Adjust.trackEvent(adjustEvent);
                        break;
                    }
                    case ADJPVerificationState.ADJPVerificationStateUnknown:
                    {
                        var adjustEvent = new AdjustEvent(AdjustConstants.Purchase_unknown);
                        Adjust.trackEvent(adjustEvent);
                        break;
                    }
                    default:
                    {
                        var adjustEvent = new AdjustEvent(AdjustConstants.Purchase_not_verified);
                        Adjust.trackEvent(adjustEvent);
                        break;
                    }
                }
            }
        }

        private static void CompletedIosPurchase(PurchaseMetadata metadata)
        {
            AdjustPurchase.VerifyPurchaseiOS(metadata.Receipt, metadata.TransactionId, metadata.ProductId,
                VerificationInfoDelegate);

            void VerificationInfoDelegate(ADJPVerificationInfo verificationInfo)
            {
                switch (verificationInfo.VerificationState)
                {
                    case ADJPVerificationState.ADJPVerificationStatePassed:
                    {
                        TrackPurchase(metadata);
                        break;
                    }
                    case ADJPVerificationState.ADJPVerificationStateFailed:
                    {
                        var adjustEvent = new AdjustEvent(AdjustConstants.Purchase_failed);
                        Adjust.trackEvent(adjustEvent);
                        break;
                    }
                    case ADJPVerificationState.ADJPVerificationStateUnknown:
                    {
                        var adjustEvent = new AdjustEvent(AdjustConstants.Purchase_unknown);
                        Adjust.trackEvent(adjustEvent);
                        break;
                    }
                    default:
                    {
                        var adjustEvent = new AdjustEvent(AdjustConstants.Purchase_not_verified);
                        Adjust.trackEvent(adjustEvent);
                        break;
                    }
                }
            }
        }

        private static void TrackPurchase(PurchaseMetadata metadata)
        {
            ReportAppmetricaPurchaseEvents(metadata);
            ReportFirebasePurchaseEvents(metadata);
            ReportAdjustPurchaseEvents(metadata);

            Debug.Log($"[Mamboo SDK] Purchase tracked");
        }

        private static void ReportAppmetricaPurchaseEvents(PurchaseMetadata metadata)
        {
            ReportPurchaseTime();
            AppMetrica.Instance.ReportRevenue(
                new YandexAppMetricaRevenue((decimal) metadata.UnitPrice, metadata.CurrencyCode)
                {
                    Quantity = 1,
                    ProductID = metadata.ProductId,
                    Payload = metadata.DeveloperPayload,
                    Receipt = new YandexAppMetricaReceipt
                    {
                        Signature = metadata.Signature,
                        TransactionID = metadata.Platform == PurchasePlatform.ANDROID
                            ? metadata.OrderId
                            : metadata.TransactionId
                    }
                });
        }

        private static void ReportFirebasePurchaseEvents(PurchaseMetadata metadata)
        {
            var productWithDollarPrice =
                ProductsConstants.products.FirstOrDefault(x => x.productId == metadata.ProductId);
            if (productWithDollarPrice != null)
            {
                var price = metadata.Platform == PurchasePlatform.ANDROID
                    ? productWithDollarPrice.priceAndroid
                    : productWithDollarPrice.priceIos;
                FirebaseAnalytics.LogEvent("in_app_purchase_1",
                    new Parameter("value", productWithDollarPrice.priceAndroid),
                    new Parameter("currency", "USD"));
                FirebaseAnalytics.LogEvent("in_app_purchase_2",
                    new Parameter("value", productWithDollarPrice.priceAndroid),
                    new Parameter("currency", "USD"));
                FirebaseAnalytics.LogEvent("in_app_purchase_3",
                    new Parameter("value", productWithDollarPrice.priceAndroid),
                    new Parameter("currency", "USD"));
                FirebaseAnalytics.LogEvent("total_revenue", new Parameter("value", productWithDollarPrice.priceAndroid),
                    new Parameter("currency", "USD"));
            }

            FirebaseAnalytics.LogEvent("in-app-purchase");
        }

        private static void ReportAdjustPurchaseEvents(PurchaseMetadata metadata)
        {
            if (metadata.Type == ProductType.Subscription && metadata.Platform == PurchasePlatform.ANDROID)
            {
                var subscription = new AdjustPlayStoreSubscription(metadata.PriceMicros, metadata.CurrencyCode,
                    metadata.ProductId, metadata.OrderId, metadata.Signature, metadata.PurchaseToken);
                subscription.setPurchaseTime(metadata.PurchaseTime);
                Adjust.trackPlayStoreSubscription(subscription);
                Debug.Log($"[Mamboo SDK] Subscription tracked");
            }
            else
            {
                var adjustEvent = new AdjustEvent(AdjustConstants.Purchase);
                adjustEvent.setRevenue(metadata.UnitPrice, metadata.CurrencyCode);
                adjustEvent.setTransactionId(metadata.OrderId);
                Adjust.trackEvent(adjustEvent);
            }
        }

        private static void ReportPurchaseTime()
        {
            var playTime = PlaytimeAchivements.playTime.value;
            var playMinute = playTime / 60;
            string purchaseMinute = null;

            if (playMinute >= 0 && playMinute < 5)
            {
                purchaseMinute = "0";
            }
            else if (playMinute >= 5 && playMinute < 10)
            {
                purchaseMinute = "5";
            }
            else if (playMinute >= 10 && playMinute < 15)
            {
                purchaseMinute = "10";
            }
            else if (playMinute >= 15 && playMinute < 20)
            {
                purchaseMinute = "15";
            }
            else if (playMinute >= 20 && playMinute < 25)
            {
                purchaseMinute = "20";
            }
            else if (playMinute >= 25 && playMinute < 30)
            {
                purchaseMinute = "25";
            }
            else if (playMinute >= 30)
            {
                purchaseMinute = "30";
            }

            AppMetrica.Instance.ReportEvent($"purchase_{purchaseMinute}_min");
        }

        public void OnInitializeFailed(InitializationFailureReason error, string message)
        {
            throw new NotImplementedException();
        }

        private class PurchaseMetadata
        {
            public ProductType Type { get; set; }
            public string ProductId { get; set; }
            public PurchasePlatform Platform { get; set; }
            public string CurrencyCode { get; set; }
            public double UnitPrice { get; set; }
            public string OrderId { get; set; }
            public string TransactionId { get; set; }
            public string Receipt { get; set; }
            public string DeveloperPayload { get; set; }
            public string Signature { get; set; }
            public string PurchaseToken { get; set; }
            public string PriceMicros { get; set; }
            public string PurchaseTime { get; set; }
        }

        public enum PurchasePlatform
        {
            ANDROID,
            IOS
        }

        public class Receipt
        {
            public string Store;
            public string TransactionID;
            public string Payload;

            public Receipt()
            {
                Store = TransactionID = Payload = "";
            }

            public Receipt(string store, string transactionID, string payload)
            {
                Store = store;
                TransactionID = transactionID;
                Payload = payload;
            }
        }

        public class PayloadAndroid
        {
            public string json;
            public string signature;

            public PayloadAndroid()
            {
                json = signature = "";
            }

            public PayloadAndroid(string _json, string _signature)
            {
                json = _json;
                signature = _signature;
            }
        }

        public static event Action<PurchaseEventArgs> PurchaseCompleted;
    }
}