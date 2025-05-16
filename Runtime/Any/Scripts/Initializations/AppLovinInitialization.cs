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
            
            MaxSdkCallbacks.Rewarded.OnAdLoadFailedEvent += OnAdLoadFailedEvent;
            MaxSdkCallbacks.Rewarded.OnAdReceivedRewardEvent += OnAdReceivedRewardEvent;
            MaxSdkCallbacks.Rewarded.OnAdHiddenEvent += OnAdHiddenEvent;
            MaxSdkCallbacks.OnSdkInitializedEvent += OnSdkInitializedEvent;
            
            MaxSdk.SetSdkKey(AppLovinConfiguration.AdvertisementSdk);
            MaxSdk.InitializeSdk();

        }

        public override void Close()
        {
            base.Close();
            
            MaxSdkCallbacks.Rewarded.OnAdLoadFailedEvent -= OnAdLoadFailedEvent;
            MaxSdkCallbacks.Rewarded.OnAdReceivedRewardEvent -= OnAdReceivedRewardEvent;
            MaxSdkCallbacks.OnSdkInitializedEvent -= OnSdkInitializedEvent;
        }

        private void OnAdHiddenEvent(string arg1, MaxSdkBase.AdInfo arg2)
        {
           Advertisement.OnAdHiddenEvent?.Invoke(arg1, arg2.ToString());
        }

        private void OnAdReceivedRewardEvent(string arg1, MaxSdkBase.Reward arg2, MaxSdkBase.AdInfo arg3)
        {
            Advertisement.OnAdReceivedRewardEvent?.Invoke(arg1, arg2.ToString());
        }

        private void OnAdLoadFailedEvent(string arg1, MaxSdkBase.ErrorInfo arg2)
        {
            Advertisement.OnAdLoadFailedEvent?.Invoke(arg1, arg2.ToString());
        }
        
        private void OnSdkInitializedEvent(MaxSdkBase.SdkConfiguration sdkConfiguration)
        {
#if UNITY_IOS
            Advertisement.RegisterReward(AppLovinConfiguration.IOSKey);
#endif
            
#if UNITY_ANDROID
            Advertisement.RegisterReward(AppLovinConfiguration.AndroidKey);
#endif
            
            Advertisement.OnSdkInitializedEvent?.Invoke(sdkConfiguration.ToString());
            
            Advertisement.Log("AppLovinInitialization");
        }
    }
}