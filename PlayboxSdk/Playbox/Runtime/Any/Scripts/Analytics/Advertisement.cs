using System;
using CI.Utils.Extentions;

namespace Playbox
{
    public class Advertisement
    {
        private string unitId;
        private bool isReady => Instance != null && IsReady();

        public event Action OnLoaded;
        public event Action<string> OnLoadedFailed;
        public event Action<string> OnPlayerClosedAd;
        public event Action<string> OnPlayerOnClicked;
        public event Action<string> OnPlayerOpenedAd;
        
        public static Action<string,string> OnAdLoadFailedEvent;
        public static Action<string,string> OnAdReceivedRewardEvent;
        public static Action<string,string> OnAdHiddenEvent;
        public static Action<string> OnSdkInitializedEvent;
        public static Action OnDisplay;
        public static Action OnFailedDisplay;
        public static Action OnRewarderedClose;
        public static Action OnRewarderedReceivedAd;
        
        public static Advertisement Instance { get; private set; }
        public static bool IsInitialized => Instance != null;
        
        public Advertisement(string unitId)
        {
            this.UnitId = unitId;
            
            InitCallback();
            Load();
        }

        public string UnitId
        {
            get => unitId;
            set => unitId = value;
        }

        public static void Initialize(string adUnitId)
        {
            if(Instance == null)
                Instance = new Advertisement(adUnitId);
            else
            {
                return;
            }
        }

        public void Load()
        {
            if (!isReady)
                MaxSdk.LoadRewardedAd(UnitId);
        }

        public static void Show()
        {
            if (!IsInitialized)
            {
                Advertisement.Log("Advertisement not initialized");
                
                return;
            }

            Instance.ShowSelf();
        }

        public void ShowSelf()
        {
            if (Instance.isReady)
            {
                MaxSdk.ShowRewardedAd(Instance.unitId);    
            }
            else
            {
                Instance.Load();
            }
        }

        public static void Log(string message)
        {
            Analytics.TrackEvent(message.PlayboxInfoD("ADS"));
        }

        public static bool IsReady()
        {
            return IsInitialized && MaxSdk.IsRewardedAdReady(Instance.unitId);
        }

        private void InitCallback()
        {
            MaxSdkCallbacks.Rewarded.OnAdLoadedEvent += OnRewardedAdLoadedEvent;
            MaxSdkCallbacks.Rewarded.OnAdLoadFailedEvent += OnRewardedAdLoadFailedEvent;
            MaxSdkCallbacks.Rewarded.OnAdDisplayedEvent += OnRewardedAdDisplayedEvent;
            MaxSdkCallbacks.Rewarded.OnAdClickedEvent += OnRewardedAdClickedEvent;
            MaxSdkCallbacks.Rewarded.OnAdRevenuePaidEvent += OnRewardedAdRevenuePaidEvent;
            MaxSdkCallbacks.Rewarded.OnAdHiddenEvent += OnRewardedAdHiddenEvent;
            MaxSdkCallbacks.Rewarded.OnAdDisplayFailedEvent += OnRewardedAdFailedToDisplayEvent;
            MaxSdkCallbacks.Rewarded.OnAdReceivedRewardEvent += OnRewardedAdReceivedRewardEvent;
        }

        private void OnRewardedAdReceivedRewardEvent(string arg1, MaxSdkBase.Reward error_info, MaxSdkBase.AdInfo info)
        {
            OnRewarderedReceivedAd?.Invoke();   
            Load();
        }

        private void OnRewardedAdFailedToDisplayEvent(string arg1, MaxSdkBase.ErrorInfo error_info, MaxSdkBase.AdInfo info)
        {
            OnFailedDisplay?.Invoke();
            Load();
        }

        private void OnRewardedAdHiddenEvent(string arg1, MaxSdkBase.AdInfo info)
        {
            OnRewarderedClose?.Invoke();
            Load();
        }

        private void OnRewardedAdRevenuePaidEvent(string arg1, MaxSdkBase.AdInfo info)
        {
            
        }

        private void OnRewardedAdClickedEvent(string arg1, MaxSdkBase.AdInfo info)
        {
            OnPlayerOnClicked?.Invoke(arg1);
        }

        private void OnRewardedAdDisplayedEvent(string arg1, MaxSdkBase.AdInfo info)
        {
            OnDisplay?.Invoke();
            Load();
        }

        private void OnRewardedAdLoadFailedEvent(string arg1, MaxSdkBase.ErrorInfo info)
        {
            OnLoadedFailed?.Invoke(info.ToString().PlayboxInfoD(arg1));
            
            Load();
        }

        private void OnRewardedAdLoadedEvent(string arg1, MaxSdkBase.AdInfo info)
        { 
            OnLoaded?.Invoke();
        }
    }
}