using System;
using CI.Utils.Extentions;

namespace Playbox
{
    public static class Advertisement
    {
        private static string unitId;
        private static bool isReady => IsReady();

        public static event Action OnLoaded;
        public static event Action<string> OnLoadedFailed;
        public static event Action<string> OnPlayerClosedAd;
        public static event Action<string> OnPlayerOnClicked;
        public static event Action<string> OnPlayerOpened;
        
        public static Action<string,string> OnAdLoadFailedEvent;
        public static Action<string,string> OnAdReceivedRewardEvent;
        public static Action<string,string> OnAdHiddenEvent;
        public static Action<string> OnSdkInitializedEvent;
        
        public static Action OnDisplay;
        public static Action OnFailedDisplay;
        public static Action OnRewarderedClose;
        public static Action OnRewarderedReceived;
        
        private static bool isInitialized = false;
        
        public static void RegisterReward(string unitId)
        {
            UnitId = unitId;
            
            InitCallback();
            Load();
        }

        public static string UnitId
        {
            get => unitId;
            set => unitId = value;
        }
        
        public static void Load()
        {
            if (!isReady)
                MaxSdk.LoadRewardedAd(UnitId);
        }

        public static void Show()
        {
            ShowSelf();
        }

        public static void ShowSelf()
        {
            if (isReady)
            {
                MaxSdk.ShowRewardedAd(unitId);    
            }
            else
            {
                Load();
            }
        }

        public static void Log(string message)
        {
            Analytics.TrackEvent(message.PlayboxInfoD("ADS"));
        }

        public static bool IsReady()
        {
            if (!isInitialized)
                return false;
            
            return MaxSdk.IsRewardedAdReady(unitId);
        }

        private static void InitCallback()
        {
            MaxSdkCallbacks.OnSdkInitializedEvent += configuration => isInitialized = true;

            MaxSdkCallbacks.Rewarded.OnAdLoadedEvent += OnRewardedAdLoadedEvent;
            MaxSdkCallbacks.Rewarded.OnAdLoadFailedEvent += OnRewardedAdLoadFailedEvent;
            MaxSdkCallbacks.Rewarded.OnAdDisplayedEvent += OnRewardedAdDisplayedEvent;
            MaxSdkCallbacks.Rewarded.OnAdClickedEvent += OnRewardedAdClickedEvent;
            MaxSdkCallbacks.Rewarded.OnAdRevenuePaidEvent += OnRewardedAdRevenuePaidEvent;
            MaxSdkCallbacks.Rewarded.OnAdHiddenEvent += OnRewardedAdHiddenEvent;
            MaxSdkCallbacks.Rewarded.OnAdDisplayFailedEvent += OnRewardedAdFailedToDisplayEvent;
            MaxSdkCallbacks.Rewarded.OnAdReceivedRewardEvent += OnRewardedAdReceivedRewardEvent;
        }

        private static void OnRewardedAdReceivedRewardEvent(string arg1, MaxSdkBase.Reward error_info, MaxSdkBase.AdInfo info)
        {
            Analytics.TrackAd(info);
            OnRewarderedReceived?.Invoke();   
           // Load();
        }

        private static void OnRewardedAdFailedToDisplayEvent(string arg1, MaxSdkBase.ErrorInfo error_info, MaxSdkBase.AdInfo info)
        {
            OnFailedDisplay?.Invoke();
            Load();
        }

        private static void OnRewardedAdHiddenEvent(string arg1, MaxSdkBase.AdInfo info)
        {
            OnRewarderedClose?.Invoke();
            Load();
        }

        private static void OnRewardedAdRevenuePaidEvent(string arg1, MaxSdkBase.AdInfo info)
        {
            
        }

        private static void OnRewardedAdClickedEvent(string arg1, MaxSdkBase.AdInfo info)
        {
            OnPlayerOnClicked?.Invoke(arg1);
        }

        private static void OnRewardedAdDisplayedEvent(string arg1, MaxSdkBase.AdInfo info)
        {
            OnDisplay?.Invoke();
            Load();
        }

        private static void OnRewardedAdLoadFailedEvent(string arg1, MaxSdkBase.ErrorInfo info)
        {
            OnLoadedFailed?.Invoke(info.ToString().PlayboxInfoD(arg1));
            
            Load();
        }

        private static void OnRewardedAdLoadedEvent(string arg1, MaxSdkBase.AdInfo info)
        { 
            OnLoaded?.Invoke();
        }
    }
}