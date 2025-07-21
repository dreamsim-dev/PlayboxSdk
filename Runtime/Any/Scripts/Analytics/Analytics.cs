using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using AppsFlyerSDK;
using CI.Utils.Extentions;
using DevToDev.Analytics;
using Firebase.Analytics;
using Firebase.Crashlytics;
using UnityEngine;
using UnityEngine.Purchasing;

/*
    af_initiated_checkout - инициация покупки
    af_level_achieved - поднятие уровня
    af_purchase - совершил покупку
    af_tutorial_completion - прошел туториал
    
    af_add_to_cart - 30 просмотров рекламы
    ad_reward - отправляет колличество просмотров рекламы
 */

namespace Playbox
{
    /// <summary>
    /// Clavicular static analytics collection class
    /// </summary>
    public static class Analytics
    {
        public static bool isAppsFlyerInit => IsValidate<AppsFlyerInitialization>();
        public static bool isAppLovinInit => IsValidate<AppLovinInitialization>();
        public static bool isDTDInit => IsValidate<DevToDevInitialization>();
        public static bool isFSBInit => IsValidate<FacebookSdkInitialization>();
        public static bool isFirebaseInit => IsValidate<FirebaseInitialization>();
        
        private static bool IsValidate<T>() where T : PlayboxBehaviour
        {
            return MainInitialization.IsValidate<T>();
        }
        
        /// <summary>
        /// Sends a custom event to AppsFlyer
        /// </summary>
        public static void SendAppsFlyerEvent(string eventName,string parameter_name, int value)
        {
            var dict = new Dictionary<string, string>();
            
            dict.Add(parameter_name, value.ToString());
            
            if (isAppsFlyerInit)
                AppsFlyer.sendEvent(eventName, dict);
        }

        /// <summary>
        /// Commits a custom event to DTD and Firebase
        /// </summary>
        public static void TrackEvent(string eventName, List<KeyValuePair<string,string>> arguments)
        {
            if(isDTDInit)
                    DTDAnalytics.CustomEvent(eventName, arguments.ToCustomParameters());
            
            //if (isFirebaseInit)
            //        FirebaseAnalytics.LogEvent(eventName,new Parameter(eventName,JsonUtility.ToJson(arguments)));
        }

        public static void TrackEvent(string eventName, KeyValuePair<string,string> eventPair)
        {
            var arguments = new Dictionary<string,string>();
            arguments.Add(eventPair.Key, eventPair.Value);
            
            if(isDTDInit)
                DTDAnalytics.CustomEvent(eventName, arguments.ToList().ToCustomParameters());
     
            //if (isFirebaseInit)
            //    FirebaseAnalytics.LogEvent(eventName,new Parameter(eventName,JsonUtility.ToJson(arguments)));
        }

        public static void TrackEvent(string eventName)
        {
            if (isFirebaseInit)
                FirebaseAnalytics.LogEvent(eventName);
            
            if (isDTDInit)
                DTDAnalytics.CustomEvent(eventName);
        }

        
        public static void Log(string message)
        {
          //  if (isFirebaseInit)
          //      FirebaseAnalytics.LogEvent(message);
            message.PlayboxInfo("Analytics");
        }

        public static void LogPurshaseInitiation(UnityEngine.Purchasing.Product product)
        {
            if(product == null)
                throw new Exception("Product is null");
            
            TrackEvent("purchasing_init",new KeyValuePair<string, string>("purchasing_init",product.definition.id));
            
            if (isAppsFlyerInit)
                AppsFlyer.sendEvent("af_initiated_checkout",new());
        }
        
