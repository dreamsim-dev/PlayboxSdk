using System;
using CI.Utils.Extentions;
using UnityEngine;

namespace Playbox
{
      public enum AdReadyStatus
        {
            Ready,
            NotReady,
            NullStatus,
            NotInitialized
        }
    
    public static class Advertisement
    {
        private static string unitId;

        public static bool isReady()
        {
            var ready = IsReadyStatus();
            return ready == AdReadyStatus.Ready;
        }


        private static bool readyFlag = false;

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
        
        private static AppLovinInitialization appLovinInitialization;
        
        public static void RegisterReward(string unitId, AppLovinInitialization aInitialization)
        {
            UnitId = unitId;
            appLovinInitialization = aInitialization;
            
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
            if (MaxSdk.IsInitialized())
                MaxSdk.LoadRewardedAd(UnitId);
        }

        public static void Load(float delay)
        {
            if (appLovinInitialization)
            {
                appLovinInitialization.DelayInvoke(() => { Load(); }, delay);
            }
        }

        public static void Show()
        {
            if (isReady())
            {
                MaxSdk.ShowRewardedAd(unitId);    
            }
            else
            {
                Load();
            }
        }

        public static void ShowSelf()
        {
            if (isReady())
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

        public static AdReadyStatus IsReadyStatus()
        {
            if (!MaxSdk.IsInitialized())
            {
                MaxSdk.InitializeSdk();
                return AdReadyStatus.NotInitialized;
            }

            if (string.IsNullOrEmpty(unitId))
            {
                return AdReadyStatus.NullStatus;
            }

            if (MaxSdk.IsRewardedAdReady(unitId))
            {
                return AdReadyStatus.Ready;
            }
            else
            {
                return AdReadyStatus.NotReady;
            }
        }

        private static void InitCallback()
        {
            MaxSdkCallbacks.Rewarded.OnAdLoadedEvent += OnRewardedAdLoadedEvent;
            MaxSdkCallbacks.Rewarded.OnAdLoadFailedEvent += OnRewardedAdLoadFailedEvent;
            MaxSdkCallbacks.Rewarded.OnAdDisplayedEvent += OnRewardedAdDisplayedEvent;
            MaxSdkCallbacks.Rewarded.OnAdClickedEvent += OnRewardedAdClickedEvent;
            MaxSdkCallbacks.Rewarded.OnAdHiddenEvent += OnRewardedAdHiddenEvent;
            MaxSdkCallbacks.Rewarded.OnAdDisplayFailedEvent += OnRewardedAdFailedToDisplayEvent;
            MaxSdkCallbacks.Rewarded.OnAdReceivedRewardEvent += OnRewardedAdReceivedRewardEvent;
        }

        private static void OnRewardedAdReceivedRewardEvent(string arg1, MaxSdkBase.Reward error_info, MaxSdkBase.AdInfo info)
        {
            Analytics.TrackAd(info);
            
            OnRewarderedReceived?.Invoke();  
            OnAdReceivedRewardEvent?.Invoke(arg1, error_info.ToString());
            Load();
            Analytics.TrackEvent("rewarded_received_display");
        }

        private static void OnRewardedAdFailedToDisplayEvent(string arg1, MaxSdkBase.ErrorInfo error_info, MaxSdkBase.AdInfo info)
        {
            OnFailedDisplay?.Invoke();
            Load();
            Analytics.TrackEvent("rewarded_failed_display");
        }

        private static void OnRewardedAdHiddenEvent(string arg1, MaxSdkBase.AdInfo info)
        {
            OnRewarderedClose?.Invoke();
            Load();
            Analytics.TrackEvent("rewarded_hidden_event");
        }

        private static void OnRewardedAdClickedEvent(string arg1, MaxSdkBase.AdInfo info)
        {
            OnPlayerOnClicked?.Invoke(arg1);
        }

        private static void OnRewardedAdDisplayedEvent(string arg1, MaxSdkBase.AdInfo info)
        {
            OnDisplay?.Invoke();
            Analytics.TrackEvent("rewarded_display");
        }

        private static void OnRewardedAdLoadFailedEvent(string arg1, MaxSdkBase.ErrorInfo info)
        {
            OnLoadedFailed?.Invoke(info.ToString().PlayboxInfoD(arg1));
            OnAdLoadFailedEvent?.Invoke(arg1, info.ToString());
            Load(1);
            Analytics.TrackEvent("rewarded_load_failed");
        }

        private static void OnRewardedAdLoadedEvent(string arg1, MaxSdkBase.AdInfo info)
        { 
            Analytics.TrackEvent("rewarded_loaded");
            OnLoaded?.Invoke();
        }
    }
}