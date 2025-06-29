﻿using System;
using System.Collections.Generic;
using AppsFlyerSDK;
using CI.Utils.Extentions;
using Playbox.Consent;
#if UNITY_EDITOR
#endif
using Playbox.SdkConfigurations;

#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;
using UnityEngine.SceneManagement;

namespace Playbox
{
    public class MainInitialization : PlayboxBehaviour
    {
        [SerializeField] private bool useInAppValidation = true;
        [SerializeField] private bool useLinkGenerator = true;
        [SerializeField] private bool isDebugSplash = false;
        [SerializeField] private bool usePlayboxIAP = false;
        
        private List<PlayboxBehaviour> behaviours = new();
        
        private const string objectName = "[Global] MainInitialization";
        
        private static Dictionary<string,bool> initStatus = new();

        public static Dictionary<string, bool> InitStatus
        {
            get => initStatus ??= new Dictionary<string, bool>();
            set => initStatus = value;
        }

        public static Action PostInitialization = delegate { };
        public static Action PreInitialization = delegate { };

        private void Awake()
        {
            GoogleUmpManager.SubscribeToPreInit();
            Initialization();
        }

        public static bool IsValidate<T>() where T : PlayboxBehaviour
        {
            if(initStatus == null)
                return false;
            
            initStatus.TryGetValue(typeof(T).Name, out bool validate);
                return validate;
        }

        public override void Initialization()
        {
            if(Application.isPlaying)
                DontDestroyOnLoad(gameObject);
            
            GlobalPlayboxConfig.Load();
            
            behaviours.Add(AddToGameObject<PlayboxSplashUGUILogger>(gameObject, isDebugSplash));
            behaviours.Add(AddToGameObject<FirebaseInitialization>(gameObject));
            behaviours.Add(AddToGameObject<DevToDevInitialization>(gameObject));
            behaviours.Add(AddToGameObject<AppLovinInitialization>(gameObject));
            PreInitialization?.Invoke();
            behaviours.Add(AddToGameObject<AppsFlyerInitialization>(gameObject));
            behaviours.Add(AddToGameObject<FacebookSdkInitialization>(gameObject));
            
            behaviours.Add(AddToGameObject<InAppVerification>(gameObject, useInAppValidation));
            behaviours.Add(AddToGameObject<InviteLinkGenerator>(gameObject, useLinkGenerator));
            behaviours.Add(AddToGameObject<IAP>(gameObject, usePlayboxIAP));
            
            InitStatus[nameof(PlayboxSplashUGUILogger)] = false;
            InitStatus[nameof(FirebaseInitialization)] = false;
            InitStatus[nameof(AppsFlyerInitialization)] = false;
            InitStatus[nameof(DevToDevInitialization)] = false;
            InitStatus[nameof(FacebookSdkInitialization)] = false;
            InitStatus[nameof(AppLovinInitialization)] = false;
            InitStatus[nameof(IAP)] = false;
            
            foreach (var item in behaviours)
            {
                if(item != null)
                    item.GetInitStatus(() =>
                    {
                        item.playboxName.PlayboxInfo("INITIALIZED");
                        InitStatus[item.playboxName] = true;
                        
                    });
            }
            
            foreach (var item in behaviours)
            {
                if(item != null)
                    item.Initialization();
            }
            
            PostInitialization?.Invoke();
        }

        private void OnDestroy()
        {
            foreach (var item in behaviours)
            { 
                if(item != null)
                    item.Close();   
            }
        }

        private void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        
        private void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
        
        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            var mainInit = FindObjectOfType<MainInitialization>();

            if (mainInit != this)
            {
                if(mainInit != null)
                    Destroy(mainInit.gameObject);
            }

        }
        
#if UNITY_EDITOR
        [MenuItem("PlayBox/Initialization/Create")]
        public static void CreateAnalyticsObject()
        { 
            var findable = GameObject.Find(objectName);

            if (findable == null)
            {
                var go = new GameObject(objectName); 
                
                go.AddComponent<MainInitialization>();
            }
            else
            {
                if (findable.TryGetComponent(out MainInitialization main))
                {
                    DestroyImmediate(main);
                }
                else
                {
                    findable.AddComponent<MainInitialization>();   
                }
            }
            
        }
        
        [MenuItem("PlayBox/Initialization/Remove")]
        public static void RemoveAnalyticsObject()
        {
            var go = GameObject.Find(objectName);

            if (go != null)
            {
                DestroyImmediate(go);
            }

        }
#endif
    }
}