        public static void LogPurchase(Product purchasedProduct, Action<bool> onValidate  = null)
        {
            if(purchasedProduct == null)
            {
                if(isFirebaseInit)
                    Crashlytics.LogException(new Exception("[PlayboxLogging] purchasedProduct is null"));
                return;
            }

            string orderId = purchasedProduct.transactionID;
            string productId = purchasedProduct.definition.id;
            var price = purchasedProduct.metadata.localizedPrice;
            string currency = purchasedProduct.metadata.isoCurrencyCode;
            
            Dictionary<string, string> eventValues = new ()
            {
                { "af_currency", currency },
                { "af_revenue", price.ToString(CultureInfo.InvariantCulture) },
                { "af_quantity", "1" },
                { "af_content_id", productId }
            };
            
            InAppVerification.Validate(purchasedProduct.definition.id,purchasedProduct.receipt,"000", (isValid) =>
            {
                onValidate?.Invoke(isValid);
                
                if (isValid)
                {
                    Events.RealCurrencyPayment(orderId, (double)price, productId, currency);
                    Events.AppsFlyerPayment(eventValues);
                }
            });
        }

        public static void LogPurchase(PurchaseEventArgs args, Action<bool> onValidate  = null)
        {
            if (args != null)
            {
                if(isFirebaseInit)
                    Crashlytics.LogException(new Exception("[PlayboxLogging] purchase Args is null"));
                    
                    
                LogPurchase(args.purchasedProduct, onValidate);
            }
        }

        public static void TrackAd(MaxSdkBase.AdInfo impressionData)
        {
            Events.AdImpression(impressionData.NetworkName, impressionData.Revenue, impressionData.Placement, impressionData.AdUnitIdentifier);
        }
        
        public static class Events
        {
            public static void LogLevelUp(int level)
            {
                if (isDTDInit)
                    DTDAnalytics.LevelUp(level);
            
                SendAppsFlyerEvent("af_level_achieved","level",level);
            }
            
            public static void LogContentView(string content)
            {
                TrackEvent(nameof(LogContentView),new KeyValuePair<string, string>(nameof(LogContentView),content));
            }
            
            public static void LogTutorial(string tutorial, ETutorialState stateLevel = ETutorialState.Complete, string step = "none")
            {
                switch (stateLevel)
                {
                    case ETutorialState.Start:
                        TrackEvent(tutorial,new KeyValuePair<string, string>("start",step));
                        break;
                
                    case ETutorialState.Skipped:
                        TrackEvent(tutorial,new KeyValuePair<string, string>("skip",step));
                        break;
                
                    case ETutorialState.Complete:
                        TrackEvent(tutorial,new KeyValuePair<string, string>("complete",step));
                        break;
                
                    case ETutorialState.StepComplete:
                        TrackEvent(tutorial,new KeyValuePair<string, string>("stepComplete",step));
                        break;
                
                    default:
                        TrackEvent(tutorial,new KeyValuePair<string, string>("completed",step));
                        break;
                }
            }
            
            /// <summary>
            /// Sends the event if the tutorial is completed
            /// </summary>
            public static void TutorialCompleted()
            {
                if (isAppsFlyerInit)
                    AppsFlyer.sendEvent("af_tutorial_completion", new());
            }
            
            /// <summary>
            /// Is sent every 30 ad views
            /// </summary>
            public static void AdToCart(int count) // more than 30 ad impressions
            {
                SendAppsFlyerEvent("af_add_to_cart","count", count);
            }
            
            /// <summary>
            /// Sends the number of video ad views
            /// </summary>
            /// 
            public static void AdRewardCount(int count) // ad views
            {
                SendAppsFlyerEvent("ad_reward","count", count);
            }

            /// <summary>
            /// Logs the player's current balance for all virtual currencies to DevToDev.
            /// </summary>
            public static void CurrentBalance(Dictionary<string, long> balance)
            {
                if (isDTDInit) DTDAnalytics.CurrentBalance(balance);
            }
            
