using System;
using CI.Utils.Extentions;
using UnityEngine;

namespace Playbox
{
    /// <summary>
    /// <param name="Ready">
    /// The commercials are loaded.
    /// Реклама загружена.
    /// </param>>
    /// <param name="NotReady">
    /// The ads are not loaded.
    /// Реклама не загружена.
    /// </param>>
    /// <param name="NullStatus">
    /// The unitId of the advertisement is equal to Null.
    /// UnitId рекламы равен Null.
    /// </param>>
    /// <param name="NotInitialized">
    /// MaxSdk is not initialized.
    /// MaxSdk не проинициализирован.
    /// </param>>
    /// </summary>
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
        /// <summary>
        /// Returns the status of the advertisement's readiness for display.
        /// Возвращает статус готовности рекламы к показу.
        /// </summary>
        public static bool isReady()
        {
            var ready = IsReadyStatus();
            return ready == AdReadyStatus.Ready;
        }
        /// <summary>
        /// Called when an advertisement is loaded.
        /// Вызывается когда реклама загружена.
        /// </summary>
        public static event Action OnLoaded;
        /// <summary>
        /// Called when advertising is not loaded.
        /// Вызывается когда реклама не загружена.
        /// </summary>
        public static event Action<string> OnLoadedFailed;
        /// <summary>
        /// Called when an advertisement has been closed by a player.
        /// Вызывается когда реклама была закрыта игроком.
        /// </summary>
        public static event Action<string> OnPlayerClosedAd;
        /// <summary>
        /// Called when an ad was clicked.
        /// Вызывается когда было произошло нажатие на рекламу.
        /// </summary>
        public static event Action<string> OnPlayerOnClicked;
        /// <summary>
        /// Called when an advertisement has not loaded.
        /// Вызывается когда реклама не загрузилась.
        /// </summary>
        public static Action<string,string> OnAdLoadFailedEvent;
        /// <summary>
        /// Called when an advertisement has been viewed.
        /// Вызывается когда реклама была просмотрена.
        /// <param name=""></param>>
        /// </summary>
        public static Action<string,string> OnAdReceivedRewardEvent;
        /// <summary>
        /// Called when an advertisement has been closed by the user.
        /// Вызывается когда реклама была закрыта пользователем.
        /// </summary>
        public static Action<string,string> OnAdHiddenEvent;
        /// <summary>
        /// Called when an ad was clicked.
        /// Вызывается когда было произошло нажатие на рекламу.
        /// </summary>
        public static Action<string> OnSdkInitializedEvent;
        
        /// <summary>
        /// Called when an advertisement is displayed.
        /// Вызывается когда реклама отобразилась.
        /// </summary>
        public static Action OnDisplay;
        /// <summary>
        /// Called when the advertisement failed to display.
        /// Вызывается когда реклама не смогла отобразится.
        /// </summary>
        public static Action OnFailedDisplay;
        /// <summary>
        /// Called when an advertisement has been closed by the user.
        /// Вызывается когда реклама была закрыта пользователем.
        /// </summary>
        public static Action OnRewarderedClose;
        /// <summary>
        /// Called when an advertisement has been viewed.
        /// Вызывается когда реклама была просмотрена.
        /// </summary>
        public static Action OnRewarderedReceived;
        
        private static AppLovinInitialization appLovinInitialization;
        
        /// <summary>
        /// A method for initializing fields for ads, callbacks, and starting to load them.
        /// Метод инициализации полей для рекламы, коллбеков и начала ее загрузки.
        /// <param name="unitId">
        /// Ad token from AppLovin(Unique for each platform).
        /// Токен рекламы из AppLovin(Уникален для каждой платформы).
        /// </param>>
        /// <param name="aInitialization">
        /// AppLovin initialization script, required for basic services to work.
        /// Скрипт инициализации AppLovin, необходим для работы основных сервисов.
        /// </param>>
        /// </summary>
        public static void RegisterReward(string unitId, AppLovinInitialization aInitialization)
        {
            UnitId = unitId;
            appLovinInitialization = aInitialization;
            
            InitCallback();
            Load();
        }

        /// <summary>
        /// UnitId of the advertisement, retrieved from AppLovin.
        /// UnitId рекламы, достается из AppLovin.
        /// </summary>
        public static string UnitId
        {
            get => unitId;
            set => unitId = value;
        }
        
        /// <summary>
        /// Loading ads.
        /// Загрузка рекламы.
        /// </summary>
        public static void Load()
        {
            if (MaxSdk.IsInitialized())
                MaxSdk.LoadRewardedAd(UnitId);
        }
        /// <summary>
        /// Loading ads after a certain amount of time.
        /// Загрузка рекламы после определенного времени.
        /// <param name="delay">
        /// The time after which the advertisement will start loading.
        /// Время после которого начнется загрузка рекламы.
        /// </param>>
        /// </summary>
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

        /// <summary>
        /// Returns the ready state of the advertisement.
        /// Возвращает состояние готовности рекламы.
        /// <inheritdoc cref="AdReadyStatus"/>>
        /// </summary>
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
        
        private static void OnRewardedAdReceivedRewardEvent(string arg1, MaxSdkBase.Reward reward, MaxSdkBase.AdInfo info)
        {
            Analytics.TrackAd(info);
            
            OnRewarderedReceived?.Invoke();  
            OnAdReceivedRewardEvent?.Invoke(arg1, reward.ToString());
            Load();
            Analytics.TrackEvent("rewarded_received_display");
        }

        private static void OnRewardedAdFailedToDisplayEvent(string arg1, MaxSdkBase.ErrorInfo reward, MaxSdkBase.AdInfo info)
        {
            OnFailedDisplay?.Invoke();
            Load();
            Analytics.TrackEvent("rewarded_failed_display");
        }

        private static void OnRewardedAdHiddenEvent(string arg1, MaxSdkBase.AdInfo info)
        {
            OnRewarderedClose?.Invoke();
            OnPlayerClosedAd?.Invoke(UnitId);
            OnAdHiddenEvent?.Invoke(arg1, info.ToString());
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