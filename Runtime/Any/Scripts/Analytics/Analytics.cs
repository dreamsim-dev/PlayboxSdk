using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using AppsFlyerSDK;
using CI.Utils.Extentions;
using DevToDev.Analytics;
using Firebase.Analytics;
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
        /// Sends the event if the tutorial is completed
        /// </summary>
        [Obsolete("Move to 'Events' subclass")]
        public static void TutorialCompleted()
        {
            Events.TutorialCompleted();
        }
        
        /// <summary>
        /// Is sent every 30 ad views
        /// </summary>
        [Obsolete("Move to 'Events' subclass")]
        public static void AdToCart(int count) // more than 30 ad impressions
        {
            Events.AdToCart(count);
        }
        
        /// <summary>
        /// Sends the number of video ad views
        /// </summary>
        ///
        [Obsolete("Move to 'Events' subclass")]
        public static void AdRewardCount(int count) // ad views
        {
            Events.AdRewardCount(count);
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
        
        [Obsolete("Move to 'Events' subclass")]
        public static void LogLevelUp(int level)
        {
            Events.LogLevelUp(level);
        }
        
        [Obsolete("Move to 'Events' subclass")]
        public static void LogContentView(string content)
        {
            Events.LogContentView(content);
        }
        
        [Obsolete("Move to 'Events' subclass")]
        public static void LogTutorial(string tutorial, ETutorialState stateLevel = ETutorialState.Complete, string step = "none")
        {
            Events.LogTutorial(tutorial, stateLevel, step);
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
        
        public static void LogPurchase(Product purchasedProduct)
        {
            
            if(purchasedProduct == null)
            {
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
                if (isValid)
                {
                    Events.RealCurrencyPayment(orderId, (double)price, productId, currency);
                    Events.AppsFlyerPayment(eventValues);
                }
            });
        }

        public static void LogPurchase(PurchaseEventArgs args)
        {
            if (args != null)
            {
                LogPurchase(args.purchasedProduct);
            }
        }

        public static void TrackAd(MaxSdkBase.AdInfo impressionData)
        {
            double revenue = impressionData.Revenue;
            
            var impressionParameters = new[] {
                new Parameter("ad_platform", "AppLovin"),
                new Parameter("ad_source", impressionData.NetworkName),
                new Parameter("ad_unit_name", impressionData.AdUnitIdentifier),
                new Parameter("ad_format", impressionData.AdFormat),
                new Parameter("value", revenue.ToString(CultureInfo.InvariantCulture)),
                new Parameter("currency", "USD"), 
            };
            
            //if (isFirebaseInit)
            //    FirebaseAnalytics.LogEvent("ad_impression", impressionParameters);
            
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

            public static void CurrentBalance(Dictionary<string, long> balance)
            {
                if (isDTDInit) DTDAnalytics.CurrentBalance(balance);
            }

            public static void CurrencyAccrual(string currencyName, int currencyAmount, string source,
                DTDAccrualType type)
            {
                if (isDTDInit) DTDAnalytics.CurrencyAccrual(currencyName, currencyAmount, source, type);
            }

            public static void RealCurrencyPayment(string orderId, double price, string productId, string currencyCode)
            {
                if (isDTDInit) DTDAnalytics.RealCurrencyPayment(orderId, price, productId, currencyCode);
            }

            public static void VirtualCurrencyPayment(string purchaseId, string purchaseType, int purchaseAmount,
                Dictionary<string, int> resources)
            {
                if (isDTDInit) DTDAnalytics.VirtualCurrencyPayment(purchaseId, purchaseType, purchaseAmount, resources);
            }

            public static void AdImpression(string network, double revenue, string placement, string unit)
            {
                if (isDTDInit) DTDAnalytics.AdImpression(network, revenue, placement, unit);
            }

            public static void Tutorial(int step)
            {
                if (isDTDInit) DTDAnalytics.Tutorial(step);
            }

            public static void SocialNetworkConnect(DTDSocialNetwork socialNetwork)
            {
                if (isDTDInit) DTDAnalytics.SocialNetworkConnect(socialNetwork);
            }

            public static void SocialNetworkPost(DTDSocialNetwork socialNetwork, string reason)
            {
                if (isDTDInit) DTDAnalytics.SocialNetworkPost(socialNetwork, reason);
            }

            public static void Referrer(Dictionary<DTDReferralProperty, string> referrer)
            {
                if (isDTDInit) DTDAnalytics.Referrer(referrer);
            }

            public static void AppsFlyerPayment(Dictionary<string,string> appsFlyerPaymentValues)
            {
                if (isAppsFlyerInit) AppsFlyer.sendEvent("af_purchase", appsFlyerPaymentValues);
            }

            public static void StartProgressionEvent(string eventName)
            {
                if (isDTDInit) DTDAnalytics.StartProgressionEvent(eventName);
            }
            
            public static void StartProgressionEvent(string eventName, DTDStartProgressionEventParameters parameters)
            {
                if (isDTDInit) DTDAnalytics.StartProgressionEvent(eventName, parameters);
            }
            
            public static void FinishProgressionEvent(string eventName)
            {
                if (isDTDInit) DTDAnalytics.FinishProgressionEvent(eventName);
            }
            
            public static void FinishProgressionEvent(string eventName, DTDFinishProgressionEventParameters parameters)
            {
                if (isDTDInit) DTDAnalytics.FinishProgressionEvent(eventName, parameters);
            }
        }
    }
}