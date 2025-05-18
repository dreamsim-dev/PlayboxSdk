using System.Collections.Generic;
using System.Linq;
using AppsFlyerSDK;
using DevToDev.Analytics;
using Facebook.Unity;
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
    public static class Analytics
    {
        public static void TutorialCompleted()
        {
            AppsFlyer.sendEvent("af_tutorial_completion", new());
        }
        
        public static void AdToCart(int count) // more than 30 ad impressions
        {
            SendEvent("af_add_to_cart","count", count);
        }
        
        public static void AdRewardCount(int count) // ad views
        {
            SendEvent("ad_reward","count", count);
        }
        
        private static void SendEvent(string eventName,string parameter_name, int value)
        {
            var dict = new Dictionary<string, string>();
            
            dict.Add(parameter_name, value.ToString());
            
            AppsFlyer.sendEvent(eventName, dict);
        }

        public static void TrackEvent(string eventName, List<KeyValuePair<string,string>> arguments)
        {
           //DTDAnalytics.CustomEvent(eventName, arguments.ToCustomParameters());
           
           //AppsFlyer.sendEvent(eventName, arguments.ToDictionary(a => a.Key, a => a.Value));
           
           FirebaseAnalytics.LogEvent(eventName,new Parameter(eventName,JsonUtility.ToJson(arguments)));
        }
        
        public static void TrackEvent(string eventName, KeyValuePair<string,string> eventPair)
        {
            var arguments = new Dictionary<string,string>();
            arguments.Add(eventPair.Key, eventPair.Value);
            
            //DTDAnalytics.CustomEvent(eventName, arguments.ToList().ToCustomParameters());
            
            //AppsFlyer.sendEvent(eventName, arguments);
           
            FirebaseAnalytics.LogEvent(eventName,new Parameter(eventName,JsonUtility.ToJson(arguments)));
        }

        public static void LogLevelUp(int level)
        {
            DTDAnalytics.LevelUp(level);
            TrackEvent("LevelUp",new KeyValuePair<string, string>("level",level.ToString()));
            SendEvent("af_level_achieved","level",level);
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

        public static void TrackEvent(string eventName)
        {
            AppsFlyer.AFLog(nameof(TrackEvent), eventName);
            
            FirebaseAnalytics.LogEvent(eventName);
        }
        
        public static void TrackSimpleEvent(string eventName, string value)
        {
            TrackEvent(eventName, new List<KeyValuePair<string,string>> {new(eventName, value)});
        }

        public static void Log(string message)
        {
            FirebaseAnalytics.LogEvent(message);
        }
        
        public static void LogError(string error)
        {
           Firebase.Crashlytics.Crashlytics.Log(error);
        }

        public static void LogPurshaseInitiation(UnityEngine.Purchasing.Product product)
        {
            TrackEvent("purchasing_init",new KeyValuePair<string, string>("purchasing_init",product.definition.id));
            
            FB.Purchase(product.definition.id,null);
            
            AppsFlyer.sendEvent("af_initiated_checkout",new());
        }

        public static void LogPurchase(PurchaseEventArgs args)
        {
            //TrackEvent("purchase",new KeyValuePair<string, string>("purchase",args.purchasedProduct.receipt));

            string orderId = args.purchasedProduct.transactionID;
            string productId = args.purchasedProduct.definition.id;
            var price = args.purchasedProduct.metadata.localizedPrice;
            string currency = args.purchasedProduct.metadata.isoCurrencyCode;
            
            DTDAnalytics.RealCurrencyPayment(orderId,(double)price, productId, currency);
            
            Dictionary<string, string> eventValues = new ()
            {
                { "af_currency", currency },
                { "af_revenue", price.ToString() },
                { "af_quantity", "1" },
                { "af_content_id", productId }
            };

            AppsFlyer.sendEvent("af_purchase", eventValues);
            FB.Purchase(productId,null);
        }

        public static void TrackAd(MaxSdkBase.AdInfo impressionData)
        {
            double revenue = impressionData.Revenue;
            
            var impressionParameters = new[] {
                new Parameter("ad_platform", "AppLovin"),
                new Parameter("ad_source", impressionData.NetworkName),
                new Parameter("ad_unit_name", impressionData.AdUnitIdentifier),
                new Parameter("ad_format", impressionData.AdFormat),
                new Parameter("value", revenue),
                new Parameter("currency", "USD"), 
            };
            
            //TO DO: Потом будем пулять в AppsFlyer тоже
            
            FirebaseAnalytics.LogEvent("ad_impression", impressionParameters);
        }
    }
}