using System;
using System.Collections.Generic;
using System.Linq;
using AppsFlyerSDK;
using CI.Utils.Extentions;
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
        public static bool isAppsFlyerInit => MainInitialization.initStatus[nameof(AppsFlyerInitialization)];
        public static bool isAppLovinInit => MainInitialization.initStatus[nameof(AppLovinInitialization)];
        public static bool isDTDInit => MainInitialization.initStatus[nameof(DevToDevInitialization)];
        public static bool isFSBInit => MainInitialization.initStatus[nameof(FacebookSdkInitialization)];
        public static bool isFirebaseInit => MainInitialization.initStatus[nameof(FirebaseInitialization)];
        
        public static void TutorialCompleted()
        {
            if (isAppsFlyerInit)
                AppsFlyer.sendEvent("af_tutorial_completion", new());
        }
        
        public static void AdToCart(int count) // more than 30 ad impressions
        {
            SendAppsFlyerEvent("af_add_to_cart","count", count);
        }
        
        public static void AdRewardCount(int count) // ad views
        {
            SendAppsFlyerEvent("ad_reward","count", count);
        }
        
        private static void SendAppsFlyerEvent(string eventName,string parameter_name, int value)
        {
            var dict = new Dictionary<string, string>();
            
            dict.Add(parameter_name, value.ToString());
            
            if (isAppsFlyerInit)
                AppsFlyer.sendEvent(eventName, dict);
        }

        public static void TrackEvent(string eventName, List<KeyValuePair<string,string>> arguments)
        {
            if(isDTDInit)
                    DTDAnalytics.CustomEvent(eventName, arguments.ToCustomParameters());
            
            if (isFirebaseInit)
                    FirebaseAnalytics.LogEvent(eventName,new Parameter(eventName,JsonUtility.ToJson(arguments)));
        }

        public static void TrackEvent(string eventName, KeyValuePair<string,string> eventPair)
        {
            var arguments = new Dictionary<string,string>();
            arguments.Add(eventPair.Key, eventPair.Value);
            
            if(isDTDInit)
                DTDAnalytics.CustomEvent(eventName, arguments.ToList().ToCustomParameters());
     
            if (isFirebaseInit)
                FirebaseAnalytics.LogEvent(eventName,new Parameter(eventName,JsonUtility.ToJson(arguments)));
        }

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

        public static void TrackEvent(string eventName)
        {
            if (isFirebaseInit)
                FirebaseAnalytics.LogEvent(eventName);
            
            if (isDTDInit)
                DTDAnalytics.CustomEvent(eventName);
        }

        
        public static void Log(string message)
        {
            if (isFirebaseInit)
                FirebaseAnalytics.LogEvent(message);
        }
        
        public static void LogError(string error)
        {
            if (isFirebaseInit)
                Firebase.Crashlytics.Crashlytics.Log(error);
        }

        public static void LogPurshaseInitiation(UnityEngine.Purchasing.Product product)
        {
            if(product == null)
                throw new Exception("Product is null");
            
            TrackEvent("purchasing_init",new KeyValuePair<string, string>("purchasing_init",product.definition.id));
            
            if (isAppsFlyerInit)
                AppsFlyer.sendEvent("af_initiated_checkout",new());
        }

        public static void LogPurchase(PurchaseEventArgs args)
        {
            InAppVerification.Validate(args.purchasedProduct.definition.id,args.purchasedProduct.receipt,"000", (isValid) =>
            {
                "Putchase Test".SplashLog(isValid ? "verified" : "not verified");
                args.purchasedProduct.definition.id.PlayboxInfo(args.purchasedProduct.receipt);
                
                if(!isValid)
                    return;
                /*
                if (isFSBInit && FB.IsInitialized)
                {
                    var dict = new Dictionary<string, object>();
                    dict.Add("product_id",args.purchasedProduct.definition.id);
                    
                    FB.LogAppEvent("purchasing_init",null,dict);   
                }
                */
                string orderId = args.purchasedProduct.transactionID;
                string productId = args.purchasedProduct.definition.id;
                var price = args.purchasedProduct.metadata.localizedPrice;
                string currency = args.purchasedProduct.metadata.isoCurrencyCode;
            
                if (isDTDInit)
                    DTDAnalytics.RealCurrencyPayment(orderId,(double)price, productId, currency);
            
                Dictionary<string, string> eventValues = new ()
                {
                    { "af_currency", currency },
                    { "af_revenue", price.ToString() },
                    { "af_quantity", "1" },
                    { "af_content_id", productId }
                };

                if (isAppsFlyerInit)
                    AppsFlyer.sendEvent("af_purchase", eventValues);
            });
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
            if (isFirebaseInit)
                FirebaseAnalytics.LogEvent("ad_impression", impressionParameters);
        }
    }
}