            /// <summary>
            /// Logs an accrual (earning) of virtual currency to DevToDev.
            /// </summary>
            public static void CurrencyAccrual(string currencyName, int currencyAmount, string source,
                DTDAccrualType type)
            {
                if (isDTDInit) DTDAnalytics.CurrencyAccrual(currencyName, currencyAmount, source, type);
            }
            /// <summary>
            /// Logs a real-money purchase transaction to DevToDev.
            /// </summary>
            public static void RealCurrencyPayment(string orderId, double price, string productId, string currencyCode)
            {
                if (isDTDInit) DTDAnalytics.RealCurrencyPayment(orderId, price, productId, currencyCode);
            }
            /// <summary>
            /// Logs a virtual currency purchase transaction to DevToDev.
            /// </summary>
            public static void VirtualCurrencyPayment(string purchaseId, string purchaseType, int purchaseAmount,
                Dictionary<string, int> resources)
            {
                if (isDTDInit) DTDAnalytics.VirtualCurrencyPayment(purchaseId, purchaseType, purchaseAmount, resources);
            }
            /// <summary>
            /// Logs an ad impression event with revenue details to DevToDev.
            /// </summary>
            public static void AdImpression(string network, double revenue, string placement, string unit)
            {
                if (isDTDInit) DTDAnalytics.AdImpression(network, revenue, placement, unit);
            }
            /// <summary>
            /// Tracks a tutorial step completion in DevToDev.
            /// </summary>
            public static void Tutorial(int step)
            {
                if (isDTDInit) DTDAnalytics.Tutorial(step);
            }
            /// <summary>
            /// Logs a successful social network connection event to DevToDev.
            /// </summary>
            public static void SocialNetworkConnect(DTDSocialNetwork socialNetwork)
            {
                if (isDTDInit) DTDAnalytics.SocialNetworkConnect(socialNetwork);
            }
            /// <summary>
            /// Logs a social network post event to DevToDev.
            /// </summary>
            public static void SocialNetworkPost(DTDSocialNetwork socialNetwork, string reason)
            {
                if (isDTDInit) DTDAnalytics.SocialNetworkPost(socialNetwork, reason);
            }
            /// <summary>
            /// Logs referral information (source, campaign, etc.) to DevToDev.
            /// </summary>
            public static void Referrer(Dictionary<DTDReferralProperty, string> referrer)
            {
                if (isDTDInit) DTDAnalytics.Referrer(referrer);
            }
            /// <summary>
            /// Sends a purchase event with provided values to AppsFlyer.
            /// </summary>
            public static void AppsFlyerPayment(Dictionary<string,string> appsFlyerPaymentValues)
            {
                if (isAppsFlyerInit) AppsFlyer.sendEvent("af_purchase", appsFlyerPaymentValues);
            }
            /// <summary>
            /// Starts tracking a progression event in DevToDev.
            /// </summary>
            public static void StartProgressionEvent(string eventName)
            {
                if (isDTDInit) DTDAnalytics.StartProgressionEvent(eventName);
            }
            /// <summary>
            /// Starts tracking a progression event with custom parameters in DevToDev.
            /// </summary>
            public static void StartProgressionEvent(string eventName, DTDStartProgressionEventParameters parameters)
            {
                if (isDTDInit) DTDAnalytics.StartProgressionEvent(eventName, parameters);
            }
            /// <summary>
            /// Marks a progression event as finished in DevToDev.
            /// </summary>
            public static void FinishProgressionEvent(string eventName)
            {
                if (isDTDInit) DTDAnalytics.FinishProgressionEvent(eventName);
            }
            /// <summary>
            /// Marks a progression event as finished with custom parameters in DevToDev.
            /// </summary>
            public static void FinishProgressionEvent(string eventName, DTDFinishProgressionEventParameters parameters)
            {
                if (isDTDInit) DTDAnalytics.FinishProgressionEvent(eventName, parameters);
            }
            /// <summary>
            /// Sends a custom log message to Firebase Crashlytics.
            /// </summary>
            public static void CrashlyticsLog(string eventName, string message)
            {
                if (isFirebaseInit)
                {
                    Crashlytics.Log($"{eventName} : {message}");
                }
            }
            
            public static void FirebaseEvent(string eventName, string message)
            {
                if (isFirebaseInit)
                {
                    FirebaseAnalytics.LogEvent($"{eventName} : {message}");
                }
            }
        }
    }
}