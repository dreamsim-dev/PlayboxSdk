using System.Collections;
using Playbox.SdkConfigurations;
using AppsFlyerSDK;
using System;
using Playbox.Consent;

using UnityEngine;


namespace Playbox
{
    public class AppsFlyerInitialization : PlayboxBehaviour
    {
        public override void Initialization()
        {
            base.Initialization();
            
            AppsFlyerConfiguration.LoadJsonConfig();
            
            if(!AppsFlyerConfiguration.Active)
                return;
            
            AppsFlyerConsent consent = new AppsFlyerConsent(
                    ConsentData.Gdpr,
                    ConsentData.ConsentForData,
                    ConsentData.ConsentForAdsPersonalized,
                    ConsentData.ConsentForAdStogare);

            AppsFlyer.setConsentData(consent);
            

#if UNITY_IOS
                AppsFlyer.initSDK(AppsFlyerConfiguration.IOSKey, AppsFlyerConfiguration.IOSAppId);
            
#elif UNITY_ANDROID
            
            AppsFlyer.initSDK(AppsFlyerConfiguration.AndroidKey, AppsFlyerConfiguration.AndroidAppId);
#endif 
            
            AppsFlyer.setSharingFilterForPartners(new string[] { });
            
            AppsFlyer.enableTCFDataCollection(true);
            
            AppsFlyer.startSDK();
            
            AppsFlyer.setIsDebug(true);      


            
            StartCoroutine(initUpd());

        }

        private IEnumerator initUpd()
        {
            while (true)
            {
                if (!string.IsNullOrEmpty(AppsFlyer.getAppsFlyerId()))
                {
                    ApproveInitialization();
                    yield break;
                }
                yield return new WaitForSeconds(1f);
            }
        }
    }
}