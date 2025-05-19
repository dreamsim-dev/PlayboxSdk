using System.Collections;
using Playbox.SdkConfigurations;
using AppsFlyerSDK;
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

      
#if UNITY_IOS

             AppsFlyer.initSDK(AppsFlyerConfiguration.IOSKey, AppsFlyerConfiguration.IOSAppId);
#elif UNITY_ANDROID
            
            AppsFlyer.initSDK(AppsFlyerConfiguration.AndroidKey, AppsFlyerConfiguration.AndroidAppId);
#endif 
            
            AppsFlyer.startSDK();

#if UNITY_EDITOR
            AppsFlyer.setIsDebug(true);      
#endif
            
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