using Playbox.SdkConfigurations;

namespace Playbox
{
    public class AppLovinInitialization : PlayboxBehaviour
    {
        public override void Initialization()
        {
            base.Initialization();
            
            AppLovinConfiguration.LoadJsonConfig();
            
            if(!AppLovinConfiguration.Active)
                return;

            MaxSdkCallbacks.OnSdkInitializedEvent += OnSdkInitializedEvent;
            
            MaxSdk.SetHasUserConsent(true);
            MaxSdk.SetSdkKey(AppLovinConfiguration.AdvertisementSdk);
            MaxSdk.InitializeSdk();

        }

        public override void Close()
        {
            base.Close();
            MaxSdkCallbacks.OnSdkInitializedEvent -= OnSdkInitializedEvent;
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