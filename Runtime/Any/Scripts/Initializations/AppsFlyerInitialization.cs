using System.Collections;
using Playbox.SdkConfigurations;
using AppsFlyerSDK;
using System;
using System.Collections.Generic;
using Playbox.Consent;

using UnityEngine;


namespace Playbox
{
    public class AppsFlyerInitialization : PlayboxBehaviour,IAppsFlyerConversionData
    {
        private string af_status;
        private string media_source;
        
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

            
            AppsFlyer.getConversionData("af_status");

            
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

        public void onConversionDataSuccess(string conversionData)
        {
            Dictionary<string, object> data = AppsFlyer.CallbackStringToDictionary(conversionData);
            
            if (data.TryGetValue("af_status", out var status))
            {
                Debug.Log("Install type: " + status);
                af_status = (string)status;
            }

            if (data.TryGetValue("media_source", out var source))
            {
                Debug.Log("Media source: " + source);
                media_source = (string)source;
            }
        }

        public void onConversionDataFail(string error)
        {
            
        }

        public void onAppOpenAttribution(string attributionData)
        {
            
        }

        public void onAppOpenAttributionFailure(string error)
        {
            
        }

        private IEnumerator PostLog()
        {
            yield return new WaitForSeconds(10);
            Analytics.Events.LogFirebase("af_status",af_status);
            Analytics.Events.LogFirebase("media_source",media_source);
        }
    }
}