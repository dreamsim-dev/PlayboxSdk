using System.Collections;
using Playbox.Consent;
using Playbox.SdkConfigurations;
using UnityEngine;

namespace Playbox
{
    public class AppLovinInitialization : PlayboxBehaviour
    {

        public override void Initialization()
        {
            
            base.Initialization();
            
            if (ConsentData.IsChildUser)
                return;
            
            AppLovinConfiguration.LoadJsonConfig();
            
            if(!AppLovinConfiguration.Active)
                return;

            MaxSdkCallbacks.OnSdkInitializedEvent += OnSdkInitializedEvent;
            
            MaxSdk.SetHasUserConsent(ConsentData.HasUserConsent);
            
            //MaxSdk.SetDoNotSell(AppLovinConsent.hasDoNotSell);
            
            MaxSdk.SetSdkKey(AppLovinConfiguration.AdvertisementSdk);
            MaxSdk.InitializeSdk();

            StartCoroutine(initUpd());

        }

        public override void Close()
        {
            base.Close();
            MaxSdkCallbacks.OnSdkInitializedEvent -= OnSdkInitializedEvent;
        }

        private IEnumerator initUpd()
        {
            while (true)
            {
                if (MaxSdk.IsInitialized())
                {
                    ApproveInitialization();
                    yield break;
                }
                else
                {
                    MaxSdk.InitializeSdk();
                }

                yield return new WaitForSeconds(1f);
            }
        }

        private void OnSdkInitializedEvent(MaxSdkBase.SdkConfiguration sdkConfiguration)
        {
#if UNITY_IOS
            Advertisement.RegisterReward(AppLovinConfiguration.IOSKey, this);
#endif
            
#if UNITY_ANDROID
            Advertisement.RegisterReward(AppLovinConfiguration.AndroidKey, this);
#endif
            
            Debug.Log("AppLovin initialized");
            
            Advertisement.OnSdkInitializedEvent?.Invoke(sdkConfiguration.ToString());
        }
        
    }
}