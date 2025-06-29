﻿using System.Collections;
using Playbox.SdkConfigurations;
using AppsFlyerSDK;

#if UNITY_IOS
using Unity.Advertisement.IosSupport;
#endif

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
                            true,
                        true,
                true,
                    true);
            
            AppsFlyer.setConsentData(consent);
      
#if UNITY_IOS

                AppsFlyer.waitForATTUserAuthorizationWithTimeoutInterval(60);

                var attStatus = ATTrackingStatusBinding.GetAuthorizationTrackingStatus();
                if (attStatus == ATTrackingStatusBinding.AuthorizationTrackingStatus.NOT_DETERMINED)
                {
                    ATTrackingStatusBinding.RequestAuthorizationTracking();
                }

                AppsFlyer.initSDK(AppsFlyerConfiguration.IOSKey, AppsFlyerConfiguration.IOSAppId);
            
#elif UNITY_ANDROID
            
            AppsFlyer.initSDK(AppsFlyerConfiguration.AndroidKey, AppsFlyerConfiguration.AndroidAppId);
#endif 
            
            AppsFlyer.setSharingFilterForPartners(new string[] { });
            
            AppsFlyer.enableTCFDataCollection(true);
            
            AppsFlyer.startSDK();

//#if UNITY_EDITOR
            AppsFlyer.setIsDebug(true);      
//#endif
            
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