using System;
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

        private static Action<bool> consentCallback;

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
            
            consentCallback?.Invoke(true);
        }

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
            
            consentCallback?.Invoke(false);
        }

        public static void ShowConsent(MonoBehaviour mono, Action<bool> callback)
        {
                
            consentCallback += (a) => callback?.Invoke(a);
            
#if PBX_DEVELOPMENT || UNITY_IOS
            
            IOSConsent.ShowConsentUI(mono);
            
#endif  
#if PBX_DEVELOPMENT || UNITY_ANDROID
            
            GoogleUmpManager.RequestConsentInfo();
#endif
            
        }
    }
}
