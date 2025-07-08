using System;
using System.Collections;
using CI.Utils.Extentions;
using UnityEngine;

namespace Playbox.Consent
{
    public class ConsentData
    {
        public static bool IsConsentComplete = false;
        public static bool Gdpr = false;
        public static bool ConsentForData = false;
        public static bool ConsentForAdsPersonalized = false;
        public static bool ConsentForAdStogare = false;

        public static bool IsChildUser = false;
        public static bool HasUserConsent = true;
        public static bool HasDoNotSell = false;

        private static Action consentCallback;

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
            IsConsentComplete = false;
            Gdpr = false;
            ConsentForData = false;
            ConsentForAdsPersonalized = false;
            ConsentForAdStogare = false;
            IsChildUser = false;
            HasUserConsent = false;
            HasDoNotSell = false;
            
            "Consent Deny".PlayboxInfo();
            //consentCallback?.Invoke(false);
        }

        static IEnumerator consentUpdate(Action consentComplete)
        {
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

        public static void ShowConsent(MonoBehaviour mono, Action callback)
        {
            mono.StartCoroutine(consentUpdate(callback));
            
#if PBX_DEVELOPMENT || UNITY_IOS
            
            IOSConsent.ShowConsentUI(mono);
            
#endif  
#if PBX_DEVELOPMENT || UNITY_ANDROID
            
            GoogleUmpManager.RequestConsentInfo();
#endif
            
        }
    }
}
