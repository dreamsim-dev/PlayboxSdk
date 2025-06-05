using System.Collections;
using Playbox.SdkConfigurations;
using UnityEngine;

namespace Playbox
{
    public class AppLovinInitialization : PlayboxBehaviour
    {
        public bool isChildUser { get; set; } = false;
        public bool hasUserConsent { get; set; } = true;
        public bool hasDoNotSell { get; set; } = false;

        public override void Initialization()
        {
            base.Initialization();
            
            if (isChildUser)
                return;
            
            AppLovinConfiguration.LoadJsonConfig();
            
            if(!AppLovinConfiguration.Active)
                return;

            MaxSdkCallbacks.OnSdkInitializedEvent += OnSdkInitializedEvent;
            
            MaxSdk.SetHasUserConsent(hasUserConsent);
            MaxSdk.SetDoNotSell(hasDoNotSell);
            
            MaxSdk.SetSdkKey(AppLovinConfiguration.AdvertisementSdk);

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
            
            Advertisement.OnSdkInitializedEvent?.Invoke(sdkConfiguration.ToString());
            
            Advertisement.Log("AppLovinInitialization");
        }
        
    }
}