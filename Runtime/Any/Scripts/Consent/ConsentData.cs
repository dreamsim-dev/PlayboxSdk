using System;
using System.Collections;
using CI.Utils.Extentions;
using GoogleMobileAds.Ump.Api;
using UnityEngine;
using Utils.Timer;

#if UNITY_IOS
using Unity.Advertisement.IosSupport;
#endif

namespace Playbox.Consent
{
    public class ConsentData
    {
        public static bool IsConsentComplete = false;
        public static bool Gdpr = false;
        public static bool ConsentForData = false;
        public static bool ConsentForAdsPersonalized = false;
        public static bool ConsentForAdStogare = false;
        public static string AdvertisingId = "";

        public static bool IsChildUser = false;
        public static bool HasUserConsent = true;
        public static bool HasDoNotSell = false;
        
        private static ConsentDebugSettings debugSettings = new ConsentDebugSettings();

        private static Action consentCallback;

        public static ConsentDebugSettings DebugSettings
        {
            get => debugSettings;
            set => debugSettings = value;
        }

        // ReSharper disable Unity.PerformanceAnalysis
        public static void ConsentAllow()
        {
            IsConsentComplete = true;
            Gdpr = true;
            ConsentForData = true;
            ConsentForAdsPersonalized = true;
            ConsentForAdStogare = true;
            IsChildUser = false;
            HasUserConsent = true;
            HasDoNotSell = true;

            "Consent Allow".PlayboxInfo();
            //consentCallback?.Invoke(true);
        }

        // ReSharper disable Unity.PerformanceAnalysis
        public static void ConsentDeny()
        {
            IsConsentComplete = true;
            Gdpr = true;
            ConsentForData = true;
            ConsentForAdsPersonalized = true;
            ConsentForAdStogare = true;
            IsChildUser = false;
            HasUserConsent = true;
            HasDoNotSell = true;
            
            "Consent Deny".PlayboxInfo();
            //consentCallback?.Invoke(false);
        }

        static IEnumerator consentUpdate(Action consentComplete)
        {
            "Starting Consent Update".PlayboxInfo();
            
            while (true)
            {
                if (IsConsentComplete)
                {
                    consentComplete?.Invoke();
                    yield break;
                }

                yield return new WaitForSeconds(0.1f);
            }
        }
        
        // ReSharper disable Unity.PerformanceAnalysis
        public static void ShowConsent(MonoBehaviour mono, Action callback, bool isDebug = false)
        {
            if(isDebug)
                GoogleUmpManager.RequestConsentInfoDebug(debugSettings);
            else
                GoogleUmpManager.RequestConsentInfo();
            
            mono.StartCoroutine(consentUpdate(() =>
            {
                
#if PBX_DEVELOPMENT || UNITY_IOS

                bool isATTComplete = false;
                
                IOSConsent.ShowATTUI(mono, () =>
                {
                    isATTComplete = true;
                    callback?.Invoke();
                    
                });
                
                if (isATTComplete)
                    return;
#endif
                
                //block for getting tracking status
                
                bool requestDone = false;
                bool TrackingEnabled = false;
                
                Application.RequestAdvertisingIdentifierAsync((advertisingId, trackingEnabled, errorMsg) =>
                {
                    AdvertisingId = advertisingId;
                    
#if UNITY_IOS
                    TrackingEnabled = ATTrackingStatusBinding.GetAuthorizationTrackingStatus()
                                      == ATTrackingStatusBinding.AuthorizationTrackingStatus.AUTHORIZED;
                    requestDone = true;
#endif
                    
#if UNITY_ANDROID
                    TrackingEnabled = trackingEnabled;
                    requestDone = true;
#endif
                });

#if UNITY_ANDROID || UNITY_EDITOR
                callback?.Invoke();
#endif
                
            }));
            
        }
    }
}
